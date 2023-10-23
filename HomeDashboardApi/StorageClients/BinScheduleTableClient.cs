using Azure.Data.Tables;

namespace HomeDashboardApi.StorageClients;
public class BinScheduleTableClient
{
    public TableClient Client { get; }

    public BinScheduleTableClient(string connectionString)
    {
        Client = new TableClient(connectionString, "BinSchedule");
    }
}