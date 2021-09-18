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
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public abstract class AuditedDbContext : DbContext, IAuditedDbContext
	{
		private readonly ILogger _logger;

		public AuditedDbContext(DbContextOptions options) : base(options)
		{
			if (this is IInfrastructure<IServiceProvider> infrastructure)
			{
				_logger = infrastructure.Instance.GetService<ILogger<AuditedDbContext>>();
			}
		}

		public DbSet<AuditEntry> AuditLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
			=> base.OnModelCreating(modelBuilder.ConfigureAuditLog());

		[SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method", Justification = "Only need to use the async method when an async value generator is used.")]
		public override async Task<Int32> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				var entries = this.GetAuditEntries(GetUsername());
				var normal = entries.Normal();
				AuditLogs.AddRange(normal);
				var result = await base.SaveChangesAsync(true, cancellationToken).ConfigureAwait(true);
				if (entries.Temporary().Any())
				{
					var temporary = entries.Temporary();
					AuditLogs.AddRange(temporary);
					await base.SaveChangesAsync(cancellationToken).ConfigureAwait(true);
				}
				return result;

			}
			catch (DbUpdateException ex)
			{
				_logger?.LogCritical(ex, "{Exception} occurred during SaveChangesAsync(): {Message}", ex.GetType(), ex.Message);
				throw;
			}
		}

		public abstract String GetUsername();
	}
}
