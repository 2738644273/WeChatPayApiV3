namespace WeChatApiBase.Options;

/// <summary>
/// 微信支付配置项
/// </summary>
public class WeChatPayOptions
{
    public string PrivateKey { get; set; }

    public string Mchid { get; set; }
    public string CertSerialnumber { get; set; }
    public string Apikey { get; set; }
    public string AppId { get; set; }
    
    public InvoiceOptions Invoice { get; set; }
}
