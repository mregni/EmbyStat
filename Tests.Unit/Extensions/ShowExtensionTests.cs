//TODO add tests
using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Extensions
{
    public class ShowExtensionTests
    {
        private readonly Show _show;

        public ShowExtensionTests()
        {
            _show = new ShowBuilder(Guid.NewGuid().ToString())
                .AddSeason("0")
                .AddSeason("1")
                .AddSeason("2")
                .Build();
        }

    }
}