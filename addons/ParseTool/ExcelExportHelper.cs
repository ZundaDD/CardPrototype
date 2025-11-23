using System;
using System.Collections.Generic;
using System.Linq;

namespace ParseTool;

public static class Parser
{
    public static string GetParseSuffix(string type)
    {
        return type switch
        {
            "int" => "Int",
            "float" => "Float",
            "string" => "String",
            "bool" => "Bool",

            var ls when ls.StartsWith("List<") => ls,

            var enumT => $"Enum<{enumT}>"
        };
    }
   
    public static int ParseInt(Dictionary<string, string> row, string key, int defaultVal = 0)
    {
        if (row.TryGetValue(key, out var val)) return int.TryParse(val, out var res) ? res : defaultVal;
        return defaultVal;
    }

    public static int GetInt(string val)
      => int.TryParse(val, out var res) ? res : default;

    public static float ParseFloat(Dictionary<string, string> row, string key, float defaultVal = 0f)
    {
        if (row.TryGetValue(key, out var val)) return float.TryParse(val, out var res) ? res : defaultVal;
        return defaultVal;
    }

    public static float GetFloat(string val)
      => float.TryParse(val, out var res) ? res : default;

    public static string ParseString(Dictionary<string, string> row, string key)
    {
        return row.TryGetValue(key, out var val) ? val : string.Empty;
    }

    public static bool ParseBool(Dictionary<string, string> row, string key)
    {
        if (row.TryGetValue(key, out var val)) return bool.Parse(val);
        return false;
    }

    public static bool GetBool(string val)
      => bool.TryParse(val, out var res) ? res : default;

    public static List<T> ParseList<T>(Dictionary<string, string> row, string key) where T : class
    {
        var type = typeof(T).Name;
        if (row.TryGetValue(key, out var val))
        {
            var tokens = string.IsNullOrEmpty(val) ? [] : val.Split('|');
            return type switch
            {
                "Int32" => tokens.Select(GetInt).ToList() as List<T>,
                "Single" => tokens.Select(GetFloat).ToList() as List<T>,
                "String" => tokens.ToList() as List<T>,
                "Boolean" => tokens.Select(GetBool).ToList() as List<T>,
                _ => throw new NotImplementedException($"不支持{type}类型")
            }; 
        }
        return new([]);
    }

    public static T ParseEnum<T>(Dictionary<string, string> row, string key) where T : struct
    {
        if (row.TryGetValue(key, out var val) && Enum.TryParse<T>(val, true, out var res))
        {
            return res;
        }
        return default;
    }
}
