namespace Pixel.IRIS5.API.Mobile.Models
{
    using System;
    using Dapper.Contrib.Extensions;

    [Table("APP_MENU")]
    public partial class APP_MENU
    {
        [Key]
        public int id { get; set; }

        public int? parent_menu_id { get; set; }

        public string menu_back_color { get; set; }

        public string menu_text_en { get; set; }
        public string menu_text_fr { get; set; }
        public string menu_text_ar { get; set; }
        public string menu_text_color { get; set; }
        public int menu_text_font_size { get; set; }

        public string image_path { get; set; }
        public int image_width { get; set; }
        public int image_height { get; set; }

        public string menu_desc_en { get; set; }
        public string menu_desc_fr { get; set; }
        public string menu_desc_ar { get; set; }
        public string menu_desc_color { get; set; }
        public int menu_desc_font_size { get; set; }

        public int need_otp { get; set; }
    }
}



