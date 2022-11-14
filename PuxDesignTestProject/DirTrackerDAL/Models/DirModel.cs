namespace DirTrackerDAL.Models;

[Serializable]
public class DirModel
{
    public string Name { get; set; }
    public int Version { get; set; }

    public List<FileModel> FileList { get; set; }
    public List<DirModel> DirList { get; set; }
}