using AutoMapper;
using CoreDX.Application.Command.UserManage;
using CoreDX.Common.Util.QueryHelper;
using CoreDX.Domain.Core.Command;
using CoreDX.Domain.Entity.Identity;
using IdentityServer.Areas.Manage.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace IdentityServer.Api
{
    [ApiController]
    [ApiVersion("1.0")]
    //[ApiVersion("1.0", Deprecated = true)] // Deprecated 表示已弃用，在响应中增加相关提时信息
    //[ApiVersionNeutral] //指示这个api不需要版本控制
    [Route("api/[controller]")]
    //[Route("api/{v:apiVersion}/[controller]")] //使用url指定api版本
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommandBus<ListUserCommand, IPagedList<ApplicationUser>> _commandBus;
        private readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager, ICommandBus<ListUserCommand, IPagedList<ApplicationUser>> commandBus, IMapper mapper)
        {
            _userManager = userManager;
            _commandBus = commandBus;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页条目数</param>
        /// <returns>用户列表</returns>
        [HttpGet]
        [Authorize]
        [Produces("application/json")] //声明接口响应 json 数据
        //[ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int? page, int? size)
        {
            var cmd = new ListUserCommand(new PageInfo(page ?? 1, size ?? 10), new QueryFilter());
            var users = await _commandBus.SendCommandAsync(cmd, default);

            return new JsonResult(
                new
                {
                    rows = users.Select(u => _mapper.Map<ApplicationUserDto>(u)),
                    total = users.PageCount, //总页数
                    page = users.PageNumber, //当前页码
                    records = users.TotalItemCount //总记录数
                }
            );
        }
    }
}
