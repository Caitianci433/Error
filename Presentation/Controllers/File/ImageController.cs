using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using UseCase;
using UseCase.UseCase.ImageUseCase;

namespace Presentation.Controllers.File
{
    /// <summary>
    /// 图片
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mediator"></param>
        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// 图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetImageAsync(int? id)
        {
            var response = await _mediator.Send(new ImageGetRequest(id));

            return new FileContentResult(response.imageData, "image/jpeg");
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PutImageAsync()
        {

            await _mediator.Send(new ImageFlushRequest());

            return Ok();
        }



    }
}
