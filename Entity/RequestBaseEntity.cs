using WeChatApiBase.Options;

namespace WeChatApiBase.Entity;

public class RequestBaseEntity
{
    public string Mchid { get; set; }
    public string PrivateKey { get; set; }

    public string Apikey { get; set; }

    /// <summary>
    /// 证书序列号
    /// </summary>
    public string CertSerialnumber { get; set; }

    public static RequestBaseEntity BuildEntityFromOptions(WeChatPayOptions options)
    {
        return new RequestBaseEntity()
        {
            Mchid = options.Mchid,
            PrivateKey = options.PrivateKey,
            Apikey = options.Apikey,
            CertSerialnumber = options.CertSerialnumber
        };
    }
}