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
	using Microsoft.EntityFrameworkCore.ChangeTracking;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.Json;

	internal class AuditEntryDto
	{
		public AuditEntryDto(EntityEntry entry)
		{
			Entry = entry;
		}

		public EntityEntry Entry { get; }
		public DateTime DateTime { get; set; } = DateTime.UtcNow;
		public String Username { get; set; }
		public AuditAction Action { get; set; }
		public String Table { get; set; }
		public List<String> Columns { get; } = new List<String>();
		public Dictionary<String, Object> KeyValues { get; } = new Dictionary<String, Object>();
		public Dictionary<String, Object> Old { get; } = new Dictionary<String, Object>();
		public Dictionary<String, Object> New { get; } = new Dictionary<String, Object>();
		public Type Type { get; set; }
		public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
		public Boolean HasTemporaryProperties => TemporaryProperties.Any();

		public AuditEntry ToDto() => new()
		{
			DateTime = DateTime,
			Username = Username,
			Action = Action,
			Table = Table,
			Changed = Columns.Count == 0 ? null : JsonSerializer.Serialize(Columns),
			PrimaryKey = JsonSerializer.Serialize(KeyValues),
			Previous = Old.Count == 0 ? null : JsonSerializer.Serialize(Old),
			Current = New.Count == 0 ? null : JsonSerializer.Serialize(New),
			Type = Type.AssemblyQualifiedName
		};
	}
}
