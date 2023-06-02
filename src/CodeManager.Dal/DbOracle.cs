using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CodeManager.Dal
{
    /// <summary>
    /// oracle数据访问类
    /// </summary>
    public class DbOracle
    {

        private readonly OracleConnection _conn;

        private OracleTransaction _tran;

        public DbOracle(string connectString)
        {
            _conn = new OracleConnection(connectString);
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTransaction() {
            if (_conn.State != ConnectionState.Open)
                _conn.Open();
            _tran = _conn.BeginTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit() {
            _tran.Commit();
            _tran.Dispose();
            _conn.Close();
            _tran = null;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            _tran.Rollback();
            _tran.Dispose();
            _conn.Close();
            _tran = null;
        }

        /// <summary>
        /// 执行sql返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> ExcecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            OracleCommand cmd = new OracleCommand();
            int row = 0;
            try
            {
                PrepareCommand(cmd, sql, parameters);
                row = await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn.Close();
                cmd.Dispose();
            }

            return row;
        }

        /// <summary>
        /// 获取首行首列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExceuteScalar(string sql, params OracleParameter[] parameters)
        {
            OracleCommand cmd = new OracleCommand();
            object result = null;

            try
            {
                PrepareCommand(cmd, sql, parameters);
                result = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn.Close();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, params OracleParameter[] parameters)
        {
            OracleCommand cmd = new OracleCommand();
            DataTable dt = null;

            try
            {
                PrepareCommand(cmd, sql, parameters);
                OracleDataAdapter adapter = new OracleDataAdapter
                {
                    SelectCommand = cmd
                };
                dt = new DataTable();
                adapter.Fill(dt);
                cmd.Parameters.Clear();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _conn.Close();
                cmd.Dispose();
            }

            return dt;
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string cmdText, params OracleParameter[] cmdParms)
        {
            IDataReader reader = default;
            OracleCommand cmd = new OracleCommand();
            
            try
            {
                PrepareCommand(cmd, cmdText, cmdParms);
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
            return reader;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <param name="tran"></param>
        private void PrepareCommand(OracleCommand cmd, string cmdText, OracleParameter[] cmdParms)
        {
            if (_conn.State != ConnectionState.Open)
                _conn.Open();

            cmd.Connection = _conn;
            cmd.CommandText = cmdText;

            if (_tran != null)
                cmd.Transaction = _tran;

            cmd.CommandType = CommandType.Text;

            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

    }
}
