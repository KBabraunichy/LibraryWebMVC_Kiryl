using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IApiRequestService, ApiRequestService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("WebServiceError");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseExceptionHandler(exceptionHandlerApp =>
{
    Results.LocalRedirect("default");
    exceptionHandlerApp.Run(async context => {

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        Console.WriteLine(context.Response);

        await context.Response.WriteAsync("<h1 style=\"text-align: center\"> Web service Error.<h1>");
    });
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();