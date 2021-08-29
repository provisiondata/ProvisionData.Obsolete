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
	using System;
	using System.Collections.Generic;
	using System.Linq;

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
				entity.Property(e => e.RowVersion).IsConcurrencyToken();
			});

			return modelBuilder;
		}

		internal static IEnumerable<AuditEntryDto> GetAuditEntries(this IAuditedDbContext dbContext, String username)
		{
			var list = new List<AuditEntryDto>();
			dbContext.ChangeTracker.DetectChanges();
			foreach (var entity in dbContext.ChangeTracker.Entries())
			{
				if (entity.Entity is AuditEntry || entity.State == EntityState.Detached || entity.State == EntityState.Unchanged)
					continue;

				var entry = new AuditEntryDto(entity)
				{
					Action = Translate(entity.State),
					Table = entity.Entity.GetType().Name,
					Username = username,
					Type = entity.Entity.GetType()
				};
				list.Add(entry);
				foreach (var property in entity.Properties)
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

					switch (entity.State)
					{
						case EntityState.Added:
							entry.Columns.Add(propertyName);
							entry.New[propertyName] = property.CurrentValue;
							break;

						case EntityState.Deleted:
							entry.Old[propertyName] = property.OriginalValue;
							break;

						case EntityState.Modified:
							entry.Old[propertyName] = property.OriginalValue;
							entry.New[propertyName] = property.CurrentValue;
							if (property.CurrentValue.Normalize() != property.OriginalValue.Normalize())
							{
								entry.Columns.Add(propertyName);
							}
							break;
					}
				}
			}
			return list;
		}

		internal static AuditAction Translate(this EntityState state)
			=> state switch
			{
				EntityState.Added => AuditAction.Created,
				EntityState.Deleted => AuditAction.Deleted,
				EntityState.Modified => AuditAction.Updated,
				_ => AuditAction.None
			};

		internal static String Normalize(this object obj) => obj?.ToString() ?? String.Empty;

		internal static AuditEntry[] Normal(this IEnumerable<AuditEntryDto> entries)
			=> entries.Where(c => !c.HasTemporaryProperties).Select(e => e.ToDto()).ToArray();

		internal static IEnumerable<AuditEntry> Temporary(this IEnumerable<AuditEntryDto> auditEntries)
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
						entry.New[prop.Metadata.Name] = prop.CurrentValue;
					}
				}
				yield return entry.ToDto();
			}
		}
	}
}
