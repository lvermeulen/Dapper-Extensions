using System;
using System.Collections.Generic;
using DapperExtensions.Sql;
using DapperExtensions.Test.Helpers;
using Xunit;

namespace DapperExtensions.Test.Sql
{
    
    public class SqlServerDialectFixture
    {
        public abstract class SqlServerDialectFixtureBase
        {
            protected SqlServerDialect Dialect;

            protected SqlServerDialectFixtureBase()
            {
	            Dialect = new SqlServerDialect();
            }
        }
        
        public class Properties : SqlServerDialectFixtureBase
        {
            [Fact]
            public void CheckSettings()
            {
                Assert.Equal('[', Dialect.OpenQuote);
                Assert.Equal(']', Dialect.CloseQuote);
                Assert.Equal(";" + Environment.NewLine, Dialect.BatchSeperator);
                Assert.Equal('@', Dialect.ParameterPrefix);
                Assert.True(Dialect.SupportsMultipleStatements);
            }
        }

        
        public class GetPagingSqlMethod : SqlServerDialectFixtureBase
        {
            [Fact]
            public void NullSql_ThrowsException()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql(null, 0, 10, new Dictionary<string, object>()));
                Assert.Equal("sql", ex.ParamName);
                Assert.Contains("cannot be null", ex.Message);
            }

            [Fact]
            public void EmptySql_ThrowsException()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql(string.Empty, 0, 10, new Dictionary<string, object>()));
                Assert.Equal("sql", ex.ParamName);
                Assert.Contains("cannot be null", ex.Message);
            }

            [Fact]
            public void NullParameters_ThrowsException()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql("SELECT [schema].[column] FROM [schema].[table]", 0, 10, null));
                Assert.Equal("parameters", ex.ParamName);
                Assert.Contains("cannot be null", ex.Message);
            }

            [Fact]
            public void Select_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
                string result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table]", 0, 10, parameters);
                Assert.Equal(sql, result);
                Assert.Single(parameters);
                Assert.Equal(parameters["@_pageStartRow"], 1);
            }

            [Fact]
            public void SelectDistinct_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT DISTINCT ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
                string result = Dialect.GetPagingSql("SELECT DISTINCT [column] FROM [schema].[table]", 0, 10, parameters);
                Assert.Equal(sql, result);
                Assert.Single(parameters);
                Assert.Equal(parameters["@_pageStartRow"], 1);
            }

            [Fact]
            public void SelectOrderBy_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT TOP(10) [_proj].[column] FROM (SELECT ROW_NUMBER() OVER(ORDER BY [column] DESC) AS [_row_number], [column] FROM [schema].[table]) [_proj] WHERE [_proj].[_row_number] >= @_pageStartRow ORDER BY [_proj].[_row_number]";
                string result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table] ORDER BY [column] DESC", 0, 10, parameters);
                Assert.Equal(sql, result);
                Assert.Single(parameters);
                Assert.Equal(parameters["@_pageStartRow"], 1);
            }
        }

        
        public class GetOrderByClauseMethod : SqlServerDialectFixtureBase
        {
            [Fact]
            public void NoOrderBy_Returns()
            {
                string result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table");
                Assert.Null(result);
            }

            [Fact]
            public void OrderBy_ReturnsItemsAfterClause()
            {
                string result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC");
                Assert.Equal("ORDER BY Column1 ASC, Column2 DESC", result);
            }

            [Fact]
            public void OrderByWithWhere_ReturnsOnlyOrderBy()
            {
                string result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC WHERE Column1 = 'value'");
                Assert.Equal("ORDER BY Column1 ASC, Column2 DESC", result);
            }
        }
    }
}