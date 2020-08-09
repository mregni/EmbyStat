using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models.Entities.Helpers;

namespace Tests.Unit.Builders
{
    public class SubtitleStreamBuilder
    {
        private readonly SubtitleStream _stream;

        public SubtitleStreamBuilder(string language)
        {
            _stream = new SubtitleStream
            {
                Language = language,
                Codec = "codec",
                DisplayTitle = language,
                Id = Guid.NewGuid().ToString(),
                IsDefault = false
            };
        }
        public SubtitleStream Build()
        {
            return _stream;
        }
    }
}
