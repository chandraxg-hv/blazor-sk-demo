using BlazorSKApp.Components;
using Microsoft.SemanticKernel;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//wire up the semantic kernel to the Blazor app
builder.Services.AddKernel()
    .Plugins.AddFromType<BlazorSKApp.WeatherPlugin>( "WeatherPlugin");
    
builder.Services.AddKernel()
    .Plugins.AddFromType<BlazorSKApp.ExchangeRatesPlugin>( "ExchangeRatesPlugin");

builder.Services.AddKernel()
    .Plugins.AddFromType<BlazorSKApp.NewsFeedPlugin>( "NewsFeedPlugin");


//get the open AI key and model name from app.settings.json file
//string apiKey = builder.Configuration.GetValue<string>("OpenAI:key");
//string modelId = builder.Configuration.GetValue<string>("OpenAI:model");

//get the open AI key and model name from appsettings.json file
string? apiKey = builder.Configuration["AzureOpenAI:key"];
string? endpoint = builder.Configuration["AzureOpenAI:endpoint"];
string? modelId = builder.Configuration["AzureOpenAI:deploymentName"];

// Check if the configuration values are provided
if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(modelId))
{
    throw new InvalidOperationException("OpenAI key, endpoint, and model ID must be provided in the configuration.");
}

// Calling OpenAI Chat Completion API
// builder.Services.AddOpenAIChatCompletion (
//     modelId,
//     apiKey: apiKey
// );

//Calling Azure OpenAI Chat Completion API
builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: modelId,
    endpoint: endpoint,
    apiKey: apiKey
);

//builder.Services.AddScoped(sp => KernelPluginFactory.CreateFromType<BlazorSKApp.WeatherPlugin>(sp));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
