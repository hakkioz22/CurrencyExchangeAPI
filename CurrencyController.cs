using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchangeAPI;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public CurrencyController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("usd")]
    public async Task<IActionResult> GetUSD()
    {
        try
        {
            const string url = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var response = await _httpClient.GetStringAsync(url);

            //parse XML Data
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            //get usd rate with XPath
            var usdNode = xmlDoc.SelectSingleNode("//Currency[@CurrencyCode='USD']/ForexSelling");
            if (usdNode == null) return NotFound("USD exchange rate not found");
            var usdRate = usdNode.InnerText;
            return Ok(new { USDExchangeRate = usdRate });

        }
        catch (HttpRequestException httpRequestException)
        {
            return StatusCode(500, $"Request failed: {httpRequestException.Message}");
        }
        catch (XmlException xmlException)
        {
            return StatusCode(500, $"Error parsing XML data: {xmlException.Message}");
        }
    }
}