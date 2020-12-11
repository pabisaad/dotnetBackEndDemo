using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.IRIS5.API.Mobile.Models
{
    using System;
    using Dapper.Contrib.Extensions;

    [Table("UW_POLICY")]
    public partial class InsuredInsurance
    {
        //[Key]      
        public int ID { get; set; }
        public string POLICY_NO { get; set; }
        public int INSURED_ID { get; set; }
        public string PRINT_NAME { get; set; }
       

    }
}
