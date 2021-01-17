using System;
using System.Collections.Generic;
using DapperExtensions.Test.Data;
using Xunit;
using Xunit.Abstractions;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    
    public class TimerFixture
    {
        private static int s_cnt = 1000;

        public class InsertTimes : SqlServerBaseFixture
        {
	        private readonly ITestOutputHelper _testOutputHelper;

	        public InsertTimes(ITestOutputHelper testOutputHelper)
	        {
		        _testOutputHelper = testOutputHelper;
	        }

	        [Fact]
            public void IdentityKey_UsingEntity()
            {
                var p = new Person
                               {
                                   FirstName = "FirstName",
                                   LastName = "LastName",
                                   DateCreated = DateTime.Now,
                                   Active = true
                               };
                Db.Insert(p);
                var start = DateTime.Now;
                var ids = new List<int>();
                for (int i = 0; i < s_cnt; i++)
                {
                    var p2 = new Person
                                    {
                                        FirstName = "FirstName" + i,
                                        LastName = "LastName" + i,
                                        DateCreated = DateTime.Now,
                                        Active = true
                                    };
                    Db.Insert(p2);
                    ids.Add(p2.Id);
                }

                double total = DateTime.Now.Subtract(start).TotalMilliseconds;
                _testOutputHelper.WriteLine("Total Time:" + total);
                _testOutputHelper.WriteLine("Average Time:" + total / s_cnt);
            }

            [Fact]
            public void IdentityKey_UsingReturnValue()
            {
                var p = new Person
                               {
                                   FirstName = "FirstName",
                                   LastName = "LastName",
                                   DateCreated = DateTime.Now,
                                   Active = true
                               };
                Db.Insert(p);
                var start = DateTime.Now;
                var ids = new List<int>();
                for (int i = 0; i < s_cnt; i++)
                {
                    var p2 = new Person
                                    {
                                        FirstName = "FirstName" + i,
                                        LastName = "LastName" + i,
                                        DateCreated = DateTime.Now,
                                        Active = true
                                    };
                    var id = Db.Insert(p2);
                    ids.Add(id);
                }

                double total = DateTime.Now.Subtract(start).TotalMilliseconds;
                _testOutputHelper.WriteLine("Total Time:" + total);
                _testOutputHelper.WriteLine("Average Time:" + total / s_cnt);
            }

            [Fact]
            public void GuidKey_UsingEntity()
            {
                var a = new Animal { Name = "Name" };
                Db.Insert(a);
                var start = DateTime.Now;
                var ids = new List<Guid>();
                for (int i = 0; i < s_cnt; i++)
                {
                    var a2 = new Animal { Name = "Name" + i };
                    Db.Insert(a2);
                    ids.Add(a2.Id);
                }

                double total = DateTime.Now.Subtract(start).TotalMilliseconds;
                _testOutputHelper.WriteLine("Total Time:" + total);
                _testOutputHelper.WriteLine("Average Time:" + total / s_cnt);
            }

            [Fact]
            public void GuidKey_UsingReturnValue()
            {
                var a = new Animal { Name = "Name" };
                Db.Insert(a);
                var start = DateTime.Now;
                var ids = new List<Guid>();
                for (int i = 0; i < s_cnt; i++)
                {
                    var a2 = new Animal { Name = "Name" + i };
                    var id = Db.Insert(a2);
                    ids.Add(id);
                }

                double total = DateTime.Now.Subtract(start).TotalMilliseconds;
                _testOutputHelper.WriteLine("Total Time:" + total);
                _testOutputHelper.WriteLine("Average Time:" + total / s_cnt);
            }

            [Fact]
            public void AssignKey_UsingEntity()
            {
                var ca = new Car { Id = string.Empty.PadLeft(15, '0'), Name = "Name" };
                Db.Insert(ca);
                var start = DateTime.Now;
                var ids = new List<string>();
                for (int i = 0; i < s_cnt; i++)
                {
                    string key = (i + 1).ToString().PadLeft(15, '0');
                    var ca2 = new Car { Id = key, Name = "Name" + i };
                    Db.Insert(ca2);
                    ids.Add(ca2.Id);
                }

                double total = DateTime.Now.Subtract(start).TotalMilliseconds;
                _testOutputHelper.WriteLine("Total Time:" + total);
                _testOutputHelper.WriteLine("Average Time:" + total / s_cnt);
            }

            [Fact]
            public void AssignKey_UsingReturnValue()
            {
                var ca = new Car { Id = string.Empty.PadLeft(15, '0'), Name = "Name" };
                Db.Insert(ca);
                var start = DateTime.Now;
                var ids = new List<string>();
                for (int i = 0; i < s_cnt; i++)
                {
                    string key = (i + 1).ToString().PadLeft(15, '0');
                    var ca2 = new Car { Id = key, Name = "Name" + i };
                    var id = Db.Insert(ca2);
                    ids.Add(id);
                }

                double total = DateTime.Now.Subtract(start).TotalMilliseconds;
                _testOutputHelper.WriteLine("Total Time:" + total);
                _testOutputHelper.WriteLine("Average Time:" + total / s_cnt);
            }
        }
    }
}