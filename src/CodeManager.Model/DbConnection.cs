namespace CodeManager.Model
{
    /// <summary>
    /// 数据库链接
    /// </summary>
    public class DbConnection
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectString { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; } = "Oracle";
    }
}