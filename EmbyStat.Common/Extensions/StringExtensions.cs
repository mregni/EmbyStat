using System;
using System.IO;
using System.Linq;
using System.Web;
using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToCleanVersionString(this string version)
        {
            return version.Replace("-dev", string.Empty).Replace("-beta", string.Empty);
        }

        public static LibraryType ToLibraryType(this string value)
        {
            switch (value)
            {
                case "movies": return LibraryType.Movies;
                case "tvshows": return LibraryType.TvShow;
                case "music": return LibraryType.Music;
                case "musicvideos": return LibraryType.MusicVideos;
                case "trailers": return LibraryType.Trailers;
                case "homevideos": return LibraryType.HomeVideos;
                case "books": return LibraryType.Books;
                case "photos": return LibraryType.Photos;
                case "games": return LibraryType.Games;
                case "livetv": return LibraryType.LiveTv;
                case "playlists": return LibraryType.Playlists;
                case "folders": return LibraryType.Folders;
                case "boxsets": return LibraryType.BoxSets;
                default: return LibraryType.Other;
            }
        }

        public static string GetLocalPath(this string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), path);
        }

        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };

        public static double[] FormatInputValue(this string value)
        {
            return FormatInputValue(value, 1);
        }

        public static double[] FormatInputValue(this string value, int multiplier)
        {
            var decodedValue = HttpUtility.UrlDecode(value);
            if (decodedValue.Contains('|'))
            {
                var left = Convert.ToDouble(decodedValue.Split('|')[0]) * multiplier;
                var right = Convert.ToDouble(decodedValue.Split('|')[1]) * multiplier;

                //switching sides if user put the biggest number on the left side.
                if (right < left)
                {
                    (left, right) = (right, left);
                }

                return new[] { left, right };
            }

            if (decodedValue.Length == 0)
            {
                return new[] { 0d };
            }

            return new[] { Convert.ToDouble(decodedValue) * multiplier };
        }
    }
}
