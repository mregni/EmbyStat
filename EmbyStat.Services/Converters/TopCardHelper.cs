using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Services.Models;
using EmbyStat.Services.Models.Cards;

namespace EmbyStat.Services.Converters
{
    public static class TopCardHelper
    {
        public static TopCard ConvertToTopCard(this Movie[] list, string title, string unit, string valueSelector, ValueTypeEnum valueTypeEnum, bool unitNeedsTranslation)
        {
            var values = list.Select(x =>
            {
                var propertyInfo = typeof(Movie).GetProperty(valueSelector);
                var value = propertyInfo?.GetValue(x, null).ToString();
                if (propertyInfo?.PropertyType == typeof(DateTimeOffset?))
                {
                    value = DateTimeOffset.Parse(value).ToString("O");
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

        public static TopCard ConvertToTopCard(this Person[] list, string title, string unit, string valueSelector)
        {
            var values = list.Select(x =>
            {
                
                var propertyInfo = typeof(Person).GetProperty(valueSelector);
                var value = propertyInfo?.GetValue(x, null).ToString();
                if (propertyInfo?.PropertyType == typeof(DateTimeOffset?))
                {
                    value = DateTimeOffset.Parse(value).ToString("O");
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
                UnitNeedsTranslation = false,
                ValueType = ValueTypeEnum.None
            };
        }
    }
}
