using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.IRIS5.API.Mobile.Models
{
    public class SmaPropertyDto
    {
        public int id { get; set; }
                
        public string property_type { get; set; }

        public string property_code { get; set; }
        
        public string property_desc { get; set; }
                        
        public int? property_table_id { get; set; }

        public string system_table { get; set; }
               
        public int? parent_table_id { get; set; }

        public bool is_mandatory { get; set; }

        public string formula { get; set; }

        //public int object_id { get; set; }
        //public string contact_field { get; set; }
        //public int? contact_field_property_type_cd { get; set; }
        //public int? ref_table_id1 { get; set; }
        //public int? ref_table_id2 { get; set; }

    }
}
