using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        List<Collection> GetMovieCollections();
    }
}
