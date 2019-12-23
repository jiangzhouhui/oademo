using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace OABordrinCommon
{

    public class DataOperator : IDisposable
    {
        #region 属性
        public DbCommand DbCommand
        {
            get
            {
                return _DbCommand;
            }
        }
        /// <summary>
        /// 获取或设置SQL命令行
        /// </summary>
        public string CommandText
        {
            get
            {
                return _DbCommand.CommandText;
            }
            set
            {
                _DbCommand.CommandText = value;
            }
        }

        /// <summary>
        /// 获取或设置查询语句的类型
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return _DbCommand.CommandType;
            }
            set
            {
                _DbCommand.CommandType = value;
            }
        }

        /// <summary>
        /// 获取或设置查询的超时时间
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                return _DbCommand.CommandTimeout;
            }
            set
            {
                _DbCommand.CommandTimeout = value;
            }
        }


        /// <summary>
        /// 获取或设置此连接所对应的逻辑数据库
        /// </summary>
        private string ConnectionName
        {
            get;
            set;
        }


        private Connection _Connection;
        private DbCommand _DbCommand = null;
        private DbConnection _DbConnection = null;
        private DbTransaction _DbTransaction = null;
        #endregion

        #region 构造函数
        private DataOperator()
        {
        }

        private IList<Connection> _ConnectionList;
        /// <summary>
        /// 创建并初始化一个数据操作对象
        /// </summary>
        /// <param name="logicDb">要操作的逻辑数据库名</param>
        /// <param name="tableName">要操作的表名</param>
        /// <param name="action">操作动作</param>
        public DataOperator(string moduleName, string connectionName)
        {
            if (_ConnectionList == null)
            {
                _ConnectionList = ConfigReader.GetConfigList<Connection>("db" + moduleName, "connectionList/connection");
            }
            //_Connection = _ConnectionList.Where(x => x.Name.Equals(connectionName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            //if (!string.IsNullOrEmpty(_Connection.EncryptedPassword))
            //{
            //    _Connection.ConnectionString = SecurityUtil.SymmetricDecrypt(_Connection.ConnectionString,
            //                                                                 _Connection.EncryptedPassword);
            //}

            _Connection.ConnectionString = "Data Source=.;Initial Catalog=InnovatorSolutions;User ID=sa;Password=***********";
            _DbConnection = new SqlConnection(_Connection.ConnectionString);
            _DbCommand = new SqlCommand();
            _DbConnection.Open();
            _DbCommand.Connection = _DbConnection;
        }

        public DataOperator(string connectionString)
        {
            _Connection = new Connection();
            _Connection.ConnectionString = connectionString;
            _DbConnection = new SqlConnection(connectionString);
            _DbCommand = new SqlCommand();
            _DbConnection.Open();
            _DbCommand.Connection = _DbConnection;
        }

        private bool disposed;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 如果还存在未提交或回滚的事物，则将事务回滚
                    if (_DbTransaction != null)
                    {
                        try
                        {
                            _DbTransaction.Rollback();
                            _DbTransaction = null;
                        }
                        catch { }
                    }

                    if (_DbCommand != null)
                    {
                        _DbCommand.Dispose();
                    }

                    if (_DbConnection.State == ConnectionState.Open)
                    {
                        try
                        {
                            _DbConnection.Close();
                        }
                        catch
                        {
                        }
                    }
                    if (_DbConnection != null)
                    {
                        _DbConnection.Dispose();
                    }
                }
                disposed = true;
            }
        }
        #endregion

        #region SQL Server和Sql Server CE参数操作
        /// <summary>
        /// 设置SQL参数的值，当参数类型为int、bit、bigint、smallint、datatime、smalldatetime、money、float、real、text、ntext、uniqueidentity、image、smallmoney、tinyint等不需要设置长度的类型时调用。
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="paraData">要设置的参数值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="direction">参数方向</param>
        public void SetParameter(string paraName, object paraData, SqlDbType dbType, ParameterDirection direction)
        {

            SqlParameter param;
            SqlCommand command = (SqlCommand)_DbCommand;
            bool blnNewParam = false;

            if (command.Parameters.IndexOf(paraName) < 0)
            {
                blnNewParam = true;
                param = command.CreateParameter();
                param.ParameterName = paraName;
            }
            else
            {
                param = command.Parameters[paraName];
            }

            param.SqlDbType = dbType;
            param.Direction = direction;
            // 如果值为空，则将值设置为DBNull.Value，即插入数据库后仍然保持null;
            if (direction != ParameterDirection.Output)
            {
                if (paraData == null)
                    param.Value = System.DBNull.Value;
                else
                    param.Value = paraData;
            }
            // 如果是新参数，添加参数到Command中
            if (blnNewParam)
                command.Parameters.Add(param);

        }

        /// <summary>
        /// 设置SQL参数的值，当参数类型为int、bit、bigint、smallint、datatime、smalldatetime、money、float、real、text、ntext、uniqueidentity、image、smallmoney、tinyint等不需要设置长度的类型时调用。
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="paraData">要设置的参数值</param>
        /// <param name="dbType">数据类型</param>
        public void SetParameter(string paraName, object paraData, SqlDbType dbType)
        {
            SetParameter(paraName, paraData, dbType, ParameterDirection.Input);
        }

        /// <summary>
        /// 设置SQL参数的值，当参数类型为varchar、nvarchar、char、nchar、binary、varbinary类型时，指定参数的长度。
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="paraData">要设置的参数值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">字符串长度</param>
        /// <param name="direction">参数方向</param>
        public void SetParameter(string paraName, object paraData, SqlDbType dbType, int size, ParameterDirection direction)
        {

            SqlParameter param;
            SqlCommand command = (SqlCommand)_DbCommand;
            bool blnNewParam = false;

            if (command.Parameters.IndexOf(paraName) < 0)
            {
                blnNewParam = true;
                param = command.CreateParameter();
                param.ParameterName = paraName;
            }
            else
            {
                param = command.Parameters[paraName];
            }

            param.SqlDbType = dbType;
            param.Direction = direction;
            // 如果值为空，则将值设置为DBNull.Value，即插入数据库后仍然保持null;
            if (paraData == null)
                param.Value = System.DBNull.Value;
            else
                param.Value = paraData;

            param.Size = size;

            // 如果是新参数，添加参数到Command中
            if (blnNewParam)
                command.Parameters.Add(param);
        }

        /// <summary>
        /// 设置SQL参数的值，当参数类型为varchar、nvarchar、char、nchar、binary、varbinary类型时，指定参数的长度。
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="paraData">要设置的参数值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">字符串长度</param>
        public void SetParameter(string paraName, object paraData, SqlDbType dbType, int size)
        {
            SetParameter(paraName, paraData, dbType, size, ParameterDirection.Input);
        }

        /// <summary>
        /// 设置SQL参数的值。数据类型为decimal、numeric时调用此方法
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="paraData">要设置的参数值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="precision">参数的最大位数。 </param>
        /// <param name="scale">参数的小数位数</param>
        /// <param name="direction">参数方向</param>
        public void SetParameter(string paraName, object paraData, SqlDbType dbType, byte precision, byte scale, ParameterDirection direction)
        {

            SqlParameter param;
            SqlCommand command = (SqlCommand)_DbCommand;
            bool blnNewParam = false;

            if (command.Parameters.IndexOf(paraName) < 0)
            {
                blnNewParam = true;
                param = command.CreateParameter();
                param.ParameterName = paraName;
            }
            else
            {
                param = command.Parameters[paraName];
            }

            param.SqlDbType = dbType;
            param.Direction = direction;
            // 如果值为空，则将值设置为DBNull.Value，即插入数据库后仍然保持null;
            if (paraData == null)
                param.Value = System.DBNull.Value;
            else
                param.Value = paraData;

            param.Precision = precision;
            param.Scale = scale;

            // 如果是新参数，添加参数到Command中
            if (blnNewParam)
                command.Parameters.Add(param);

        }

        /// <summary>
        /// 设置SQL参数的值。数据类型为decimal、numeric时调用此方法
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="paraData">要设置的参数值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="precision">参数的最大位数。 </param>
        /// <param name="scale">参数的小数位数</param>
        public void SetParameter(string paraName, object paraData, SqlDbType dbType, byte precision, byte scale)
        {
            SetParameter(paraName, paraData, dbType, precision, scale, ParameterDirection.Input);
        }
        #endregion

        #region 执行Sql操作
        /// <summary>
        /// 执行Sql操作，并返回影响的行数
        /// </summary>
        /// <returns>影响操作的行数</returns>
        public int ExecuteNonQuery()
        {
            // 设置命令的事务
            if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                _DbCommand.Transaction = _DbTransaction;

            if (_Connection.ErrorLog)
            {
                // 如果需要写日志，则用try{}catch{}
                try
                {
                    return _DbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    WriteErrorLog(ex);
                    throw;
                }
            }
            else
            {
                return _DbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行Sql操作，并返回执行结果的第一行第一列数据
        /// </summary>
        /// <returns>执行后的第一行第一列数据</returns>
        public object ExecuteScalar()
        {
            // 设置命令的事务
            if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                _DbCommand.Transaction = _DbTransaction;

            if (_Connection.ErrorLog)
            {
                // 如果需要写日志，则用try{}catch{}
                try
                {
                    return _DbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    WriteErrorLog(ex);
                    throw;
                }
            }
            else
            {
                return _DbCommand.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行并返回DataTable。如果查询语句为复合查询（返回多个结果的），此操作只返回第一个查询的结果。
        /// </summary>
        /// <returns>执行结果</returns>
        public DataTable ExecuteDataTable(string tableName = "DataTable")
        {
            IDataReader reader = null;
            DataTable dtReturn = new DataTable(tableName);
            try
            {
                // 设置命令的事务
                if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                    _DbCommand.Transaction = _DbTransaction;

                // 执行DBCommand，返回DataReader对象
                reader = _DbCommand.ExecuteReader();

                // 如果返回的DataTable对象还没有构建结构，则第一次根据DataReader返回的记录创建表结构
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    System.Type type = reader.GetFieldType(i);
                    DataColumn column = new DataColumn(columnName, type);
                    dtReturn.Columns.Add(column);
                }

                while (reader.Read())
                {
                    // 在DataTable中添加一条记录
                    DataRow dr = dtReturn.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        dr[columnName] = reader.GetValue(i);
                    }
                    dtReturn.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
            finally
            {
                // 一定要关闭DataReader，以释放被占用的数据库连接资源
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return dtReturn;
        }

        /// <summary>
        /// 执行并返回DataSet。
        /// </summary>
        /// <returns>执行结果</returns>
        public DataSet ExecuteDataSet(string dataSetName)
        {
            IDataReader reader = null;
            DataSet dsReturn = new DataSet(dataSetName);

            try
            {
                // 设置命令的事务
                if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                    _DbCommand.Transaction = _DbTransaction;

                // 执行DBCommand，返回DataReader对象
                reader = _DbCommand.ExecuteReader();

                do
                {
                    DataTable dtResult = new DataTable();
                    // 如果返回的DataTable对象还没有构建结构，则第一次根据DataReader返回的记录创建表结构
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        System.Type type = reader.GetFieldType(i);
                        DataColumn column = new DataColumn(columnName, type);
                        dtResult.Columns.Add(column);
                    }

                    while (reader.Read())
                    {
                        // 在DataTable中添加一条记录
                        DataRow dr = dtResult.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            dr[columnName] = reader.GetValue(i);
                        }
                        dtResult.Rows.Add(dr);
                    }
                    dsReturn.Tables.Add(dtResult);
                } while (reader.NextResult());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
            finally
            {
                // 一定要关闭DataReader，以释放被占用的数据库连接资源
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            return dsReturn;
        }

        private DataTable ExecuteDataTable(IDataReader reader, string tableName = "DataTable")
        {
            DataTable dt = new DataTable(tableName);

            // 如果返回的DataTable对象还没有构建结构，则第一次根据DataReader返回的记录创建表结构
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                System.Type type = reader.GetFieldType(i);
                DataColumn column = new DataColumn(columnName, type);
                dt.Columns.Add(column);
            }

            while (reader.Read())
            {
                // 在DataTable中添加一条记录
                DataRow dr = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    dr[columnName] = reader.GetValue(i);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 分页查询 注:仅支持MS SQL Server
        /// 调用存储过程SqlDataPaging,会造成sql注入，小心使用
        /// </summary>
        public List<T> ExecuteDataPager<T>(string tableName, string fieldList, string whereClause, string orderBy, bool isDesc, int pageSize, int pageIndex, out int recordsCount, out int pagesCount) where T : new()
        {
            IDataReader reader = null;
            List<T> list = new List<T>();
            recordsCount = 0;
            pagesCount = 0;
            if (pageIndex < 1)
            {
                pageIndex = 0;
            }
            try
            {
                if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                    _DbCommand.Transaction = _DbTransaction;
                _DbCommand.CommandText = "SqlDataPaging";
                _DbCommand.CommandType = CommandType.StoredProcedure;
                SetParameter("@tbName", tableName, SqlDbType.VarChar, 255);
                SetParameter("@tbFields", fieldList, SqlDbType.VarChar, 4000);
                SetParameter("@orderField", orderBy, SqlDbType.VarChar, 255);
                SetParameter("@pageSize", pageSize, SqlDbType.Int);
                SetParameter("@pageIndex", pageIndex, SqlDbType.Int);
                SetParameter("@strWhere", whereClause, SqlDbType.VarChar, 1500);
                SetParameter("@orderType", isDesc ? 1 : 0, SqlDbType.Bit);
                SetParameter("@totalCount", 0, SqlDbType.Int, ParameterDirection.Output);

                _DbCommand.ExecuteNonQuery();
                int.TryParse(_DbCommand.Parameters["@totalCount"].Value.ToString(), out recordsCount);
                int.TryParse(Math.Ceiling(recordsCount * 1d / pageSize).ToString(), out pagesCount);
                list = ConvertDataReaderToList<T>(_DbCommand.ExecuteReader());
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            return list;
        }

        private List<T> ConvertDataReaderToList<T>(IDataReader reader) where T : new()
        {
            List<T> list = new List<T>();
            // 获取实体的所有属性
            Dictionary<PropertyInfo, int> dicProperties = new Dictionary<PropertyInfo, int>();
            System.Reflection.PropertyInfo[] p = typeof(T).GetProperties();
            for (int i = 0; i < p.Length; i++)
            {
                for (int j = 0; j < reader.FieldCount; j++)
                {
                    // 字段名和属性名不区分大小写
                    if (p[i].Name.Equals(reader.GetName(j), StringComparison.OrdinalIgnoreCase))
                    {
                        dicProperties.Add(p[i], j);
                        break;
                    }
                }
            }
            while (reader.Read())
            {
                // 如果记录在获取范围内，则在列表中添加一条记录
                T item = new T();

                foreach (PropertyInfo property in dicProperties.Keys)
                {
                    object obj = reader.GetValue(dicProperties[property]);
                    if (obj == System.DBNull.Value || obj == null)
                    {
                        if (property.PropertyType.IsPrimitive)
                        {
                            // 如果属性为基础类型，则设置默认值
                            obj = GetTypeDefaultValue(property.PropertyType);
                        }
                        else
                        {
                            obj = null;
                        }
                    }
                    property.SetValue(item, obj, null);
                }
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 获取实体列表内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageNumber">当前页码，计数器从1开始</param>
        /// <returns></returns>
        public List<T> ExecuteEntityList<T>(string tableName, string fieldList, string whereClause, string orderBy, int pageSize, int pageNumber) where T : new()
        {
            BuildQuery(tableName, fieldList, whereClause, orderBy, pageSize, pageNumber);
            return ExecuteEntityList<T>();
        }

        /// <summary>
        /// 获取实体列表内容，此方法会导致查询结果的全部扫描，如果查询结果数较大，性能会受到影响。
        /// 如果不需要总记录数，请使用BindingList<T> ExecuteEntityList<T>(int, int)方法以提高性能
        /// 对于查询结果数较大的分页，建议先通过select count(1)查询获取总记录数后，再通过BindingList<T> ExecuteEntityList<T>(int,int)方法获取指定页的数据。
        /// 这样会避免扫描全部数据而带来性能影响。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageNumber">当前页码，计数器从1开始</param>
        /// <param name="recordCount">总记录数</param>
        /// <param name="pageCount">总页数</param>
        /// <returns></returns>
        public List<T> ExecuteEntityList<T>(string tableName, string fieldList, string whereClause, string orderBy, int pageSize, int pageNumber, out int recordCount, out int pageCount) where T : new()
        {
            recordCount = GetRecordCount(tableName, whereClause);
            int.TryParse(Math.Ceiling(recordCount * 1d / pageSize).ToString(), out pageCount);
            return ExecuteEntityList<T>(tableName, fieldList, whereClause, orderBy, pageSize, pageNumber);
        }

        public List<T> ExecuteEntityList<T>(string commandText, string orderBy, int pageSize, int pageNumber, out int recordCount, out int pageCount) where T : new()
        {
            string tempTableCommand = @"WITH TempTable  AS (" + commandText + ")";

            _DbCommand.CommandType = CommandType.Text;
            CommandText = tempTableCommand + "  SELECT COUNT(1) FROM TempTable WITH(NOLOCK)";
            recordCount = int.Parse(ExecuteScalar().ToString());
            int.TryParse(Math.Ceiling(recordCount * 1d / pageSize).ToString(), out pageCount);
            #region 判断OrderBy前是不是加了order by，如果是，则去掉
            if (orderBy.Trim().StartsWith("order by ", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = orderBy.Trim().Substring(9);
            }
            #endregion

            int startIndex = (pageNumber - 1) * pageSize + 1;
            int endIndex = startIndex + pageSize - 1;

            CommandText = tempTableCommand + @" 
SELECT * FROM
(
	SELECT *,ROW_NUMBER() OVER (ORDER BY " + orderBy + @") AS RowNumber FROM  TempTable  WITH(NOLOCK)";
            CommandText += @") AS thetemp
WHERE (RowNumber >= @pageStart) AND (RowNumber <= @pageEnd)";
            SetParameter("@pageStart", startIndex, SqlDbType.Int);
            SetParameter("@pageEnd", endIndex, SqlDbType.Int);
            return ExecuteEntityList<T>();
        }

        public List<T> ExecuteEntityList<T>(string tableName, string fieldList, string whereClause, PagerEntity pagerEntity) where T : new()
        {
            pagerEntity.RecordCount = GetRecordCount(tableName, whereClause);
            int pagesCount;
            int.TryParse(Math.Ceiling(pagerEntity.RecordCount * 1d / pagerEntity.PageSize).ToString(), out pagesCount);
            pagerEntity.PagesCount = pagesCount;
            return ExecuteEntityList<T>(tableName, fieldList, whereClause, pagerEntity.Orderby, pagerEntity.PageSize, pagerEntity.PageIndex);
        }

        public List<T> ExecuteEntityList<T>(string commandText, PagerEntity pagerEntity) where T : new()
        {
            if (pagerEntity == null)
            {
                _DbCommand.CommandText = commandText;
                return ExecuteEntityList<T>();
            }
            int recordCount;
            int pageCount;
            var ans = ExecuteEntityList<T>(commandText, pagerEntity.Orderby, pagerEntity.PageSize, pagerEntity.PageIndex, out recordCount, out pageCount);
            pagerEntity.RecordCount = recordCount;
            pagerEntity.PagesCount = pageCount;
            return ans;
        }


        /// <summary>
        /// 获取实体列表内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ExecuteEntityList<T>() where T : new()
        {
            IDataReader reader = null;
            List<T> lstItem = new List<T>();

            try
            {
                // 设置命令的事务
                if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                    _DbCommand.Transaction = _DbTransaction;

                // 执行DBCommand，返回DataReader对象
                reader = _DbCommand.ExecuteReader();

                // 获取实体的所有属性
                Dictionary<PropertyInfo, int> dicProperties = new Dictionary<PropertyInfo, int>();
                System.Reflection.PropertyInfo[] p = typeof(T).GetProperties();
                for (int i = 0; i < p.Length; i++)
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        // 字段名和属性名不区分大小写
                        if (p[i].Name.Equals(reader.GetName(j), StringComparison.OrdinalIgnoreCase))
                        {
                            dicProperties.Add(p[i], j);
                            break;
                        }
                    }
                }

                while (reader.Read())
                {
                    // 在列表中添加一条记录
                    T item = new T();

                    foreach (PropertyInfo property in dicProperties.Keys)
                    {
                        object obj = reader.GetValue(dicProperties[property]);
                        if (obj == System.DBNull.Value || obj == null)
                        {
                            if (property.PropertyType.IsPrimitive)
                            {
                                // 如果属性为基础类型，则设置默认值
                                obj = GetTypeDefaultValue(property.PropertyType);
                            }
                            else
                            {
                                obj = null;
                            }
                        }
                        property.SetValue(item, obj, null);
                    }

                    //dtReturn.Rows.Add(dr);
                    lstItem.Add(item);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
            finally
            {
                // 一定要关闭DataReader，以释放被占用的数据库连接资源
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return lstItem;
        }

        /// <summary>
        /// 获取实体列表内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ExecuteList<T>()
        {
            IDataReader reader = null;
            List<T> lstItem = new List<T>();

            try
            {
                // 设置命令的事务
                if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                    _DbCommand.Transaction = _DbTransaction;

                // 执行DBCommand，返回DataReader对象
                reader = _DbCommand.ExecuteReader();
                while (reader.Read())
                {
                    object obj = reader.GetValue(0);
                    if (obj != null && obj != System.DBNull.Value)
                    {
                        lstItem.Add((T)obj);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
            finally
            {
                // 一定要关闭DataReader，以释放被占用的数据库连接资源
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
            return lstItem;
        }

        /// <summary>
        /// 获取执行结果的第一条记录
        /// </summary>
        /// <returns>执行结果的第一条实体记录</returns>
        public T ExecuteEntity<T>() where T : new()
        {
            IDataReader reader = null;

            try
            {
                // 设置命令的事务
                if (_DbTransaction != null && _DbCommand.Transaction != _DbTransaction)
                    _DbCommand.Transaction = _DbTransaction;

                // 执行DBCommand，返回DataReader对象
                reader = _DbCommand.ExecuteReader();

                // 获取实体的所有属性
                Dictionary<PropertyInfo, int> dicProperties = new Dictionary<PropertyInfo, int>();
                System.Reflection.PropertyInfo[] p = typeof(T).GetProperties();
                for (int i = 0; i < p.Length; i++)
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        // 字段名和属性名不区分大小写
                        if (p[i].Name.Equals(reader.GetName(j), StringComparison.OrdinalIgnoreCase))
                        {
                            dicProperties.Add(p[i], j);
                            break;
                        }
                    }
                }

                if (reader.Read())
                {
                    // 在列表中添加一条记录
                    T item = new T();

                    foreach (PropertyInfo property in dicProperties.Keys)
                    {
                        object obj = reader.GetValue(dicProperties[property]);
                        if (obj == System.DBNull.Value || obj == null)
                        {
                            if (property.PropertyType.IsPrimitive)
                            {
                                // 如果属性为基础类型，则设置默认值
                                obj = GetTypeDefaultValue(property.PropertyType);
                            }
                            else
                            {
                                obj = null;
                            }
                        }
                        property.SetValue(item, obj, null);
                    }

                    //dtReturn.Rows.Add(dr);
                    return item;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
                throw;
            }
            finally
            {
                // 一定要关闭DataReader，以释放被占用的数据库连接资源
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
        }

        /// <summary>
        /// 根据表名、字段列表、查询条件和排序方式，根据数据库系统类型，自动返回适合该数据库系统的分页查询Sql语句。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldList">字段列表</param>
        /// <param name="whereClause">WHERE条件（请不要加WHERE子句）</param>
        /// <param name="orderBy">排序字段列表（不要加Order BY子句）</param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        private void BuildQuery(string tableName, string fieldList, string whereClause, string orderBy, int pageSize, int pageNumber)
        {
            _DbCommand.CommandType = CommandType.Text;
            #region 判断用户在whereClause前是不是加了WHERE，如果是，则替换掉
            if (whereClause.Trim().StartsWith("where ", StringComparison.OrdinalIgnoreCase))
            {
                whereClause = whereClause.Trim().Substring(6);
            }
            #endregion

            #region 判断OrderBy前是不是加了order by，如果是，则去掉
            if (orderBy.Trim().StartsWith("order by ", StringComparison.OrdinalIgnoreCase))
            {
                orderBy = orderBy.Trim().Substring(9);
            }
            #endregion

            int startIndex = (pageNumber - 1) * pageSize + 1;
            int endIndex = startIndex + pageSize - 1;


            #region SQL Server数据库时（需要Sql Server 2005以上版本才支持此语法）
            CommandText = @"SELECT " + fieldList + @" FROM
(
	SELECT *,ROW_NUMBER() OVER (ORDER BY " + orderBy + @") AS RowNumber FROM " + tableName + " NOLOCK";

            if (!string.IsNullOrEmpty(whereClause.Trim()))
            {
                CommandText += " WHERE " + whereClause;
            }

            CommandText += @") AS TempTable
WHERE (RowNumber >= @pageStart) AND (RowNumber <= @pageEnd)";
            SetParameter("@pageStart", startIndex, SqlDbType.Int);
            SetParameter("@pageEnd", endIndex, SqlDbType.Int);
            #endregion
        }

        /// <summary>
        /// 根据查询
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldList"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public int GetRecordCount(string tableName, string whereClause)
        {
            #region 判断用户在whereClause前是不是加了WHERE，如果是，则替换掉
            if (whereClause.Trim().StartsWith("where ", StringComparison.OrdinalIgnoreCase))
            {
                whereClause = whereClause.Trim().Substring(6);
            }
            #endregion
            _DbCommand.CommandType = CommandType.Text;
            CommandText = "SELECT COUNT(1) FROM " + tableName + " NOLOCK";
            if (whereClause.Trim() != "")
            {
                CommandText += " WHERE " + whereClause;
            }
            int recordCount = int.Parse(ExecuteScalar().ToString());

            return recordCount;
        }

        /// <summary>
        /// 字符串转换方法
        /// </summary>
        /// <param name="t">数据类型</param>
        /// <returns></returns>
        private object GetTypeDefaultValue(Type t)
        {
            if (t == typeof(System.Boolean))
                return false;
            else if (t == typeof(System.Byte))
                return (System.Byte)0;
            else if (t == typeof(System.SByte))
                return (System.SByte)0;
            else if (t == typeof(System.Int16))
                return (System.Int16)0;
            else if (t == typeof(System.UInt16))
                return (System.UInt16)0;
            else if (t == typeof(System.Int32))
                return (System.Int32)0;
            else if (t == typeof(System.UInt32))
                return (System.UInt32)0;
            else if (t == typeof(System.Int64))
                return (System.Int64)0;
            else if (t == typeof(System.UInt64))
                return (System.UInt64)0;
            else if (t == typeof(System.IntPtr))
                return (System.IntPtr)0;
            else if (t == typeof(System.UIntPtr))
                return (System.UIntPtr)0;
            else if (t == typeof(System.Char))
                return (System.Char)0;
            else if (t == typeof(System.Double))
                return (System.Double)0;
            else if (t == typeof(System.Single))
                return (System.Single)0;
            else
                return 0;
        }
        #endregion

        #region 日志处理
        /// <summary>
        /// 将错误信息根据数据库连接的配置节写入日志文件
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        private void WriteErrorLog(Exception ex)
        {
            if (_Connection.ErrorLog)
            {
                string logFile = Path.Combine(_Connection.ErrorLogPath, DateTime.Now.ToString("yyyyMMdd") + ".log");
                FileStream fs = null;
                StreamWriter sw = null;

                try
                {
                    // 需要将错误信息写入到日志文件
                    string strError = "错误时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
        + "\r\n错误信息：" + ex.ToString()
        + "\r\n错误堆栈：" + ex.StackTrace
        + "\r\n错误语句：\r\n" + _DbCommand.CommandText;

                    foreach (DbParameter param in _DbCommand.Parameters)
                    {
                        strError += "\r\n" + param.ParameterName + "='" + param.Value.ToString() + "'";
                    }

                    strError += "\r\n==============================================================";

                    // 将错误记录到日志中
                    fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.Read);
                    sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    sw.Write(strError);
                    sw.Flush();
                }
                catch { }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
        }
        #endregion 日志处理

        #region 事物操作
        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTransaction()
        {
            if (_DbTransaction != null)
                throw new Exception("当前上下文存在未提交或未回滚的事物，不允许在同一个上下文中嵌套事物。如需事物嵌套，请新建上下文连接！");
            _DbTransaction = _DbConnection.BeginTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            if (_DbTransaction == null)
                throw new Exception("事物不存在，或事物已提交/回滚！");
            _DbTransaction.Commit();
            _DbTransaction.Dispose();
            _DbTransaction = null;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            if (_DbTransaction != null)
            {
                _DbTransaction.Rollback();
                _DbTransaction.Dispose();
                _DbTransaction = null;
            }
        }
        #endregion

        #region 与数据库结构相关的方法
        /// <summary>
        /// 根据数据库中的所有表
        /// </summary>
        /// <returns>数据库中所有的表</returns>
        public DataTable GetTables()
        {
            return _DbConnection.GetSchema(SqlClientMetaDataCollectionNames.Tables, new string[] { null, null, null, "BASE TABLE" });
        }

        /// <summary>
        /// 获取数据中所有的视图
        /// </summary>
        /// <returns>数据库中所有视图列表</returns>
        public DataTable GetViews()
        {
            return _DbConnection.GetSchema(SqlClientMetaDataCollectionNames.Views);
        }

        /// <summary>
        /// 获取数据库中指定表的所有字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>表的字段列表</returns>
        public DataTable GetColumns(string tableName)
        {
            return _DbConnection.GetSchema(SqlClientMetaDataCollectionNames.Columns, new string[] { null, null, tableName });
        }

        public DataTable GetPrimaryKeyColumns(string tableName)
        {
            DataTable dtPrimaryKeyColumns = new DataTable();
            dtPrimaryKeyColumns.Columns.Add("COLUMN_NAME", typeof(string));
            return dtPrimaryKeyColumns;
        }
        #endregion
    }

}

