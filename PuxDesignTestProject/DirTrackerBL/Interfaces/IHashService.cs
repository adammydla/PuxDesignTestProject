namespace DirTrackerBL.Interfaces;

public interface IHashService
{
    public Task<byte[]> HashStream(FileStream fileContent);
}