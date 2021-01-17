﻿using System.Collections.Generic;

namespace DapperExtensions.Sql
{
    public class MySqlDialect : SqlDialectBase
    {
        public override char OpenQuote => '`';

        public override char CloseQuote => '`';

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {            
            int startValue = page * resultsPerPage;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            string result = $"{sql} LIMIT @firstResult, @maxResults";
            parameters.Add("@firstResult", firstResult);
            parameters.Add("@maxResults", maxResults);
            return result;
        }
    }
}