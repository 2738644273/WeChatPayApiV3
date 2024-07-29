using Newtonsoft.Json;
using StackExchange.Redis;

namespace WeChatApiBase.Service.Impl;
/// <summary>
/// 分布式缓存
/// </summary>
 public class RedisCacheService : ICacheService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisCacheService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }

    public string Get(string key)
    {
        return _database.StringGet(key);
    }

    public bool Remove(string key)
    {
        return _database.KeyDelete(key);
    }

    public void RemoveAll(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            _database.KeyDelete(key);
        }
    }

    public bool Exists(string key)
    {
        return _database.KeyExists(key);
    }

    public bool Expire(string key, int expireSeconds = -1)
    {
        if (expireSeconds > 0)
        {
            return _database.KeyExpire(key, TimeSpan.FromSeconds(expireSeconds));
        }
        return false;
    }

    public bool Add(string key, string value, int expireSeconds = -1)
    {
        if (expireSeconds > 0)
        {
            return _database.StringSet(key, value, TimeSpan.FromSeconds(expireSeconds));
        }
        return _database.StringSet(key, value);
    }

    public bool AddNx(string key, string value)
    {
        return _database.StringSet(key, value, when: When.NotExists);
    }

    public bool AddNxExpire(string key, string value, int expireSeconds = -1)
    {
        if (expireSeconds > 0)
        {
            return _database.StringSet(key, value, TimeSpan.FromSeconds(expireSeconds), When.NotExists);
        }
        return _database.StringSet(key, value, null,When.NotExists);
    }

    public bool AddObject(string key, object value, int expireSeconds = -1)
    {
        var json = JsonConvert.SerializeObject(value);
        return Add(key, json, expireSeconds);
    }

    public T GetObject<T>(string key) where T : class
    {
        var value = _database.StringGet(key);
        return value.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<T>(value);
    }

    public void AddHash(string key, Dictionary<string, string> value)
    {
        var entries = new HashEntry[value.Count];
        int i = 0;
        foreach (var kvp in value)
        {
            entries[i++] = new HashEntry(kvp.Key, kvp.Value);
        }
        _database.HashSet(key, entries);
    }

    public void AddHash<T>(string key, T t)
    {
        var json = JsonConvert.SerializeObject(t);
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        AddHash(key, dict);
    }

    public T GetHash<T>(string key) where T : class
    {
        var entries = _database.HashGetAll(key);
        if (entries.Length == 0) return null;

        var dict = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            dict[entry.Name] = entry.Value;
        }

        var json = JsonConvert.SerializeObject(dict);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public void LPush(string key, string val)
    {
        _database.ListLeftPush(key, val);
    }

    public void RPush(string key, string val)
    {
        _database.ListRightPush(key, val);
    }

    public object ListDequeue(string key)
    {
        return _database.ListLeftPop(key);
    }

    public T ListDequeue<T>(string key) where T : class
    {
        var value = _database.ListLeftPop(key);
        return value.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<T>(value);
    }

    public void ListRemove(string key, int keepIndex)
    {
        var length = _database.ListLength(key);
        if (length > keepIndex)
        {
            _database.ListTrim(key, keepIndex, length - 1);
        }
    }

    public object Eval(string script, string key, params object[] args)
    {
        var prepared = LuaScript.Prepare(script);
        return _database.ScriptEvaluate(prepared, new { key, args });
    }

    public void Dispose()
    {
        _redis.Dispose();
    }
}