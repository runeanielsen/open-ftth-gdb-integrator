using MediatR;
using System.Threading;
using System.Threading.Tasks;
using OpenFTTH.GDBIntegrator.RouteNetwork;
using OpenFTTH.GDBIntegrator.GeoDatabase;

namespace OpenFTTH.GDBIntegrator.Integrator.Commands
{
    public class NewLonelyRouteSegmentCommand : IRequest
    {
        public RouteSegment RouteSegment { get; set; }
    }

    public class NewLonelyRouteSegmentCommandHandler : IRequestHandler<NewLonelyRouteSegmentCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly IGeoDatabase _geoDatabase;

        public NewLonelyRouteSegmentCommandHandler(IMediator mediator, IGeoDatabase geoDatabase)
        {
            _mediator = mediator;
            _geoDatabase = geoDatabase;
        }

        public async Task<Unit> Handle(NewLonelyRouteSegmentCommand request, CancellationToken cancellationToken)
        {
            var routeSegment = request.RouteSegment;

            await _geoDatabase.InsertRouteNode(routeSegment.FindStartNode());
            await _geoDatabase.InsertRouteNode(routeSegment.FindEndNode());

            return default;
        }
    }
}