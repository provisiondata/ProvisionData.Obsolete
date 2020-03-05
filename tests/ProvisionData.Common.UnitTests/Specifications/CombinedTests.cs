﻿/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
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

namespace ProvisionData.UnitTests.Specifications
{
    using System;
    using System.Linq.Expressions;
    using FluentAssertions;
    using ProvisionData.Specifications;
    using Xunit;

    public class Specifications
    {
        internal class IsEven : AbstractSpecification<Int32>
        {
            public override Expression<Func<Int32, Boolean>> Predicate
                => n => (n % 2) == 0;
        }

        internal class IsMultiple : AbstractSpecification<Int32>
        {
            private readonly Int32 _factor;

            public IsMultiple(Int32 factor) => _factor = factor;

            public override Expression<Func<Int32, Boolean>> Predicate
                => n => (n % _factor) == 0;
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(5, false)]
        [InlineData(9, false)]
        [InlineData(10, true)]
        [InlineData(11, false)]
        [InlineData(15, false)]
        [InlineData(20, true)]
        public void Can_be_combined(Int32 input, Boolean expected)
        {
            ISpecification<Int32> isTen = new IsEven().And(new IsMultiple(5));

            isTen.IsSatisfiedBy(input).Should().Be(expected);
        }
    }
}
