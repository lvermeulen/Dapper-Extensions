﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperExtensions.Sql
{
    public interface ISqlDialect
    {
        char OpenQuote { get; }
        char CloseQuote { get; }
        string BatchSeperator { get; }
        bool SupportsMultipleStatements { get; }
        char ParameterPrefix { get; }
        string EmptyExpression { get; }
        string GetTableName(string schemaName, string tableName, string alias);
        string GetColumnName(string prefix, string columnName, string alias);
        string GetIdentitySql(string tableName);
        string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters);
        string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters);
        bool IsQuoted(string value);
        string QuoteString(string value);
    }

    public abstract class SqlDialectBase : ISqlDialect
    {
        public virtual char OpenQuote => '"';

        public virtual char CloseQuote => '"';

        public virtual string BatchSeperator => ";" + Environment.NewLine;

        public virtual bool SupportsMultipleStatements => true;

        public virtual char ParameterPrefix => '@';

        public string EmptyExpression => "1=1";

        public virtual string GetTableName(string schemaName, string tableName, string alias)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "tableName cannot be null or empty.");
            }

            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                result.AppendFormat(QuoteString(schemaName) + ".");
            }

            result.AppendFormat(QuoteString(tableName));

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }
            return result.ToString();
        }

        public virtual string GetColumnName(string prefix, string columnName, string alias)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException(nameof(columnName), "columnName cannot be null or empty.");
            }

            var result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                result.AppendFormat(QuoteString(prefix) + ".");
            }

            result.AppendFormat(QuoteString(columnName));

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }

            return result.ToString();
        }

        public abstract string GetIdentitySql(string tableName);
        public abstract string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters);
        public abstract string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters);

        public virtual bool IsQuoted(string value)
        {
            if (value.Trim()[0] == OpenQuote)
            {
                return value.Trim().Last() == CloseQuote;
            }

            return false;
        }

        public virtual string QuoteString(string value)
        {
            if (IsQuoted(value) || value == "*")
            {
                return value;
            }
            return $"{OpenQuote}{value.Trim()}{CloseQuote}";
        }

        public virtual string UnQuoteString(string value)
        {
            return IsQuoted(value) ? value.Substring(1, value.Length - 2) : value;
        }
    }
}
