namespace Bufa.Functions.ProcessDocs.Models;

public class DocEntity
{
    public string PersonName { get; set; }
    public string PersonGovId { get; set; }

    public string PartitionKey { get ; set ; }
    public string RowKey { get ; set ; }
}