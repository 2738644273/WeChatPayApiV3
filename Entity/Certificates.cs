namespace WeChatApiBase.Entity;

public class Certificates:RequestBaseEntity
{
    /// <summary>
    /// 微信回调通知中的 Wechatpay-Timestamp 标头
    /// </summary>
    public string WebhookTimestamp { get; set; }
    /// <summary>
    /// 微信回调通知中的 Wechatpay-Nonce 标头
    /// </summary>
    public string WebhookNonce { get; set; }
    /// <summary>
    /// 微信回调通知中请求正文
    /// </summary>
    public string WebhookBody { get; set; }
    /// <summary>
    /// 微信回调通知中的 Wechatpay-Signature 标头
    /// </summary>
    public string WebhookSignature { get; set; }
    /// <summary>
    /// 微信回调通知中的 Wechatpay-Serial 标头
    /// </summary>
    public string WebhookSerialNumber { get; set; }
    
}