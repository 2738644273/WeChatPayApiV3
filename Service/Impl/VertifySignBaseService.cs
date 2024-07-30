using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using WeChatApiBase.Entity;
using WeChatApiBase.Entity.Certs;
using WeChatApiBase.Utilities;

namespace WeChatApiBase.Service.Impl;

public class VertifySignBaseService : IVertifySignBaseService
{
    /// <summary>
    /// 网关
    /// </summary>
    private const string APIGETWAY = "https://api.mch.weixin.qq.com";

    private const string WeChatCertificate = "WeChat:Vertify:Certificate";
    private readonly WeChatV3Helper _weChatV3Helper;
    private readonly ILogger _logger;
    private readonly ICacheService _cacheService;

    public VertifySignBaseService(
        WeChatV3Helper weChatV3Helper,
        ILogger logger,
        ICacheService cacheService)
    {
        _cacheService = cacheService;
        _weChatV3Helper = weChatV3Helper;
        _logger = logger;
    }

    /// <summary>
    /// 验证签名
    /// </summary>
    /// <param name="certificatesParams"></param>
    /// <returns></returns>
    public async Task<bool> VertifySignAsync(Certificates certificatesParams)
    {
        try
        {
            #region 请求API获取证书

            var certs = _cacheService.GetObject<CertificatesRet>(WeChatCertificate);
            if (certs == null)
            {
                _logger.LogInformation("没有发现证书数据，初始化证书缓存");
                string _certUrl = APIGETWAY + "/v3/certificates";
                Certificates certificates = certificatesParams.SimpleConvert<Certificates>();
                var response = await _weChatV3Helper.GetRequestAsync(_certUrl, certificates);
                string body = await response.Content.ReadAsStringAsync();
                certs = JsonConvert.DeserializeObject<CertificatesRet>(body); //证书列表
                _cacheService.AddObject(WeChatCertificate, certs,11*3600);
            }

            #endregion

            #region 解密证书

            List<X509Certificate2> x509Certs = new List<X509Certificate2>();
            foreach (var item in certs.data)
            {
                CertsModel model = new CertsModel()
                {
                    SerialNo = item.serial_no,
                    EffectiveTime = item.effective_time,
                    ExpireTime = item.expire_time,
                    PlainCertificate = AesGcmHelper.AesGcmDecrypt(item.encrypt_certificate.associated_data,
                        item.encrypt_certificate.nonce, item.encrypt_certificate.ciphertext,
                        certificatesParams.Apikey)
                };
                X509Certificate2 x509Cert = new X509Certificate2(Encoding.UTF8.GetBytes(model.PlainCertificate));
                x509Certs.Add(x509Cert);
            }

            #endregion

            #region 签名校验

            var cert = x509Certs.Where(r => r.SerialNumber == certificatesParams.WebhookSerialNumber)
                .FirstOrDefault(); //根据序列号获取证书
            var msg = _weChatV3Helper.BuildMessageString(certificatesParams.WebhookTimestamp,
                certificatesParams.WebhookNonce,
                certificatesParams.WebhookBody);

            byte[] data = Encoding.UTF8.GetBytes(msg);

            var rsaParam = cert.GetRSAPublicKey().ExportParameters(false);
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParam);

            var isOk = rsa.VerifyData(data, CryptoConfig.MapNameToOID("SHA256"),
                Convert.FromBase64String(certificatesParams.WebhookSignature)); //签名校验

            return isOk;

            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "证书申请失败");
            throw;
        }
    }
}