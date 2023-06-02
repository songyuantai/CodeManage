using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManager.Model
{
    public class DbTable
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 连接Id
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表空间名称
        /// </summary>
        public string TableSpaceName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
    }
}
