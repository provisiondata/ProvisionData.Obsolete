using System.Diagnostics.CodeAnalysis;

// General
[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
[assembly: SuppressMessage("Style", "IDE0011:Add braces", Justification = "<Pending>")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Only when the scope is clear.")]

// Specific

// Unit Testing
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>", Scope = "namespaceanddescendants", Target = "ProvisionData.Common.UnitTests")]
