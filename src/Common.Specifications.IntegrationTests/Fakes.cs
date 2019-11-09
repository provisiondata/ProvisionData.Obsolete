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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    [Flags]
    internal enum Gender
    {
        Unknown = 0,
        Female = 1,
        Male = 2,
        Other = 4
    }

    internal class User
    {
        public User(String name, DateTime dateOfBirth, Gender gender)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        public String Name { get; }
        public DateTime DateOfBirth { get; }
        public Gender Gender { get; }
    }

    internal class InMemoryRepository
    {
        public static readonly IReadOnlyCollection<User> Users = new List<User>()
        {
            new User("Doug", new DateTime(1974, 10, 16), Gender.Male),
            new User("Charmaine", new DateTime(1975, 11, 11), Gender.Female),
            new User("Shyloh", new DateTime(2007, 10, 15), Gender.Male),
            new User("Piper", new DateTime(2008,5,19), Gender.Female),
            new User("Geordi", new DateTime(2011,9,14), Gender.Male)
        };

        public IEnumerable<User> GetUsers(ISpecification<User> spec)
        {
            foreach (var user in Users)
            {
                if (spec.IsSatisfiedBy(user))
                    yield return user;
            }
        }
    }

    internal interface IUserSpecification : ISpecification<User, IUserSpecificationVisitor>
    {
    }

    internal interface IUserSpecificationVisitor : ISpecificationVisitor<IUserSpecificationVisitor, User>
    {
        void Visit(UserIsAgeOfMajority spec);
        void Visit(UserHasGender userHasGender);
    }

    internal class UserIsAgeOfMajority : IUserSpecification
    {
        public UserIsAgeOfMajority(Int32 ageOfMajority)
            : this(DateTime.UtcNow.Date.AddYears(0 - ageOfMajority))
        {
        }

        public UserIsAgeOfMajority(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public DateTime DateTime { get; }

        public void Accept(IUserSpecificationVisitor visitor) => visitor.Visit(this);

        public Boolean IsSatisfiedBy(User entity)
            => entity.DateOfBirth <= DateTime;
    }

    internal class UserHasGender : IUserSpecification
    {
        public UserHasGender(Gender gender) => Gender = gender;

        public Gender Gender { get; }

        public void Accept(IUserSpecificationVisitor visitor) => visitor.Visit(this);

        public Boolean IsSatisfiedBy(User entity)
            => (Gender & entity.Gender) != 0;
    }

    internal class EfUser
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }
    }

    internal class UserExpressionVisitor : ExpressionVisitor<IUserSpecificationVisitor, User, EfUser>, IUserSpecificationVisitor
    {
        public void Visit(UserIsAgeOfMajority spec)
        {
            var cutoff = spec.DateTime;
            Expr = dto => dto.DateOfBirth <= cutoff;
        }

        public void Visit(UserHasGender spec)
        {
            var gender = spec.Gender;
            Expr = dto => (dto.Gender & gender) != 0;
        }

        public override Expression<Func<EfUser, Boolean>> ConvertSpecToExpression(ISpecification<User, IUserSpecificationVisitor> spec)
        {
            var visitor = new UserExpressionVisitor();
            spec.Accept(visitor);
            return visitor.Expr;
        }
    }

    internal sealed class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }

        public DbSet<EfUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<EfUser>(x => x.HasData(InMemoryRepository.Users.Select(x => new EfUser() { Id = Guid.NewGuid(), Name = x.Name, DateOfBirth = x.DateOfBirth, Gender = x.Gender })));
    }

    internal sealed class TestRepository : IDisposable
    {
        private readonly SqlConnection _connection = new SqlConnection("Server=.;Database=SpecificationsTest;Integrated Security=true;MultipleActiveResultSets=True;");
        private readonly DbContextOptions<TestContext> _options;

        public TestRepository()
        {
            _connection.Open();

            _options = new DbContextOptionsBuilder<TestContext>()
                    .UseSqlServer(_connection)
                    .Options;

            using (var context = new TestContext(_options))
            {
                context.Database.EnsureCreated();
            }
        }

        public IEnumerable<User> GetUsers(ISpecification<User, IUserSpecificationVisitor> spec)
        {
            var visitor = new UserExpressionVisitor();
            spec.Accept(visitor);
            var expression = visitor.Expr;

            using (var db = new TestContext(_options))
            {
                foreach (var dto in db.Users.Where(expression).AsEnumerable())
                {
                    yield return ConvertPersistenceToDomain(dto);
                }
            }
        }

        private static User ConvertPersistenceToDomain(EfUser entity)
            => new User(entity.Name, entity.DateOfBirth, entity.Gender);

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
