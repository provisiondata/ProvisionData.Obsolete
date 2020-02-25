using System.Diagnostics.CodeAnalysis;

// General
[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
[assembly: SuppressMessage("Style", "IDE0011:Add braces", Justification = "<Pending>")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Only when the scope is clear.")]

// Specific

// Unit Testing
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not necessary for Unit Test names", Scope = "NamespaceAndDescendants", Target = "ProvisionData.UnitTests")]
[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not necessary for Unit Test names", Scope = "NamespaceAndDescendants", Target = "ProvisionData.Specifications.IntegrationTests")]
[assembly: SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not necessary for Unit Test names", Scope = "NamespaceAndDescendants", Target = "ProvisionData.UnitTests")]
[assembly: SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "Not necessary for Unit Test names", Scope = "NamespaceAndDescendants", Target = "ProvisionData.Specifications.IntegrationTests")]
