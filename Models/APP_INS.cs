namespace Pixel.IRIS5.API.Mobile.Models
{
    using System;
    using Dapper.Contrib.Extensions;

    [Table("APP_INS")]
    public partial class APP_INS
    {
        [Key]
        public int id { get; set; }

        public int lob_id { get; set; }

        public string ins_back_color { get; set; }
        
        public string ins_text_en { get; set; }
        public string ins_text_fr { get; set; }
        public string ins_text_ar { get; set; }
        public string ins_text_color { get; set; }
        public int ins_text_font_size { get; set; }

        public string image_path { get; set; }
        public int image_width { get; set; }
        public int image_height { get; set; }
        
        public string ins_desc_en { get; set; }
        public string ins_desc_fr { get; set; }
        public string ins_desc_ar { get; set; }
        public string ins_desc_color { get; set; }
        public int ins_desc_font_size { get; set; }
                
        public int ins_order { get; set; }
    }
}



