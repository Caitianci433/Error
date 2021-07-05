using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UseCase.Attribute;

namespace UseCase.Behavior
{
    [Order(2)]
    public class ExceptionBehavirPiepline<TRequest, TResponse> : AbstractPipelineBehavior<TRequest, TResponse>
        where TRequest : URequest<TResponse>
        where TResponse : UResponse
    {
        private readonly ServiceFactory _serviceFactory;

        public ExceptionBehavirPiepline(ServiceFactory serviceFactory) 
        {
            _serviceFactory = serviceFactory;
        }


        public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var usecase =  _serviceFactory.GetInstance<IRequestHandler<TRequest, TResponse>>();
            var handle = usecase.GetType().GetMethod("Handle");
            var attr = handle.GetCustomAttribute(typeof(ExceptionAttribute));

            if (attr!=null)
            {
                try
                {
                    return await next();
                }
                catch (Exception)
                {

                    return (TResponse)Activator.CreateInstance<TResponse>();
                }
            }

            return await next();
        }
    }
}
