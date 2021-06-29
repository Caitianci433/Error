using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UseCase;

namespace Presentation.Controllers
{
    /// <summary>
    /// 主控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        /// <param name="configuration"></param>
        public MainController(ILogger<MainController> logger, IMediator mediator, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Main-Get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAsync()
        {
            //var request = new MainRequest();
            //var response = await _mediator.Send(request);

            await Task.Delay(100);

            return new ContentResult { Content = "Test", ContentType = "application/json" };
        }


        /// <summary>
        /// 验证与授权
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Authenticate()
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            //秘钥，就是标头，这里用Hmacsha256算法，需要256bit的密钥
            var securityKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.GetValue<string>("Secret"))), SecurityAlgorithms.HmacSha256);
            //Claim，JwtRegisteredClaimNames中预定义了好多种默认的参数名，也可以像下面的Guid一样自己定义键名.
            //ClaimTypes也预定义了好多类型如role、email、name。Role用于赋予权限，不同的角色可以访问不同的接口
            //相当于有效载荷
            var claims = new Claim[] 
            {
                new Claim(JwtRegisteredClaimNames.Iss,jwtConfig.GetValue<string>("Iss")),
                new Claim(JwtRegisteredClaimNames.Aud,jwtConfig.GetValue<string>("Aud")),
                new Claim("Guid",Guid.NewGuid().ToString("D")),
                new Claim(ClaimTypes.Role,"system"),
                new Claim(ClaimTypes.Role,"admin"),
             };
            SecurityToken securityToken = new JwtSecurityToken(
                signingCredentials: securityKey,
                expires: DateTime.Now.AddMinutes(2),//过期时间
                claims: claims
            );
            //生成jwt令牌
            return Content(new JwtSecurityTokenHandler().WriteToken(securityToken));
        }

    }

    
}
