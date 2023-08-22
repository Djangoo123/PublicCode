using CompagnyTools.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompagnyTools.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficeDataController : ControllerBase
    {
        // GET: OfficeDataController
        public List<User>? Index()
        {
            return null;
        }
    }
}
