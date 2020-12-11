using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Pixel.IRIS5.API.Mobile.Models;
using Pixel.IRIS5.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    [Route("api/[controller]")]
    public class CSSettingController : Controller
    {
        string connectionSQL = GlobalVariables.ConnectionString;

        [HttpPost("GetSetting")]
        public APP_SETTING GetSetting([FromBody]JObject _objData)
        {
            APP_SETTING result = new APP_SETTING();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    result = dbConn.QueryFirstOrDefault<APP_SETTING>("SELECT * FROM APP_SETTING");

                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        [HttpPost("SaveSetting")]
        public string SaveSetting([FromBody]JObject _objData)
        {
            string _result = "";

            try
            {
                APP_SETTING setting = _objData["SettingData"].ToObject<APP_SETTING>();

                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    dbConn.Execute(@"UPDATE APP_SETTING SET                                         
                                        PRIMARY_THEME_COLOR = @PRIMARY_THEME_COLOR,                 
                                        SECONDARY_THEME_COLOR = @SECONDARY_THEME_COLOR,                                      
                                        EMAIL_USER_NAME = @EMAIL_USER_NAME,                                      
                                        EMAIL_USER_PWD = @EMAIL_USER_PWD,                                      
                                        EMAIL_SERVER = @EMAIL_SERVER,                                      
                                        EMAIL_SERVER_PORT = @EMAIL_SERVER_PORT,                                      
                                        SMS_USER_NAME = @SMS_USER_NAME,                                      
                                        SMS_USER_PWD = @SMS_USER_PWD,                                      
                                        SMS_HTTP_URL = @SMS_HTTP_URL,                                      
                                        BACKGROUND_COLOR = @BACKGROUND_COLOR,                                      
                                        APP_IMAGE_URL = @APP_IMAGE_URL",
                                      new
                                      {
                                          setting.primary_theme_color,
                                          setting.secondary_theme_color,
                                          setting.email_user_name,
                                          setting.email_user_pwd,
                                          setting.email_server,
                                          setting.email_server_port,
                                          setting.sms_user_name,
                                          setting.sms_user_pwd,
                                          setting.sms_http_url,
                                          setting.background_color,
                                          setting.app_image_url
                                      });
                }

                return _result;
            }
            catch (Exception ex)
            {
                return DataHelper.GetDataErrorMessage(-1, ex.Message, ex.InnerException);
            }
        }
    }
}
