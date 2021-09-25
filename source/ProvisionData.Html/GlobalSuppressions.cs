// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0056:Use index operator", Justification = "Using index operators break net48.", Scope = "member", Target = "~M:ProvisionData.Extensions.HtmlToTextExtensions.ConvertTo(HtmlAgilityPack.HtmlNode,System.IO.TextWriter,ProvisionData.Extensions.PreceedingDomTextInfo)")]
[assembly: SuppressMessage("Usage", "CA2249:Consider using 'string.Contains' instead of 'string.IndexOf'", Justification = "Using Contains breaks net48.", Scope = "member", Target = "~M:ProvisionData.Extensions.HtmlToTextExtensions.ConvertTo(HtmlAgilityPack.HtmlNode,System.IO.TextWriter,ProvisionData.Extensions.PreceedingDomTextInfo)")]
