namespace ProvisionData.EntityFrameworkCore.Auditing
{
	using System;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class AuditAttribute : Attribute
	{
		/// <summary>
		/// When applied to a class, it opts the entire class out of auditing.
		/// </summary>
		public AuditAttribute(Boolean ignore = false, Boolean sensitive = false)
		{
			Sensitive = sensitive;
			Ignore = ignore;
		}

		public Boolean Sensitive { get; }
		public Boolean Ignore { get; }
		public Boolean IsSalt { get; }
	}
}
