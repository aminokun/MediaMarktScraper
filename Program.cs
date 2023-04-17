    using System;
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.IO;
    using System.Globalization;
    using System.Formats.Asn1;
    using MySql.Data.MySqlClient;
    using System.Collections;
    using System.Web;

    namespace scraper
    {
        class Program
        {
            static void Main(string[] args)
            {
                RefreshDatabase();
                string url = "https://www.mediamarkt.nl/nl/category/smartphones-283.html";
                var links = GetPhoneLinks(url);
                List<Phone> Phones = GetPhones(links);
                ExportToDatabase(Phones);
            }

            private static void RefreshDatabase()
            {
                string connectionString = "Server=192.168.178.27,3306;Database=Phones;Uid=Scraper;Pwd=123Scraper21!;";

                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand deleteCommand = new MySqlCommand("DELETE FROM phones", connection);
                deleteCommand.ExecuteNonQuery();

                MySqlCommand resetID = new MySqlCommand("ALTER TABLE phones AUTO_INCREMENT = 1", connection);
                resetID.ExecuteNonQuery();

                connection.Close();
            }
            private static void ExportToDatabase(List<Phone> Phones)
            {

                string connectionString = "Server=192.168.178.27,3306;Database=Phones;Uid=Scraper;Pwd=123Scraper21!;";

                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand deleteCommand = new MySqlCommand("DELETE FROM phones", connection);
                deleteCommand.ExecuteNonQuery();

                MySqlCommand resetID = new MySqlCommand("ALTER TABLE phones AUTO_INCREMENT = 1", connection);
                resetID.ExecuteNonQuery();


                foreach (var item in Phones)
                {
                    MySqlCommand command = new MySqlCommand("INSERT INTO phones (ImageUrl, Title, Price) VALUES (@ImageUrl, @Title, @Price)", connection);
                    command.Parameters.AddWithValue("@ImageUrl", item.ImageUrl);
                    command.Parameters.AddWithValue("@Title", item.Title);
                    command.Parameters.AddWithValue("@Price", item.Price);
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }

            private static List<Phone> GetPhones(List<string> links)
            {
                var Phones = new List<Phone>();
                foreach (var link in links)
                {
                    var doc = GetDocument(link);
                    var Phone = new Phone();

                    Phone.Title = HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode("//h1[@color=\"#000\"]").InnerText);

                    var xpath = "//div[@order=\"0\"]/span[2]";
                    var price_raw = doc.DocumentNode.SelectSingleNode(xpath).InnerText;
                    Phone.Price = ExtractPrice(price_raw);

                    // Extract image URL and convert to absolute URL
                    var imgNode = doc.DocumentNode.SelectSingleNode("//picture/img/@src\r\n");
                    if (imgNode != null)
                    {
                        var imageUrl = imgNode.Attributes["src"]?.Value;
                        Phone.ImageUrl = imageUrl;
                    }

                    Phones.Add(Phone);
                }
                return Phones;
            }

            static double ExtractPrice(string raw)
            {
                var reg = new Regex(@"[\d\.,]+", RegexOptions.Compiled);
                var m = reg.Match(raw);
                if (!m.Success)
                    return 0;

                var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var priceStr = m.Value.Replace(".", decimalSeparator).Replace(",", decimalSeparator);
                return Convert.ToDouble(priceStr);
            }


        static List<string> GetPhoneLinks(string url)
        {
            var doc = GetDocument(url);
            if (doc == null)
            {
                // Handle the case where the HTML document could not be loaded
                return new List<string>();
            }

            var linkNodes = doc.DocumentNode.SelectNodes("//a[@data-test=\"mms-product-list-item-link\"]");
            if (linkNodes == null)
            {
                // Handle the case where no nodes were selected with the given XPath expression
                return new List<string>();
            }

            var baseUri = new Uri(url);
            var links = new List<string>();
            foreach (var node in linkNodes)
            {
                var link = node.Attributes["href"]?.Value;
                if (!string.IsNullOrEmpty(link))
                {
                    link = new Uri(baseUri, link).AbsoluteUri;
                    links.Add(link);
                }
            }
            return links;
        }


        static HtmlDocument GetDocument(string url)
            {
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                return doc;
            }


        }
    }
