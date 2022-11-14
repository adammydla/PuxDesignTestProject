using DirTrackerDAL.Interfaces;
using DirTrackerDAL.Models;

namespace DirTrackerDAL.Managers;

public class XmlSerializer : IXmlSerializer
{
    public string Serialize(DirModel dir)
    {
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(dir.GetType());

        using (var textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, dir);
            return textWriter.ToString();
        }
    }

    public DirModel Deserialize(string xmlFile)
    {
        var ser = new System.Xml.Serialization.XmlSerializer(typeof(DirModel));

        using (var sr = new StringReader(xmlFile))
        {
            return (DirModel)ser.Deserialize(sr);
        }
    }
}