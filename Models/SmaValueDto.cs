using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.IRIS5.API.Mobile.Models
{
    public class SmaValueDto
    {
        public int id { get; set; }
        
        public string value_code { get; set; }

        public string value_desc { get; set; }

        public int parent_value_id { get; set; }

        public int? property_table_id { get; set; }
        public int? ref_value_id1 { get; set; }
        public int? ref_value_id2 { get; set; }        
    }
}
