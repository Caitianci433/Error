using MediatR;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UseCase.Attribute;

namespace UseCase.Behavior
{
    [Order(1)]
    public class TransitionalBehavirPiepline<TRequest, TResponse> : AbstractPipelineBehavior<TRequest, TResponse>
        where TRequest : URequest<TResponse>
        where TResponse : UResponse
    {

        private readonly ServiceFactory _serviceFactory;

        public TransitionalBehavirPiepline(ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var usecase = _serviceFactory.GetInstance<IRequestHandler<TRequest, TResponse>>();
            var handle = usecase.GetType().GetMethod("Handle");
            var attr = handle.GetCustomAttribute(typeof(TransitionalAttribute));

            if (attr != null)
            {
                System.Console.WriteLine("ll");

            }
            return await next();
        }
    }
}
