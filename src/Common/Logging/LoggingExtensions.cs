using System;
using Microsoft.Extensions.Logging;
using ProvisionData.GitVersion;

namespace ProvisionData.Logging
{
    public static class LoggingExtensions
    {
        private const String MessageTemplate = "{AssemblyName} {FullSemVer} {CommitDate} {ShortSha}";

        public static void LogVersionInfo<T>(this Serilog.ILogger logger)
        {
            var info = GitVersionInfo.ForAssemblyContaining<T>();
            if (info is null)
                return;

            logger.Information(MessageTemplate, info.AssemblyName, "v" + info.FullSemVer, info.CommitDate, info.ShortSha);
        }

        public static void LogVersionInfo<T>(this ILogger<T> logger)
        {
            var info = GitVersionInfo.ForAssemblyContaining<T>();
            if (info is null)
                return;

            logger.LogInformation(MessageTemplate, info.AssemblyName, "v" + info.FullSemVer, info.CommitDate, info.ShortSha);
        }
    }
}
