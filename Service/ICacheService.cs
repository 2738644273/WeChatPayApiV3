namespace WeChatApiBase.Service;

 public interface ICacheService
    {
        #region 基本操作
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        string Get(string key);
 
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Remove(string key);
 
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        void RemoveAll(IEnumerable<string> keys);
 
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Exists(string key);
 
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="expireSeconds">过期时间，单位：秒</param>
        /// <returns></returns>
        bool Expire(string key, int expireSeconds = -1);
        #endregion
 
        #region String类型缓存
        /// <summary>
        /// 添加String缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expireSeconds">缓存时长</param>
        /// <returns></returns>
        bool Add(string key, string value, int expireSeconds = -1);
 
        /// <summary>
        /// 添加String缓存，前提是这个key不存在，否则不执行。
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        bool AddNx(string key, string value);
 
        /// <summary>
        /// 添加String缓存，前提是这个key不存在，否则不执行。
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expireSeconds">缓存时长</param>
        /// <returns></returns>
        bool AddNxExpire(string key, string value, int expireSeconds = -1);
        /// <summary>
        /// 添加String缓存（对象自动转换Json格式存入）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存对象</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <returns></returns>
        bool AddObject(string key, object value, int expireSeconds = -1);
 
        /// <summary>
        /// 获取缓存（Redis存储Json格式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>对象</returns>
        T GetObject<T>(string key) where T : class;
        #endregion
 
        #region Hash类型缓存
        /// <summary>
        /// 添加Hash缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <returns></returns>
        void AddHash(string key, Dictionary<string, string> value);
 
        /// <summary>
        /// 添加Hash缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存Key</param>
        /// <param name="t">缓存对象</param>
        void AddHash<T>(string key, T t);
 
        /// <summary>
        /// 获取Hash缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>对象</returns>
        T GetHash<T>(string key) where T : class;
        #endregion
 
        #region List类型缓存
        /// <summary>
        /// List写入head
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="val">缓存Value</param>
        void LPush(string key, string val);
 
        /// <summary>
        /// List写入尾部
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="val">缓存Value</param>
        void RPush(string key, string val);
 
        /// <summary>
        /// List出队 lpop
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        object ListDequeue(string key);
 
        /// <summary>
        /// List出队 lpop
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        T ListDequeue<T>(string key) where T : class;
 
        /// <summary>
        /// 移除list中的数据，keepIndex为保留的位置到最后一个元素如list 元素为1.2.3.....100
        /// 需要移除前3个数，keepindex应该为4
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="keepIndex"></param>
        void ListRemove(string key, int keepIndex);
        #endregion
 
        #region lua脚本
        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="script">脚本</param>
        /// <param name="key">KEYS[1]</param>
        /// <param name="args">ARGV[1]</param>
        /// <returns></returns>
        object Eval(string script, string key, params object[] args);
        #endregion
    }