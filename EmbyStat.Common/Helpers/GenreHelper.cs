﻿using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;
using MediaBrowser.Model.Dto;

namespace EmbyStat.Common.Helpers
{
    public static class GenreHelper
    {
        public static Genre ConvertToGenre(BaseItemDto genre)
        {
            return new Genre
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
    }
}
