﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml;

namespace Scraper.Core;


public class PersonInfoDto
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string RealName { get; set; }
    public string Born { get; set; }
    //public DateTime BirthDate => MD.PersianDateTime.PersianDateTime()
    public string Died { get; set; }
    public string Occupation { get; set; }
    public string Nationality { get; set; }
    public string Genres { get; set; }
    public string YearsActive { get; set; }
}
