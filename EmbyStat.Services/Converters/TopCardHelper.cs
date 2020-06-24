using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Services.Models;
using EmbyStat.Services.Models.Cards;

namespace EmbyStat.Services.Converters
{
    public static class TopCardHelper
    {
        public static TopCard ConvertToTopCard(this Movie[] list, string title, string unit, string valueSelector, ValueType valueType, bool unitNeedsTranslation)
        {
            var values = list.Select(x =>
            {
                var propertyInfo = typeof(Movie).GetProperty(valueSelector);
                var value = propertyInfo?.GetValue(x, null).ToString();
                if (propertyInfo?.PropertyType == typeof(DateTimeOffset?))
                {
                    value = DateTimeOffset.Parse(value).ToString("O");
                }

                return new LabelValuePair
                {
                    Value = value,
                    Label = x.Name
                };
            }).ToArray();


            return new TopCard
            {
                Title = title,
                Values = values,
                Unit = unit,
                Image = list[0].Primary,
                MediaId = list[0].Id,
                UnitNeedsTranslation = unitNeedsTranslation,
                ValueType = valueType
            };
        }

        public static TopCard ConvertToTopCard(this Movie[] list, string title, string unit, string valueSelector, ValueType valueType)
        {
            return ConvertToTopCard(list, title, unit, valueSelector, valueType, true);
        }

        public static TopCard ConvertToTopCard(this Movie[] list, string title, string unit, string valueSelector, bool unitNeedsTranslation)
        {
            return ConvertToTopCard(list, title, unit, valueSelector, ValueType.none, unitNeedsTranslation);
        }
    }

    public enum ValueType
    {
        none = 0,
        ticks = 1,
        date = 2
    }
}
