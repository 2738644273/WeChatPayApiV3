using WeChatApiBase.Entity;

namespace WeChatApiBase.Service;
/// <summary>
/// 微信签名验证服务
/// </summary>
public interface IVertifySignBaseService
{ 
    /// <summary>
    /// 签名验证
    /// </summary>
    /// <param name="certificatesParams"></param>
    /// <returns></returns>
    Task<bool> VertifySignAsync(Certificates certificatesParams);
}