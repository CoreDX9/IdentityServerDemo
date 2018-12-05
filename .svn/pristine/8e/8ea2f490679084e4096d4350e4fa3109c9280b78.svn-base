using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 身份控制器
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// 获取身份信息
        /// </summary>
        /// <returns>身份信息</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}