using System;
using System.Text;
using Topos.Serialization;
using Newtonsoft.Json.Linq;
using OpenFTTH.GDBIntegrator.RouteNetwork;
using OpenFTTH.GDBIntegrator.Integrator.ConsumerMessages;
using OpenFTTH.Events.Core.Infos;
using OpenFTTH.Events.RouteNetwork.Infos;
using OpenFTTH.GDBIntegrator.Subscriber.Kafka.Serialize.Mapper;

namespace OpenFTTH.GDBIntegrator.Subscriber.Kafka.Serialize
{
    public class RouteNodeSerializer : IMessageSerializer
    {
        private readonly ISerializationMapper _serializationMapper;

        public RouteNodeSerializer(ISerializationMapper serializationMapper)
        {
            _serializationMapper = serializationMapper;
        }

        public ReceivedLogicalMessage Deserialize(ReceivedTransportMessage message)
        {
            if (message is null)
                throw new ArgumentNullException($"{nameof(ReceivedTransportMessage)} is null");

            if (message.Body is null || message.Body.Length == 0)
                return new ReceivedLogicalMessage(message.Headers, new RouteNodeMessage(), message.Position);

            var messageBody = Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);

            dynamic parsedRouteNodeMessage = JObject.Parse(messageBody);
            var payload = parsedRouteNodeMessage.payload;

            if (IsTombStoneMessage(payload))
                return new ReceivedLogicalMessage(message.Headers, new RouteNodeMessage(), message.Position);

            var routeNodeMessage = CreateRouteNodeMessage(payload);

            return new ReceivedLogicalMessage(message.Headers, routeNodeMessage, message.Position);
        }

        private bool IsTombStoneMessage(dynamic payload)
        {
            JToken afterPayload = payload["after"];
            return afterPayload.Type == JTokenType.Null;
        }

        private RouteNodeMessage CreateRouteNodeMessage(dynamic payload)
        {
            var payloadBefore = payload.before;
            var payloadAfter = payload.after;

            RouteNode routeNodeBefore = null;
            if ((JObject)payloadBefore != null)
                routeNodeBefore = CreateRouteNode(payloadBefore);

            var routeNodeAfter = CreateRouteNode(payloadAfter);

            return new RouteNodeMessage(routeNodeBefore, routeNodeAfter);
        }

        private RouteNode CreateRouteNode(dynamic routeNode)
        {
            return new RouteNode
            {
                ApplicationInfo = routeNode.application_info.ToString(),
                ApplicationName = routeNode.application_name.ToString(),
                Coord = Convert.FromBase64String(routeNode.coord.wkb.ToString()),
                MarkAsDeleted = (bool)routeNode.marked_to_be_deleted,
                DeleteMe = (bool)routeNode.delete_me,
                Mrid = new Guid(routeNode.mrid.ToString()),
                Username = routeNode.user_name.ToString(),
                WorkTaskMrid = routeNode.work_task_mrid.ToString() == string.Empty ? System.Guid.Empty : new Guid(routeNode.work_task_mrid.ToString()),
                LifeCycleInfo = new LifecycleInfo(
                    _serializationMapper.MapDeploymentState((string)routeNode.lifecycle_deployment_state),
                    (DateTime?)routeNode.lifecycle_installation_date,
                    (DateTime?)routeNode.lifecycle_removal_date
                    ),
                MappingInfo = new MappingInfo(
                    _serializationMapper.MapMappingMethod((string)routeNode.mapping_method),
                    (string)routeNode.mapping_vertical_accuracy,
                    (string)routeNode.mapping_horizontal_accuracy,
                    (DateTime?)routeNode.mapping_survey_date,
                    (string)routeNode.mapping_source_info
                    ),
                NamingInfo = new NamingInfo(
                    (string)routeNode.naming_name,
                    (string)routeNode.naming_description
                    ),
                RouteNodeInfo = new RouteNodeInfo(
                    _serializationMapper.MapRouteNodeKind((string)routeNode.routenode_kind),
                    _serializationMapper.MapRouteNodeFunction((string)routeNode.routenode_function)
                    ),
                SafetyInfo = new SafetyInfo(
                    (string)routeNode.safety_classification,
                    (string)routeNode.safety_remark
                    )
            };
        }

        public TransportMessage Serialize(LogicalMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
