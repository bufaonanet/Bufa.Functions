using System;
using System.Xml.Serialization;

namespace Bufa.Functions.ProcessDocs.Models;

[Serializable]
[XmlRoot("nota_fiscal")]
public class DocFile
{
    [XmlElement("nome")]
    public string PersonName { get; set; }
    
    [XmlElement("cpf")]
    public string PersonGovId { get; set; }
    
    public long FileSize { get; set; }
    
    public string FileName { get; set; }
    
    public DateTime ProcessDate { get; set; }
}