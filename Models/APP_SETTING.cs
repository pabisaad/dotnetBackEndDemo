namespace Pixel.IRIS5.API.Mobile.Models
{
    using System;
    using Dapper.Contrib.Extensions;

    [Table("APP_SETTING")]
    public partial class APP_SETTING
    {
        //[Key]      
        public string primary_theme_color { get; set; }
        public string secondary_theme_color { get; set; }
        public string email_user_name { get; set; }
        public string email_user_pwd { get; set; }
        public string email_server { get; set; }
        public string email_server_port { get; set; }
        public string sms_user_name { get; set; }
        public string sms_user_pwd { get; set; }
        public string sms_http_url { get; set; }
        public string allowed_cob { get; set; }
        public string background_color { get; set; }
        public string app_image_url { get; set; }

    }
}



