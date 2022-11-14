namespace DirTrackerDAL.Interfaces;

public interface IDalFileManager
{
    public Task<string?> ReadFileContent();

    public Task WriteFileContent(string text);
}