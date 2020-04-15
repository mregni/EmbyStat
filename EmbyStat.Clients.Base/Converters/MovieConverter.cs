using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Clients.Base.Converters
{
    public static class MovieConverter
    {
        public static Movie ConvertToMovie(this BaseItemDto dto, string collectionId)
        {
            var movie = new Movie
            {
                Id = dto.Id,
                CollectionId = collectionId,
                Name = dto.Name,
                ParentId = dto.ParentId,
                OriginalTitle = dto.OriginalTitle,
                DateCreated = dto.DateCreated,
                Path = dto.Path,
                SortName = dto.SortName,
                MediaSources = dto.MediaSources.Select(y => new MediaSource
                {
                    Id = y.Id,
                    Path = y.Path,
                    BitRate = y.Bitrate,
                    Container = y.Container,
                    Protocol = y.Protocol.ToString(),
                    RunTimeTicks = y.RunTimeTicks,
                    SizeInMb = Math.Round(y.Size / (double)1024 / 1024 ?? 0, MidpointRounding.AwayFromZero)
                }).ToList(),
                RunTimeTicks = dto.RunTimeTicks,
                Container = dto.Container,
                CommunityRating = dto.CommunityRating,
                MediaType = dto.MediaType,
                OfficialRating = dto.OfficialRating,
                PremiereDate = dto.PremiereDate,
                ProductionYear = dto.ProductionYear,
                Video3DFormat = dto.Video3DFormat ?? 0,
                Genres = dto.Genres
            };

            dto.MapImageTags(movie);
            dto.MapPeople(movie);
            dto.MapProviderIds(movie);
            dto.MapStreams(movie);
            dto.MapMediaSources(movie);

            return movie;
        }
    }
}
