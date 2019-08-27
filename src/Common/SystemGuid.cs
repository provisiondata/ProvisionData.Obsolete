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

using System;
using System.Diagnostics;

namespace ProvisionData
{

    [DebuggerNonUserCode]
    public static class SystemGuid
    {
        private static Func<Guid> _getGuid = GetGuidInternal;

        [DebuggerNonUserCode]
        public static void GuidIs(Guid guid) => _getGuid = () => guid;

        [DebuggerNonUserCode]
        public static void GuidIs(String guid)
        {
            var g = Guid.Parse(guid);
            _getGuid = () => g;
        }

        [DebuggerNonUserCode]
        public static Guid NewGuid() => _getGuid?.Invoke() ?? GetGuidInternal();

        [DebuggerNonUserCode]
        public static void Reset() => _getGuid = GetGuidInternal;

        private static Guid GetGuidInternal() => CombGuid.NewGuid();
    }
}
