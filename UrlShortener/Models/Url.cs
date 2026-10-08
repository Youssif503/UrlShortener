using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Models
{
    public class Url
    {
        public string ShortUrl { get; set; }
        public string LongUrl {get; set; }
        public DateTime? CreatedAt {get;set;} = DateTime.UtcNow;

    }
}