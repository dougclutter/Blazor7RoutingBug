using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// This worked in .NET 6
//app.UsePathBase("/myroot");
//app.UseBlazorFrameworkFiles("/myblazorapp");

// This doesn't work either
//app.UseBlazorFrameworkFiles("/myroot/myblazorapp");
//app.UseStaticFiles();

// And finally, the approach from 
// https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/multiple-hosted-webassembly?view=aspnetcore-7.0
// doesn't work either.
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/myroot/myblazorapp"), first =>
    {
        first.UseBlazorFrameworkFiles("/myroot/myblazorapp");
        first.UseStaticFiles();
        first.UseStaticFiles("/myroot/myblazorapp");
        first.UseRouting();

        first.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/myroot/myblazorapp/{*path:nonfile}",
                "myroot/myblazorapp/index.html");
        });
    });

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.Run();
