using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Bufa.Functions.ProcessDocs;

public static class ServiceBusQueueTrigger
{
    [FunctionName("ServiceBusQueueTrigger")]
    public static async Task RunAsync(
        [ServiceBusTrigger("queue-filestreaming", Connection = "service-bus-connection")]
        string myQueueItem,
        ILogger log)
    {
        log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

        var bytes = Convert.FromBase64String(myQueueItem);
        var content = new MemoryStream(bytes);

        try
        {
            var storageConnectionsString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var container = new BlobContainerClient(
                storageConnectionsString,
                "content-files"
            );
            
            var blobName = $"file-{DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm:ss")}";
            await container.UploadBlobAsync(blobName, content);

            log.LogInformation("Arquivo inserido com sucesso");
        }
        catch (Exception ex)
        {
            log.LogCritical(ex.Message);
        }
    }
}