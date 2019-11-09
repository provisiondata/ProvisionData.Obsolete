/*******************************************************************************
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

namespace ProvisionData.Specifications
{
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class SpecificationsTests
    {
        [Fact]
        public void All_specification_in_memory()
        {
            var spec = new AllSpecification<User, IUserSpecificationVisitor>();

            var list = new InMemoryRepository().GetUsers(spec);

            list.Count().Should().Be(5);
        }

        [Fact]
        public void And_specification_in_memory()
        {
            var spec = new UserIsAgeOfMajority(18).And(new UserHasGender(Gender.Male));

            var list = new InMemoryRepository().GetUsers(spec);

            list.Count().Should().Be(1);
        }

        [Fact]
        public void Not_specification_in_memory()
        {
            var spec = new UserIsAgeOfMajority(18).Not();

            var list = new InMemoryRepository().GetUsers(spec);

            list.Count().Should().Be(3);
        }

        [Fact]
        public void Or_specification_in_memory()
        {
            var spec = new UserIsAgeOfMajority(18).Or(new UserHasGender(Gender.Male));

            var list = new InMemoryRepository().GetUsers(spec);

            list.Count().Should().Be(4);
        }

        [Fact]
        public void UserIsAgeOfMajority()
        {
            using (var repo = new TestRepository())
            {
                var spec = new UserIsAgeOfMajority(18);

                repo.GetUsers(spec).Count().Should().Be(2);
            }
        }

        [Fact]
        public void UserHasGender()
        {
            using (var repo = new TestRepository())
            {
                var spec = new UserHasGender(Gender.Female);

                repo.GetUsers(spec).Count().Should().Be(2);
            }
        }

        [Fact]
        public void All_specification_in_EF()
        {
            using (var repo = new TestRepository())
            {
                var spec = new AllSpecification<User, IUserSpecificationVisitor>();

                repo.GetUsers(spec).Count().Should().Be(5);
            }
        }

        [Fact]
        public void And_specification_in_EF()
        {
            using (var repo = new TestRepository())
            {
                var spec = new UserIsAgeOfMajority(18).And(new UserHasGender(Gender.Male));

                repo.GetUsers(spec).Count().Should().Be(1);
            }
        }

        [Fact]
        public void Not_specification_in_EF()
        {
            using (var repo = new TestRepository())
            {
                var spec = new UserIsAgeOfMajority(18).Not();

                repo.GetUsers(spec).Count().Should().Be(3);
            }
        }

        [Fact]
        public void Or_specification_in_EF()
        {
            using (var repo = new TestRepository())
            {
                var spec = new UserIsAgeOfMajority(18).Or(new UserHasGender(Gender.Male));

                repo.GetUsers(spec).Count().Should().Be(4);
            }
        }
    }
}
