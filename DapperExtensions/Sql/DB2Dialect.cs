using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperExtensions.Sql
{
    public class Db2Dialect : SqlDialectBase
    {
        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CAST(IDENTITY_VAL_LOCAL() AS BIGINT) AS \"ID\" FROM SYSIBM.SYSDUMMY1";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            int startValue = ((page - 1) * resultsPerPage) + 1;
            int endValue = (page * resultsPerPage);
            return GetSetSql(sql, startValue, endValue, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            int selectIndex = GetSelectEnd(sql) + 1;
            string orderByClause = GetOrderByClause(sql) ?? "ORDER BY CURRENT_TIMESTAMP";


            string projectedColumns = GetColumnNames(sql).Aggregate(new StringBuilder(), (sb, s) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(GetColumnName("_TEMP", s, null)), sb => sb.ToString());
            string newSql = sql
                .Replace(" " + orderByClause, string.Empty)
                .Insert(selectIndex, $"ROW_NUMBER() OVER(ORDER BY {orderByClause.Substring(9)}) AS {GetColumnName(null, "_ROW_NUMBER", null)}, ");

            string result = $"SELECT {projectedColumns.Trim()} FROM ({newSql}) AS \"_TEMP\" WHERE {GetColumnName("_TEMP", "_ROW_NUMBER", null)} BETWEEN @_pageStartRow AND @_pageEndRow";

            parameters.Add("@_pageStartRow", firstResult);
            parameters.Add("@_pageEndRow", maxResults);
            return result;
        }

        protected string GetOrderByClause(string sql)
        {
            int orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.InvariantCultureIgnoreCase);
            if (orderByIndex == -1)
            {
                return null;
            }

            string result = sql.Substring(orderByIndex).Trim();

            int whereIndex = result.IndexOf(" WHERE ", StringComparison.InvariantCultureIgnoreCase);
            if (whereIndex == -1)
            {
                return result;
            }

            return result.Substring(0, whereIndex).Trim();
        }

        protected int GetFromStart(string sql)
        {
            int selectCount = 0;
            string[] words = sql.Split(' ');
            int fromIndex = 0;
            foreach (string word in words)
            {
                if (word.Equals("SELECT", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount++;
                }

                if (word.Equals("FROM", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount--;
                    if (selectCount == 0)
                    {
                        break;
                    }
                }

                fromIndex += word.Length + 1;
            }

            return fromIndex;
        }

        protected virtual int GetSelectEnd(string sql)
        {
            if (sql.StartsWith("SELECT DISTINCT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 15;
            }

            if (sql.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 6;
            }

            throw new ArgumentException("SQL must be a SELECT statement.", nameof(sql));
        }

        protected virtual IList<string> GetColumnNames(string sql)
        {
            int start = GetSelectEnd(sql);
            int stop = GetFromStart(sql);
            string[] columnSql = sql.Substring(start, stop - start).Split(',');
            var result = new List<string>();
            foreach (string c in columnSql)
            {
                int index = c.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                {
                    result.Add(c.Substring(index + 4).Trim());
                    continue;
                }

                string[] colParts = c.Split('.');
                result.Add(colParts[colParts.Length - 1].Trim());
            }

            return result;
        }
    }
}