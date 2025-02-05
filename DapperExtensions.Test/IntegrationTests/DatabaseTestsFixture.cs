﻿//using DapperExtensions.Sql;

namespace DapperExtensions.Test.IntegrationTests
{
    public class DatabaseTestsFixture
    {/*
        public class PredicateTests : DatabaseConnection
        {
            [Fact]
            public void Eq_EnumerableType_GeneratesAndRunsProperSql()
            {
                Person p1 = new Person { Active = true, FirstName = "Alpha", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Person p2 = new Person { Active = true, FirstName = "Beta", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Person p3 = new Person { Active = true, FirstName = "Gamma", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Impl.Insert<Person>(Connection, new[] { p1, p2, p3 }, null, null);

                var predicate = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, new[] { "Alpha", "Gamma" });
                var result = Impl.GetList<Person>(Connection, predicate, null, null, null, true);
                Assert.Equal(2, result.Count());
                Assert.True(result.Any(r => r.FirstName == "Alpha"));
                Assert.True(result.Any(r => r.FirstName == "Gamma"));
            }

            [Fact]
            public void Exists_GeneratesAndRunsProperSql()
            {
                Person p1 = new Person { Active = true, FirstName = "Alpha", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Person p2 = new Person { Active = true, FirstName = "Beta", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Person p3 = new Person { Active = true, FirstName = "Gamma", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Impl.Insert<Person>(Connection, new[] { p1, p2, p3 }, null, null);

                Animal a1 = new Animal { Name = "Gamma" };
                Animal a2 = new Animal { Name = "Beta" };
                Impl.Insert<Animal>(Connection, new[] { a1, a2 }, null, null);

                var subPredicate = Predicates.Property<Person, Animal>(p => p.FirstName, Operator.Eq, a => a.Name);
                var predicate = Predicates.Exists<Animal>(subPredicate);

                var result = Impl.GetList<Person>(Connection, predicate, null, null, null, true);
                Assert.Equal(2, result.Count());
                Assert.True(result.Any(r => r.FirstName == "Beta"));
                Assert.True(result.Any(r => r.FirstName == "Gamma"));
            }

            [Fact]
            public void Between_GeneratesAndRunsProperSql()
            {
                Person p1 = new Person { Active = true, FirstName = "Alpha", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Person p2 = new Person { Active = true, FirstName = "Beta", LastName = "Bar", DateCreated = DateTime.UtcNow };
                Person p3 = new Person { Active = true, FirstName = "Gamma", LastName = "Omega", DateCreated = DateTime.UtcNow };
                Impl.Insert<Person>(Connection, new[] { p1, p2, p3 }, null, null);

                var pred = Predicates.Between<Person>(p => p.LastName,
                                                      new BetweenValues { Value1 = "Aaa", Value2 = "Bzz" });
                var result = Impl.GetList<Person>(Connection, pred, null, null, null, true).ToList();
                Assert.Equal(2, result.Count);
                Assert.Equal("Alpha", result[0].FirstName);
                Assert.Equal("Beta", result[1].FirstName);
            }
        }

        public class CustomMapperTests : DatabaseConnection
        {
            [Fact]
            public void GeneratesAndRunsProperSql()
            {
                Impl = new DapperExtensions.DapperExtensionsImpl(typeof(CustomMapper), TestHelpers.GetGenerator());
                Foo f = new Foo { FirstName = "Foo", LastName = "Bar", DateOfBirth = DateTime.UtcNow.AddYears(-20) };
                Impl.Insert(Connection, f, null, null);
            }
        }*/
    }
}