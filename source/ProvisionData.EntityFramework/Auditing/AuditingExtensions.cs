/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.EntityFrameworkCore.Auditing
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.ChangeTracking;
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Security.Cryptography;
	using System.Text;

	public static class AuditingExtensions
	{
		public static ModelBuilder ConfigureAuditLog(this ModelBuilder modelBuilder)
		{
			if (modelBuilder is null)
			{
				throw new ArgumentNullException(nameof(modelBuilder));
			}

			modelBuilder.Entity<AuditEntry>(entity =>
			{
				entity.ToTable("AuditLog");
				entity.HasKey(e => e.Id).HasName("PK_AudiLog");

				entity.Property(e => e.Id).IsRequired(true);
				entity.Property(e => e.DateTime).IsRequired(true);
				entity.Property(e => e.User).HasMaxLength(100).IsRequired(true);
				entity.Property(e => e.Action).IsRequired(true);
				entity.Property(e => e.Entity).HasMaxLength(256).IsRequired(true);
				entity.Property(e => e.Changed).IsRequired(false);
				entity.Property(e => e.PrimaryKey).IsRequired(true);
				entity.Property(e => e.Previous).IsRequired(false);
				entity.Property(e => e.Current).IsRequired(false);
				entity.Property(e => e.Type).IsRequired(true);
			});

			return modelBuilder;
		}

		internal static IEnumerable<AuditEntryMetaData> GetAuditEntries(this IAuditedDbContext dbContext, String username)
		{
			var list = new List<AuditEntryMetaData>();
			dbContext.ChangeTracker.DetectChanges();
			foreach (var entity in dbContext.ChangeTracker.Entries())
			{
				if (IsIgnored(entity))
				{
					continue;
				}

				using var hasher = new SHA256Managed();
				String Hash(Object obj, Byte[] salt) => BitConverter.ToString(hasher.ComputeHash(GetBytes(obj.ToString(), salt))).Replace("-", "");

				if (entity.State == EntityState.Detached || entity.State == EntityState.Unchanged)
					continue;

				var salt = Guid.NewGuid().ToByteArray();
				var entry = new AuditEntryMetaData(entity)
				{
					Action = Translate(entity.State),
					Entity = entity.Entity.GetType().Name,
					Username = username,
					Type = entity.Entity.GetType()
				};
				list.Add(entry);
				foreach (var property in entity.Properties.OrderBy(p => p.Metadata.Name))
				{
					if (property.IsTemporary)
					{
						entry.TemporaryProperties.Add(property);
						continue;
					}

					String propertyName = property.Metadata.Name;
					if (property.Metadata.IsPrimaryKey())
					{
						entry.KeyValues[propertyName] = property.CurrentValue;
						continue;
					}

					if (IsIgnored(property))
						continue;

					var isSensitive = IsSensitive(property);
					switch (entity.State)
					{
						case EntityState.Added:
							entry.Changed.Add(propertyName);
							entry.Current[propertyName] = isSensitive ? Hash(property.CurrentValue, salt) : property.CurrentValue;
							break;

						case EntityState.Deleted:
							entry.Previous[propertyName] = isSensitive ? Hash(property.OriginalValue, salt) : property.OriginalValue;
							break;

						case EntityState.Modified:
							entry.Previous[propertyName] = isSensitive ? Hash(property.OriginalValue, salt) : property.OriginalValue;
							entry.Current[propertyName] = isSensitive ? Hash(property.CurrentValue, salt) : property.CurrentValue;
							if (Normalize(property.CurrentValue) != Normalize(property.OriginalValue))
							{
								entry.Changed.Add(propertyName);
							}
							break;
					}
				}
			}
			return list;
		}

		private static String Normalize(Object obj) => obj?.ToString() ?? String.Empty;
		private static Byte[] GetBytes(String plainText, Byte[] salt)
		{
			var plain = Encoding.UTF8.GetBytes(plainText);
			var bytes = new Byte[plain.Length + salt.Length];

			for (Int32 i = 0; i < plainText.Length; i++)
			{
				bytes[i] = plain[i];
			}
			for (Int32 i = 0; i < salt.Length; i++)
			{
				bytes[plainText.Length + i] = salt[i];
			}
			return bytes;
		}


		private static readonly ConcurrentDictionary<Type, Boolean> TypeCache = new();
		private static readonly ConcurrentDictionary<String, Boolean> PropertyCache = new();
		private static readonly ConcurrentDictionary<String, Boolean> SensitiveCache = new();

		private static Boolean IsIgnored(EntityEntry entity)
			=> TypeCache.GetOrAdd(entity.Entity.GetType(), (type) =>
			{
				var classAttr = type.GetCustomAttribute<AuditAttribute>();
				var result = classAttr is not null && (!type.IsClass || classAttr.Ignore == true);
				return result;
			});

		private static Boolean IsIgnored(PropertyEntry property)
		{
			return PropertyCache.GetOrAdd(GetKey(property), (k) =>
			{
				var info = GetPropertyInfo(property);

				if (property.Metadata.IsConcurrencyToken || info.PropertyType.IsArray)
					return true;

				var attr = info.GetCustomAttribute<AuditAttribute>();
				var result = attr is not null && attr.Ignore == true;
				return result;
			});
		}

		private static PropertyInfo GetPropertyInfo(PropertyEntry property)
		{
			var eType = property.EntityEntry.Entity.GetType();
			var pType = eType.GetProperty(property.Metadata.Name);
			return pType;
		}

		private static Boolean IsSensitive(PropertyEntry property)
			=> SensitiveCache.GetOrAdd(GetKey(property), (k) =>
			{
				var info = GetPropertyInfo(property);
				var attr = info.GetCustomAttribute<AuditAttribute>();
				var result = attr is not null && attr.Sensitive == true;
				return result;
			});

		private const String dot = ".";
		private static String GetKey(PropertyEntry property)
			=> property.Metadata.DeclaringEntityType.Name + dot + property.Metadata.Name;

		internal static AuditAction Translate(this EntityState state)
			=> state switch
			{
				EntityState.Added => AuditAction.Created,
				EntityState.Deleted => AuditAction.Deleted,
				EntityState.Modified => AuditAction.Updated,
				_ => AuditAction.None
			};

		internal static AuditEntry[] Normal(this IEnumerable<AuditEntryMetaData> entries)
			=> entries.Where(c => !c.HasTemporaryProperties).Select(e => e.ToDto()).ToArray();

		internal static IEnumerable<AuditEntry> Temporary(this IEnumerable<AuditEntryMetaData> auditEntries)
		{
			if (auditEntries == null || !auditEntries.Any())
			{
				yield break;
			}

			foreach (var entry in auditEntries.Where(c => c.HasTemporaryProperties))
			{
				foreach (var prop in entry.TemporaryProperties)
				{
					if (prop.Metadata.IsPrimaryKey())
					{
						entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
					}
					else
					{
						entry.Current[prop.Metadata.Name] = prop.CurrentValue;
					}
				}
				yield return entry.ToDto();
			}
		}
	}
}
