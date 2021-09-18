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
	using Bogus;
	using FluentAssertions;
	using Microsoft.EntityFrameworkCore;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Xunit;
	using Xunit.Abstractions;

	public class AuditingTests : IntegrationTestBase
	{
		private readonly List<Product> Products = new();

		public AuditingTests(ITestOutputHelper output) : base()
		{
			Output = output;
		}

		protected AuditingTests(DbContextOptions<TestDbContext> options, ITestOutputHelper output)
			: base(options)
		{
			Output = output;
		}

		public ITestOutputHelper Output { get; }
		public Customer Customer { get; private set; }

		protected override async Task SeedAsync(TestDbContext dbContext)
		{
			using (var db = new TestDbContext(ContextOptions))
			{
				Customer = Customer.Fake();
				db.Customers.Add(Customer);

				Products.AddRange(Faker.Make(10, Product.Fake));
				db.Products.AddRange(Products);

				await db.SaveChangesAsync().ConfigureAwait(false);
			}

			await base.SeedAsync(dbContext);
		}

		[Fact]
		public async Task Adding()
		{
			using (var db = new TestDbContext(ContextOptions))
			{
				// Act
				var ziva = new FamilyMember(Guid.NewGuid(), "Ziva", new DateTime(2015, 07, 01));
				db.Members.Add(ziva);
				await db.SaveChangesAsync();

				// Assert
				var entry = await db.AuditLogs.OrderByDescending(e => e.DateTime).FirstAsync();

				LogEntry(entry);
				entry.Action.Should().Be(AuditAction.Created);
				entry.Previous.Should().BeNull();
				entry.Current.Should().StartWith("{\"DateOfBirth\":\"2015-07-01T00:00:00\",\"Name\":\"Ziva\",\"Sensitive\":");
				entry.Changed.Should().Be("[\"DateOfBirth\",\"Name\",\"Sensitive\"]");
				Validate(entry);
			}
		}

		[Fact]
		public async Task Updating()
		{
			using (var db = new TestDbContext(ContextOptions))
			{
				var doug = await db.Members.SingleAsync(e => e.Id == Family[0].Id);

				// Act
				doug.Name = "Douglas";
				db.Members.Update(doug);
				await db.SaveChangesAsync();
			}

			using (var db = new TestDbContext(ContextOptions))
			{
				// Assert
				var entry = await db.AuditLogs.OrderByDescending(e => e.DateTime).FirstAsync();

				LogEntry(entry);

				entry.Action.Should().Be(AuditAction.Updated);
				entry.Previous.Should().StartWith("{\"DateOfBirth\":\"1974-10-16T00:00:00\",\"Name\":\"Doug\",\"Sensitive\":");
				entry.Current.Should().StartWith("{\"DateOfBirth\":\"1974-10-16T00:00:00\",\"Name\":\"Douglas\",\"Sensitive\":");
				entry.Changed.Should().Be("[\"Name\"]");

				Validate(entry);
			}
		}

		[Fact]
		public async Task Deleting()
		{
			// Act
			using (var db = new TestDbContext(ContextOptions))
			{
				var charmaine = await db.Members.SingleAsync(e => e.Id == Family[1].Id);
				db.Members.Remove(charmaine);
				await db.SaveChangesAsync();
			}

			using (var db = new TestDbContext(ContextOptions))
			{
				// Assert
				var entry = await db.AuditLogs.OrderByDescending(e => e.DateTime).FirstAsync();

				entry.Action.Should().Be(AuditAction.Deleted);
				entry.Previous.Should().StartWith("{\"DateOfBirth\":\"1975-11-11T00:00:00\",\"Name\":\"Charmaine\",\"Sensitive\":");
				entry.Current.Should().BeNull();
				entry.Changed.Should().BeNull();
				LogEntry(entry);
				Validate(entry);
			}
		}

		[Fact]
		public async Task AuditingAsync()
		{
			using (var db = new TestDbContext(ContextOptions))
			{
				// Arrange
				// add
				var ziva = new FamilyMember(Guid.NewGuid(), "Ziva", new DateTime(2015, 07, 01));
				db.Members.Add(ziva);
				await db.SaveChangesAsync();
				// update
				var doug = await db.Members.SingleAsync(e => e.Id == Family[0].Id);
				doug.Name = "Douglas";
				db.Members.Update(doug);
				await db.SaveChangesAsync();
				// delete
				var charmaine = await db.Members.SingleAsync(e => e.Id == Family[1].Id);
				db.Members.Remove(charmaine);
				await db.SaveChangesAsync();
			}

			using (var db = new TestDbContext(ContextOptions))
			{
				// Assert
				var entries = await db.AuditLogs
					.Where(e => e.Type == FamilyMember.TypeName)
					.OrderByDescending(e => e.DateTime)
					.ToListAsync();

				foreach (var entry in entries)
				{
					LogEntry(entry);
					Validate(entry);
				}

				entries.Count.Should().Be(8);
			}
		}


		[Fact]
		public async Task Sequential_Saves()
		{
			using (var db = new TestDbContext(ContextOptions))
			{
				var invoice = Invoice.Fake();
				invoice.CustomerId = Customer.Id;
				db.Invoices.Add(invoice);
				await db.SaveChangesAsync();

				var p = Products[0];
				invoice.LineItems.Add(new LineItem() { Name = p.Name, Description = p.Description, UnitPrice = p.UnitPrice, ProductID = p.Id });
				db.Invoices.Update(invoice);
				await db.SaveChangesAsync();
			}

			using (var db = new TestDbContext(ContextOptions))
			{
				// Assert
				var entries = await db.AuditLogs.OrderByDescending(e => e.DateTime).ToListAsync();

				entries.ForEach(LogEntry);
				entries.First().Action.Should().Be(AuditAction.Updated);
			}
		}

		private void Validate(AuditEntry entry)
		{
			entry.Action.Should().NotBe(AuditAction.None);
			Enum.IsDefined(entry.Action).Should().BeTrue();
			entry.DateTime.Should().NotBe(new DateTime());
			entry.User.Should().NotBeNullOrWhiteSpace();
			entry.Entity.Should().NotBeNullOrWhiteSpace();
			entry.PrimaryKey.Should().NotBeNullOrWhiteSpace();
			entry.Type.Should().StartWith("ProvisionData.EntityFrameworkCore.FamilyMember");
		}

		private void LogEntry(AuditEntry entry)
		{
			Output.WriteLine($"    Date: {entry.DateTime}");
			Output.WriteLine($"    User: {entry.User}");
			Output.WriteLine($"  Action: {entry.Action}");
			Output.WriteLine($"  Entity: {entry.Entity}");
			Output.WriteLine($"     Key: {entry.PrimaryKey}");
			Output.WriteLine($"Previous: {entry.Previous}");
			Output.WriteLine($" Current: {entry.Current}");
			Output.WriteLine($" Changed: {entry.Changed}");
			Output.WriteLine($"    Type: {entry.Type}");
			Output.WriteLine(String.Empty);
		}
	}
}
