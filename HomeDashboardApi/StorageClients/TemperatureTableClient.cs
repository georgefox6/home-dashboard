using Azure.Data.Tables;

namespace HomeDashboardApi.StorageClients; 
public class TemperatureTableClient
{   public TableClient Client { get; }

    public TemperatureTableClient(string connectionString)
    {
        Client = new TableClient(connectionString, "temperature");
    }
}