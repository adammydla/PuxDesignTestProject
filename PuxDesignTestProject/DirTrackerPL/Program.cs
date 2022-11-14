using DirTrackerBL.Facades;
using DirTrackerBL.Interfaces;
using DirTrackerBL.Services;
using DirTrackerDAL;
using DirTrackerDAL.Interfaces;
using DirTrackerDAL.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IDalFileManager>(x => new DalFileManager("Tracker.xml"));
builder.Services.AddScoped<IXmlSerializer, XmlSerializer>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITrackerFacade, TrackerFacade>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();