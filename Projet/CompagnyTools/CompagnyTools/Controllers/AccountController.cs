using CompagnyTools.Models;
using Microsoft.AspNetCore.Mvc;
using CompagnyTools.Interface;
using AutoMapper;
using DAL.Entities;

namespace CompagnyTools.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _iAccount;
        private readonly IMapper _mapper;

        public AccountController(IAccount iAccount, IMapper mapper)
        {
            _iAccount = iAccount;
            _mapper = mapper;       
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAllUsers")]
        public ActionResult<List<UserModel>> GetAllUsers()
        {
            try
            {
                List<Users>? data = _iAccount.GetAllUsers();
                List<UserModel> result = _mapper.Map<List<UserModel>>(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        /// <summary>
        /// Entry point for user's creation
        /// </summary>
        /// <param name="model">user informations</param>
        /// <returns></returns>
        [HttpPost("createUser")]
        public ActionResult<UserModel> CreateUser([FromBody] LoginModel model)
        {
            if (model != null)
            {
                Users user = _iAccount.CreateUser(model);
                if(user != null) 
                {
                    UserModel result = _mapper.Map<UserModel>(user);
                    return Ok(result);
                }
                else
                {
                    return NoContent();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Delete given user
        /// </summary>
        /// <param name="Id">user's id</param>
        /// <returns></returns>
        [HttpPost("DeleteUser")]
        public ActionResult DeleteUser([FromBody] int[] Ids)
        {
            bool process = _iAccount.DeleteUser(Ids);
            if (process)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
