using System.Threading.Tasks;
using System.Collections.Generic;
using OpenFTTH.GDBIntegrator.RouteNetwork;

namespace OpenFTTH.GDBIntegrator.GeoDatabase
{
    public interface IGeoDatabase
    {
        Task<List<RouteNode>> GetIntersectingStartRouteNodes(RouteSegment routeSegment);
        Task<List<RouteNode>> GetIntersectingEndRouteNodes(RouteSegment routeSegment);
        Task InsertRouteNode(RouteNode routeNode);
    }
}
