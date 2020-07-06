using Xunit;
using FluentAssertions;

namespace OpenFTTH.GDBIntegrator.Config.Tests
{
    public class KafkaSettingsTests
    {
        [Fact]
        public void KafkaSettings_ShouldInitalizeValues_OnConstruction()
        {
            var server = "192.13.2.1";
            var positionFilePath = "/tmp/";
            var topic = "event.route-network";
            var consumer = "postgis-consumer";

            var kafkaSettings = new KafkaSetting
            {
                Server = server,
                PositionFilePath = positionFilePath,
                Topic = topic,
                Consumer = consumer
            };

            kafkaSettings.Server.Should().BeEquivalentTo(server);
            kafkaSettings.PositionFilePath.Should().BeEquivalentTo(positionFilePath);
            kafkaSettings.Topic.Should().BeEquivalentTo(topic);
            kafkaSettings.Consumer.Should().BeEquivalentTo(consumer);
        }
    }
}
