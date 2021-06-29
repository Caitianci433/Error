using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    /// <summary>
    /// Redis测试控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestRedisController : Controller
    {
        private readonly IDistributedCache _cacheService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public  TestRedisController(IDistributedCache distributedCache) 
        {
            _cacheService = distributedCache;
        }
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TestGetAsync()
        {
            var time =await _cacheService.GetStringAsync("time");

           

            return new ContentResult { Content = JsonConvert.DeserializeObject<string>(time), ContentType = "application/json" };
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TestPostAsync()
        {
            var cacheOneMinute = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) };
            await _cacheService.SetStringAsync("time", JsonConvert.SerializeObject(DateTime.Now), cacheOneMinute);

            return new ContentResult { Content = "Test", ContentType = "application/json" };
        }
    }
}
