using CodeManager.Core;
using CodeManager.Dal;
using CodeManager.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SQLite;
using System.Reflection;

namespace CodeManager.Busi
{
    public class BllSysDb
    {
        private readonly DbSqlite _db;
        public BllSysDb(IConfiguration config)
        {
            var connectString = config["SysDb"].Replace("${path}", AppDomain.CurrentDomain.BaseDirectory);
            _db = new DbSqlite(connectString);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public List<T> GetList<T>() where T : class, new()
        {
            var sql = $"SELECT * FROM {typeof(T).Name}";

            var dataTable = _db.GetDataTable(sql);
            return Tool.ToList<T>(dataTable);
        }

        public List<T> GetList<T>(List<string> ids) where T : class, new()
        {

            var idList = ids.Select(m => $"'{m}'");

            var sql = $"SELECT * FROM {typeof(T).Name} WHERE ID IN ({string.Join(",", idList)})";

            var dataTable = _db.GetDataTable(sql);
            return Tool.ToList<T>(dataTable);
        }

        /// <summary>
        /// 获取model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetModel<T>(string id) where T : class, new()
        {
            var sql = $"SELECT * FROM {typeof(T).Name} WHERE ID = '{id}'";
            var dataTable = _db.GetDataTable(sql);
            return Tool.ToList<T>(dataTable).FirstOrDefault();
        }

        /// <summary>
        /// 获取连接关联的表格
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DbTable> GetConnectTableList(string id)
        {
            var sql = $"SELECT * FROM DbTable where ConnectionId = '{id}'";

            var dataTable = _db.GetDataTable(sql);
            return Tool.ToList<DbTable>(dataTable);
        }

        /// <summary>
        /// 获取连接关联的所有列
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DbTableColumn> GetConnectTableColumnList(string id)
        {
            var sql = $"SELECT * FROM DbTableColumn where ConnectionId = '{id}'";

            var dataTable = _db.GetDataTable(sql);
            return Tool.ToList<DbTableColumn>(dataTable);
        }

        /// <summary>
        /// 根据表格id查询列
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DbTableColumn> GetTableColumnList(params string[] id)
        {
            var idList = id.Select(m => $"'{m}'");
            var sql = $"SELECT * FROM DbTableColumn where TableId in ({string.Join(",", idList)})";
            var dataTable = _db.GetDataTable(sql);
            return Tool.ToList<DbTableColumn>(dataTable);
        }

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteConnections(string ids)
        {
            var id = ids.Split(',').Select(m => $"'{m}'");
            var idStr = string.Join(",", id);
            var sql = $"DELETE FROM DbConnection where Id in ({idStr})";
            _db.ExecuteNonQuery(sql);

            sql = $"DELETE FROM DbTable where ConnectionId in ({idStr})";
            _db.ExecuteNonQuery(sql);

            sql = $"DELETE FROM DbTableColumn where ConnectionId in ({idStr})";
            _db.ExecuteNonQuery(sql);

            return true;
        }

        /// <summary>
        /// 删除表格
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteTables(string ids)
        {
            var id = ids.Split(',').Select(m => $"'{m}'");
            var idStr = string.Join(",", id);

            var sql = $"DELETE FROM DbTable where Id in ({idStr})";
            _db.ExecuteNonQuery(sql);

            sql = $"DELETE FROM DbTableColumn where TableId in ({idStr})";
            _db.ExecuteNonQuery(sql);

            return true;
        }

        public bool DelTables(string id)
        {
            var sql = $"DELETE FROM DbTable where ConnectionId = @id";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@id", id)
            };
            return _db.ExecuteNonQuery(sql, parameters) > 0;
        }

        public bool DelColumns(string id)
        {
            var sql = $"DELETE FROM DbTableColumn where ConnectionId = '{id}'";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@id", id)
            };
            return _db.ExecuteNonQuery(sql, parameters) > 0;
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add<T>(T model)
        {
            var result = false;
            var table = typeof(T).Name;
            var colNames = new List<string>();
            var parameters = new List<SQLiteParameter>();

            foreach (var p in typeof(T).GetProperties())
            {
                parameters.Add(new SQLiteParameter("@" + p.Name, p.GetValue(model)));
                colNames.Add(p.Name);
            }

            if (null != table && parameters.Count > 0)
            {
                var sql = $@"
                    INSERT INTO {table} ({string.Join(',', colNames)})
                    VALUES({string.Join(',', colNames.Select(c => "@" + c))})
                ";

                result = _db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
            }

            return result;
        }

        public bool BatchAdd<T>(List<T> list)
        {
            return _db.BatchAdd(list);  
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update<T>(T model) where T : class, new()
        {
            var result = false;
            var table = typeof(T).Name;
            var colNames = new List<string>();
            var parameters = new List<SQLiteParameter>();
            PropertyInfo pk = null;

            foreach (var p in typeof(T).GetProperties())
            {
                if(p.Name != "Id")
                {
                    parameters.Add(new SQLiteParameter("@" + p.Name, p.GetValue(model)));
                    colNames.Add($"{p.Name}=@{p.Name}");
                }
                else
                {
                    pk = p;
                }
            }

            if (null != pk && parameters.Count > 0)
            {
                parameters.Add(new SQLiteParameter("@" + pk.Name, pk.GetValue(model)));

                var sql = $@"
                    UPDATE {table} SET {string.Join(',', colNames)}
                    WHERE Id=@Id 
                ";

                result = _db.ExecuteNonQuery(sql, parameters.ToArray()) > 0;
            }

            return result;
        }



    }
}