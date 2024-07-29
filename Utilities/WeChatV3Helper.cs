using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using WeChatApiBase.Entity;

namespace WeChatApiBase.Utilities;

/// <summary>
/// 微信v3接口请求
/// </summary>
public class WeChatV3Helper
{
    private readonly IHttpClientFactory _httpClientFactory;
   
    private readonly ILogger<WeChatV3Helper> _logger;

    public WeChatV3Helper(
        IHttpClientFactory httpClientFactory,
        ILogger<WeChatV3Helper> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    
    /// <summary>
    /// 发送带body请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="requestBase"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<HttpResponseMessage>  HasBodyRequestAsync<T>(HttpMethod httpMethod,string url, RequestBodyEntity<T> requestBase)
    {
        string body = JsonConvert.SerializeObject(requestBase.RequestBody,new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        HttpResponseMessage response;
        using (var request = new HttpRequestMessage(httpMethod, url))
        using (var client = _httpClientFactory.CreateClient(url))
        {
            var auth = BuildAuthHearder(httpMethod.ToString().ToUpper(), body, new Uri(url).AbsolutePath, requestBase.PrivateKey,
                requestBase.Mchid, requestBase.CertSerialnumber);
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue(".NET", "6")));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", auth);
            request.Content = new StringContent(body);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
         
            response = await client.SendAsync(request);
        }
 
        return response;
    }

    /// <summary>
    /// 发送get请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="requestBase"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> GetRequestAsync(string url, RequestBaseEntity requestBase)
    {
        HttpResponseMessage response;
        using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        using (var client = _httpClientFactory.CreateClient(url))
        {
            var auth = BuildAuthHearder("GET", "", new Uri(url).AbsolutePath, requestBase.PrivateKey,
                requestBase.Mchid, requestBase.CertSerialnumber);
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue(".NET", "6")));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", auth);
            response = await client.SendAsync(request);
        }

        return response;
    }

    /// <summary>
    /// 发送下载请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="requestBase"></param>
    /// <returns></returns>
    /// <summary>
    /// 发送get请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="requestBase"></param>
    /// <returns></returns>
    public async Task<byte[]> DownloadRequestAsync<T>(string url, RequestBodyEntity<T> requestbody)
    {
        string query = BuildQueryString(requestbody.RequestBody);
        url+=$"?{query}";
        using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        using (var client = _httpClientFactory.CreateClient(url))
        {
            var auth = BuildAuthHearder("GET", "", (new Uri(url).AbsolutePath+ $"?{query}"), requestbody.PrivateKey,
                requestbody.Mchid, requestbody.CertSerialnumber);
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue(".NET", "6")));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", auth);
            HttpResponseMessage response = await client.SendAsync(request);

            return await response.Content.ReadAsByteArrayAsync();

        }
    }
    /// <summary>
    /// 发送get请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="requestBase"></param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> GetRequestAsync<T>(string url, RequestBodyEntity<T> requestbody)
    {
        HttpResponseMessage response;
        string query = BuildQueryString(requestbody.RequestBody);
        url  += $"?{query}";
        using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        using (var client = _httpClientFactory.CreateClient(url))
        {
            var auth = BuildAuthHearder("GET", "", (new Uri(url).AbsolutePath+ $"?{query}"), requestbody.PrivateKey,
                requestbody.Mchid, requestbody.CertSerialnumber);
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue(".NET", "6")));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", auth);
            response = await client.SendAsync(request);
        }

        return response;
    }
    /// <summary>
    /// 构建查询字符串
    /// </summary>
    /// <param name="body"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public string BuildQueryString<T>(T body)
    {
        // 构建查询字符串
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var property in typeof(T).GetProperties())
        {
            var value = property.GetValue(body, null)?.ToString();
            if (value != null)
            {
                query[property.Name] = value;
            }
        }

        return query.ToString();
    }
    /// <summary>
    /// 构建签名消息体
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="nonce"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public string BuildMessageString(string timestamp, string nonce, string body) =>
        $"{timestamp}\n{nonce}\n{body}\n";


    /// <summary>
    /// 构造签名请求头
    /// </summary>
    /// <param name="method"></param>
    /// <param name="body"></param>
    /// <param name="uri"></param>
    /// <param name="privateKey"></param>
    /// <param name="_mchID"></param>
    /// <param name="_serialNo"></param>
    /// <returns></returns>
    public string BuildAuthHearder(string method, string body, string uri, string privateKey, string _mchID,
        string _serialNo)
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        string nonce = Path.GetRandomFileName();

        string message = $"{method}\n{uri}\n{timestamp}\n{nonce}\n{body}\n";
        string signature = RequestSign(message, privateKey);
        string auth =
            $"mchid=\"{_mchID}\",nonce_str=\"{nonce}\",timestamp=\"{timestamp}\",serial_no=\"{_serialNo}\",signature=\"{signature}\"";
        return $"WECHATPAY2-SHA256-RSA2048 {auth}";
    }

    /// <summary>
    /// 请求签名
    /// </summary>
    /// <param name="message"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public string RequestSign(string message, string privateKey)
    {
        byte[] keyData = Convert.FromBase64String(privateKey);
        using (CngKey cngKey = CngKey.Import(keyData, CngKeyBlobFormat.Pkcs8PrivateBlob))
        using (RSACng rsa = new RSACng(cngKey))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        }
    }
}