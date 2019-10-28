using System;
using FluentAssertions;
using Xunit;

namespace ProvisionData.Extensions
{
    public class HtmlToTextTests
    {
        [Theory]
        [InlineData("<p>Hello, World!</p>", "Hello, World!")]
        [InlineData("<p>Hello<br />World!</p>", "Hello\r\nWorld!")]
        [InlineData("<ul><li>Hello, World!</li></ul>", "* Hello, World!")]
        [InlineData("<ol><li>Hello, World!</li></ol>", "1. Hello, World!")]
        public void Test(String input, String expected)
        {
            input.HtmlToText().Should().Be(expected);
        }
    }
}
