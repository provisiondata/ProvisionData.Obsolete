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

namespace ProvisionData.EntityFrameworkCore
{
	using Bogus;
	using Microsoft.Data.Sqlite;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using System;
	using System.Data.Common;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading.Tasks;
	using Xunit;

	public abstract class IntegrationTestBase : IAsyncLifetime
	{
		private const String ConnectionString = "Filename=:memory:";
		protected static readonly FamilyMember[] Family = new[]
		{
			new FamilyMember(Guid.NewGuid(), "Doug", new DateTime(1974, 10, 16)),
			new FamilyMember(Guid.NewGuid(), "Charmaine", new DateTime(1975, 11, 11)),
			new FamilyMember(Guid.NewGuid(), "Shyloh", new DateTime(2007, 10, 15)),
			new FamilyMember(Guid.NewGuid(), "Piper", new DateTime(2008, 5, 19)),
			new FamilyMember(Guid.NewGuid(), "Geordi", new DateTime(2011, 9, 14)) { Sensitive = "This property is Sensitive.", Ignored ="This should be ignored." }
		};

		protected static readonly Faker Faker = new();

		private DbConnection _connection;

		protected internal IntegrationTestBase()
			: this(GetOptions())
		{ }

		private static DbContextOptions<TestDbContext> GetOptions()
			=> new DbContextOptionsBuilder<TestDbContext>()
				.UseSqlite(GetOpenDatabaseConnection())
				.EnableSensitiveDataLogging(true)
				.Options;

		protected IntegrationTestBase(DbContextOptions<TestDbContext> options)
		{
			ContextOptions = options;
			_connection = RelationalOptionsExtension.Extract(ContextOptions).Connection
				?? throw new InvalidOperationException("The supplied DbContextOptions<TestDbContext> {nameof(options)} does not contain an open DbConnection.");
		}

		protected DbContextOptions<TestDbContext> ContextOptions { get; private set; }

		public async Task InitializeAsync()
		{
			using (var context = new TestDbContext(ContextOptions))
			{
				await context.Database.EnsureCreatedAsync();

				if (_connection.State != System.Data.ConnectionState.Open)
					await _connection.OpenAsync();

				await Respawn(_connection);

				await SeedAsync(context);
			}
		}

		protected virtual async Task Respawn(DbConnection connection)
		{
			using (var context = new TestDbContext(ContextOptions))
			{
				await context.Database.EnsureDeletedAsync();

				await context.Database.EnsureCreatedAsync();
			}
		}

		[SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method", Justification = "<Pending>")]
		protected virtual async Task SeedAsync(TestDbContext dbContext)
		{
			Debug.Assert(!await dbContext.Members.AnyAsync());

			dbContext.AddRange(Family);

			await dbContext.SaveChangesAsync();
		}

		public Task DisposeAsync()
		{
			_connection?.Dispose();
			_connection = null;
			return Task.CompletedTask;
		}

		private static DbConnection GetOpenDatabaseConnection()
		{
			var connection = new SqliteConnection(ConnectionString);
			connection.Open();
			return connection;
		}
	}
}
