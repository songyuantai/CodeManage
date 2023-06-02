using CodeManager.Core;
using CodeManager.Dal;
using CodeManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManager.Busi
{
    public class BllSysOracle
    {
        DbOracle _db;
        public BllSysOracle(string connectString)
        {
            _db = new DbOracle(connectString);
        }

        public List<DbTable> GetTables(string owerName, string connectionId)
        {
            var sql = $@"SELECT T.OWNER, 
                                T.TABLE_NAME, 
                                T.TABLESPACE_NAME,
                                C.COMMENTS
                         FROM ALL_TABLES T 
                         LEFT JOIN USER_TAB_COMMENTS C ON C.TABLE_NAME = T.TABLE_NAME
                         WHERE OWNER='{owerName}' 
                         ORDER BY T.TABLE_NAME";
            var dataTable = _db.GetDataTable(sql);
            var list = new List<DbTable>();
            foreach (DataRow row in dataTable.Rows)
            {
                var item = new DbTable
                {
                   Id = App.NewGuid(),
                   ConnectionId = connectionId,
                   TableName = row["TABLE_NAME"].ToString(),
                   TableSpaceName = row["TABLESPACE_NAME"].ToString(),
                   Comments = row["COMMENTS"].ToString(),
                };
                list.Add(item);
            }

            return list;
        }

        public List<DbTableColumn> GetColumns(string owner, string connectionId, List<DbTable> tables)
        {
            var pkDics = GetPrimaryKeys(owner);

            var sql = $@"SELECT A.COLUMN_NAME,
                                A.DATA_TYPE,
                                A.NULLABLE,
                                C.COMMENTS, 
                                A.DATA_PRECISION, 
                                A.DATA_SCALE, 
                                A.DATA_LENGTH,
                                A.TABLE_NAME
                        FROM USER_TAB_COLUMNS A 
                        JOIN USER_COL_COMMENTS C ON C.COLUMN_NAME = A.COLUMN_NAME AND C.TABLE_NAME = A.TABLE_NAME
                        JOIN ALL_TABLES T ON T.TABLE_NAME = A.TABLE_NAME
                        WHERE T.OWNER='{owner}'
                        ORDER BY A.COLUMN_ID ASC";
            var dataTable = _db.GetDataTable(sql);
            var list = new List<DbTableColumn>();
            foreach(DataRow row in dataTable.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                var table = tables.FirstOrDefault(t => t.TableName == tableName);
                if(null != table)
                {
                    var item = new DbTableColumn
                    {
                        ColumnName = row["COLUMN_NAME"].ToString(),
                        DataType = row["DATA_TYPE"].ToString(),
                        Nullable = row["NULLABLE"].ToString() == "Y",
                        Comments = row["COMMENTS"].ToString(),
                        DataPrecision = Tool.ToInt(row["DATA_PRECISION"].ToString()),
                        DataScale = Tool.ToInt(row["DATA_SCALE"].ToString()),
                        DataLength = Tool.ToInt(row["DATA_LENGTH"].ToString()),
                        Id = App.NewGuid(),
                        TableId = table.Id,
                        ConnectionId = connectionId,
                    };

                    if(pkDics.Any(p => p.Item1 == item.ColumnName && p.Item2 == tableName))
                    {
                        item.IsPrimaryKey = true;
                    }

                    if (item.DataType == "NUMBER" || item.DataType == "BLOB" || item.DataType == "CLOB")
                    {
                        item.DataLength = 0;
                    }

                    list.Add(item);
                }


            }
            return list;
        }

        public List<(string, string)> GetPrimaryKeys(string owner)
        {
            var sql = @$"SELECT A.COLUMN_NAME, A.TABLE_NAME
                        FROM USER_CONS_COLUMNS A
                        JOIN USER_CONSTRAINTS B ON A.CONSTRAINT_NAME = B.CONSTRAINT_NAME AND B.CONSTRAINT_TYPE = 'P'
                        JOIN ALL_TABLES T ON T.TABLE_NAME = A.TABLE_NAME
                        WHERE T.OWNER='{owner}'";
            var dic = new List<(string, string)>();
            var dt = _db.GetDataTable(sql);
            if (null != dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    dic.Add((row[0].ToString(), row[1].ToString()));
                }
            }
            return dic;
        }

        public void Merge(BllSysDb sysDb, Model.DbConnection connection)
        {
            var connectId = connection.Id;
            sysDb.DelTables(connectId);
            sysDb.DelColumns(connectId);
            var owner = connection.UserName?.ToUpper();
            var tables = GetTables(owner, connectId);
            var columns = GetColumns(owner, connectId, tables);
            sysDb.BatchAdd(tables);
            sysDb.BatchAdd(columns);
        }
    }
}
