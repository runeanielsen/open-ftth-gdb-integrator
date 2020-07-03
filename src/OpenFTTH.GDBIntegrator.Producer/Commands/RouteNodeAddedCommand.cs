using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Topos.Producer;

namespace OpenFTTH.GDBIntegrator.Producer.Commands
{
    public class RouteNodeAddedCommand : IRequest
    {
        public string EventType => "RouteNodeAddedCommand";
        public string EventId { get; set; }
        public string EventTs => DateTime.UtcNow.ToString();
        public string CmdId => Guid.NewGuid().ToString();
        public string NodeId { get; set; }
        public string Geometry { get; set; }
    }

    public class RouteNodeAddedCommandHandler : IRequestHandler<RouteNodeAddedCommand>
    {
        private readonly IProducer _producer;

        public RouteNodeAddedCommandHandler(IProducer producer)
        {
            _producer = producer;
        }

        public async Task<Unit> Handle(RouteNodeAddedCommand request, CancellationToken cancellationToken)
        {
            await _producer.Produce("event.route-network", new ToposMessage(request));
            return default;
        }
    }
}