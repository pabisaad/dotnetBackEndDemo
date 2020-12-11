using System;
using System.Runtime.Serialization;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    public class ProposalData
    {
        public int proposal_id { get; set; }
        public DateTime proposal_date { get; set; }
        public string product_code { get; set; }

        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }

        public DateTime dob { get; set; }
        public decimal balance { get; set; }
        public string cur_code { get; set; }

        // 2400 Request 
        // 2402 Approved
        // 2403 Rejected
        public int status { get; set; }
    }
}