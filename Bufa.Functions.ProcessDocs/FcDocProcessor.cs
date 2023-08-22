using System.Threading.Tasks;
using System;
using System.Text.Json;
using Azure.Storage.Blobs;
using Bufa.Functions.ProcessDocs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Bufa.Functions.ProcessDocs;

public static class FcDocProcessor
{
    [FunctionName("FcDocProcessor")]
    [return: Table("docsinfo", Connection = "AzureWebJobsStorage")]
    public static async Task<DocEntity> RunAsync(
        [QueueTrigger("queueprocess", Connection = "AzureWebJobsStorage")]
        string myQueueItem,
        ILogger log)
    {
        log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
    
        var queueItem = JsonSerializer.Deserialize<DocFile>(myQueueItem);
    
        var connectionStorage = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    
        var blobClient = new BlobClient(
            connectionStorage,
            "tobeprocess",
            queueItem.FileName
        );
    
        var currentBlob = await blobClient.DownloadStreamingAsync();
    
        var blobContainerClient = new BlobContainerClient(
            connectionStorage,
            "processdone"
        );
        
        await blobContainerClient.UploadBlobAsync(queueItem.FileName, currentBlob.Value.Content);
        
        await blobClient.DeleteIfExistsAsync();
        
        return new DocEntity
        {
            PartitionKey = "nota_fiscal",
            RowKey = Guid.NewGuid().ToString(),
            PersonGovId = queueItem.PersonGovId,
            PersonName = queueItem.PersonName
        };
    }
}