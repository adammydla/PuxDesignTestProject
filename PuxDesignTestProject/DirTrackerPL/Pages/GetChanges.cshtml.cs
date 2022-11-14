using DirTrackerBL.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PuxDesignTestProject.Pages;

public class GetChanges : PageModel
{
    [BindProperty] public string DirPath { get; set; }

    public List<Change> Changes { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return RedirectToPage("./ShowChanges", new { dirPath = DirPath });
    }
}