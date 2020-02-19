using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Api.Blazor
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/blazor/[controller]")]
    [Authorize]
    public class UserNameController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return HttpContext.User.Identity.Name;
        }
    }
}
