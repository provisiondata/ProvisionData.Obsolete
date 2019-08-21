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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProvisionData
{
    public class NaturalComparer<T> : IComparer<T>
    {
        private static readonly Regex _split = new Regex(@"([0-9]+)", RegexOptions.Compiled);
        private static readonly Regex _replace = new Regex(@"\s+", RegexOptions.Compiled);

        private readonly IComparer<IEnumerable<Object>> _comparer = new EnumerableComparer<Object>();

        private Object[] _convert(IEnumerable<String> str)
        {
            var list = new List<Object>();
            foreach (var s in str)
            {
                if (Int32.TryParse(s, out var result))
                {
                    list.Add(result);
                }
                else
                {
                    list.Add(s);
                }
            }
            return list.ToArray();
        }

        public Int32 Compare(T x, T y)
        {
            var xr = _replace.Replace(x.ToString(), "");
            var xs = _split.Split(xr);
            var xc = _convert(xs);
            var yr = _replace.Replace(y.ToString(), "");
            var ys = _split.Split(yr);
            var yc = _convert(ys);
            return _comparer.Compare(xc, yc);
        }
    }
}
