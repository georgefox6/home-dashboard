using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using AggregationFunction.Data;

public static class MqttTimerFunction
{
    private static IMqttClient mqttClient;
    private static List<MqttApplicationMessage> receivedMessages = new List<MqttApplicationMessage>();

    [FunctionName("MqttTimerFunction")]
    public static void Run(
        [TimerTrigger("0 */1 * * * *")] TimerInfo timer,
        ILogger log)
    {
        // Process the received MQTT messages and save the aggregated result to Azure Table Storage
        if (receivedMessages.Count > 0)
        {
            var aggregatedData = AggregateMessages();

            // Save the aggregated data to Azure Table Storage
            SaveToTableStorage(aggregatedData);

            receivedMessages.Clear();

            log.LogInformation($"Aggregated data saved to Azure Table Storage: {aggregatedData}");
        }

        // Subscribe to MQTT topic and start receiving messages
        if (mqttClient == null)
        {
            InitializeMqttClient();
            SubscribeToMqttTopic("esp32/temperature");
            SubscribeToMqttTopic("esp32/humidity");
        }
    }

    private static void InitializeMqttClient()
    {
        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", 1883) // Replace with your MQTT server details
            .WithClientId("8CEfH")
            .Build();

        mqttClient = new MqttFactory().CreateMqttClient();

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            receivedMessages.Add(e.ApplicationMessage);
            return Task.CompletedTask;
        };

        mqttClient.ConnectAsync(mqttOptions).GetAwaiter().GetResult();
    }

    private static void SubscribeToMqttTopic(string topic)
    {
        mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build()).GetAwaiter().GetResult();
    }

    private static AggregatedData AggregateMessages()
    {
        double totalTemperature = 0;
        double totalHumidity = 0;
        int numberOfTemperatureMessages = 0;        
        int numberOfHumidityMessages = 0;

        foreach (var message in receivedMessages)
        {
            if (message.Topic == "esp32/temperature")
            {
                var temperature = Double.Parse(message.ConvertPayloadToString());
                totalTemperature += temperature;
                numberOfTemperatureMessages++;
            }
            if (message.Topic == "esp32/humidity")
            {
                var humidity = Double.Parse(message.ConvertPayloadToString());
                totalHumidity += humidity;
                numberOfHumidityMessages++;
            }
        }
        var averageTemperature = totalTemperature / numberOfTemperatureMessages;
        var averageHumidity = totalHumidity / numberOfHumidityMessages;


        return new AggregatedData(averageTemperature.ToString(), averageHumidity.ToString());
    }

    private static void SaveToTableStorage(AggregatedData data)
    {
        string storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
        string tableName = "temperature";

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference(tableName);

        // Create a new entity for the data and save it to Azure Table Storage
        string dateOfTemperature = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        string partitionKey = "Attic";
        string rowKey = dateOfTemperature;

        var entity = new DynamicTableEntity(partitionKey, rowKey);
        entity.Properties.Add("Temperature", new EntityProperty(data.Temperature));
        entity.Properties.Add("Humidity", new EntityProperty(data.Humidity));
        entity.Properties.Add("Room", new EntityProperty("Attic"));
        entity.Properties.Add("DateTime", new EntityProperty(dateOfTemperature));

        TableOperation insertOperation = TableOperation.Insert(entity);
        table.ExecuteAsync(insertOperation).GetAwaiter().GetResult();
    }
}
