using DirTrackerBL.Enums;
using DirTrackerBL.Interfaces;
using DirTrackerBL.Records;
using DirTrackerDAL.Models;

namespace DirTrackerBL.EntryManagers;

public class DirEntryManager
{
    private readonly IHashService _hashFunc;
    private readonly IFileService _fileManager;
    public DirModel? DirEntry { get; set; }
    private string Path { get; }
    public List<Change> ChangeList { get; }

    public DirEntryManager(DirModel? dirEntry, string path, IHashService hashFunc,
        IFileService fileManager, List<Change> changeList)
    {
        DirEntry = dirEntry;
        Path = path;
        _hashFunc = hashFunc;
        _fileManager = fileManager;
        ChangeList = changeList;
    }

    private void DeletedNestedRecord(string path, DirModel directory)
    {
        foreach (var fileObj in directory.FileList)
            ChangeList.Add(
                new Change(path + fileObj.Name, ChangeStatus.Deleted, -1));

        foreach (var dirObj in directory.DirList)
        {
            DeletedNestedRecord(path + dirObj.Name, dirObj);
            ChangeList.Add(
                new Change(path + dirObj.Name, ChangeStatus.Deleted, -1));
        }
    }

    private async Task<bool> FileIterator(string dirPath)
    {
        var files = _fileManager.GetFileNamesInDir(dirPath);

        var changed = false;
        var fileEntryList = new List<FileModel>();
        foreach (var fileObj in DirEntry.FileList)
        {
            var file = new FileEntryManager(fileObj, dirPath, _hashFunc,
                _fileManager, ChangeList);
            var result = await file.ValidateFile();

            if (result != ChangeStatus.Deleted)
                fileEntryList.Add(fileObj);
            else
                changed = true;

            files.Remove(fileObj.Name);
        }

        DirEntry.FileList = fileEntryList;

        foreach (var remaining in files)
        {
            var file = new FileEntryManager(null, dirPath, _hashFunc,
                _fileManager, ChangeList);
            await file.AddFile(remaining);
            DirEntry.FileList.Add(file.FileEntry);
            changed = true;
        }

        return changed;
    }


    private async Task<bool> DirIterator(string dirPath)
    {
        var dirs = _fileManager.GetDirNamesInDir(dirPath);

        var changed = false;
        var dirEntryList = new List<DirModel>();
        foreach (var dirObj in DirEntry.DirList)
        {
            var dir = new DirEntryManager(dirObj, dirPath, _hashFunc,
                _fileManager, ChangeList);
            var result = await dir.ValidateDir();

            if (result != ChangeStatus.Deleted)
                dirEntryList.Add(dirObj);
            else
                changed = true;

            dirs.Remove(dirObj.Name);
        }

        DirEntry.DirList = dirEntryList;

        foreach (var remaining in dirs)
        {
            var dir = new DirEntryManager(null, dirPath, _hashFunc,
                _fileManager, ChangeList);
            await dir.AddDir(remaining);
            DirEntry.DirList.Add(dir.DirEntry);
            changed = true;
        }

        return changed;
    }

    public async Task<ChangeStatus> ValidateDir()
    {
        var dirPath = Path + DirEntry.Name;
        if (!_fileManager.DirExists(dirPath))
        {
            ChangeList.Add(
                new Change(dirPath, ChangeStatus.Deleted, -1));
            DeletedNestedRecord(dirPath, DirEntry);
            return ChangeStatus.Deleted;
        }

        if (await FileIterator(dirPath) || await DirIterator(dirPath))
        {
            DirEntry.Version += 1;
            ChangeList.Add(
                new Change(dirPath, ChangeStatus.Modified, DirEntry.Version));
            return ChangeStatus.Modified;
        }

        return ChangeStatus.Same;
    }

    public async Task AddDir(string name)
    {
        DirEntry = new DirModel
        {
            Name = name,
            Version = 1,
            FileList = new List<FileModel>(),
            DirList = new List<DirModel>()
        };

        var dirPath = Path + DirEntry.Name;
        await FileIterator(dirPath);
        await DirIterator(dirPath);

        ChangeList.Add(
            new Change(dirPath, ChangeStatus.Added, DirEntry.Version));
    }
}