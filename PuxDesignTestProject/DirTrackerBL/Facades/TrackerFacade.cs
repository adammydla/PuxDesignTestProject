using DirTrackerBL.EntryManagers;
using DirTrackerBL.Enums;
using DirTrackerBL.Interfaces;
using DirTrackerBL.Records;
using DirTrackerDAL.Interfaces;

namespace DirTrackerBL.Facades;

public class TrackerFacade : ITrackerFacade
{
    private readonly IDalFileManager _dalFileManager;
    private readonly IXmlSerializer _serializer;
    private readonly IHashService _hasher;
    private readonly IFileService _blFileManager;

    public TrackerFacade(IDalFileManager dalFileManager, IXmlSerializer serializer,
        IHashService hasher, IFileService blFileManager)
    {
        _dalFileManager = dalFileManager;
        _serializer = serializer;
        _hasher = hasher;
        _blFileManager = blFileManager;
    }

    private async Task<Tuple<DirEntryManager, InputStatus>> GoThroughDir(
        List<Change> changeList, string path, string xml)
    {
        var finalStatus = InputStatus.NewDir;
        DirEntryManager dir;
        var fullPath = Path.GetFullPath(path);
        if (string.IsNullOrEmpty(xml))
        {
            dir = new DirEntryManager(null, "", _hasher, _blFileManager, changeList);
            await dir.AddDir(fullPath);
        }
        else
        {
            var startingDir = _serializer.Deserialize(xml);
            dir = new DirEntryManager(startingDir, "", _hasher, _blFileManager, changeList);

            if (startingDir.Name == fullPath)
            {
                await dir.ValidateDir();
                finalStatus = InputStatus.SameDir;
            }
            else
            {
                await dir.AddDir(fullPath);
            }
        }

        return new Tuple<DirEntryManager, InputStatus>(dir, finalStatus);
    }

    public async Task<Tuple<List<Change>, InputStatus>> GetChanges(string path)
    {
        var changesList = new List<Change>();
        if (!_blFileManager.DirExists(path))
            return new Tuple<List<Change>, InputStatus>
                (changesList, InputStatus.InvalidDir);

        var xml = await _dalFileManager.ReadFileContent();

        (DirEntryManager dir, InputStatus returnStatus) = await GoThroughDir(changesList, path, xml);

        xml = _serializer.Serialize(dir.DirEntry);
        await _dalFileManager.WriteFileContent(xml);
        return new Tuple<List<Change>, InputStatus>(changesList, returnStatus);
    }
}