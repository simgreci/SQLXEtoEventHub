﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace xetohub.core
{
    public interface IDatabaseContext : IDisposable
    {
        SqlConnection Connection { get; }
    }
    public class DatabaseContext : IDatabaseContext
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = new SqlConnection(_connectionString);

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                return _connection;
            }
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
    }
}
