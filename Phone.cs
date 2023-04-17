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
        public double Price { get; set; }
        public string? UPC { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}, Price: {Price}";
        }
    }
}
