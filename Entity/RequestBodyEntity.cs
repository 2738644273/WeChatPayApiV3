using WeChatApiBase.Options;

namespace WeChatApiBase.Entity;

public class RequestBodyEntity<T> : RequestBaseEntity
{
    /// <summary>
    /// 请求实体
    /// </summary>
    public T RequestBody { get; set; }

    public RequestBodyEntity<T> BuildBody(T body)
    {
        RequestBody = body;
        return this;
    }
    public static RequestBodyEntity<T> BuildEntityFromOptions(WeChatPayOptions options)
    {
        return new RequestBodyEntity<T>()
        {
            Mchid = options.Mchid,
            PrivateKey = options.PrivateKey,
            Apikey = options.Apikey,
            CertSerialnumber = options.CertSerialnumber
        };
    }
}