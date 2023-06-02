using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeManager.Core
{
    public static class Tool
    {

        /// <summary>
        /// 把列表转换为DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            var type = typeof(T);

            var properties = type.GetProperties().ToList();

            var newDt = new DataTable(type.Name);

            properties.ForEach(propertie =>
            {
                Type columnType;
                if (propertie.PropertyType.IsGenericType && propertie.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = propertie.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    columnType = propertie.PropertyType;
                }

                newDt.Columns.Add(propertie.Name, columnType);
            });

            foreach (var item in list)
            {
                var newRow = newDt.NewRow();

                properties.ForEach(propertie =>
                {
                    newRow[propertie.Name] = propertie.GetValue(item, null) ?? DBNull.Value;
                });

                newDt.Rows.Add(newRow);
            }

            return newDt;
        }

        /// <summary>
        /// dataTable转list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(DataTable table) where T : class, new()
        {
            var result = new List<T>();
            if (null != table && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var model = new T();
                    foreach (PropertyInfo current in typeof(T).GetProperties())
                    {
                        var filedName = current.Name;
                        if (table.Columns.Contains(filedName))
                        {
                            var value = Convert.ChangeType(row[filedName], current.PropertyType);
                            current.SetValue(model, value);
                        }

                    }

                    result.Add(model);
                }

            }

            return result;
        }

        /// <summary>
        /// 字符串安全转整形
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(string value, int defaultValue = 0)
        {
            if(!int.TryParse(value, out int result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static string GetTypeFromDbType(string dbType, bool isNullable, int precision)
        {
            string result;
            if("NUMBER" == dbType)
            {
                if(precision > 0)
                {
                    result = isNullable ? "decimal?" : "decimal";
                }
                else
                {
                    result = isNullable ? "int?" : "int";
                }
            } 
            else if("DATE" == dbType)
            {
                result = isNullable ? "DateTime?" : "DateTime";
            }
            else if("BLOB" == dbType)
            {
                result = "byte[]";
            }
            else
            {
                result = "string";
            }
            return result;
        }

        public static string PascalToCamel(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var array = value.Split("_").Select(m => m.Length <= 1 ? m.ToUpper() : m.Substring(0, 1).ToUpper() + m[1..].ToLower());
            return string.Concat(array);
        }

        /// <summary>
        /// 从IDataReader转换为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(IDataReader dr) where T : class, new()
        {
            var result = new List<T>();
            if (null != dr)
            {
                var schemaTable = dr.GetSchemaTable();
                try
                {
                    while (dr.Read())
                    {
                        var model = new T();
                        foreach (PropertyInfo current in typeof(T).GetProperties())
                        {
                            var filedName = current.Name;

                            if (schemaTable.Select($"ColumnName='{filedName}'").Length > 0)
                            {
                                if (dr.GetValue(dr.GetOrdinal(filedName)) != DBNull.Value)
                                {
                                    var value = Convert.ChangeType(dr.GetValue(dr.GetOrdinal(filedName)), current.PropertyType);
                                    current.SetValue(model, value);
                                }
                            }

                        }

                        result.Add(model);
                    }
                }
                finally
                {
                    dr.Dispose();
                }

            }

            return result;
        }
    }
}
