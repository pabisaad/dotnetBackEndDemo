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
using Pixel.IRIS5.Models;
using Pixel.IRIS5.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    [Route("api/[controller]")]
    public class CSProductController : Controller
    {
        string connectionSQL = GlobalVariables.ConnectionString;

        [HttpPost("GetAllCSProduct")]
        public Array GetAllCSProduct([FromBody]JObject _objData)
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
	                                    APP_INS_PROD.ID,
	                                    APP_INS_PROD.PROD_TEXT_EN,
	                                    APP_INS_PROD.PROD_TEXT_FR,
	                                    APP_INS_PROD.PROD_DESC_AR,
	                                    APP_INS_PROD.PROD_ORDER
                                    FROM APP_INS_PROD
                                    ORDER BY PROD_ORDER";

                    result.AddRange(dbConn.Query<object>(strQuery).ToList());
                }
                returnArray[0] = result.ToArray();

                ArrayList _lst = new ArrayList
                {
                    new string[5] { "APP_INS_PROD", "", "", "", "hidden" },
                    new string[5] { "ID", "ID", "int","", "visible"  },
                    new string[5] { "PROD_TEXT_EN", "EN", "string","", "visible"  },
                    new string[5] { "PROD_TEXT_FR", "FR", "string","", "visible"  },
                    new string[5] { "PROD_DESC_AR", "AR", "string", "", "visible" },
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

        [HttpPost("GetAllowedProducts")]
        public List<UW_PROD> GetAllowedProducts([FromBody]JObject _objData)
        {
            List<UW_PROD> result = new List<UW_PROD>();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    var strQuery = @"SELECT 
	                                    ID,
	                                    PROD_CODE,
	                                    PROD_DESC
                                    FROM UW_PROD
                                    WHERE COB_ID IN (SELECT 
                                                        ID	                                   
                                                    FROM UW_COB
                                                    WHERE ID IN (" + _objData["AllowedCob"].ToString() + ")" +
                                                    " AND LOB_ID = " + _objData["LobID"] + ")";

                    result.AddRange(dbConn.Query<UW_PROD>(strQuery).ToList());


                    return result;
                }
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        [HttpPost("GetProductByID")]
        public APP_INS_PROD GetProductByID([FromBody]JObject _objData)
        {
            APP_INS_PROD result = new APP_INS_PROD();

            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    result = dbConn.QueryFirstOrDefault<APP_INS_PROD>("SELECT * FROM APP_INS_PROD WHERE ID = " + _objData["ID"]);

                    return result;
                }
            }
            catch (Exception)
            {
                return result;
            }
        }

        [HttpPost("SaveProduct")]
        public string SaveProduct([FromBody]JObject _objData)
        {
            string _result = "";

            try
            {
                APP_INS_PROD product = _objData["ProductData"].ToObject<APP_INS_PROD>();

                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    if (product.id > 0)
                    {
                        _result = product.id.ToString();

                        dbConn.Execute(@"UPDATE APP_INS_PROD SET                                         
                                        INS_ID = @INS_ID,                                      
                                        PROD_BACK_COLOR = @PROD_BACK_COLOR,                                      
                                        PROD_DESC_AR = @PROD_DESC_AR,                                      
                                        PROD_DESC_COLOR = @PROD_DESC_COLOR,                                      
                                        PROD_DESC_EN = @PROD_DESC_EN,                                      
                                        PROD_DESC_FONT_SIZE = @PROD_DESC_FONT_SIZE,                                      
                                        PROD_DESC_FR = @PROD_DESC_FR,                                      
                                        PROD_ID = @PROD_ID,                                      
                                        PROD_ORDER = @PROD_ORDER,                                      
                                        BOX_HEIGHT = @BOX_HEIGHT,                                      
                                        FULL_INFO_PATH = @FULL_INFO_PATH,                                      
                                        PROD_TEXT_AR = @PROD_TEXT_AR,                                      
                                        PROD_TEXT_COLOR = @PROD_TEXT_COLOR,                                      
                                        PROD_TEXT_EN = @PROD_TEXT_EN,                                   
                                        PROD_TEXT_FONT_SIZE = @PROD_TEXT_FONT_SIZE,                                   
                                        PROD_TEXT_FR = @PROD_TEXT_FR
                                        WHERE ID = @ID",
                                       new
                                       {
                                           product.id,
                                           product.ins_id,
                                           product.prod_back_color,
                                           product.prod_desc_ar,
                                           product.prod_desc_color,
                                           product.prod_desc_en,
                                           product.prod_desc_font_size,
                                           product.prod_desc_fr,
                                           product.prod_id,
                                           product.prod_order,
                                           product.box_height,
                                           product.full_info_path,
                                           product.prod_text_ar,
                                           product.prod_text_color,
                                           product.prod_text_en,
                                           product.prod_text_font_size,
                                           product.prod_text_fr
                                       });
                    }
                    else
                    {
                        _result = dbConn.Insert(new APP_INS_PROD
                        {
                            ins_id = product.ins_id,
                            prod_back_color = product.prod_back_color,
                            prod_desc_ar = product.prod_desc_ar,
                            prod_desc_color = product.prod_desc_color,
                            prod_desc_en = product.prod_desc_en,
                            prod_desc_font_size = product.prod_desc_font_size,
                            prod_desc_fr = product.prod_desc_fr,
                            prod_id = product.prod_id,
                            prod_order = product.prod_order,
                            box_height = product.box_height,
                            full_info_path = product.full_info_path,
                            prod_text_ar = product.prod_text_ar,
                            prod_text_color = product.prod_text_color,
                            prod_text_en = product.prod_text_en,
                            prod_text_font_size = product.prod_text_font_size,
                            prod_text_fr = product.prod_text_fr
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

        [HttpPost("DeleteProduct")]
        public string DeleteProduct([FromBody]JObject _objData)
        {
            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = connectionSQL;
                    dbConn.Open();

                    var result = dbConn.Execute("DELETE FROM APP_INS_PROD WHERE ID = " + _objData["ID"].ToString());

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return DataHelper.GetDataErrorMessage(-1, ex.Message, ex.InnerException);
            }
        }


        [HttpPost("GetInsuredProducts")]
        public List<InsuredInsurance> GetInsuredProducts([FromBody]JObject _objData)
        {
            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = connectionSQL;
                dbConn.Open();
                string query = string.Empty;
                var policyNo = _objData["PolicyNo"].ToString();
                if (policyNo != "")
                { query = $" and (POLICY_NO= '{policyNo}') "; }
                var result = dbConn.Query<InsuredInsurance>($"SELECT * FROM UW_POLICY WHERE (INSURED_ID ={_objData["InsuredID"]}) {query} order by POLICY_NO").ToList();

                return result;
            }
        }

        [HttpPost("GetProductInstallements")]
        public List<PolicyInstallements> GetProductInstallements([FromBody]JObject _objData)
        {
            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = connectionSQL;
                dbConn.Open();
                var result = dbConn.Query<PolicyInstallements>($"SELECT RP_MVMT_INST.ID, RP_MVMT_INST.LINKED_TO_INST_ID, RP_MVMT_INST.MVMT_ID, RP_MVMT_INST.INST_NO, RP_MVMT_INST.DUE_DATE, RP_MVMT_INST.AMOUNT, RP_MVMT_INST.VAT_AMT, RP_MVMT_INST.PAID_AMT, RP_MVMT_INST.INTEREST_AMT, RP_MVMT_INST.PDC_AMT, RP_MVMT_INST.GRACE_PERIOD, RP_MVMT_INST.IS_POSTED, RP_MVMT_INST.LEDGER_TRS_ID, RP_MVMT_INST.PPWD, RP_MVMT_INST.COLLECTION_CHARGE, RP_MVMT_INST.REFERENCE, RP_MVMT_INST.IS_PAYER, RP_MVMT_INST.CREATED_ON, RP_MVMT_INST.CREATED_BY, RP_MVMT_INST.EDITED_ON, RP_MVMT_INST.EDITED_BY, RP_MVMT_INST.ALLOCATED_DB, RP_MVMT_INST.ALLOCATED_CR FROM RP_MVMT LEFT OUTER JOIN RP_MVMT_INST ON RP_MVMT.ID = RP_MVMT_INST.MVMT_ID WHERE(RP_MVMT.LINK_ID = 1727656) AND(RP_MVMT.LINK_TYPE_ID = 1)").ToList();

                return result;
            }
        }
    }
}
