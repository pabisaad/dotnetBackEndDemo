using System.Runtime.Serialization;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    public class PolicyCollectionDto
    { 
        public string policy_no { get; set; } 
        public decimal paid_amt { get; set; }  
        public string cur_code { get; set; }
    }
}