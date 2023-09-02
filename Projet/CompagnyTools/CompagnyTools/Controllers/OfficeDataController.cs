using AutoMapper;
using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CompagnyTools.Controllers
{
    [Route("api/[controller]")]
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        /// <summary>
        /// Entry point for updating our office grid data
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
        /// Entry point for desk duplication
        /// </summary>
        /// <param name="model">desk data</param>
        /// <returns></returns>
        [HttpPost("duplicateDesk")]
        public ActionResult<DeskModel> DuplicateDesk([FromBody] DeskModel model)
        {
            try
            {
                // Map our entry item
                DataOffice deskToDuplicate = _mapper.Map<DataOffice>(model);

                // call service to duplicate our item
                deskToDuplicate = _iOffice.DuplicateDesk(deskToDuplicate);

                // Re-map in the other way
                model = _mapper.Map<DeskModel>(deskToDuplicate);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Entry point for the creation of a desk to designate an entire space of the map
        /// </summary>
        /// <param name="designationName">office designation</param>
        /// <returns></returns>
        [HttpPost("createSeparator")]
        public ActionResult<DeskModel> CreateDeskSeparator([FromBody] string designationName)
        {
            try
            {
                DataOffice item = _iOffice.CreateDeskSeparator(designationName);
                DeskModel model = _mapper.Map<DeskModel>(item);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Entry point to delete a specified desk
        /// </summary>
        /// <param name="model">Id of the deleted item</param>
        /// <returns></returns>
        [HttpPost("deleteDesk")]
        public ActionResult<int> DeleteDesk([FromBody] DeskModel model)
        {
            try
            {
                int IdDeletedItem = _iOffice.DeleteDesk(model.Id);
                return Ok(IdDeletedItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Entry point for creating a default map by some parameters
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
