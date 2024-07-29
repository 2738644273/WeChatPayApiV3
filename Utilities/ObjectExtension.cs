
using Newtonsoft.Json;

namespace WeChatApiBase.Utilities;

public static class ObjectExtension
{
    /// <summary>
    /// 简单对象转换器
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T  SimpleConvert<T>(this Object obj)
    {
        if (obj is null)
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }
    
    
}