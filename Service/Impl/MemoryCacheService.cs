using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace WeChatApiBase.Service.Impl;
/// <summary>
/// 内存缓存
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    #region Basic Operations

    public string Get(string key)
    {
        if (_memoryCache.TryGetValue(key, out string value))
            return value;
        return null;
    }

    public bool Remove(string key)
    {
        _memoryCache.Remove(key);
        return true;
    }

    public void RemoveAll(IEnumerable<string> keys)
    {
        foreach (var key in keys)
            _memoryCache.Remove(key);
    }

    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public bool Expire(string key, int expireSeconds = -1)
    {
        if (_memoryCache.TryGetValue(key, out object value))
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
            return true;
        }

        return false;
    }

    #endregion

    #region String Cache

    public bool Add(string key, string value, int expireSeconds = -1)
    {
        try
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool AddNx(string key, string value)
    {
        return _memoryCache.TryGetValue(key, out _) ? false : Add(key, value);
    }

    public bool AddNxExpire(string key, string value, int expireSeconds = -1)
    {
        if (_memoryCache.TryGetValue(key, out _))
            return false;
        return Add(key, value, expireSeconds);
    }

    public bool AddObject(string key, object value, int expireSeconds = -1)
    {
        try
        {
            _memoryCache.Set(key, JsonConvert.SerializeObject(value), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds)
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public T GetObject<T>(string key) where T : class
    {
        if (_memoryCache.TryGetValue(key, out string value))
            return JsonConvert.DeserializeObject<T>(value);
        return null;
    }

    #endregion

    #region Hash Cache

    public void AddHash(string key, Dictionary<string, string> value)
    {
        _memoryCache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
    }

    public void AddHash<T>(string key, T t)
    {
        _memoryCache.Set(key, JsonConvert.SerializeObject(t), new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
    }

    public T GetHash<T>(string key) where T : class
    {
        if (_memoryCache.TryGetValue(key, out string value))
            return JsonConvert.DeserializeObject<T>(value);
        return null;
    }

    #endregion

    #region List Cache

    public void LPush(string key, string val)
    {
        if (_memoryCache.TryGetValue(key, out List<string> list))
        {
            list.Insert(0, val);
            _memoryCache.Set(key, list, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }
        else
        {
            _memoryCache.Set(key, new List<string> { val }, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }
    }

    public void RPush(string key, string val)
    {
        if (_memoryCache.TryGetValue(key, out List<string> list))
        {
            list.Add(val);
            _memoryCache.Set(key, list, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }
        else
        {
            _memoryCache.Set(key, new List<string> { val }, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }
    }

    public object ListDequeue(string key)
    {
        if (_memoryCache.TryGetValue(key, out List<string> list) && list.Count > 0)
        {
            var value = list[0];
            list.RemoveAt(0);
            _memoryCache.Set(key, list, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
            return value;
        }

        return null;
    }

    public T ListDequeue<T>(string key) where T : class
    {
        if (_memoryCache.TryGetValue(key, out List<string> list) && list.Count > 0)
        {
            var value = list[0];
            list.RemoveAt(0);
            _memoryCache.Set(key, list, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
            return JsonConvert.DeserializeObject<T>(value);
        }

        return null;
    }

    public void ListRemove(string key, int keepIndex)
    {
        if (_memoryCache.TryGetValue(key, out List<string> list) && list.Count > keepIndex)
        {
            list.RemoveRange(0, keepIndex);
            _memoryCache.Set(key, list, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }
    }

    #endregion

    public object Eval(string script, string key, params object[] args)
    {
        throw new NotImplementedException();
    }
}