using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Pixel.IRIS5.API.Mobile.Models;
using Pixel.IRIS5.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    [Route("api/[controller]")]
    public class CSInsController : Controller
    {
        string connectionSQL = GlobalVariables.ConnectionString;

        [HttpPost("GetAllCSIns")]
        public Array GetAllCSIns([FromBody]JObject _objData)
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
	                                    APP_INS.ID,
	                                    APP_INS.INS_TEXT_EN,
	                                    APP_INS.INS_TEXT_FR,
	                                    APP_INS.INS_DESC_AR,
	                                    APP_INS.INS_ORDER
                                    FROM APP_INS
                                    ORDER BY INS_ORDER";

                    result.AddRange(dbConn.Query<object>(strQuery).ToList());
                }
                returnArray[0] = result.ToArray();

                ArrayList _lst = new ArrayList
                {
                    new string[5] { "APP_INS", "", "", "", "hidden" },
                    new string[5] { "ID", "ID", "int","", "visible"  },
                    new string[5] { "INS_TEXT_EN", "EN", "string","", "visible"  },
                    new string[5] { "INS_TEXT_FR", "FR", "string","", "visible"  },
                    new string[5] { "INS_DESC_AR", "AR", "string", "", "visible" },
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

        [HttpPost("GetInsList")]
        public List<APP_INS> GetInsList([FromBody]JObject _objData)
        {
            List<APP_INS> result = new List<APP_INS>();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();
                    var strQuery = @"SELECT
	                                    APP_INS.ID,
	                                    APP_INS.LOB_ID,
	                                    APP_INS.INS_TEXT_EN,
	                                    APP_INS.INS_TEXT_FR,
	                                    APP_INS.INS_DESC_AR,
	                                    APP_INS.INS_ORDER
                                    FROM APP_INS
                                    ORDER BY INS_ORDER";

                    result.AddRange(dbConn.Query<APP_INS>(strQuery).ToList());
                }               

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        [HttpPost("GetInsByID")]
        public APP_INS GetInsByID([FromBody]JObject _objData)
        {
            APP_INS result = new APP_INS();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    result = dbConn.QueryFirstOrDefault<APP_INS>("SELECT * FROM APP_INS WHERE ID = " + _objData["ID"]);

                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        [HttpPost("SaveIns")]
        public string SaveIns([FromBody]JObject _objData)
        {
            string _result = "";

            try
            {
                APP_INS ins = _objData["InsData"].ToObject<APP_INS>();

                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    if (ins.id > 0)
                    {
                        _result = ins.id.ToString();

                        dbConn.Execute(@"UPDATE APP_INS SET                                         
                                        IMAGE_HEIGHT = @IMAGE_HEIGHT,                                      
                                        IMAGE_PATH = @IMAGE_PATH,                                      
                                        IMAGE_WIDTH = @IMAGE_WIDTH,                                      
                                        INS_BACK_COLOR = @INS_BACK_COLOR,                                      
                                        INS_DESC_AR = @INS_DESC_AR,                                      
                                        INS_DESC_COLOR = @INS_DESC_COLOR,                                      
                                        INS_DESC_EN = @INS_DESC_EN,                                      
                                        INS_DESC_FONT_SIZE = @INS_DESC_FONT_SIZE,                                      
                                        INS_DESC_FR = @INS_DESC_FR,                                      
                                        INS_ORDER = @INS_ORDER,                                      
                                        INS_TEXT_AR = @INS_TEXT_AR,                                      
                                        INS_TEXT_COLOR = @INS_TEXT_COLOR,                                      
                                        INS_TEXT_EN = @INS_TEXT_EN,                                      
                                        INS_TEXT_FONT_SIZE = @INS_TEXT_FONT_SIZE,                                   
                                        INS_TEXT_FR = @INS_TEXT_FR     
                                        WHERE ID = @ID",
                                       new
                                       {
                                           ins.id,
                                           ins.image_height,
                                           ins.image_path,
                                           ins.image_width,
                                           ins.ins_back_color,
                                           ins.ins_desc_ar,
                                           ins.ins_desc_color,
                                           ins.ins_desc_en,
                                           ins.ins_desc_font_size,
                                           ins.ins_desc_fr,
                                           ins.ins_order,
                                           ins.ins_text_ar,
                                           ins.ins_text_color,
                                           ins.ins_text_en,
                                           ins.ins_text_font_size,
                                           ins.ins_text_fr
                                       });
                    }
                    else
                    {
                        _result = dbConn.Insert(new APP_INS
                        {
                            image_height = ins.image_height,
                            image_path = ins.image_path,
                            image_width = ins.image_width,
                            ins_back_color = ins.ins_back_color,
                            ins_desc_ar = ins.ins_desc_ar,
                            ins_desc_color = ins.ins_desc_color,
                            ins_desc_en = ins.ins_desc_en,
                            ins_desc_font_size = ins.ins_desc_font_size,
                            ins_desc_fr = ins.ins_desc_fr,
                            ins_order = ins.ins_order,
                            ins_text_ar = ins.ins_text_ar,
                            ins_text_color = ins.ins_text_color,
                            ins_text_en = ins.ins_text_en,
                            ins_text_font_size = ins.ins_text_font_size,
                            ins_text_fr = ins.ins_text_fr
                        }).ToString();
                    }
                }

                return _result;
            }
            catch (Exception ex)
            {
                return DataHelper.GetDataErrorMessage(-1, ex.Message, ex.InnerException);
            }
        }

        //[HttpPost("DeleteIns")]
        //public string DeleteIns([FromBody]JObject _objData)
        //{
        //    try
        //    {
        //        using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
        //        {
        //            dbConn.ConnectionString = connectionSQL;
        //            dbConn.Open();

        //            var result = dbConn.Execute("DELETE FROM APP_INS WHERE ID = " + _objData["ID"].ToString());

        //            return "success";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return DataHelper.GetDataErrorMessage(-1, ex.Message, ex.InnerException);
        //    }
        //}
    }
}
