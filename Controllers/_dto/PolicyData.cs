using System.Runtime.Serialization;

namespace Pixel.IRIS5.API.Mobile.Controllers
{ 
    public class PolicyBalance
    { 
        public int root_id { get; set; } 
        public string policy_no { get; set; } 
        public string cur_code { get; set; }
        public decimal balance { get; set; }
    }
}