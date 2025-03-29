using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;

namespace Scraper.Core.Models;


public class PersonInfoDto
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string RealName { get; set; }
    public string Born { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime DeathDate { get; set; }
    public string Died { get; set; }
    public string Occupation { get; set; }
    public string Nationality { get; set; }
    public string Genres { get; set; }
    public string YearsActive { get; set; }
    public string Description { get; set; }
    public string University { get; set; }
    public string Books { get; set; } 
}
