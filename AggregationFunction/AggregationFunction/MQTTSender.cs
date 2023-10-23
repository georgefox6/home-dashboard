using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

public static class MqttPublishFunction
{
    private static IMqttClient mqttClient;

    [FunctionName("MqttPublishFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger("post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string message = requestBody;

        // Publish the message to the MQTT topic
        await PublishToMqttTopic("temperature", message);

        log.LogInformation($"Message published to MQTT topic: {message}");

        return new OkResult();
    }

    private static async Task PublishToMqttTopic(string topic, string message)
    {
        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", 1883) // Replace with your MQTT server details
            .WithClientId("p4Kqx")
            .Build();

        mqttClient = new MqttFactory().CreateMqttClient();

        await mqttClient.ConnectAsync(mqttOptions);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message)
            .WithRetainFlag()
            .Build();

        await mqttClient.PublishAsync(applicationMessage);

        await mqttClient.DisconnectAsync();
    }
}
