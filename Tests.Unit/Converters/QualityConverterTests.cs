using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Converters
{
    public class QualityConverterTests
    {
        [Fact]
        public void ConvertToQualityString_Should_Return_Dvd()
        {
            int? quality = null;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.Dvd);
        }

        [Fact]
        public void ConvertToQualityString_Should_Return_4K()
        {
            int? quality = 3800;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.FourK);
        }

        [Fact]
        public void ConvertToQualityString_Should_Return_QHD()
        {
            int? quality = 2500;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.Qhd);
        }

        [Fact]
        public void ConvertToQualityString_Should_Return_FullHD()
        {
            int? quality = 1900;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.FullHd);
        }

        [Fact]
        public void ConvertToQualityString_Should_Return_HDReady()
        {
            int? quality = 1260;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.HdReady);
        }

        [Fact]
        public void ConvertToQualityString_Should_Return_480P()
        {
            int? quality = 700;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.FourHunderdEighty);
        }

        [Fact]
        public void ConvertToQualityString_Should_Return_Dvd_Lower()
        {
            int? quality = null;
            var qualityString = quality.ConvertToQualityString();
            qualityString.Should().Be(Constants.Dvd);
        }
    }
}
