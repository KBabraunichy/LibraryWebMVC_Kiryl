global using static LibraryWebMVC.Utils.ApiServiceConstants;
using LibraryWebMVC.Interfaces;
using LibraryWebMVC.Services;
using Polly;
using RestEase.HttpClientFactory;
using System.Net;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var restEaseRetryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.BadRequest)
                .WaitAndRetryAsync(MaxRetries, times => TimeSpan.FromMilliseconds(times * 100));

var retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(MaxRetries, times => TimeSpan.FromMilliseconds(times * 100));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IApiRequestService, ApiRequestService>();
builder.Services.AddRestEaseClient<IRestEaseService>(builder.Configuration["WebAPI:BaseUrl"]).AddPolicyHandler(restEaseRetryPolicy);

builder.Services.AddHttpClient("ApiRequestService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["WebAPI:BaseUrl"]);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddPolicyHandler(retryPolicy);

builder.Services.AddSingleton<IApiAuthService, ApiAuthService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("WebServiceError");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
/*app.UseExceptionHandler(exceptionHandlerApp =>
{
    Results.LocalRedirect("default");
    exceptionHandlerApp.Run(async context => {

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        Console.WriteLine(context.Response);

        await context.Response.WriteAsync("<h1 style=\"text-align: center\"> Web service Error.<h1>");
    });
});*/
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();