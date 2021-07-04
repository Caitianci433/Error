using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCase
{
    /// <summary>
    /// 请求
    /// </summary>
    public interface URequest<TResponse> : IRequest<TResponse> where TResponse: UResponse
    {

    }

    /// <summary>
    /// 响应
    /// </summary>
    public interface UResponse
    {
        bool IsError { get; set; }

        string ErrorMessage { get; set; }
    }


    /// <summary>
    /// UseCase Handler
    /// </summary>
    public interface IUseCaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
        where TRequest: URequest<TResponse>
        where TResponse:UResponse
    { 

    }



}
