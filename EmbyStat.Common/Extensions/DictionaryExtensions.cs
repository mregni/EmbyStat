﻿using System;
using System.Collections.Generic;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Extensions;

public static class DictionaryExtensions
{
    public static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            items.TryAdd(key, value);
        }
    }

    private static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, IEnumerable<string> values, char separator)
    {
        items.AddIfNotNull(key, string.Join(separator, values));
    }

    public static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, IEnumerable<string> values)
    {
        items.AddIfNotNull(key, values, '|');
    }

    public static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, DateTime? value)
    {
        if (value.HasValue)
        {
            items.TryAdd(key, value.Value.ToString("O"));
        }
    }

    public static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, int? value)
    {
        if (value.HasValue)
        {
            items.TryAdd(key, value.Value.ToString());
        }
    }

    public static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, bool? value)
    {
        if (value.HasValue)
        {
            items.TryAdd(key, value.Value.ToString());
        }
    }
    
    public static void AddIfNotNull<T1>(this Dictionary<T1, string> items, T1 key, SortOrder? value)
    {
        if (value.HasValue)
        {
            items.TryAdd(key, value.Value.ToString());
        }
    }
}