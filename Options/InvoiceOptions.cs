namespace WeChatApiBase.Options;

/// <summary>
/// 发票配置项
/// </summary>
public class InvoiceOptions
{
    /// <summary>
    /// 发票通知回调地址
    /// </summary>
    public string Invoice_Callback_Url { get; set; }
    /// <summary>
    /// 场景
    /// </summary>
    public string scene { get; set; }

}