using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManager.Model
{
    public class DbTableColumn
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 连接id
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 表id
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 数据精度
        /// </summary>
        public int DataPrecision { get; set; }

        /// <summary>
        /// 有效位数
        /// </summary>
        public int DataScale { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public decimal DataLength { get; set; }

    }
}
