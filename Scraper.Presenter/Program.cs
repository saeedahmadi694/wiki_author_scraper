﻿// See https://aka.ms/new-console-template for more information

using Scraper.Core;

Console.WriteLine("Hello, World!");
var scraper = new ScraperCore();
await scraper.WikiScraper();