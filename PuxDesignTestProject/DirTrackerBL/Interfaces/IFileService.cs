namespace DirTrackerBL.Interfaces;

public interface IFileService
{
    public bool FileExists(string path);

    public bool DirExists(string path);

    public Task<byte[]> HashFile(string path, IHashService hasher);

    public List<string> GetFileNamesInDir(string path);

    public List<string> GetDirNamesInDir(string path);
}