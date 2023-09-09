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
        public ActionResult<List<OfficeModel>> GetData()
        {
            try
            {
                List<DataOffice> data = _iOffice.OfficeData();
                List<OfficeModel> result = _mapper.Map<List<OfficeModel>>(data);
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
        public ActionResult<List<OfficeModel>> UpdateOfficeMap([FromBody] List<OfficeModel> model)
        {
            try
            {
                if(model != null)
                {
                    _iOffice.UpdateOfficeData(model);
                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);    
                }
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
        public ActionResult<OfficeModel> DuplicateDesk([FromBody] OfficeModel model)
        {
            try
            {
                if(model != null)
                {
                    // Map our entry item
                    DataOffice deskToDuplicate = _mapper.Map<DataOffice>(model);

                    // call service to duplicate our item
                    deskToDuplicate = _iOffice.DuplicateDesk(deskToDuplicate);

                    // Re-map in the other way
                    model = _mapper.Map<OfficeModel>(deskToDuplicate);

                    return Ok(model);
                }
                else
                {
                    return BadRequest(model);
                }
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
        public ActionResult<OfficeModel> CreateDeskSeparator([FromBody] string designationName)
        {
            try
            {
                if(designationName != null)
                {
                    DataOffice item = _iOffice.CreateDeskSeparator(designationName);
                    OfficeModel model = _mapper.Map<OfficeModel>(item);

                    return Ok(model);
                }
                else
                {
                    return BadRequest(designationName);    
                }
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
        public ActionResult<int> DeleteDesk([FromBody] OfficeModel model)
        {
            try
            {
                if(model != null)
                {
                    int IdDeletedItem = _iOffice.DeleteDesk(model.Id);
                    return Ok(IdDeletedItem);
                }
                else
                {
                    return BadRequest(model);
                }
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
        public ActionResult<List<OfficeModel>> CreateAMap([FromBody] MapCreationModel model)
        {
            try
            {
                if(model != null)
                {
                    _iOffice.CreateAMap(model);
                    return Ok();
                }
                else
                {
                    return BadRequest(model);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        /// <summary>
        /// Entry point for creating a reservation on a desk in the office
        /// </summary>
        /// <param name="model">data model</param>
        /// <returns></returns>
        [HttpPost("reserveLocation")]
        public ActionResult ReserveLocation([FromBody] OfficeModel model)
        {
            try
            {
                if (model != null)
                {
                    _iOffice.CreateReservation(model);
                    return Ok();
                }
                else 
                {
                    return BadRequest(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("getReservationData")]
        public ActionResult<ReservationResultModel>? GetReservationData([FromBody] int deskId)
        {
            try
            {
                ReservationResultModel? data = _iOffice.GetReservationResult(deskId);
                if(data != null)
                {
                    return Ok(data);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
