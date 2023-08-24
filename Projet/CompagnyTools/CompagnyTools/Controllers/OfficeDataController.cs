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

        /// <summary>
        /// Retrieving data from the office to create our map
        /// </summary>
        /// <returns>office data (desks)</returns>
        [HttpGet("getData")]
        public ActionResult<List<DeskModel>> GetData()
        {
            try
            {
                List<DeskModel> result = _iOffice.OfficeData();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }


        /// <summary>
        /// Updating our office grid data
        /// </summary>
        /// <param name="model">Office map data</param>
        /// <returns>office data (desks)</returns>
        [HttpPost("updateOfficeMap")]
        public ActionResult<List<DeskModel>> UpdateOfficeMap([FromBody] List<DeskModel> model)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
