using CodeManager.Core;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Threading.Tasks;

namespace CodeManager.Dal
{
    /// <summary>
    /// sqlite数据访问类
    /// </summary>
    public class DbSqlite
    {
        private string _connectString;
        public DbSqlite(string connectString)
        {
            _connectString = connectString;
        }

        public bool BatchAdd<T>(List<T> model)
        {
            var result = true;
            var table = typeof(T).Name;
            var colNames = new List<string>();
            foreach (var p in typeof(T).GetProperties())
            {
                colNames.Add(p.Name);
            }

            var sql = $@"
                    INSERT INTO {table} ({string.Join(',', colNames)})
                    VALUES({string.Join(',', colNames.Select(c => "@" + c))})
                ";

            using (SQLiteConnection connection = new SQLiteConnection(_connectString))
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {

                        foreach (var item in model)
                        {
                            command.CommandText = sql;
                            command.Parameters.Clear();
                            foreach (var p in typeof(T).GetProperties())
                            {
                                command.Parameters.Add(new SQLiteParameter("@" + p.Name, p.GetValue(item)));
                            }

                            var rows = command.ExecuteNonQuery();
                            if(rows == 0)
                            {
                                result = false;
                                transaction.Rollback();
                                break;
                            }
                        }
                    }
                    transaction.Commit();
                }
            }


            return result;
        }

        /// <summary>
        /// 执行语句返回受影响的行数
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string strSql, params SQLiteParameter[] parameters)
        {
            int rows = 0;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectString))
                {
                    connection.Open();
                    using (DbTransaction transaction = connection.BeginTransaction())
                    {
                        using (SQLiteCommand command = new SQLiteCommand(connection))
                        {
                            command.CommandText = strSql;
                            if (parameters != null && parameters.Length > 0)
                            {
                                command.Parameters.AddRange(parameters);
                            }

                            rows = command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

            }
            catch (Exception ex)
            {
                rows = 0;
            }

            return rows;
        }

        /// <summary>
        /// 取datatable
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>
        /// 返回DataTable
        /// </returns>
        public DataTable GetDataTable(string strSql)
        {
            DataTable dt = null;
            try
            {
                using var connection = new SQLiteConnection(_connectString);
                using var cmd = new SQLiteCommand(strSql, connection);
                var adapter = new SQLiteDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                connection.Close();
                SQLiteConnection.ClearAllPools();
            }
            catch (Exception)
            {
                dt = null;
            }

            return dt;
        }
    }
}
