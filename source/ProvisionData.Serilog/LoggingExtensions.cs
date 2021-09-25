/*******************************************************************************
 * MIT License
 *
 * Copyright 2021 Provision Data Systems Inc.  https://provisiondata.com
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

namespace ProvisionData.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using ProvisionData.GitVersion;

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
