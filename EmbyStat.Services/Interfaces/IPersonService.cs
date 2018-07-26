using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models;

namespace EmbyStat.Services.Interfaces
{
    public interface IPersonService
    {
        Task<Person> GetPersonById(Guid id);
    }
}
