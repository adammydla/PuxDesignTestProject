using DirTrackerBL.Enums;
using DirTrackerBL.Interfaces;
using DirTrackerBL.Records;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PuxDesignTestProject.Pages;

public class ShowChanges : PageModel
{
    private ITrackerFacade _tracker;

    public ShowChanges(ITrackerFacade tracker)
    {
        _tracker = tracker;
    }

    public List<Change> Changes { get; set; }
    public InputStatus Status { get; set; }

    public async Task OnGet(string dirPath)
    {
        (Changes, Status) = await _tracker.GetChanges(dirPath);
        Changes = Changes
            .OrderBy(x => x.Version)
            .ToList();
    }
}