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
	using ProvisionData.EntityFrameworkCore.Auditing;
	using System;
	using System.Collections.Generic;

	public class FamilyMember
	{
		public static readonly String TypeName = $"{typeof(FamilyMember).FullName}, {typeof(FamilyMember).Assembly.GetName().Name}";

		public FamilyMember() { }

		public FamilyMember(Guid uid, String name, DateTime dateOfBirth)
		{
			Id = uid;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			DateOfBirth = dateOfBirth;
		}

		public Guid Id { get; set; }
		public String Name { get; set; }
		public DateTime DateOfBirth { get; set; }

		public virtual ICollection<String> FavoriteFoods { get; } = new List<String>();

		[Audit(ignore: true)]
		public String Ignored { get; set; } = String.Empty;
		[Audit(sensitive: true)]
		public String Sensitive { get; set; } = String.Empty;
	}

	[Audit]
	public class IgnoredClass
	{
		public Guid Id { get; set; } = Guid.NewGuid();
	}

	public enum Gender
	{
		Unspecified,
		Male,
		Female,
		Undisclosed
	}
}
