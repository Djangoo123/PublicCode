using CompagnyTools.Entities;
using CompagnyTools.Interface;
using CompagnyTools.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompagnyTools.Controllers
{
    [Route("api/officeData")]
    [ApiController]
    public class OfficeDataController : ControllerBase
    {
        private readonly IOffice _iOffice;

        public OfficeDataController(IOffice iOffice)
        {
            this._iOffice = iOffice;

        }

        [HttpGet("getData")]
        public DeskModel? GetData()
        {
            var result = _iOffice.OfficeData();
            return result;
        }
    }
}
