using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UseCase.Attribute;

namespace UseCase.UseCase.ImageUseCase
{
    #region ImageGetRequest
    public class ImageGetRequest : URequest<ImageGetResponse>
    {
        public ImageGetRequest(int? id) 
        {
            Id = id;
        }

        public int? Id { get; }
    }
    #endregion

    #region ImageGetResponse
    public class ImageGetResponse : UResponse
    {
        public ImageGetResponse(byte[] imageData)
        {
            this.imageData = imageData;
        }

        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public byte[] imageData { get; }
    }
    #endregion 

    interface IImageGetUseCase : IUseCaseHandler<ImageGetRequest, ImageGetResponse> { }

    public class ImageGetUseCase : IImageGetUseCase
    {
        private readonly IImageRepository _imageRepository;
        private readonly IConfiguration _configuration;

        public ImageGetUseCase(IImageRepository imageRepository, IConfiguration configuration) 
        {
            _imageRepository = imageRepository;
            _configuration = configuration;
        }

        [Exception]
        [Transitional]
        public async Task<ImageGetResponse> Handle(ImageGetRequest request, CancellationToken cancellationToken)
        {
            var ID = request.Id;

            if (null == ID)
            {
                ID = new Random().Next(0,await _imageRepository.FlushImageCount());
            }

            var path = await _imageRepository.FetchImagePath(ID.Value);

            var data = await Utils.FileHelpers.ReadFileAsync($"{_configuration["FilePath:image"]}/{path}");

            return new ImageGetResponse(data);
        }
    }
}
