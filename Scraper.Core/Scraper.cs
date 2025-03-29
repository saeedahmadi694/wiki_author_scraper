using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scraper.Core.Models;
using System;
using System.Text;
using System.Xml;

namespace Scraper.Core;


public class ScraperCore
{

    public async Task WikiScraper(string firstName, string lastName)
    {
        string url = "https://fa.wikipedia.org/wiki/";

        try
        {
            string html = await FetchHtml(url);
            var person = ExtractInfobox(html);
            var description = ExtractParagraphsAfterInfobox(html);
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

            else if (header != null && data != null)
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
                    case "دانشگاه":
                        dto.University = value;
                        break;
                    case "کار(های) برجسته":
                        dto.Books = value;
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

    private string ExtractParagraphsAfterInfobox(string html)
    {
        var description = new StringBuilder();
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        var contentDiv = doc.DocumentNode
            .SelectSingleNode("//div[contains(@class, 'mw-parser-output')]");

        if (contentDiv == null)
        {
            Console.WriteLine("Content div not found.");
            return "";
        }

        foreach (var node in contentDiv.ChildNodes)
        {

            if (node.Name == "div" && node.Attributes.Any(r => r.Value.Contains("mw-heading mw-heading2")))
                break;
            if (node.Name == "p")
                description.AppendLine(node.InnerText.Trim());

        }
        return description.ToString();

    }
}
