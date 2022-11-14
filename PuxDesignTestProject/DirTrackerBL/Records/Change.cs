using DirTrackerBL.Enums;

namespace DirTrackerBL.Records;

public record Change(string Name, ChangeStatus ChangeType, int Version)
{
    public ChangeStatus ChangeType = ChangeType;
    public string Name = Name;
    public int Version = Version;
}