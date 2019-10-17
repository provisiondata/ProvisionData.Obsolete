using System.IO;
using FluentAssertions;
using Serilog;
using Xunit;

namespace ProvisionData.Logging
{
    public class LoggingExtensionsTests_Serilog
    {
        [Fact]
        public void Serilog_works()
        {
            var messages = new StringWriter();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TextWriter(messages)
                .WriteTo.Debug()
                .CreateLogger();

            Log.Logger.LogVersionInfo<LoggingExtensionsTests_Serilog>();

            messages.ToString().Should().NotBeNullOrWhiteSpace();
        }
    }
}
