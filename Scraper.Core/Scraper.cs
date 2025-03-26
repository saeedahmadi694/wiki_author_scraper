using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Scraper.Core;


public static class ScraperCore
{
    public static async Task WikiScraper()
    {
        // Set the author name for search
        string authorName = "J. K. Rowling";
        // Build the Wikipedia API URL for Persian Wikipedia
        string searchUrl = $"https://fa.wikipedia.org/wiki/%D8%AC%D9%84%D8%A7%D9%84_%D8%A2%D9%84_%D8%A7%D8%AD%D9%85%D8%AF";

        using HttpClient client = new();
        try
        {
            // Search for the author page
            var searchResponse = await client.GetStringAsync(searchUrl);
            JArray searchResult = JArray.Parse(searchResponse);

            // The fourth element contains the URLs of matching pages
            if (searchResult.Count >= 4 && searchResult[3].HasValues)
            {
                string pageUrl = searchResult[3][0].ToString();
                Console.WriteLine($"Found page: {pageUrl}");

                // Fetch the HTML content of the page
                var pageHtml = await client.GetStringAsync(pageUrl);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(pageHtml);

                // Create a dictionary to store author data
                Dictionary<string, object> authorData = new Dictionary<string, object>();

                // Use the page title as the "name" (commonly known name)
                var titleNode = doc.DocumentNode.SelectSingleNode("//h1");
                if (titleNode != null)
                {
                    authorData["name"] = titleNode.InnerText.Trim();
                }

                // Locate the infobox (commonly has class 'infobox')
                var infobox = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'infobox')]");
                if (infobox != null)
                {
                    // Process each row in the infobox table
                    var rows = infobox.SelectNodes(".//tr");
                    if (rows != null)
                    {
                        foreach (var row in rows)
                        {
                            var header = row.SelectSingleNode(".//th");
                            var data = row.SelectSingleNode(".//td");
                            if (header != null && data != null)
                            {
                                string key = header.InnerText.Trim();
                                string value = data.InnerText.Trim();

                                // Map the Persian labels to our JSON schema keys
                                if (key.Contains("نام کامل"))
                                {
                                    authorData["fullName"] = value;
                                }
                                else if (key.Contains("تاریخ تولد"))
                                {
                                    authorData["birthDate"] = value;
                                }
                                else if (key.Contains("محل تولد"))
                                {
                                    authorData["birthPlace"] = value;
                                }
                                else if (key.Contains("ملیت"))
                                {
                                    authorData["nationality"] = value;
                                }
                                else if (key.Contains("ژانر") || key.Contains("سبک"))
                                {
                                    // Split by comma or Persian comma
                                    authorData["genres"] = value.Split(new char[] { ',', '،' }, StringSplitOptions.RemoveEmptyEntries);
                                }
                                else if (key.Contains("آثار برجسته"))
                                {
                                    authorData["notableWorks"] = value.Split(new char[] { ',', '،' }, StringSplitOptions.RemoveEmptyEntries);
                                }
                                else if (key.Contains("جوایز"))
                                {
                                    authorData["awards"] = value.Split(new char[] { ',', '،' }, StringSplitOptions.RemoveEmptyEntries);
                                }
                            }
                        }
                    }
                }

                // Get the first paragraph from the content area as biography
                var paraNode = doc.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']/p");
                if (paraNode != null)
                {
                    authorData["biography"] = paraNode.InnerText.Trim();
                }

                // Convert the dictionary to JSON format
                string jsonResult = JsonConvert.SerializeObject(authorData, Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine("\nExtracted Author Data:");
                Console.WriteLine(jsonResult);
            }
            else
            {
                Console.WriteLine("No matching page found for the given author.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

}
