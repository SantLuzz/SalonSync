using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SalonSync.Web.Common.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.AddConfiguration();
builder.AddRoots();
builder.AddSecurity();
builder.AddExternalComponets();
builder.AddServices();
builder.AddLocalization();

await builder.Build().RunAsync();
