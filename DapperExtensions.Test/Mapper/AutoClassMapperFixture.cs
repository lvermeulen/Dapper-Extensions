using System;
using System.Linq;
using DapperExtensions.Mapper;
using Xunit;

namespace DapperExtensions.Test.Mapper
{
    //
    public class AutoClassMapperFixture
    {
        //
        public class AutoClassMapperTableName
        {
            [Fact]
            public void Constructor_ReturnsProperName()
            {
                var m = GetMapper<Foo>();
                Assert.Equal("Foo", m.TableName);
            }

            [Fact]
            public void SettingTableName_ReturnsProperName()
            {
                var m = GetMapper<Foo>();
                m.Table("Barz");
                Assert.Equal("Barz", m.TableName);
            }

            [Fact]
            public void Sets_IdPropertyToKeyWhenFirstProperty()
            {
                var m = GetMapper<IdIsFirst>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.True(map.ColumnName == "Id");
            }

            [Fact]
            public void Sets_IdPropertyToKeyWhenFoundInClass()
            {
                var m = GetMapper<IdIsSecond>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.True(map.ColumnName == "Id");
            }

            [Fact]
            public void Sets_IdFirstPropertyEndingInIdWhenNoIdPropertyFound()
            {
                var m = GetMapper<IdDoesNotExist>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.True(map.ColumnName == "SomeId");
            }
            
            private AutoClassMapper<T> GetMapper<T>() where T : class
            {
                return new AutoClassMapper<T>();
            }
        }

        
        public class CustomAutoMapperTableName
        {
            [Fact]
            public void ReturnsProperPluralization()
            {
                var m = GetMapper<Foo>();
                Assert.Equal("Foo", m.TableName);
            }

            [Fact]
            public void ReturnsProperResultsForExceptions()
            {
                var m = GetMapper<Foo2>();
                Assert.Equal("TheFoo", m.TableName);
            }

            private CustomAutoMapper<T> GetMapper<T>() where T : class
            {
                return new CustomAutoMapper<T>();
            }

            public class CustomAutoMapper<T> : AutoClassMapper<T> where T : class
            {
                public override void Table(string tableName)
                {
                    if (tableName.Equals("Foo2", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TableName = "TheFoo";
                    }
                    else
                    {
                        base.Table(tableName);
                    }
                }
            }
        }

        private class Foo
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class Foo2
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }


        private class IdIsFirst
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class IdIsSecond
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }

        private class IdDoesNotExist
        {
            public Guid SomeId { get; set; }
            public Guid ParentId { get; set; }
        }
    }
}
