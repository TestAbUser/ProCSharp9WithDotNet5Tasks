using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Configuration;
using System.Threading.Channels;

namespace AutoLot.MVC.Models
{
    public class DealerInfo
    {
        public string DealerName { get; set; }
        public string City { get; set; }

        public string State { get; set; }
    }
}
