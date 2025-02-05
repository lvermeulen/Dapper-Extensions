﻿using System;
using System.Collections.Generic;
using System.Data;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;

namespace DapperExtensions
{
    public interface IDatabase : IDisposable
    {
        bool HasActiveTransaction { get; }
        IDbConnection Connection { get; }
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
        void RunInTransaction(Action action);
        T RunInTransaction<T>(Func<T> func);
        T Get<T>(dynamic id, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        T Get<T>(dynamic id, int? commandTimeout = null) where T : class;
        void Insert<T>(IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        void Insert<T>(IEnumerable<T> entities, int? commandTimeout = null) where T : class;
        dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        dynamic Insert<T>(T entity, int? commandTimeout = null) where T : class;
        bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout = null, bool ignoreAllKeyProperties = false) where T : class;
        bool Update<T>(T entity, int? commandTimeout = null, bool ignoreAllKeyProperties = false) where T : class;
        bool Delete<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        bool Delete<T>(T entity, int? commandTimeout = null) where T : class;
        bool Delete<T>(object predicate, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        bool Delete<T>(object predicate, int? commandTimeout = null) where T : class;
        IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetList<T>(object predicate = null, IList<ISort> sort = null, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;
        IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, int? commandTimeout, bool buffered) where T : class;        
        int Count<T>(object predicate, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        int Count<T>(object predicate, int? commandTimeout = null) where T : class;
        IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout = null);
        IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout = null);
        void ClearCache();
        Guid GetNextGuid();
        IClassMapper GetMap<T>() where T : class;
    }

    public class Database : IDatabase
    {
        private readonly IDapperImplementor _dapper;

        private IDbTransaction _transaction;

        public Database(IDbConnection connection, ISqlGenerator sqlGenerator)
        {
            _dapper = new DapperImplementor(sqlGenerator);
            Connection = connection;
            
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public bool HasActiveTransaction => _transaction != null;

        public IDbConnection Connection { get; }

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
            {
	            _transaction?.Rollback();

                Connection.Close();
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = Connection.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        public void RunInTransaction(Action action)
        {
            BeginTransaction();
            try
            {
                action();
                Commit();
            }
            catch (Exception)
            {
                if (HasActiveTransaction)
                {
                    Rollback();
                }

                throw;
            }
        }

        public T RunInTransaction<T>(Func<T> func)
        {
            BeginTransaction();
            try
            {
                var result = func();
                Commit();
                return result;
            }
            catch (Exception)
            {
                if (HasActiveTransaction)
                {
                    Rollback();
                }

                throw;
            }
        }
        
        public T Get<T>(dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return (T)_dapper.Get<T>(Connection, id, transaction, commandTimeout);
        }

        public T Get<T>(dynamic id, int? commandTimeout) where T : class
        {
            return (T)_dapper.Get<T>(Connection, id, _transaction, commandTimeout);
        }

        public void Insert<T>(IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            _dapper.Insert(Connection, entities, transaction, commandTimeout);
        }

        public void Insert<T>(IEnumerable<T> entities, int? commandTimeout) where T : class
        {
            _dapper.Insert(Connection, entities, _transaction, commandTimeout);
        }

        public dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Insert(Connection, entity, transaction, commandTimeout);
        }

        public dynamic Insert<T>(T entity, int? commandTimeout) where T : class
        {
            return _dapper.Insert(Connection, entity, _transaction, commandTimeout);
        }

        public bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout, bool ignoreAllKeyProperties) where T : class
        {
            return _dapper.Update(Connection, entity, transaction, commandTimeout, ignoreAllKeyProperties);
        }

        public bool Update<T>(T entity, int? commandTimeout, bool ignoreAllKeyProperties) where T : class
        {
            return _dapper.Update(Connection, entity, _transaction, commandTimeout, ignoreAllKeyProperties);
        }

        public bool Delete<T>(T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Delete(Connection, entity, transaction, commandTimeout);
        }

        public bool Delete<T>(T entity, int? commandTimeout) where T : class
        {
            return _dapper.Delete(Connection, entity, _transaction, commandTimeout);
        }

        public bool Delete<T>(object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Delete<T>(Connection, predicate, transaction, commandTimeout);
        }

        public bool Delete<T>(object predicate, int? commandTimeout) where T : class
        {
            return _dapper.Delete<T>(Connection, predicate, _transaction, commandTimeout);
        }

        public IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetList<T>(Connection, predicate, sort, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetList<T>(Connection, predicate, sort, _transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetPage<T>(Connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetPage<T>(Connection, predicate, sort, page, resultsPerPage, _transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetSet<T>(Connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, int? commandTimeout, bool buffered) where T : class
        {
            return _dapper.GetSet<T>(Connection, predicate, sort, firstResult, maxResults, _transaction, commandTimeout, buffered);
        }

        public int Count<T>(object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            return _dapper.Count<T>(Connection, predicate, transaction, commandTimeout);
        }

        public int Count<T>(object predicate, int? commandTimeout) where T : class
        {
            return _dapper.Count<T>(Connection, predicate, _transaction, commandTimeout);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            return _dapper.GetMultiple(Connection, predicate, transaction, commandTimeout);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout)
        {
            return _dapper.GetMultiple(Connection, predicate, _transaction, commandTimeout);
        }

        public void ClearCache()
        {
            _dapper.SqlGenerator.Configuration.ClearCache();
        }

        public Guid GetNextGuid()
        {
            return _dapper.SqlGenerator.Configuration.GetNextGuid();
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return _dapper.SqlGenerator.Configuration.GetMap<T>();
        }
    }
}