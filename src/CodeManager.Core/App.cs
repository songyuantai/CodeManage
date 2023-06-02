namespace CodeManager.Core
{
    /// <summary>
    /// App
    /// </summary>
    public class App
    {
        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <returns></returns>
        public static string NewGuid() => Guid.NewGuid().ToString();
    }
}