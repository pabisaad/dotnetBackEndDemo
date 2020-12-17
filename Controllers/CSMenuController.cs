using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json.Linq;
using Pixel.IRIS5.API.Mobile.Models;
using Pixel.IRIS5.API.Web.Controllers.UW;
using Pixel.IRIS5.Models;
using Pixel.IRIS5.Shared;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    [Route("api/[controller]")]
    public class CSMenuController : Controller
    {
        string connectionSQL = GlobalVariables.ConnectionString;

        [HttpPost("GetAllCSMenu")]
        public Array GetAllCSMenu([FromBody]JObject _objData)
        {
            Array[] returnArray = new Array[2];

            ArrayList result = new ArrayList();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();
                    var strQuery = @"SELECT
	                                    APP_MENU.ID,
	                                    APP_MENU.MENU_TEXT_EN,
	                                    APP_MENU.MENU_TEXT_FR,
	                                    APP_MENU.MENU_TEXT_AR
                                    FROM APP_MENU";

                    result.AddRange(dbConn.Query<object>(strQuery).ToList());
                }
                returnArray[0] = result.ToArray();

                ArrayList _lst = new ArrayList
                {
                    //test
                    new string[5] { "APP_MENU", "", "", "", "hidden" },
                    new string[5] { "ID", "ID", "int","", "visible"  },
                    new string[5] { "MENU_TEXT_EN", "EN", "string","", "visible"  },
                    new string[5] { "MENU_TEXT_FR", "FR", "string","", "visible"  },
                    new string[5] { "MENU_TEXT_AR", "AR", "string", "", "visible" },
                    new string[5] { "", "", "string", "", "visible" }
              };

                returnArray[1] = _lst.ToArray();

                return returnArray;
            }
            catch (Exception)
            {
                return returnArray;
            }
        }

        [HttpPost("GetHomePageMenu")]
        public List<APP_MENU> GetHomePageMenu([FromBody]JObject _objData)
        { 
            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = connectionSQL;
                dbConn.Open();

                var result = dbConn.Query<APP_MENU>("SELECT * FROM APP_MENU ORDER BY ID ").ToList();
                 
                return result;
            }
        }

        [HttpPost("GetMenuByID")]
        public APP_MENU GetMenuByID([FromBody]JObject _objData)
        {
            APP_MENU result = new APP_MENU();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    result = dbConn.QueryFirstOrDefault<APP_MENU>("SELECT * FROM APP_MENU WHERE ID = " + _objData["ID"]);

                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        [HttpPost("SaveMenu")]
        public string SaveMenu([FromBody]JObject _objData)
        {
            string _result = "";

            try
            {
                APP_MENU menu = _objData["MenuData"].ToObject<APP_MENU>();

                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    dbConn.Execute(@"UPDATE APP_MENU SET                                         
                                        IMAGE_HEIGHT = @IMAGE_HEIGHT,                                      
                                        IMAGE_PATH = @IMAGE_PATH,                                      
                                        IMAGE_WIDTH = @IMAGE_WIDTH,                                      
                                        MENU_BACK_COLOR = @MENU_BACK_COLOR,                                      
                                        MENU_DESC_AR = @MENU_DESC_AR,                                      
                                        MENU_DESC_COLOR = @MENU_DESC_COLOR,                                      
                                        MENU_DESC_EN = @MENU_DESC_EN,                                      
                                        MENU_DESC_FONT_SIZE = @MENU_DESC_FONT_SIZE,                                      
                                        MENU_DESC_FR = @MENU_DESC_FR,                                      
                                        MENU_TEXT_AR = @MENU_TEXT_AR,                                      
                                        MENU_TEXT_COLOR = @MENU_TEXT_COLOR,                                      
                                        MENU_TEXT_EN = @MENU_TEXT_EN,                                      
                                        MENU_TEXT_FONT_SIZE = @MENU_TEXT_FONT_SIZE,                                      
                                        MENU_TEXT_FR = @MENU_TEXT_FR,                                   
                                        NEED_OTP = @NEED_OTP     
                                        WHERE ID = @ID",
                                      new
                                      {
                                          menu.id,
                                          menu.image_height,
                                          menu.image_path,
                                          menu.image_width,
                                          menu.menu_back_color,
                                          menu.menu_desc_ar,
                                          menu.menu_desc_color,
                                          menu.menu_desc_en,
                                          menu.menu_desc_font_size,
                                          menu.menu_desc_fr,
                                          menu.menu_text_ar,
                                          menu.menu_text_color,
                                          menu.menu_text_en,
                                          menu.menu_text_font_size,
                                          menu.menu_text_fr,
                                          menu.need_otp                         
                                      });
                }

                return _result;
            }
            catch (Exception ex)
            {
                return DataHelper.GetDataErrorMessage(-1, ex.Message, ex.InnerException);
            }
        }

        [HttpPost("GetInsMenu")]
        public List<APP_INS> GetInsMenu([FromBody]JObject _objData)
        {
            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = connectionSQL;
                dbConn.Open();

                var result = dbConn.Query<APP_INS>("SELECT * FROM APP_INS ORDER BY INS_ORDER").ToList();

                return result;
            }
        }

        [HttpPost("GetInsProdMenu")]
        public List<APP_INS_PROD> GetInsProdMenu([FromBody]JObject _objData)
        {
            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = connectionSQL;
                dbConn.Open();

                var result = dbConn.Query<APP_INS_PROD>("SELECT * FROM APP_INS_PROD ORDER BY INS_ID, PROD_ORDER").ToList();

                return result;
            }
        }
         
    }
}