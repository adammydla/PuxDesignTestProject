using DirTrackerBL.Enums;
using DirTrackerBL.Records;

namespace DirTrackerBL.Interfaces;

public interface ITrackerFacade
{
    public Task<Tuple<List<Change>, InputStatus>> GetChanges(string path);
}