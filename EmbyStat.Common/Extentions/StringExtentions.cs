using System.IO;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Extentions
{
    public static class StringExtentions
    {
        public static bool ToBoolean(this string value)
        {
            return value == "True";
        }

        public static CollectionType ToCollectionType(this string value)
        {
            switch (value)
            {
                case "movies": return CollectionType.Movies;
                case "tvshows": return CollectionType.TvShow;
                case "music": return CollectionType.Music;
                case "musicvideos": return CollectionType.MusicVideos;
                case "trailers": return CollectionType.Trailers;
                case "homevideos": return CollectionType.HomeVideos;
                case "books": return CollectionType.Books;
                case "photos": return CollectionType.Photos;
                case "games": return CollectionType.Games;
                case "livetv": return CollectionType.LiveTv;
                case "playlists": return CollectionType.Playlists;
                case "folders": return CollectionType.Folders;
                default: return CollectionType.Other;
            }
        }

        public static string ToCleanVersionString(this string version)
        {
            return version.Replace("-dev", string.Empty).Replace("-beta", string.Empty);
        }

        public static string GetLocalPath(this string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), path);
        }
    }
}
