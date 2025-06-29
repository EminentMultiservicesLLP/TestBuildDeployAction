using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CommonLayer;
using CommonLayer.Extensions;

namespace CommonDataLayer.DataAccess
{
    /// <summary>
    /// DBHelper class enables to execute Sql Objects for the connection parameters specified into web.config or App.config file.
    /// </summary>
    public class DBHelper :IDisposable
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(DBHelper));

        #region "Private Variables"
        private ConnectionManager _connectionManager = null;
        private CommandBuilder _commandBuilder = null;
        private DataAdapterManager _dbAdapterManager = new DataAdapterManager();
        //private static DBHelper _dbHelper = null;
        private IDbConnection _connection = null;
        private IDbTransaction _transaction = null;
        private string _providerName = string.Empty;
        private AssemblyProvider _assemblyProvider = null;
        #endregion

        #region "Constructor Methods"

        /// <summary>
        /// This Constructor creates instance of the class for defaultConnection 
        /// </summary>
        public DBHelper()
        {
            _connectionManager = new ConnectionManager();
            _commandBuilder = new CommandBuilder();
            _dbAdapterManager = new DataAdapterManager();
            //_connection = _connectionManager.GetConnection();
            _providerName = _connectionManager.ProviderName;
            _assemblyProvider = new AssemblyProvider(_providerName);
        }

        /// <summary>
        /// This constructor should be used for creation of the instance for the specified app settings connection name
        /// </summary>
        /// <param name="connectionName">App Setting's connection name</param>
        public DBHelper(string connectionName)
        {
            _connectionManager = new ConnectionManager(connectionName);
            _commandBuilder = new CommandBuilder(Configuration.GetProviderName(connectionName));
            _dbAdapterManager = new DataAdapterManager(Configuration.GetProviderName(connectionName));
            //_connection = _connectionManager.GetConnection();
            _providerName = _connectionManager.ProviderName;
            _assemblyProvider = new AssemblyProvider(_providerName);
        }

        /// <summary>
        /// Constructor creates instance of the class for the specified connection string and provider name
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="providerName">Provider name</param>
        public DBHelper(string connectionString, string providerName)
        {
            _connectionManager = new ConnectionManager(connectionString, providerName);
            _commandBuilder = new CommandBuilder(providerName);
            _dbAdapterManager = new DataAdapterManager(providerName);
            //_connection = _connectionManager.GetConnection();
            _providerName = _connectionManager.ProviderName;
            _assemblyProvider = new AssemblyProvider(_providerName);
        }

        public object ExecuteNonQueryForOutParameter<T>(object createPaperMaster, DBParameterCollection paramCollection, CommandType storedProcedure, string v)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Methods returning instance of singleton class"

        /// <summary>
        /// Creates and reuturns instance of DBHelper class for the default connection        
        /// </summary>
        /// <returns>Instance of DBHelper class</returns>
        //public static DBHelper GetInstance()
        //{
        //    if (_dbHelper == null)
        //        _dbHelper = new DBHelper();

        //    return _dbHelper;
        //}

        /// <summary>
        ///  Creates and reuturns instance of DBHelper class for the connection string key specified into App.config/ web.config file
        /// </summary>
        /// <param name="connectionName">Connection key specified into web.config/ App.config</param>
        /// <returns>Instance of DBHelper class</returns>
        //public static DBHelper GetInstance(string connectionName)
        //{
        //    if (_dbHelper == null)
        //        _dbHelper = new DBHelper(connectionName);

        //    return _dbHelper;
        //}

        /// <summary>
        /// Creates and reuturns instance of DBHelper class for the coonection string and provider name specified as constructor parameter
        /// </summary>
        /// <param name="connectionString">ConnectionString</param>
        /// <param name="providerName">Provider Name</param>
        /// <returns>Instance of DBHelper class</returns>
        //public static DBHelper GetInstance(string connectionString, string providerName)
        //{
        //    if (_dbHelper == null)
        //        _dbHelper = new DBHelper(connectionString, providerName);

        //    return _dbHelper;
        //}
        #endregion

        #region "Transaction Methods"        

        /// <summary>
        /// Gets the database transaction
        /// After successful execution of command call CommitTransaction(transaction)
        /// In case of failure call RollbackTransaction(transaction)
        /// </summary>
        public IDbTransaction BeginTransaction()
        {   _transaction = GetConnObject().BeginTransaction();
            return _transaction;
        }


        /// <summary>
        /// Commits changes to the database
        /// </summary>
        /// <param name="transaction">Database Transcation to be committed</param>
        public void CommitTransaction(IDbTransaction transaction)
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception err)
            {
                Loggger.LogError("Error in DBHelper class CommitTransaction method :" + err.StackTrace);
                throw err;
            }           
        }

        public void CommitTransaction()
        {
            try
            {
                if(_transaction.IsNotNull()) _transaction.Commit();
            }
            catch (Exception err)
            {
                Loggger.LogError("Error in DBHelper class CommitTransaction method :" + err.StackTrace);
                throw err;
            }
        }


        /// <summary>
        /// Rollback changes to the database
        /// </summary>
        /// <param name="transaction">Database Transaction to be rolled back</param>
        public void RollbackTransaction(IDbTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception err)
            {
                Loggger.LogError("Error in DBHelper class RollbackTransaction method :" + err.StackTrace);
                throw err;
            }           
        }

        public void RollbackTransaction()
        {
            try
            {
                if (_transaction.IsNotNull()) _transaction.Rollback();
            }
            catch (Exception err)
            {
                Loggger.LogError("Error in DBHelper class RollbackTransaction method :" + err.StackTrace);
                throw err;
            }
        }


        /// <summary>
        /// Gets Connection String
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionManager.ConnectionString;
            }
        }

        public string Database
        {
            get
            {
                //IDbConnection connection = AssemblyProvider.GetInstance(Provider).Factory.CreateConnection();
                IDbConnection connection = _assemblyProvider.Factory.CreateConnection();
                connection.ConnectionString = ConnectionString;
                return connection.Database;
            }
        }


        /// <summary>
        /// Gets Provider Name
        /// </summary>
        public string Provider
        {
            get
            {
                return _providerName;
            }
        }



        #endregion

        #region "Execute Scalar"       
        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, CommandType commandType)
        {            
            return ExecuteScalar(commandText, _transaction, commandType);           
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <param name="commandType">Text or Stored Procedure</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, IDbTransaction transaction, CommandType commandType)
        {
            return ExecuteScalar(commandText, new DBParameterCollection(), transaction, commandType);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Parameter to be associated</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameter param, CommandType commandType)
        {           
            return ExecuteScalar(commandText, param, _transaction, commandType);            
        }


        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Database parameter</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <param name="commandType">Text or Stored Procedure</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameter param, IDbTransaction transaction, CommandType commandType)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteScalar(commandText, paramCollection, transaction, commandType);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameterCollection paramCollection, CommandType commandType)
        {
            return ExecuteScalar(commandText, paramCollection, _transaction, commandType);            
        }

        public IDbCommand ExecuteScalar(string commandText, DBParameterCollection paramCollection, CommandType commandType, out object result)
        {
            IDbConnection connection = _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            try
            {
                result = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in DBHelper class ExecuteScalar method :", ex);
                throw ex;
            }
            return command;
        }


        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Database parameter Collection</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <param name="commandType">Text or Stored Procedure</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction, CommandType commandType)
        {
            object objScalar = null;
            IDbConnection connection = transaction != null ? transaction.Connection : _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);            
            command.Transaction = transaction;
            try
            {
                objScalar = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (transaction == null)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }

                if (command != null)
                    command.Dispose();
            }
            return objScalar;
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText)
        {            
            return ExecuteScalar(commandText, _transaction);
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, IDbTransaction transaction)
        {
            return ExecuteScalar(commandText, transaction, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Parameter to be associated</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameter param)
        {
            return ExecuteScalar(commandText, param, _transaction);
        }


        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Parameter to be associated</param>
        /// <param name="transaction">Database Transacion</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameter param, IDbTransaction transaction)
        {
            return ExecuteScalar(commandText, param, transaction, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter collection to be associated.</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, DBParameterCollection paramCollection)
        {
            return ExecuteScalar(commandText, paramCollection, _transaction);
        }

        /// <summary>
        ///  Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Database  Parameter Collection</param>
        /// <param name="transaction">Database Transacion (Use DBHelper.Transaction property.)</param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction)
        {
            return ExecuteScalar(commandText, paramCollection, transaction, CommandType.Text);
        }
        #endregion ExecuteScalar

        #region ExecuteNonQuery

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, _transaction, commandType);            
        }


        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, IDbTransaction transaction, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, new DBParameterCollection(), transaction, commandType);            
        }

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameter param, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, param, _transaction, commandType);           
        }


        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameter param, IDbTransaction transaction, CommandType commandType)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteNonQuery(commandText, paramCollection, transaction, commandType);
        }

        public T ExecuteNonQueryForOutParameter<T>(string commandText, DBParameter param,  CommandType commandType, string parameterName)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return (T) ExecuteNonQueryForOutParameter<T>(commandText, paramCollection, _transaction, commandType, parameterName);
        }

        public T ExecuteNonQueryForOutParameter<T>(string commandText, DBParameterCollection paramCollection, CommandType commandType, string parameterName)
        {
            return (T)ExecuteNonQueryForOutParameter<T>(commandText, paramCollection, _transaction, commandType, parameterName);
        }

        //public T ExecuteNonQueryForOutParameter<T>(string commandText, DBParameterCollection paramCollection, CommandType commandType, T parameterName)
        //{
        //    return (T)ExecuteNonQueryForOutParameter<T>(commandText, paramCollection, _transaction, commandType, parameterName);
        //}

        public T ExecuteNonQueryForOutParameter<T>(string commandText, DBParameter param, IDbTransaction transaction, CommandType commandType, string parameterName)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return (T)ExecuteNonQueryForOutParameter<T>(commandText, paramCollection, transaction, commandType, parameterName);
        }


        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameterCollection paramCollection, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, paramCollection, _transaction, commandType);            
        }

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>
        /// <param name="paramCollection">Parameter Collection to be associated with the comman</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction, CommandType commandType)
        {
            int rowsAffected = 0;
            IDbConnection connection = transaction != null ? transaction.Connection :  _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            command.Transaction = transaction;

            TryCatch.Run(() =>
            {
                rowsAffected = command.ExecuteNonQuery();
            }).IfNotNull(ex => { Loggger.LogError("Error in DBHelper class ExecuteNonQuery method :", ex); throw ex; })
            .Finally(() =>
            {
                if (transaction == null)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                if (command != null)
                    command.Dispose();
            });
            return rowsAffected;
        }

        public IDbCommand ExecuteNonQuery(string commandText, DBParameterCollection paramCollection, CommandType commandType, out int result)
        {
            IDbConnection connection = _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in DBHelper class ExecuteNonQuery method :", ex);
                throw ex;
            }
            return command;
        }


        public T ExecuteNonQueryForOutParameter<T>(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction, CommandType commandType, string parameterName)
        {
            var result = default(T);
            IDbConnection connection = transaction != null ? transaction.Connection : _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            command.Transaction = transaction;

            TryCatch.Run(() =>
            {
                command.ExecuteNonQuery();
                if (!string.IsNullOrWhiteSpace(parameterName))
                    result = (T) GetParameterValue(parameterName, command);

            }).IfNotNull(ex => {Loggger.LogError("Error in DBHelper class ExecuteNonQueryForOutParameter<T> method :", ex);  throw ex; })
            .Finally(() =>
            {
                if (transaction == null)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                if (command != null)
                    command.Dispose();
            });
            return result;
        }

        
        public Dictionary<string, object> ExecuteNonQueryForOutParameter(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction, CommandType commandType)
        {
            var result = new Dictionary<string, object>();
            IDbConnection connection = transaction != null ? transaction.Connection : _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            command.Transaction = transaction;

            TryCatch.Run(() =>
            {
                command.ExecuteNonQuery();
                //for(int i=0; i< parameters.Count; i++)
                foreach(var param in paramCollection.Parameters)
                {
                    if(param.ParamDirection == ParameterDirection.Output)
                        if (!string.IsNullOrWhiteSpace(param.Name) && paramCollection.Get(param.Name) != null)
                            result.Add(param.Name, GetParameterValue(param.Name, command)); 
                }
            }).IfNotNull(ex => { Loggger.LogError("Error in DBHelper class ExecuteNonQueryForOutParameter method :", ex); throw ex; })
            .Finally(() =>
            {
                if (transaction == null)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                if (command != null)
                    command.Dispose();
            });
            return result;
        }


        public Dictionary<string, object> ExecuteNonQueryForOutParameter(string commandText, DBParameterCollection paramCollection, CommandType commandType)
        {
            var result = new Dictionary<string, object>();
            IDbConnection connection = _transaction != null ? _transaction.Connection : _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            command.Transaction = _transaction;

            TryCatch.Run(() =>
            {
                command.ExecuteNonQuery();
                //for(int i=0; i< parameters.Count; i++)
                foreach (var param in paramCollection.Parameters)
                {
                    if (param.ParamDirection == ParameterDirection.Output)
                        if (!string.IsNullOrWhiteSpace(param.Name) && paramCollection.Get(param.Name) != null)
                            result.Add(param.Name, GetParameterValue(param.Name, command));
                }
            }).IfNotNull(ex => { Loggger.LogError("Error in DBHelper class Dictionary<string, object> ExecuteNonQueryForOutParameter method :", ex); throw ex; })
            .Finally(() =>
            {
                if (_transaction == null)
                {
                    if (connection != null)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                if (command != null)
                    command.Dispose();
            });
            return result;
        }
        /// <summary>
        /// Executes Sql Command and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, _transaction);
        }


        /// <summary>
        /// Executes Sql Command and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, IDbTransaction transaction)
        {
            return ExecuteNonQuery(commandText, transaction, CommandType.Text);
        }

        /// <summary>
        /// Executes Sql Command and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameter param)
        {
            return ExecuteNonQuery(commandText, param, _transaction);
        }

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameter param, IDbTransaction transaction)
        {
            return ExecuteNonQuery(commandText, param, transaction, CommandType.Text);
        }

        /// <summary>
        /// Executes Sql Command and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter Collection to be associated with the command</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameterCollection paramCollection)
        {
            return ExecuteNonQuery(commandText, paramCollection, _transaction);
        }


        /// <summary>
        /// Executes Sql Command and returns number of rows affected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter Collection to be associated with the command</param>
        /// <param name="transaction">Current Database Transaction (Use Helper.Transaction to get transaction)</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction)
        {
            return ExecuteNonQuery(commandText, paramCollection, transaction, CommandType.Text);
        }
        #endregion

        #region GetDataSet

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, DBParameter param, CommandType commandType)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteDataSet(commandText, paramCollection, commandType);
        }
                   

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, DBParameterCollection paramCollection, CommandType commandType)
        {
            DataSet dataSet = new DataSet();
            IDbConnection connection =  _connectionManager.GetConnection();
            IDataAdapter adapter = _dbAdapterManager.GetDataAdapter(commandText, connection, paramCollection, commandType);
           
            try
            {
                adapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in DBHelper class ExecuteDataSet method :", ex);
                throw ex;
            }
            finally
            {              
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
              
            }
            return dataSet;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {            
            return ExecuteDataSet(commandText, new DBParameterCollection(), commandType);
        }

      


        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText)
        {            
            return ExecuteDataSet(commandText, new DBParameterCollection(), CommandType.Text);
        }


        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, DBParameter param)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteDataSet(commandText, paramCollection);
        }


     
        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="paramCollection">Parameter collection to be associated with the command</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, DBParameterCollection paramCollection)
        {
            return ExecuteDataSet(commandText, paramCollection, CommandType.Text);
        }
        #endregion

        #region "ExecuteDataTable"
        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="tableName">Table name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, DBParameterCollection paramCollection, CommandType commandType)
        {
            DataTable dtReturn = new DataTable();
            IDbConnection connection = null;
            try
            {
                connection = _connectionManager.GetConnection();
                dtReturn = _dbAdapterManager.GetDataTable(commandText, paramCollection, connection, tableName, commandType);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in DBHelper class ExecuteDataTable method :", ex);
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

            }
            return dtReturn;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command8 or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, DBParameterCollection paramCollection, CommandType commandType)
        {
            return ExecuteDataTable(commandText, string.Empty, paramCollection, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="tableName">Table name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, DBParameterCollection paramCollection)
        {
            return ExecuteDataTable(commandText, tableName, paramCollection, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, DBParameterCollection paramCollection)
        {
            return ExecuteDataTable(commandText, string.Empty, paramCollection, CommandType.Text);
        }


        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>
        /// <param name="tableName">Table name</param>
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, DBParameter param, CommandType commandType)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteDataTable(commandText, tableName, paramCollection, commandType);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, DBParameter param, CommandType commandType)
        {
            return ExecuteDataTable(commandText, string.Empty, param, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="tableName">Table name</param>
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <returns>Result in  of DataTable</rethe formturns>
        public DataTable ExecuteDataTable(string commandText, string tableName, DBParameter param)
        {
            return ExecuteDataTable(commandText, tableName, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, DBParameter param)
        {
            return ExecuteDataTable(commandText, string.Empty, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="tableName">Table name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, CommandType commandType)
        {
            return ExecuteDataTable(commandText, tableName, new DBParameterCollection(), commandType);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return ExecuteDataTable(commandText, string.Empty, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="tableName">Table name</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName)
        {
            return ExecuteDataTable(commandText, tableName, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>   
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, string.Empty, CommandType.Text);
        }
        #endregion

        #region "ExecuteReader"
        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection connection, CommandType commandType)
        {
            return ExecuteDataReader(commandText, connection, new DBParameterCollection(), commandType);
        }

        /// <summary>
        /// Executes the Sql Command and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection connection)
        {
            return ExecuteDataReader(commandText, connection, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="param">Parameter to be associated with the Sql Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection connection, DBParameter param, CommandType commandType)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteDataReader(commandText, connection, paramCollection, commandType);             
        }

        /// <summary>
        /// Executes the Sql Command and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="param">Parameter to be associated with the Sql Command.</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection connection, DBParameter param)
        {
            return ExecuteDataReader(commandText, connection, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="paramCollection">Parameter to be associated with the Sql Command or Stored Procedure.</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection connection, DBParameterCollection paramCollection)
        {
            return ExecuteDataReader(commandText, connection, paramCollection, CommandType.Text);
        }


        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="paramCollection">Parameter to be associated with the Sql Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection connection, DBParameterCollection paramCollection, CommandType commandType)
        {
            IDataReader dataReader = null;
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            dataReader = command.ExecuteReader();
            command.Dispose();
            return dataReader;
        }


        /// <summary>
        /// Executes the Sql Command and returns the IDataReader. Do remember to Commit or Rollback the transaction
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Database Parameter</param>
        /// <param name="transaction">Database Transaction (Use DBHelper.Transaction property for getting the transaction.)</param>
        /// <returns>Data Reader</returns>
        public IDataReader ExecuteDataReader(string commandText, DBParameter param, IDbTransaction transaction)
        {        
            return ExecuteDataReader(commandText, param, transaction, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the IDataReader. Do remember to Commit or Rollback the transaction
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Proc Name</param>
        /// <param name="param">Database Parameter</param>
        /// <param name="transaction">Database Transaction (Use DBHelper.Transaction property for getting the transaction.)</param>
        /// <param name="commandType">Text/ Stored Procedure</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, DBParameter param, IDbTransaction transaction, CommandType commandType)
        {            
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param);
            return ExecuteDataReader(commandText, paramCollection, transaction, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and returns the IDataReader. Do remember to Commit or Rollback the transaction
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="paramCollection">Database Parameter Collection</param>
        /// <param name="transaction">Database Transaction (Use DBHelper.Transaction property for getting the transaction.)</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction)
        {
            return ExecuteDataReader(commandText, paramCollection, transaction, CommandType.Text);
        }


        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the IDataReader. Do remember to Commit or Rollback the transaction
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Proc name</param>
        /// <param name="paramCollection">Database Parameter Collection</param>
        /// <param name="transaction">Database Transaction (Use DBHelper.Transaction property for getting the transaction.)</param>
        /// <param name="commandType">Text/ Stored Procedure</param>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, DBParameterCollection paramCollection, IDbTransaction transaction, CommandType commandType)
        {
            IDataReader dataReader = null;
            IDbConnection connection = transaction.Connection;
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);
            command.Transaction = transaction;
            dataReader = command.ExecuteReader();
            command.Dispose();
            return dataReader;
        }

        #endregion

        #region "Methods to Prepare the Commands"

        /// <summary>
        /// Prepares command for the passed SQL Command Or Stored Procedure. 
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText">SQL Command or Stored Procedure name</param>
        /// <param name="commandType">Type of Command i.e. Text or Stored Procedure</param>
        /// <returns>Command ready for execute</returns>
        public IDbCommand GetCommand(string commandText, CommandType commandType)
        {
            IDbConnection connection = _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, commandType);
            return command;
        }


        public IDbCommand GetCommand(string commandText, IDbTransaction transaction, CommandType commandType)
        {
            IDbConnection connection = transaction != null ? transaction.Connection : _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, commandType);
            return command;
        }
        /// <summary>
        /// Prepares command for the passed SQL Command.
        /// Command is prepared for SQL Command only not for the stored procedures.
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText">SQL Command</param>        
        /// <returns>Command ready for execute</returns>
        public IDbCommand GetCommand(string commandText)
        {
            return GetCommand(commandText, CommandType.Text);
        }

        public IDbCommand GetCommand(string commandText, IDbTransaction transaction)
        {
            return GetCommand(commandText, transaction, CommandType.Text);
        }

        /// <summary>
        /// Prepares command for the passed SQL Command or Stored Procedure.
        /// Command is prepared for SQL Command only not for the stored procedures.
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText">SQL Command or Stored Procedure name</param>
        /// <param name="parameter">Database parameter</param>
        /// <param name="commandType">Type of Command i.e. Text or Stored Procedure</param>
        /// <returns>Command ready for execute</returns>
        public IDbCommand GetCommand(string commandText, DBParameter parameter, CommandType commandType)
        {
            IDbConnection connection = _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, parameter, commandType);
            return command;
        }

        /// <summary>
        /// Prepares command for the passed SQL Command.
        /// Command is prepared for SQL Command only not for the stored procedures.
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText">SQL Command</param>
        /// <param name="parameter">Database parameter</param>
        /// <returns>Command ready for execute</returns>
        public IDbCommand GetCommand(string commandText, DBParameter parameter)
        {
            return GetCommand(commandText, parameter, CommandType.Text);
        }


        public IDbCommand GetCommand(string commandText, DBParameter parameter, IDbTransaction transaction)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(parameter);
            return GetCommand(commandText, paramCollection, transaction, CommandType.Text);
        }

        /// <summary>
        /// Prepares command for the passed SQL Command or Stored Procedure.
        /// Command is prepared for SQL Command only not for the stored procedures. 
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText">SQL Command or Stored Procedure name</param>
        /// <param name="parameterCollection">Database parameter collection</param>
        /// <param name="commandType">Type of Command i.e. Text or Stored Procedure</param>
        /// <returns>Command ready for execute</returns>
        public IDbCommand GetCommand(string commandText, DBParameterCollection parameterCollection, CommandType commandType)
        {
            IDbConnection connection = _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, parameterCollection, commandType);
            return command;
        }

        /// <summary>
        /// Prepares command for the passed SQL Command or Stored Procedure.        
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameterCollection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IDbCommand GetCommand(string commandText, DBParameterCollection parameterCollection, IDbTransaction transaction, CommandType commandType)
        {
            IDbConnection connection = transaction != null ? transaction.Connection : _connectionManager.GetConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, parameterCollection, commandType);
            return command;
        }

        /// <summary>
        /// Prepares command for the passed SQL Command.
        /// Command is prepared for SQL Command only not for the stored procedures.
        /// Use DisposeCommand method after use of the command.
        /// </summary>
        /// <param name="commandText">SQL Command</param>
        /// <param name="parameterCollection">Database parameter collection</param>
        /// <returns>Command ready for execute</returns>
        public IDbCommand GetCommand(string commandText, DBParameterCollection parameterCollection)
        {
            return GetCommand(commandText, parameterCollection, CommandType.Text);
        }

        public IDbCommand GetCommand(string commandText, DBParameterCollection parameterCollection, IDbTransaction transaction)
        {
            return GetCommand(commandText, parameterCollection, transaction, CommandType.Text);
        }

        #endregion

        #region "Methods To Retrieve the Parameter Values"

        /// <summary>
        /// Returns the database parameter value from the specified command
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="command">Command from which value to be retrieved</param>
        /// <returns>Parameter value</returns>
        public object GetParameterValue(string parameterName, IDbCommand command)
        {
            object retValue = null;
            IDataParameter param = (IDataParameter)command.Parameters[parameterName];
            retValue = param.Value;
            return retValue;
        }

        /// <summary>
        /// Returns the database parameter value from the specified command
        /// </summary>
        /// <param name="index">Index of the parameter</param>
        /// <param name="command">Command from which value to be retrieved</param>
        /// <returns>Parameter value</returns>
        public object GetParameterValue(int index, IDbCommand command)
        {
            object retValue = null;
            IDataParameter param = (IDataParameter)command.Parameters[index];
            retValue = param.Value;
            return retValue;
        }

        #endregion

        #region "Methods to Dispose the Command"

        /// <summary>
        /// Closes and Disposes the Connection associated and then disposes the command.
        /// </summary>
        /// <param name="command">Command which needs to be closed</param>
        public void DisposeCommand(IDbCommand command)
        {
            if (command == null)
                return;

            if (command.Connection != null)
            {
                command.Connection.Close();
                command.Connection.Dispose();
            }

            command.Dispose();
        }

        #endregion
       
        /// <summary>
        /// Creates and opens the database connection for the connection parameters specified into web.config or App.config file.
        /// </summary>
        /// <returns>Database connection object in the opened state. </returns>
        public IDbConnection GetConnObject()
        {
            return _connectionManager.GetConnection();
        }

        public bool InsertDataUsingSqlBulkCopy<T>(string tableName, string[,] mappingColumns, IEnumerable<T> sourceEnumerable, IDbTransaction transaction)
        {
            try
            {
                IDbConnection connection;
                if (transaction != null)
                {
                    connection = transaction.Connection;
                }
                else
                {
                    connection = new SqlConnection(ConnectionString);
                }

                var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction);
                bulkCopy.DestinationTableName = tableName;
                for (int i = 0; i < mappingColumns.Length / 2; i++)
                    bulkCopy.ColumnMappings.Add(mappingColumns[i, 0], mappingColumns[i, 1]);

                using (var dataReader = new ObjectDataReader<T>(sourceEnumerable))
                {
                    bulkCopy.WriteToServer(dataReader);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {

                transaction.Rollback();
                Loggger.LogError("Error in DBHelper class InsertDataUsingSqlBulkCopy method :", ex);
                throw ex;
            }
        }
        public void Dispose()
        {
            if (_dbAdapterManager != null)
                _dbAdapterManager = null;

            if (_connectionManager != null)
                _connectionManager = null;

            if (_commandBuilder != null)
                _commandBuilder = null;
            
            if (_connection != null)
            {
                _connection.Close();
              
                _connection.Dispose();
            }
        }
    }
}
