
using System.ComponentModel;
using Microsoft.SemanticKernel;


namespace BlazorSKApp;

public class WeatherPlugin
{
    [KernelFunction("weather_data")]
    [Description("Get the weather data for a city ")]
    [return : Description("A list of weather for a city")]

    public async Task<string?> GetWeatherData (string location)
    {
        var config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        // Get configuration values
        string? apiKey = config["Weather.com:key"];
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("API key is missing in the configuration.");
        }

        string url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={location}&aqi=no";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }

        }
        catch (HttpRequestException e)
        {
            // raise an exception using HTTP error code
            throw new HttpRequestException($"Request error: {e.Message}", e, e.StatusCode);
        }
        catch (Exception e)
        {
            // raise an exception using HTTP error code
            throw new Exception($"An error occurred: {e.Message}", e);
        }

    }
}
