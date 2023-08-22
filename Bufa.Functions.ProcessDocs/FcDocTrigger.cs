using System;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Bufa.Functions.ProcessDocs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Bufa.Functions.ProcessDocs;

[StorageAccount("AzureWebJobsStorage")]
public static class FcDocTrigger
{
    [FunctionName("FcDocTrigger")]
    [return: Queue("queueprocess")]
    public static string Run(
        [BlobTrigger("tobeprocess/{name}")] Stream myBlob,
        string name,
        ILogger log)
    {
        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        var currentDoc = ProcessDocument(myBlob, name);

        return JsonSerializer.Serialize(currentDoc);
    }
    
    private static DocFile ProcessDocument(Stream blob, string name)
    {
        var serializer = new XmlSerializer(typeof(DocFile));
        var notaFiscal = (DocFile)serializer.Deserialize(blob);

        if (notaFiscal is null)
            return new DocFile();

        notaFiscal.FileName = name;
        notaFiscal.FileSize = blob.Length;
        notaFiscal.ProcessDate = DateTime.UtcNow;

        return notaFiscal;
    }
    
    // private static DocFile ProcessDocument2(Stream blob, string name)
    // {
    //     XmlDocument document = new XmlDocument();
    //     using (Stream stream = blob)
    //     {
    //         using (XmlReader reader = XmlReader.Create(stream))
    //         {
    //             if (stream.Position > 0)
    //                 stream.Position = 0;
    //
    //             document.Load(stream);
    //         }
    //     }
    //
    //     var cpfFromXml = document.SelectSingleNode("nota_fiscal/cpf").InnerText;
    //     var nomeFromXml = document.SelectSingleNode("nota_fiscal/nome").InnerText;
    //
    //     var doc = new DocFile()
    //     {
    //         FileName = name,
    //         FileSize = blob.Length,
    //         PersonName = nomeFromXml,
    //         PersonGovId = cpfFromXml,
    //         ProcessDate = DateTime.UtcNow
    //     };
    //
    //     return doc;
    // }

}