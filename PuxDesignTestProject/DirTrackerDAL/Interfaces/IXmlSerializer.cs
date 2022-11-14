using DirTrackerDAL.Models;

namespace DirTrackerDAL.Interfaces;

public interface IXmlSerializer
{
    public string Serialize(DirModel dir);

    public DirModel Deserialize(string xmlFile);
}