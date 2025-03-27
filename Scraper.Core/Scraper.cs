using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;

namespace Scraper.Core;


public class ScraperCore
{
    public async Task WikiScraper()
    {
        string url = "https://fa.wikipedia.org/wiki/%D8%AC%D8%B1%D8%AC_%D8%A7%D9%88%D8%B1%D9%88%D9%84";

        try
        {
            string html = await FetchHtml(url);
            //string content = ExtractMainContent(html);
            var person = ExtractInfobox(html);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    private async Task<string> FetchHtml(string url)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        return await client.GetStringAsync(url);
    }

    private PersonInfoDto ExtractInfobox(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        var dto = new PersonInfoDto();

        var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'infobox')]");

        if (table == null)
            return dto;

        foreach (var row in table.SelectNodes(".//tr"))
        {
            var header = row.SelectSingleNode("./th");
            var data = row.SelectSingleNode("./td");


            if (header != null && header.GetAttributeValue("class", "").Contains("infobox-above"))
            {
                dto.Name = header.InnerText.Trim();
                continue;
            }
            else if (header != null && header.GetAttributeValue("class", "").Contains("infobox-image"))
            {
                var img = row.SelectSingleNode(".//img");
                var src = img?.GetAttributeValue("src", "");
                if (!string.IsNullOrEmpty(src))
                    dto.ImageUrl = "https:" + src;

                continue;
            }

            else if(header != null && data != null)
            {
                string label = header.InnerText.Trim();
                string value = data.InnerText.Trim();

                switch (label)
                {
                    case "نام اصلی":
                        dto.RealName = value;
                        break;
                    case "زاده":
                        dto.Born = value;
                        break;
                    case "درگذشته":
                        dto.Died = value;
                        break;
                    case "پیشه":
                        dto.Occupation = value;
                        break;
                    case "ملیت":
                        dto.Nationality = value;
                        break;
                    case "سبک(های) نوشتاری":
                        dto.Genres = value;
                        break;
                    case "سال‌های فعالیت":
                        dto.YearsActive = value;
                        break;
                }
            }

        }


        Console.WriteLine($"Name: {dto.Name}");
        Console.WriteLine($"Image: {dto.ImageUrl}");
        Console.WriteLine($"Real Name: {dto.RealName}");
        Console.WriteLine($"Born: {dto.Born}");
        Console.WriteLine($"Died: {dto.Died}");
        Console.WriteLine($"Occupation: {dto.Occupation}");
        Console.WriteLine($"Nationality: {dto.Nationality}");
        Console.WriteLine($"Genres: {dto.Genres}");
        Console.WriteLine($"Years Active: {dto.YearsActive}");

        return dto;
    }
    static string ExtractMainContent(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        var contentNode = doc.DocumentNode.SelectSingleNode("//ul[@id='mw-panel-toc-list']");

        if (contentNode == null)
            throw new Exception("Main content not found.");

        // Get all paragraph texts
        var paragraphs = contentNode.SelectNodes(".//p");

        if (paragraphs == null)
            return "No content found.";

        string result = "";
        foreach (var p in paragraphs)
        {
            string text = p.InnerText.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                result += text + "\n\n";
            }
        }

        return result;
    }
}
