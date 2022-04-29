using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Services.Models.Cards;
using ValueType = EmbyStat.Services.Models.Cards.ValueType;

namespace EmbyStat.Services.Converters;

public static class MediaTopCardHelper
{
    #region Media

    public static TopCard ConvertToTopCard(this IEnumerable<Media> list, string title, string unit,
        string valueSelector, ValueType valueType)
    {
        return ConvertToTopCard(list, title, unit, valueSelector, valueType, true);
    }

    private static TopCard ConvertToTopCard(IEnumerable<Media> list, string title, string unit, string valueSelector,
        ValueType valueType, bool unitNeedsTranslation)
    {
        var values = list.Select(x =>
        {
            var propertyInfo = typeof(Media).GetProperty(valueSelector);
            var value = propertyInfo?.GetValue(x, null)?.ToString();
            if (propertyInfo?.PropertyType == typeof(DateTime?) && !string.IsNullOrWhiteSpace(value))
            {
                value = DateTime.Parse(value).ToString("s");
            }

            return new TopCardItem
            {
                Value = value,
                Label = x.Name,
                MediaId = x.Id,
                Image = x.Primary
            };
        }).ToArray();


        return new TopCard
        {
            Title = title,
            Values = values,
            Unit = unit,
            UnitNeedsTranslation = unitNeedsTranslation,
            ValueType = valueType
        };
    }

    #endregion
}

public static class ExtraTopCardHelper
{

    #region Extra

    public static TopCard ConvertToTopCard<T>(this Dictionary<T, int> list, string title, string unit, bool unitNeedsTranslation) where T : Extra
    {
        var values = list.Select(x => new TopCardItem
        {
            Value = x.Value.ToString(),
            Label = x.Key.Name,
            MediaId = x.Key.Id,
            Image = x.Key.Primary
        }).ToArray();

        return new TopCard
        {
            Title = title,
            Values = values,
            Unit = unit,
            UnitNeedsTranslation = unitNeedsTranslation,
            ValueType = ValueType.None
        };
    }
        
    public static TopCard ConvertToTopCard(this IEnumerable<Show> list, string title, string unit, bool unitNeedsTranslation, ValueType type)
    {
        var values = list.Select(x => new TopCardItem
        {
            Value = x.SizeInMb.ToString(CultureInfo.CurrentCulture),
            Label = x.Name,
            MediaId = x.Id,
            Image = x.Primary
        }).ToArray();

        return new TopCard
        {
            Title = title,
            Values = values,
            Unit = unit,
            UnitNeedsTranslation = unitNeedsTranslation,
            ValueType = type
        };
    }
        
    public static TopCard ConvertToTopCard(this IEnumerable<Extra> list, string title, string unit, string valueSelector,
        ValueType valueType)
    {
        return ConvertToTopCard(list, title, unit, valueSelector, valueType, true);
    }

    public static TopCard ConvertToTopCard(this IEnumerable<Extra> list, string title, string unit,
        string valueSelector, bool unitNeedsTranslation)
    {
        return ConvertToTopCard(list, title, unit, valueSelector, ValueType.None, unitNeedsTranslation);
    }

    private static TopCard ConvertToTopCard(IEnumerable<Extra> list, string title, string unit, string valueSelector,
        ValueType valueType, bool unitNeedsTranslation)
    {
        var values = list.Select(x =>
        {
            var propertyInfo = typeof(Extra).GetProperty(valueSelector);
            var value = propertyInfo?.GetValue(x, null)?.ToString();
            if (propertyInfo?.PropertyType == typeof(DateTime?) && !string.IsNullOrWhiteSpace(value))
            {
                value = DateTime.Parse(value).ToString("s");
            }

            return new TopCardItem
            {
                Value = value,
                Label = x.Name,
                MediaId = x.Id,
                Image = x.Primary
            };
        }).ToArray();


        return new TopCard
        {
            Title = title,
            Values = values,
            Unit = unit,
            UnitNeedsTranslation = unitNeedsTranslation,
            ValueType = valueType
        };
    }

    #endregion
}