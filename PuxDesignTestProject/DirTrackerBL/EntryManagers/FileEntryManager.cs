using DirTrackerBL.Enums;
using DirTrackerBL.Interfaces;
using DirTrackerBL.Records;
using DirTrackerBL.Services;
using DirTrackerDAL.Models;

namespace DirTrackerBL.EntryManagers;

public class FileEntryManager
{
    private readonly IHashService _hashFunc;
    private readonly IFileService _fileManager;
    public FileModel? FileEntry { get; set; }
    private string Path { get; }
    public List<Change> ChangeList { get; }

    public FileEntryManager(FileModel? fileEntry, string path, IHashService hashFunc,
        IFileService fileManager, List<Change> changeList)
    {
        FileEntry = fileEntry;
        Path = path;
        _hashFunc = hashFunc;
        _fileManager = fileManager;
        ChangeList = changeList;
    }

    public async Task<ChangeStatus> ValidateFile()
    {
        var filePath = Path + FileEntry.Name;
        if (!_fileManager.FileExists(filePath))
        {
            ChangeList.Add(
                new Change(filePath, ChangeStatus.Deleted, -1));
            return ChangeStatus.Deleted;
        }

        var hash = await _fileManager.HashFile(filePath, _hashFunc);

        if (!FileEntry.Hash.SequenceEqual(hash))
        {
            FileEntry.Hash = hash;
            FileEntry.Version += 1;
            ChangeList.Add(
                new Change(filePath, ChangeStatus.Modified, FileEntry.Version));
            return ChangeStatus.Modified;
        }

        return ChangeStatus.Same;
    }

    public async Task AddFile(string name)
    {
        FileEntry = new FileModel
        {
            Name = name,
            Version = 1
        };

        var filePath = Path + FileEntry.Name;
        FileEntry.Hash = await _fileManager.HashFile(filePath, _hashFunc);

        ChangeList.Add(
            new Change(filePath, ChangeStatus.Added, FileEntry.Version));
    }
}