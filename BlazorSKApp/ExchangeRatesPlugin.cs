
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace BlazorSKApp;

public class ExchangeRatesPlugin
{
    [KernelFunction("exchangerate_data")]
    [Description("Get the exchange rate data for currency ")]
    [return : Description("A list of exchange rate data for currency")]

    public async Task<string?> GetExchangeRate (string basecurrency, List<string> listcurrency)
    {
        // Convert the list of currencies to a comma-separated string
        string currency = string.Join(",", listcurrency);

        // Create a configuration builder to read the appsettings.json file
        var config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        // Get configuration values
        string? apiKey = config["ExchangeRate.io:key"];
   
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("API key is missing in the configuration.");
        }

        //check if the base currency is in the list of symbols
        //call exchangerates symbols api
        string symbolsUrl = $"https://api.exchangeratesapi.io/v1/symbols?access_key={apiKey}&base={basecurrency}";
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(symbolsUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //check if the base currency is in the list of symbols
                if (!responseBody.Contains(basecurrency))
                {
                    throw new InvalidOperationException($"Base currency {basecurrency} is not valid.");
                }
                else
                {
                    return responseBody;
                }
            }
        }
        catch (HttpRequestException e)
        {
            throw new HttpRequestException($"Request error: {e.Message}", e);
        }
        catch (Exception e)
        {
            throw new HttpRequestException($"Request error: {e.Message}", e);   
        }
    }
}
