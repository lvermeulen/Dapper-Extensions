using System.Collections.Generic;

namespace DapperExtensions.Sql
{
    public class PostgreSqlDialect : SqlDialectBase
    {
        public override string GetIdentitySql(string tableName)
        {
            return "SELECT LASTVAL() AS Id";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
		{
			int startValue = page * resultsPerPage;
			return GetSetSql(sql, startValue, resultsPerPage, parameters);
		}
		
		public override string GetSetSql(string sql, int pageNumber, int maxResults, IDictionary<string, object> parameters)
		{
			string result = $"{sql} LIMIT @maxResults OFFSET @pageStartRowNbr";
			parameters.Add("@maxResults", maxResults);
			parameters.Add("@pageStartRowNbr", pageNumber * maxResults);
			return result;
		}

        public override string GetColumnName(string prefix, string columnName, string alias)
        {
            return base.GetColumnName(null, columnName, alias).ToLower();
        }

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            return base.GetTableName(schemaName, tableName, alias).ToLower();
        }
    }

}
