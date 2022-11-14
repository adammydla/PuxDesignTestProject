using DirTrackerDAL.Interfaces;

namespace DirTrackerDAL.Managers;

public class DalFileManager : IDalFileManager
{
    private readonly string _path;

    public DalFileManager(string path)
    {
        _path = path;
    }

    public async Task<string?> ReadFileContent()
    {
        if (!File.Exists(_path)) return null;
        return await File.ReadAllTextAsync(_path);
    }

    public async Task WriteFileContent(string text)
    {
        await File.WriteAllTextAsync(_path, text);
    }
}