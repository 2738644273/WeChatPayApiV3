namespace WeChatApiBase.Entity.Notify;
/// <summary>
/// 微信支付通过后的回调 模型
/// </summary>

public class NotifyModel
{

        /// <summary>
        /// 通知的唯一ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 通知创建的时间，遵循rfc3339标准格式，
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 通知的资源数据类型，支付成功通知为encrypt-resource
        /// </summary>
        public string resource_type { get; set; }
        /// <summary>
        /// 通知的类型，支付成功通知的类型为TRANSACTION.SUCCESS
        /// </summary>
        public string event_type { get; set; }
  
        /// <summary>
        /// 通知资源数据 json格式，
        /// </summary>
        public NotifyResourceModel resource { get; set; }
   
}