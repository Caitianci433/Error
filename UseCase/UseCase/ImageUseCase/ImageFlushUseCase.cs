using Infrastructure.Entity;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UseCase.UseCase.ImageUseCase
{
    #region ImageFlushRequest
    public class ImageFlushRequest : URequest<ImageFlushResponse>
    {
    }
    #endregion

    #region ImageFlushResponse
    public class ImageFlushResponse : UResponse
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

    }
    #endregion 

    interface IImageFlushUseCase : IUseCaseHandler<ImageFlushRequest, ImageFlushResponse> { }

    public class ImageFlushUseCase : IImageFlushUseCase
    {

        private readonly IImageRepository _imageRepository;
        private readonly IConfiguration _configuration;

        public ImageFlushUseCase(IImageRepository imageRepository, IConfiguration configuration)
        {
            _imageRepository = imageRepository;
            _configuration = configuration;
        }

        public async Task<ImageFlushResponse> Handle(ImageFlushRequest request, CancellationToken cancellationToken)
        {
            DirectoryInfo direcinfo = new DirectoryInfo(_configuration["FilePath:image"]);
            var images = direcinfo.GetFiles().Select(s=>new Image { path = s.Name });
            await _imageRepository.FlushImages(images);
            return new ImageFlushResponse();

        }
    }
}
