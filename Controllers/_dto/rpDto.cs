using System;
using System.Runtime.Serialization;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    public class RpDto
    {        
        public int company_id { get; set; }
        public int profile_type_id { get; set; }
        public int profile_id { get; set; }
        public int account_id { get; set; }
        public string print_name { get; set; }
        public int mvmt_id { get; set; }
        public int? inst_id { get; set; }
        public DateTime due_date { get; set; }
        public decimal due_amt { get; set; }   
    }
}