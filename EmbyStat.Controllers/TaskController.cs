using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly IMapper _mapper;

        public TaskController(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}