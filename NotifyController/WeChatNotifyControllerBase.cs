using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeChatApiBase.Entity;
using WeChatApiBase.Entity.Notify;
using WeChatApiBase.Options;
using WeChatApiBase.Service;
using WeChatApiBase.Utilities;

namespace WeChatApiBase.NotifyController;

/// <summary>
/// 微信通知回调基础控制器
/// </summary>

public abstract class WeChatNotifyControllerBase:ControllerBase
{
    protected readonly ILogger _logger;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IOptions<WeChatPayOptions> _wechatOptions;
    protected readonly IVertifySignBaseService _vertifySignBaseService;
    public WeChatNotifyControllerBase(ILogger<WeChatNotifyControllerBase> logger,
        IHttpContextAccessor httpContextAccessor,
        IOptions<WeChatPayOptions> wechatOptions,
        IVertifySignBaseService vertifySignBaseService
    )
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _wechatOptions = wechatOptions;
        _vertifySignBaseService = vertifySignBaseService;
    }
    /// <summary>
    /// 通知
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected abstract Task<IActionResult> CallBack(NotifyModel notifyModel,string data);
      /// <summary>
    /// 微信支付回调通知接口
    /// </summary>
    /// <returns></returns>
    [HttpPost("notify-url")]
    public async Task<IActionResult> NotifyUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        try
        {
            #region 获取微信回调内容
            string requestBodyJson = String.Empty;
            using (var srd = new StreamReader(request.Body))
            {
                requestBodyJson = await srd.ReadToEndAsync();
            }
            #endregion
            #region 验证签名防止数据伪造
            string signature = request.Headers["Wechatpay-Signature"];
            string timestamp = request.Headers["Wechatpay-Timestamp"];
            string nonce = request.Headers["Wechatpay-Nonce"];
            string serial = request.Headers["Wechatpay-Serial"];
            _logger.LogInformation($"获取到timestamp:{timestamp} ,nonce:{nonce} ,signature:{signature},serial:{serial}");
            if (string.IsNullOrWhiteSpace(signature))
            {
                throw new Exception("请求没有签名！");
            }

           var cparams =  new Certificates()
            {
                WebhookSerialNumber = serial,
                WebhookBody = requestBodyJson,
                WebhookNonce = nonce,
                WebhookSignature = signature,
                WebhookTimestamp = timestamp,
                Apikey = _wechatOptions.Value.Apikey,
                CertSerialnumber = _wechatOptions.Value.CertSerialnumber,
                Mchid = _wechatOptions.Value.Mchid,
                PrivateKey = _wechatOptions.Value.PrivateKey
            };
            var isOK = await _vertifySignBaseService.VertifySignAsync(cparams);
            //签名是否通过
            if (!isOK)
            {
                _logger.LogWarning($"签名验证失败：timestamp:{timestamp} ,nonce:{nonce} ,signature:{signature}");
                return new JsonResult(new { code = "401", message = "sign vertify fail" });
            }
            #endregion

            #region 解密内容
            string APIV3Key = _wechatOptions.Value.Apikey;
            NotifyModel notifyUrlModel = JsonConvert.DeserializeObject<NotifyModel>(requestBodyJson);
            var decryptStr = AesGcmHelper.AesGcmDecrypt(notifyUrlModel.resource.associated_data, notifyUrlModel.resource.nonce, notifyUrlModel.resource.ciphertext, APIV3Key);
           _logger.LogInformation("解密数据："+decryptStr);
            return await CallBack(notifyUrlModel,decryptStr);
            #endregion
 
             
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new JsonResult(new { code = "FAIL", message = e.Message });
        }
    }

      
}
