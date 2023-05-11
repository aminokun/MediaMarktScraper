using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scraper
{
    public class Phone
    {
        public string? ImageUrl { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public int? ArtNr { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}, Price: {Price}";
        }
    }
}
