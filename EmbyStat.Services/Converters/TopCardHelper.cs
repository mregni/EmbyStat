using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Services.Models.Cards;

namespace EmbyStat.Services.Converters
{
    public static class TopCardHelper
    {
        public static TopCard ConvertToTopCard<T>(T[] list, string title, string unit, string valueSelector, ValueTypeEnum valueTypeEnum, bool unitNeedsTranslation) where T : Extra
        {
            var values = list.Select(x =>
            {
                var propertyInfo = typeof(T).GetProperty(valueSelector);
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
                ValueType = valueTypeEnum
            };
        }

        public static TopCard ConvertToTopCard(this Movie[] list, string title, string unit, string valueSelector, ValueTypeEnum valueTypeEnum)
        {
            return ConvertToTopCard(list, title, unit, valueSelector, valueTypeEnum, true);
        }

        public static TopCard ConvertToTopCard(this Movie[] list, string title, string unit, string valueSelector, bool unitNeedsTranslation)
        {
            return ConvertToTopCard(list, title, unit, valueSelector, ValueTypeEnum.None, unitNeedsTranslation);
        }

        public static TopCard ConvertToTopCard(this Show[] list, string title, string unit, string valueSelector, ValueTypeEnum valueTypeEnum)
        {
            return ConvertToTopCard(list, title, unit, valueSelector, valueTypeEnum, true);
        }

        public static TopCard ConvertToTopCard(this Show[] list, string title, string unit, string valueSelector, bool unitNeedsTranslation)
        {
            return ConvertToTopCard(list, title, unit, valueSelector, ValueTypeEnum.None, unitNeedsTranslation);
        }

        public static TopCard ConvertToTopCard(this Dictionary<Show, int> list, string title, string unit)
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
                UnitNeedsTranslation = false,
                ValueType = ValueTypeEnum.None
            };
        }

        public static TopCard ConvertToTopCard(this Person[] list, string title, string unit, string valueSelector, int count = 5)
        {
            var values = list
                .Where(x => x != null)
                .Select(x =>
                {
                    var propertyInfo = typeof(Person).GetProperty(valueSelector);
                    var value = propertyInfo?.GetValue(x, null).ToString();
                    if (propertyInfo?.PropertyType == typeof(DateTime?))
                    {
                        value = DateTime.Parse(value).ToString("O");
                    }

                    return new TopCardItem
                    {
                        Value = value,
                        Label = x.Name,
                        MediaId = x.Id,
                        Image = x.Primary
                    };
                })
                .Take(count)
                .ToArray();

            if (!values.Any())
            {
                return null;
            }

            return new TopCard
            {
                Title = title,
                Values = values,
                Unit = unit,
                UnitNeedsTranslation = false,
                ValueType = ValueTypeEnum.None
            };
        }
    }
}
