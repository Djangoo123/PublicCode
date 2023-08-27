using AutoMapper;
using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CompagnyTools.Controllers
{
    [Route("api/officeData")]
    [ApiController]
    public class OfficeDataController : ControllerBase
    {
        private readonly IOffice _iOffice;
        private readonly IMapper _mapper;

        public OfficeDataController(IOffice iOffice, IMapper mapper)
        {
            _iOffice = iOffice;
            _mapper = mapper;

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
                //List<DataOffice> userViewModel = _mapper.Map<List<DataOffice>>(result);
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
                _iOffice.UpdateOfficeData(model);
                return Ok(model);
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
        [HttpPost("createAMap")]
        public ActionResult<List<DeskModel>> CreateAMap([FromBody] MapCreationModel model)
        {
            try
            {
                _iOffice.CreateAMap(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
