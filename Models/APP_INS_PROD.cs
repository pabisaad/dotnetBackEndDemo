namespace Pixel.IRIS5.API.Mobile.Models
{
    using System;
    using Dapper.Contrib.Extensions;

    [Table("APP_INS_PROD")]
    public partial class APP_INS_PROD
    {
        [Key]
        public int id { get; set; }

        public int ins_id { get; set; }

        public int? prod_id { get; set; }
        
        public string prod_back_color { get; set; }

        public string prod_text_en { get; set; }
        public string prod_text_fr { get; set; }
        public string prod_text_ar { get; set; }
        public string prod_text_color { get; set; }
        public int prod_text_font_size { get; set; }

        public string prod_desc_en { get; set; }
        public string prod_desc_fr { get; set; }
        public string prod_desc_ar { get; set; }
        public string prod_desc_color { get; set; }
        public int prod_desc_font_size { get; set; }

        public int prod_order { get; set; }

        public int box_height { get; set; }
        
        public string full_info_path { get; set; }
     }
}



