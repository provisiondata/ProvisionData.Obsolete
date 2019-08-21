/*******************************************************************************
 * MIT License
 *
 * Copyright 2019 Provision Data Systems Inc.  https://provisiondata.com
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

using System.Text.RegularExpressions;

namespace ProvisionData
{
    public static class Ptn
    {
        public static Regex Integer = new Regex("^[0-9]+$", RegexOptions.Compiled);
        public static Regex Separator = new Regex(@"\s+", RegexOptions.Compiled);
        public static Regex Newline = new Regex(@"\r?\n", RegexOptions.Compiled);
        public static Regex LeadingWS = new Regex(@"^\s+", RegexOptions.Compiled);
        public static Regex TXT = new Regex(@"\sTXT\s", RegexOptions.Compiled);
        public static Regex ORIGIN = new Regex(@"\s$ORIGIN\s", RegexOptions.Compiled);
        public static Regex TTL = new Regex(@"\s$TTL\s", RegexOptions.Compiled);
        public static Regex SOA = new Regex(@"\sSOA\s", RegexOptions.Compiled);
        public static Regex NS = new Regex(@"\sNS\s", RegexOptions.Compiled);
        public static Regex A = new Regex(@"\sA\s", RegexOptions.Compiled);
        public static Regex AAAA = new Regex(@"\sAAAA\s", RegexOptions.Compiled);
        public static Regex CNAME = new Regex(@"\sCNAME\s", RegexOptions.Compiled);
        public static Regex MX = new Regex(@"\sMX\s", RegexOptions.Compiled);
        public static Regex PTR = new Regex(@"\sPTR\s", RegexOptions.Compiled);
        public static Regex SRV = new Regex(@"\sSRV\s", RegexOptions.Compiled);
        public static Regex SPF = new Regex(@"\sSPF\s", RegexOptions.Compiled);
    }
}
