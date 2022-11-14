namespace DirTrackerDAL.Models;

[Serializable]
public class FileModel
{
    public string Name { get; set; }
    public byte[] Hash { get; set; }
    public int Version { get; set; }
}