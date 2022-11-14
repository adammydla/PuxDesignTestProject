using DirTrackerBL.Interfaces;

namespace DirTrackerBL.Services;

public class FileService : IFileService
{
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool DirExists(string path)
    {
        return Directory.Exists(path);
    }

    public async Task<byte[]> HashFile(string path, IHashService hasher)
    {
        using (var fileStream = File.OpenRead(path))
        {
            return await hasher.HashStream(fileStream);
        }
    }

    private List<string> GetRecordNames(IEnumerable<string> pathList)
    {
        return pathList.Select(x => "\\" + x
                .Split("\\")
                .Last())
            .ToList();
    }

    public List<string> GetFileNamesInDir(string path)
    {
        var paths = Directory.EnumerateFiles(path);
        return GetRecordNames(paths);
    }

    public List<string> GetDirNamesInDir(string path)
    {
        var paths = Directory.EnumerateDirectories(path);
        return GetRecordNames(paths);
    }
}