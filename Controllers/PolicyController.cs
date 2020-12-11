using System;
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
using Pixel.IRIS5.API.Web;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    [Route("api/[controller]")]
    public class AppPolicyController : Controller
    {
        //NOTUSED YET
        //[HttpPost("GetProductDimensions")]
        //public List<Models.ProdDimDto> GetProductDimensions([FromBody]JObject _objData)
        //{
        //    using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
        //    {
        //        dbConn.ConnectionString = GlobalVariables.ConnectionString;
        //        dbConn.Open();

        //        //var strQuery=
        //        //    @"SELECT 
        //        //        UW_PROD_DIM.PROD_ID , 
        //        //  UW_PROD_DIM.OBJECT_PROPERTY_ID,
        //        //  OB_PROPERTY.PROPERTY_TYPE_ID, 
        //        //  OB_PROPERTY.PROPERTY_DESC, 
        //        //  OB_PROPERTY_TYPE.PROPERTY_TYPE_DESC,
        //        //  OB_PROPERTY.PROPERTY_TABLE_ID,
        //        //  OB_PROPERTY_TABLE.PROPERTY_TABLE_DESC
        //        //    FROM UW_PROD_DIM 
        //        //     LEFT JOIN OB_OBJECT_PROPERTY ON UW_PROD_DIM.OBJECT_PROPERTY_ID = OB_OBJECT_PROPERTY.ID  
        //        //     LEFT JOIN OB_PROPERTY ON OB_OBJECT_PROPERTY.PROPERTY_ID = OB_PROPERTY.ID  
        //        //     LEFT JOIN OB_PROPERTY_TYPE ON OB_PROPERTY.PROPERTY_TYPE_ID = OB_PROPERTY_TYPE.ID  
        //        //     LEFT JOIN OB_PROPERTY_TABLE ON OB_PROPERTY.PROPERTY_TABLE_ID = OB_PROPERTY_TABLE.ID  
        //        //    WHERE UW_PROD_DIM.PROD_ID IN (SELECT PROD_ID FROM APP_INS_PROD)  ";

        //        var strQuery = @"SELECT 
        //                         ID,
        //                         PROD_ID,
        //                         MEASURE_TYPE_ID,
        //                         OBJECT_PROPERTY_ID
        //                        FROM UW_PROD_DIM
        //                        WHERE PROD_ID = " + _objData["ID"];

        //        var result = dbConn.Query<Pixel.IRIS5.API.Mobile.Models.ProdDimDto>(strQuery).ToList();

        //        return result;
        //    }
        //}

        //NOTUSED YET
        //[HttpPost("GetProductDimensionValues")]
        //public List<ProdDimVlDto> GetProductDimensionValues([FromBody]JObject _objData)
        //{
        //    using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
        //    {
        //        dbConn.ConnectionString = GlobalVariables.ConnectionString;
        //        dbConn.Open();

        //        var strQuery = @"SELECT	
        //                UW_PROD_DIM.PROD_ID, 
		      //          UW_PROD_DIM_VL.PROD_DIM_ID,
		      //          OB_PROPERTY.PROPERTY_TYPE_ID,
        //                OB_PROPERTY.PROPERTY_DESC,
        //                OB_PROPERTY_TYPE.PROPERTY_TYPE_DESC,
        //                OB_PROPERTY.PROPERTY_TABLE_ID,
        //                OB_PROPERTY_TABLE.PROPERTY_TABLE_DESC,
		      //          UW_PROD_DIM_VL.VALUE_A, 
		      //          UW_PROD_DIM_VL.VALUE_B
        //        FROM UW_PROD_DIM_VL
        //            LEFT JOIN UW_PROD_DIM ON UW_PROD_DIM_VL.PROD_DIM_ID = UW_PROD_DIM.ID
        //            LEFT JOIN OB_OBJECT_PROPERTY ON UW_PROD_DIM.OBJECT_PROPERTY_ID = OB_OBJECT_PROPERTY.ID
        //            LEFT JOIN OB_PROPERTY ON OB_OBJECT_PROPERTY.PROPERTY_ID = OB_PROPERTY.ID
        //            LEFT JOIN OB_PROPERTY_TYPE ON OB_PROPERTY.PROPERTY_TYPE_ID = OB_PROPERTY_TYPE.ID
        //            LEFT JOIN OB_PROPERTY_TABLE ON OB_PROPERTY.PROPERTY_TABLE_ID = OB_PROPERTY_TABLE.ID
        //        WHERE UW_PROD_DIM.PROD_ID IN(SELECT PROD_ID FROM APP_INS_PROD)";

        //        //var subQuery = smaHelper.SmaPropertyValueSubQuery("UW_PROD_DIM_VL.ID", "VALUE_A", "UW_PROD_DIM.OBJECT_PROPERTY_ID");
        //        //var strQuery = @"SELECT 
        //        //             UW_PROD_DIM_VL.ID,
        //        //             UW_PROD_DIM_VL.PROD_DIM_ID,
        //        //             UW_PROD_DIM_VL.SORTING,
        //        //             UW_PROD_DIM_VL.VALUE_A,
        //        //             UW_PROD_DIM_VL.VALUE_B, " + subQuery + " AS DISPLAYED_VALUE " +
        //        //        @" FROM UW_PROD_DIM_VL
        //        //            LEFT JOIN UW_PROD_DIM ON UW_PROD_DIM_VL.PROD_DIM_ID = UW_PROD_DIM.ID
        //        //            LEFT JOIN UW_COINS ON CAST(UW_PROD_DIM_VL.VALUE_A as varchar(100)) = CAST(UW_COINS.ID as varchar(100)) AND UW_PROD_DIM.MEASURE_TYPE_ID = 1191
        //        //            WHERE PROD_ID = " + _objData["ID"];

        //        var result = dbConn.Query<ProdDimVlDto>(strQuery).ToList(); ;

        //        return result;
        //    }
        //}

        [HttpPost("GetSmaProperties")]
        public List<SmaPropertyDto> GetSmaProperties([FromBody]JObject _objData)
        {
            var lst = new List<SmaPropertyDto>();

       //     --OB_PROPERTY.PROPERTY_TYPE_ID, 
       //                     --OB_PROPERTY.CONTACT_FIELD, 
       //                     --OB_PROPERTY.CONTACT_FIELD_PROPERTY_TYPE_ID, 
       //                  	--OB_PROPERTY_TABLE.REF_TABLE_ID1,
							//--OB_PROPERTY_TABLE.REF_TABLE_ID2
            var szSql = @"SELECT
                            OB_OBJECT_PROPERTY.ID, 
                            OB_OBJECT_PROPERTY.PROPERTY_DESC, 
                            OB_OBJECT_PROPERTY.FORMULA, 
                            OB_PROPERTY_TYPE.PROPERTY_TYPE_DESC AS PROPERTY_TYPE,
                            OB_PROPERTY.PROPERTY_TABLE_ID, 
                            OB_PROPERTY.SYSTEM_TABLE, 
                            OB_PROPERTY.PROPERTY_CODE,
							OB_PROPERTY_TABLE.PARENT_TABLE_ID,
                            CASE WHEN (SELECT OBJECT_PROPERTY_ID FROM UW_PROD_DIM WHERE UW_PROD_DIM.OBJECT_PROPERTY_ID = OB_OBJECT_PROPERTY.ID AND UW_PROD_DIM.PROD_ID = UW_PROD.ID) IS NULL THEN 0 ELSE 1 END AS IS_MANDATORY
						  
                        FROM UW_PROD
	                    LEFT JOIN OB_OBJECT_PROPERTY ON UW_PROD.OBJECT_ID = OB_OBJECT_PROPERTY.OBJECT_ID
                        LEFT JOIN OB_PROPERTY ON OB_OBJECT_PROPERTY.PROPERTY_ID = OB_PROPERTY.ID  
                        LEFT JOIN OB_PROPERTY_TYPE ON OB_PROPERTY.PROPERTY_TYPE_ID = OB_PROPERTY_TYPE.ID  
                        LEFT JOIN OB_PROPERTY_TABLE ON OB_PROPERTY.PROPERTY_TABLE_ID = OB_PROPERTY_TABLE.ID 
                        WHERE IS_USED_IN_WEB = 1 " +
                      " AND UW_PROD.ID = " + _objData["ProductID"];

            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = GlobalVariables.ConnectionString;
                dbConn.Open();

                lst.AddRange(dbConn.Query<SmaPropertyDto>(szSql));
            }

            return lst;
        }

        [HttpPost("GetSmaTableValues")]
        public List<SmaValueDto> GetSmaTableValues([FromBody]JObject _objData)
        {
            var lst = new List<SmaValueDto>();

            var szSql = @"SELECT 
                            OB_PROPERTY_TABLE_VALUE.ID,
                            OB_PROPERTY_TABLE_VALUE.PROPERTY_VALUE_CODE AS VALUE_CODE,
                    	    OB_PROPERTY_TABLE_VALUE.THE_VALUE AS VALUE_DESC,
                    	    OB_PROPERTY_TABLE_VALUE.PARENT_VALUE_ID,
                            OB_PROPERTY_TABLE_VALUE.PROPERTY_TABLE_ID,
                            OB_PROPERTY_TABLE_VALUE.REF_VALUE_ID1,
                            OB_PROPERTY_TABLE_VALUE.REF_VALUE_ID2
                            FROM OB_PROPERTY_TABLE_VALUE    
                            WHERE PROPERTY_TABLE_ID = " + _objData["TableID"].ToString();
            if (Convert.ToInt32(_objData["ParentValueID"].ToString()) > 0)
            {
                szSql += " AND PARENT_VALUE_ID = " + Convert.ToInt32(_objData["ParentValueID"].ToString());
            }
            szSql += " ORDER BY THE_VALUE";

            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = GlobalVariables.ConnectionString;
                dbConn.Open();

                lst.AddRange(dbConn.Query<SmaValueDto>(szSql));

            }

            return lst;
        }

        [HttpPost("GetSystemTableValues")]
        public List<SmaValueDto> GetSystemTableValues([FromBody]JObject _objData)
        {
            var lst = new List<SmaValueDto>();
            string tableName = _objData["TableName"].ToString();

            var szSql = "";

            switch (tableName.ToUpper())
            {
                case "GENDER":
                    szSql = @"SELECT
                                ID,
                                TYPE_CODE AS VALUE_CODE,
                                TYPE_DESC AS VALUE_DESC
                        FROM CORE_TYPE WHERE TYPE_TABLE ='Gender' ";
                    break;
                case "MARITAL_STATUS":
                    szSql = @"SELECT
                                ID,
                                TYPE_CODE AS VALUE_CODE,
                                TYPE_DESC AS VALUE_DESC
                        FROM CORE_TYPE WHERE TYPE_TABLE ='MaritalStatus' ";
                    break;
                case "PF_JOB_TITLE":
                    szSql = @"SELECT
                                ID,
                                TITLE_CODE AS VALUE_CODE,
                                TITLE_DESC AS VALUE_DESC
                        FROM PF_TITLE ";
                    break;
                case "PF_PROFESSION":
                    szSql = @"SELECT
                                ID,
                                PROFESSION_CODE AS VALUE_CODE,
                                PROFESSION_DESC AS VALUE_DESC
                        FROM PF_PROFESSION ";
                    break;
                case "PF_CATEGORY":
                    szSql = @"SELECT
                                ID,
                                CATEGORY_CODE AS VALUE_CODE,
                                CATEGORY_DESC AS VALUE_DESC
                        FROM PF_CATEGORY ";
                    break;
                case "PF_RELATION":
                    szSql = @"SELECT
                                ID,
                                RELATION_CODE AS VALUE_CODE,
                                RELATION_DESC AS VALUE_DESC
                        FROM PF_RELATION ";
                    break;
                case "PROFESSION_CLASS":
                    szSql = @"SELECT
                                ID,
                                PROFESSION_CLASS AS VALUE_CODE,
                                PROFESSION_CLASS AS VALUE_DESC
                        FROM PF_PROFESSION ";
                    break;
                case "CORE_COUNTRY":
                    szSql = @"SELECT
                                ID,
                                COUNTRY_CODE AS VALUE_CODE,
                                COUNTRY_DESC AS VALUE_DESC
                        FROM CORE_COUNTRY ";
                    break;
                case "CORE_CURRENCY":
                    szSql = @"SELECT
                                ID,
                                CUR_CODE AS VALUE_CODE,
                                CUR_DESC AS VALUE_DESC
                        FROM CORE_CURRENCY ";
                    break;
                case "CORE_DEPARTMENT":
                    szSql = @"SELECT
                                ID,
                                DEP_CODE AS VALUE_CODE,
                                DEP_DESC AS VALUE_DESC
                        FROM CORE_DEPARTMENT";
                    break;
                case "CORE_GEO_AREA":
                    szSql = @"SELECT
                                ID,
                                AREA_CODE AS VALUE_CODE,
                                AREA_DESC AS VALUE_DESC,
                                ZONE_ID AS PARENT_VALUE_ID
                        FROM CORE_GEO_AREA";
                    break;
                case "CORE_GEO_CITY":
                    szSql = @"SELECT
                                ID,
                                CITY_CODE AS VALUE_CODE,
                                CITY_DESC AS VALUE_DESC,
                                AREA_ID AS PARENT_VALUE_ID
                        FROM CORE_GEO_CITY ";
                    break;
                case "CORE_GEO_STREET":
                    szSql = @"SELECT
                                ID,
                                STREET_CODE AS VALUE_CODE,
                                STREET_DESC AS VALUE_DESC,
                                CITY_ID AS PARENT_VALUE_ID
                        FROM CORE_GEO_STREET ";
                    break;
                case "CORE_GEO_ZONE":
                    szSql = @"SELECT
                                ID,
                                ZONE_CODE AS VALUE_CODE,
                                ZONE_DESC AS VALUE_DESC,
                                COUNTRY_ID AS PARENT_VALUE_ID
                        FROM CORE_GEO_ZONE ";
                    break;
                case "CORE_NATIONALITY":
                    szSql = @"SELECT
                                ID,
                                '' AS VALUE_CODE,
                                NATIONALITY_DESC AS VALUE_DESC
                        FROM CORE_COUNTRY ";
                    break;
            }

            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = GlobalVariables.ConnectionString;
                dbConn.Open();

                lst.AddRange(dbConn.Query<SmaValueDto>(szSql));

                return lst;
            }
        }


        [HttpPost("CreatePolicy")]
        public Task<Stream> CreatePolicy([FromBody]JObject _objData)
        { 
            var _productId = Convert.ToInt32(_objData["ProductID"].ToString());
            var clientInfo = _objData["ClientInfo"].ToObject<ClientInfo>();
            var smaInfo = ((JArray)_objData["SmaInfo"]).ToObject<List<SmaInfo>>();
            //var _TrsRefNo = _objData["TrsRefNo"].ToString();
            //var _effectiveDate = Convert.ToDateTime(_objData["EffectiveDate"].ToString());

            //var _DOB = Convert.ToDateTime("1965/07/12");// _objData["DOB"].ToString());
            //var _FirstName = "First Name";// _objData["FirstName"].ToString();
            //var _MiddleName = "First Name";// _objData["MiddleName"].ToString();
            //var _LastName = "First Name";//_objData["LastName"].ToString();
            ////var _CountryCode = "961";// Convert.ToInt32(_objData["CountryCode"].ToString());
            //var _MobileCountry = "961";//_objData["MobileCountry"].ToString();
            //var _MobileArea = "3";//_objData["MobileArea"].ToString();
            //var _MobileNo = "302537";//_objData["MobileNo"].ToString();
            //var _Email = "antoine@pixel.com.lb";//_objData["_Email"].ToString();


            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = GlobalVariables.ConnectionString;
                dbConn.Open();

                var productInfo = dbConn.QueryFirstOrDefault<UW_PROD>("SELECT * FROM UW_PROD WHERE ID = " + _productId);
                
                Structures.CobInfo cobInfo = new Structures.CobInfo();
                cobInfo = UWHelper.fnGetCobInfo(productInfo.cob_id, false, -1, true);

                var oCurrencyRates = Helper.GetCurrencyRates(appOptions.ConnectionString, DateTime.Today, productInfo.default_cur_id);
                List<UW_COMPUTATION_ITEM> lstComputationsRules = new List<UW_COMPUTATION_ITEM>();


                DateTime? forcedIssueDate = null;

                if (appOptions.IssuigDateBasis == (int)Enums.UwIssuingDateBasis.ByBranch)
                {
                    forcedIssueDate = Helper.GetForcedIssueDateByBranch(GlobalVariables.ConnectionString,
                                                                            appOptions.CompanyID,
                                                                            appOptions.BranchID);
                }
                else
                {
                    forcedIssueDate = Helper.GetForcedIssueDateByLob(GlobalVariables.ConnectionString,
                                                                            appOptions.CompanyID,
                                                                            productInfo.lob_id);
                }                              

                // Create the insured
                var iInsuredID = 0;
                var iAddressID = 0;

                MobileAppHelper mobileAppHelper = new MobileAppHelper();
                mobileAppHelper.AddNewInsured(
                                clientInfo.name, "","",
                                //_FirstName, _MiddleName, _LastName,
                                null, //_DOB,
                                //_CountryCode, 
                                clientInfo.countryCode,// _MobileCountry,
                                null,//_MobileArea,
                                clientInfo.phone, //_MobileNo,
                                clientInfo.email, //_Email,
                                ref iInsuredID
                                //ref iAddressID 
                                );

                PolicyDto policyDto = new PolicyDto();

                #region Creating the policy root

                policyDto.root.company_id = appOptions.CompanyID;
                policyDto.root.branch_id = appOptions.BranchID;
                policyDto.root.lob_id = productInfo.lob_id;
                policyDto.root.object_id = productInfo.object_id;
                policyDto.root.is_group = 0;
                policyDto.root.ri_managed_by = (int)Enums.RiManagement.Policy;

                #endregion Creating the policy root

                #region Creating the policy 

                //policyDto.policy.PolicyID = IdGenerator.GetNextTempId();
                policyDto.policy.root_id = policyDto.root.id;
                policyDto.policy.contract_type_id = (int)Enums.ContractType.Policy;
                if (forcedIssueDate != null)
                {
                    policyDto.policy.date_issue = Convert.ToDateTime(forcedIssueDate);
                }
                else
                {
                    policyDto.policy.date_issue = Helper.GetServerSystemDate(appOptions.ConnectionString);
                }
                policyDto.policy.date_effective = policyDto.policy.date_issue;// _effectiveDate;
                policyDto.policy.date_expiry = policyDto.policy.date_effective.AddYears(1);//.AddDays(m_TravelPeriod);
                policyDto.policy.uw_year = policyDto.policy.date_effective.Year;
                policyDto.policy.computation_id = productInfo.computation_id;
                policyDto.policy.prod_id = productInfo.id;
                policyDto.policy.object_id = productInfo.object_id;
                policyDto.policy.cob_id = productInfo.cob_id;
                policyDto.policy.is_cover_rate_on_net = productInfo.is_net;
                policyDto.policy.sum_insured_cur_id = productInfo.default_cur_id;
                policyDto.policy.premium_cur_id = productInfo.default_cur_id;
                policyDto.policy.base_cur_rate = oCurrencyRates.base_cur_rate;
                policyDto.policy.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                policyDto.policy.created_on = DateTime.Now;
                policyDto.policy.created_by = appOptions.UserID;
                policyDto.policy.language_id = 1;
                policyDto.policy.print_copies = 2;
                policyDto.policy.insured_id = iInsuredID;
                policyDto.policy.broker_id = appOptions.BrokerID;
                policyDto.policy.client_id = iInsuredID;
                //policyDto.policy.SubBrokerID = m_SubBrokerID;
                policyDto.policy.account_id = appOptions.BrokerAccountID;
                policyDto.policy.payer_type = (int)Enums.PayerType.Broker;
                policyDto.policy.payer_id = appOptions.BrokerID;
                policyDto.policy.print_name = clientInfo.name;// _FirstName + " " + _MiddleName + " " + _LastName;
                policyDto.policy.language_id = 0;
                policyDto.policy.source_channel = 163; // mobile app

                if (iAddressID > 0)
                {
                    policyDto.policy.residency_address_id = iAddressID;
                    policyDto.policy.billing_address_id = iAddressID;
                }

                //var ePolProfile = new UW_POLICY_PROFILE();
                //ePolProfile.policy_id = policyDto.policy.id;
                //ePolProfile.profile_type_id = 14;
                //ePolProfile.profile_id = m_AccExecutiveID;
                //policyDto.policy_profile.Add(ePolProfile);

                ProdDto prodDto = new ProdDto();

                ProductHelper.FillProductDto(ref prodDto, _productId, policyDto.policy.date_effective.ToString("yyyy/MM/dd"), 1);
                                               
                foreach (var prdCvr in prodDto.prod_cvr.Where(w => w.is_main_cover == 1 || w.is_mandatory == 1))
                {
                    var ePolCover = new UW_POLICY_COVER();
                    ePolCover.flag = "A";
                    ePolCover.policy_id = policyDto.policy.id;
                    ePolCover.cover_id = prdCvr.cover_id;
                    ePolCover.effective_date = policyDto.policy.date_effective;
                    ePolCover.expiry_date = policyDto.policy.date_expiry;
                    ePolCover.is_added_to_sum_insured = prdCvr.is_added_to_sum_insured;
                    //for (int k = 0; k < oDtSublineCovers.Rows.Count; k++)
                    //{
                    //    if (Convert.ToInt32(oDtSublineCovers.Rows[k]["CoverID"]) == ePolCover.cover_id)
                    //    {
                    //        ePolCover.is_added_to_sum_insured = Convert.ToBoolean(oDtSublineCovers.Rows[k]["isAddedToSumInsured"]);
                    //        //if (ePolCover.isAddedToSumInsured)
                    //        //{
                    //        //    nSumInsured += ePolCover.SumInsured;
                    //        //}

                    //        break;
                    //    }
                    //}

                    policyDto.policy_cover.Add(ePolCover);
                }

                foreach (var item in smaInfo)
                {
                    var ePolInterest = new UW_POLICY_PROPERTY();
                    ePolInterest.policy_id = policyDto.policy.id;
                    ePolInterest.object_property_id = item.id;
                    ePolInterest.the_value = item.stringifiedRes;                    
                    policyDto.policy_property.Add(ePolInterest);
                }

                //for (int i = 0; i < oDtObjProperties.Count; i++)
                //{
                //    var ePolInterest = new UW_POLICY_PROPERTY();
                //    ePolInterest.policy_id = policyDto.policy.id;
                //    ePolInterest.object_property_id = Convert.ToInt32(oDtObjProperties.Rows[i]["ObjectPropertyID"]);
                //    if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 17)
                //    {
                //        ePolInterest.the_value = m_Age.ToString();
                //    }
                //    else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 700)//553)
                //    {
                //        ePolInterest.the_value = m_DOB.ToString();
                //    }
                //    else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 787)
                //    {
                //        ePolInterest.the_value = m_TravelPeriod.ToString();
                //    }
                //    else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 26)
                //    {
                //        ePolInterest.the_value = m_PassportNo.ToString();
                //    }
                //    else
                //    {
                //        ePolInterest.the_value = "";
                //    }
                //    policyDto.policy_property.Add(ePolInterest);
                //}

                #endregion Creating the policy

                try
                {
                    #region Finalizing the policy 


                    var m_ErrorApplyProduct = ProductHelper.ApplyProduct(ref policyDto,
                                                            prodDto,
                                                            true,
                                                            true,
                                                            cobInfo.rule_policies_end_at_noon,
                                                            appOptions.BaseCurID,
                                                            appOptions.CounterCurID);

                    decimal nNet = 0m;
                    decimal nTotal = 0m;

                    if (productInfo.is_net == 1 && productInfo.is_fixed_total == 1)
                    {
                        foreach (var item in policyDto.policy_cover)
                        {
                            nNet += item.net_premium;
                            nTotal += item.total_premium;
                        }
                    }
                    else
                    {
                        foreach (var item in policyDto.policy_cover)
                        {
                            if (policyDto.policy.is_cover_rate_on_net == 1)
                            {
                                nNet += item.net_premium;
                            }
                            else
                            {
                                nTotal += item.total_premium;
                            }
                        }
                    }

                    decimal nSumInsured = 0m;
                    foreach (var item in policyDto.policy_cover)
                    {
                        if (item.is_added_to_sum_insured == 1)
                        {
                            nSumInsured += item.sum_insured;
                        }
                    }

                    ComputationHelper computationHelper = new ComputationHelper();
                    List<UW_COMPUTATION_ITEM> lstComputationItems = new List<UW_COMPUTATION_ITEM>();
                    lstComputationItems = computationHelper.GetComputationData(policyDto.policy.computation_id);

                    computationHelper.p_base_cur_id = appOptions.BaseCurID;
                    computationHelper.p_counter_cur_id = appOptions.CounterCurID;
                    computationHelper.p_stamp_refund_period = 1;//TODO
                    computationHelper.p_tax_refund_period = 1;//TODO

                    computationHelper.AddComputationComponents(ref policyDto,
                                                                policyDto.policy.computation_id,
                                                                lstComputationItems,
                                                                ((productInfo.is_net == 1) ? nNet: nTotal),
                                                                Convert.ToBoolean(productInfo.is_net),
                                                                false,
                                                                0M,
                                                                Convert.ToBoolean(productInfo.is_detailed_computation),
                                                                DateTime.MinValue, //(_certificateDto.policy.end_type != "" ? _certificateDto.policy.date_issue : DateTime.MinValue),
                                                                0, //_cobInfo.fraction_subtracted_from_total,
                                                                0);// _cobInfo.fraction_added_to_total);



                    policyDto.policy.sum_insured = nSumInsured;

                    CommissionHelper commissionHelper = new CommissionHelper();
                    //TODO: calculateAccountExecutiveCommission
                    var calculateAccountExecutiveCommission = 0;
                    commissionHelper.CalculateCommissions(ref policyDto, calculateAccountExecutiveCommission, productInfo.is_commission_by_cover);

                    var saveResult = PolicyHelper.SavePolicy(policyDto, appOptions.UserID);
                    if (saveResult.Contains("Error"))
                    {
                        return null;
                    }
                    else
                    {
                        var result = GetPolicyAsPdf(Convert.ToInt32(saveResult));
                        return result;
                    }
                     
                    //var szErrors = "";
                    //var drCur = DatabaseCommon.GetDataReader(m_ConnectionString,
                    //                                         "SELECT CUR_CODE FROM CORE_CURRENCY WHERE CurID = " +
                    //                                         newPol.premium_cur_id);
                    //if (drCur != null && drCur.HasRows)
                    //{
                    //    drCur.Read();

                    //    decimal NewReceiptID = mobileAppHelper.InsertNewReceipt(szNewPOLICY_NO,
                    //                                                            nTotal,
                    //                                                            drCur["CUR_CODE"].ToString(),
                    //                                                            ref szErrors,
                    //                                                            policyDto.policy.print_name,
                    //                                                            false);

                    //    if (NewReceiptID != -1)
                    //    {   
                    //        #region preparing the policy layout

                    //        typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.UW.PolicyTravelApp, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    //        typeReportSource.Parameters.Add("pPolicyID", newPol.id);
                    //        typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

                    //        typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
                    //        typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));


                    //        Telerik.Reporting.Processing.RenderingResult resultPOL = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

                    //        string fileNamePOL = resultPOL.DocumentName + "_" + newPol.id + "." + resultPOL.Extension;
                    //        string path = m_PdfFilesPath;
                    //        string filePathPOL = System.IO.Path.Combine(path, fileNamePOL);

                    //        try
                    //        {
                    //            using (System.IO.FileStream fs = new System.IO.FileStream(filePathPOL, System.IO.FileMode.Create))
                    //            {
                    //                fs.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //        }

                    //        #endregion preparing the policy layout

                    //        ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
                    //        ms.Position = 0;

                    //        return ms;
                    //    }
                    //}
                    //else
                    //{
                    //    return ms;
                    //}

                    #endregion preparing the receipt ;ayout
                }
                catch (Exception ex)
                {
                    //MemoryStream ms = new MemoryStream();
                    //return ms;
                    return null;
                }
            }
        }

        async Task<Stream> GetPolicyAsPdf(int _policyId)
        {
            HttpClient httpClient = new HttpClient();

            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ID", _policyId.ToString())
            });

            HttpResponseMessage response = await httpClient.PostAsync(appOptions.layoutUrl + "Policy/GetPdf", stringContent);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStreamAsync();

            return responseBody;
        }

        #region Travel

        //[HttpPost("TravelGetPrice")]
        //public decimal TravelGetPrice([FromBody]JObject _objData)
        //{
        //    m_Age = DateTime.Today.Year - Convert.ToDateTime(_objData["DOB"].ToString()).Year;
        //    m_DOB = Convert.ToDateTime(_objData["DOB"].ToString());
        //    var _productCode = Convert.ToInt32(_objData["ProductCode"].ToString());
        //    m_TravelPeriod = Convert.ToInt32(_objData["TravelPeriod"].ToString());

        //    using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
        //    {
        //        dbConn.ConnectionString = GlobalVariables.ConnectionString;
        //        dbConn.Open();

        //        m_ProductInfo = dbConn.QueryFirstOrDefault<UW_PROD>("SELECT * FROM UW_PROD WHERE PROD_CODE = '" + _productCode + "'", m_CompanyID);

        //        MobileAppHelper mobileAppHelper = new MobileAppHelper();
        //        decimal nThePrice = mobileAppHelper.GetProductPrice();

        //        return nThePrice;
        //    }
        //}

        //[HttpPost("TravelCreatePolicy")]
        //public Stream TravelCreatePolicy([FromBody]JObject _objData)
        //{
        //    var _productCode = _objData["ProductCode"].ToString();
        //    var _DOB = Convert.ToDateTime(_objData["DOB"].ToString());
        //    var _TravelPeriod = Convert.ToInt32(_objData["TravelPeriod"].ToString());
        //    var _FirstName = _objData["FirstName"].ToString();
        //    var _MiddleName = _objData["MiddleName"].ToString();
        //    var _LastName = _objData["LastName"].ToString();
        //    var _CountryCode = Convert.ToInt32(_objData["CountryCode"].ToString());
        //    var _MobileCountry = _objData["MobileCountry"].ToString();
        //    var _MobileArea = _objData["MobileArea"].ToString();
        //    var _MobileNo = _objData["MobileNo"].ToString();
        //    var _Email = _objData["_Email"].ToString();
        //    var _PassportNo = _objData["PassportNo"].ToString();
        //    var _effectiveDate = Convert.ToDateTime(_objData["EffectiveDate"].ToString());
        //    var _TrsRefNo = _objData["TrsRefNo"].ToString();

        //    var szNewPOLICY_NO = "";
        //    m_Age = DateTime.Today.Year - _DOB.Year;
        //    m_DOB = _DOB;
        //    m_TravelPeriod = _TravelPeriod;
        //    m_PassportNo = _PassportNo;

        //    using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
        //    {
        //        dbConn.ConnectionString = GlobalVariables.ConnectionString;
        //        dbConn.Open();

        //        m_ProductInfo = dbConn.QueryFirstOrDefault<UW_PROD>("SELECT * FROM UW_PROD WHERE PROD_CODE = '" + _productCode + "'", m_CompanyID);

        //        var oSubLineInfo = m_ServiceUW.fnGetSubLineInfo(m_ConnectionString, m_ProductInfo.cob_id, false, Convert.ToInt16(DateTime.Today.Year), false).FirstOrDefault();

        //        var oCurrencyRates = Helper.GetCurrencyRates(m_ConnectionString, DateTime.Today, m_ProductInfo.default_cur_id);
        //        List<UW_COMPUTATION_ITEM> lstComputationsRules = new List<UW_COMPUTATION_ITEM>();


        //        this.m_ForcedIssueDate = null;

        //        if (m_IssuigDateBasis == (int)Enums.UwIssuingDateBasis.ByBranch)
        //        {
        //            this.m_ForcedIssueDate = Helper.GetForcedIssueDateByBranch(GlobalVariables.ConnectionString,
        //                                                                    m_CompanyID,
        //                                                                    m_BranchID);
        //        }
        //        else
        //        {
        //            this.m_ForcedIssueDate = Helper.GetForcedIssueDateByLob(GlobalVariables.ConnectionString,
        //                                                                    m_CompanyID,
        //                                                                    m_ProductInfo.lob_id);
        //        }

        //        //if (this.m_ForcedIssueDate != null)
        //        //{
        //        //    dIssue = Convert.ToDateTime(this.m_ForcedIssueDate);
        //        //}
        //        //else
        //        //{
        //        //    dIssue = Helper.GetServerSystemDate(this.m_ForcedIssueDate);
        //        //}


        //        // Create the insured
        //        var iInsuredID = 0;
        //        var iAddressID = 0;

        //        MobileAppHelper mobileAppHelper = new MobileAppHelper();
        //        mobileAppHelper.AddNewInsured(
        //                        _FirstName, _MiddleName, _LastName,
        //                        _DOB,
        //                        //_CountryCode, 
        //                        _MobileCountry,
        //                        _MobileArea,
        //                        _MobileNo,
        //                        _Email,
        //                        ref iInsuredID,
        //                        //ref iAddressID,
        //                        m_CompanyID,
        //                        m_BrokerID,
        //                        m_UserID);

        //        PolicyDto policyDto = new PolicyDto();

        //        #region Creating the policy root

        //        policyDto.root.company_id = m_CompanyID;
        //        policyDto.root.branch_id = m_BranchID;
        //        policyDto.root.lob_id = m_ProductInfo.lob_id;
        //        policyDto.root.object_id = m_ProductInfo.object_id;
        //        policyDto.root.is_group = 0;
        //        policyDto.root.ri_managed_by = (int)Enums.RiManagement.Policy;

        //        #endregion Creating the policy root

        //        #region Creating the policy 

        //        //policyDto.policy.PolicyID = IdGenerator.GetNextTempId();
        //        policyDto.policy.root_id = policyDto.root.id;
        //        policyDto.policy.contract_type_id = (int)Enums.ContractType.Policy;
        //        if (m_ForcedIssueDate != null)
        //        {
        //            policyDto.policy.date_issue = Convert.ToDateTime(m_ForcedIssueDate);
        //        }
        //        else
        //        {
        //            policyDto.policy.date_issue = DateTime.Today;
        //        }
        //        policyDto.policy.date_effective = _effectiveDate;
        //        policyDto.policy.date_expiry = policyDto.policy.date_effective.AddDays(m_TravelPeriod);
        //        policyDto.policy.uw_year = policyDto.policy.date_effective.Year;
        //        policyDto.policy.computation_id = m_ProductInfo.computation_id;
        //        policyDto.policy.prod_id = m_ProductInfo.id;
        //        policyDto.policy.object_id = m_ProductInfo.object_id;
        //        policyDto.policy.cob_id = m_ProductInfo.cob_id;
        //        policyDto.policy.is_cover_rate_on_net = m_ProductInfo.is_net;
        //        policyDto.policy.sum_insured_cur_id = m_ProductInfo.default_cur_id;
        //        policyDto.policy.premium_cur_id = m_ProductInfo.default_cur_id;
        //        policyDto.policy.base_cur_rate = oCurrencyRates.base_cur_rate;
        //        policyDto.policy.counter_cur_rate = oCurrencyRates.counter_cur_rate;
        //        policyDto.policy.created_on = DateTime.Now;
        //        policyDto.policy.created_by = m_UserID;
        //        policyDto.policy.print_copies = 2;
        //        policyDto.policy.insured_id = iInsuredID;
        //        policyDto.policy.broker_id = m_BrokerID;
        //        policyDto.policy.client_id = iInsuredID;
        //        //policyDto.policy.SubBrokerID = m_SubBrokerID;
        //        policyDto.policy.account_id = m_BrokerAccountID;
        //        policyDto.policy.print_name = _FirstName + " " + _MiddleName + " " + _LastName;
        //        policyDto.policy.language_id = 0;
        //        if (iAddressID > 0)
        //        {
        //            policyDto.policy.residency_address_id = iAddressID;
        //            policyDto.policy.billing_address_id = iAddressID;
        //        }


        //        var ePolProfile = new UW_POLICY_PROFILE();
        //        ePolProfile.policy_id = policyDto.policy.id;
        //        ePolProfile.profile_type_id = 14;
        //        ePolProfile.profile_id = m_AccExecutiveID;
        //        policyDto.policy_profile.Add(ePolProfile);

        //        this.PrepareProductTablesToCreatePolicy(m_ProductInfo,
        //                                                false,
        //                                                -1,
        //                                                policyDto.policy.date_effective.ToString("yyyy/MM/dd"),
        //                                                true,
        //                                                0);
        //        //  decimal nSumInsured = 0;
        //        for (int i = 0; i < oDtCovers.Rows.Count; i++)
        //        {
        //            var ePolCover = new UW_POLICY_COVER();
        //            ePolCover.flag = "A";
        //            ePolCover.policy_id = policyDto.policy.id;
        //            ePolCover.cover_id = Convert.ToInt32(oDtCovers.Rows[i]["CoverID"]);
        //            ePolCover.effective_date = policyDto.policy.date_effective;
        //            ePolCover.expiry_date = policyDto.policy.date_expiry;

        //            for (int k = 0; k < oDtSublineCovers.Rows.Count; k++)
        //            {
        //                if (Convert.ToInt32(oDtSublineCovers.Rows[k]["CoverID"]) == ePolCover.cover_id)
        //                {
        //                    ePolCover.is_added_to_sum_insured = Convert.ToBoolean(oDtSublineCovers.Rows[k]["isAddedToSumInsured"]);
        //                    //if (ePolCover.isAddedToSumInsured)
        //                    //{
        //                    //    nSumInsured += ePolCover.SumInsured;
        //                    //}

        //                    break;
        //                }
        //            }

        //            policyDto.policy_cover.Add(ePolCover);
        //        }

        //        // policyDto.policy.SumInsured = nSumInsured;

        //        for (int i = 0; i < oDtObjProperties.Count; i++)
        //        {
        //            var ePolInterest = new UW_POLICY_PROPERTY();
        //            ePolInterest.policy_id = policyDto.policy.id;
        //            ePolInterest.object_property_id = Convert.ToInt32(oDtObjProperties.Rows[i]["ObjectPropertyID"]);
        //            if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 17)
        //            {
        //                ePolInterest.the_value = m_Age.ToString();
        //            }
        //            else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 700)//553)
        //            {
        //                ePolInterest.the_value = m_DOB.ToString();
        //            }
        //            else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 787)
        //            {
        //                ePolInterest.the_value = m_TravelPeriod.ToString();
        //            }
        //            else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 26)
        //            {
        //                ePolInterest.the_value = m_PassportNo.ToString();
        //            }
        //            else
        //            {
        //                ePolInterest.the_value = "";
        //            }
        //            policyDto.policy_property.Add(ePolInterest);
        //        }

        //        #endregion Creating the policy


        //        try
        //        {
        //            #region Finalizing the policy 

        //            var newPolicyID = PolicyHelper.SavePolicy(policyDto, m_UserID);

        //            MemoryStream ms = new MemoryStream();

        //            var newPol = dbConn.QueryFirstOrDefault<UW_POLICY>("SELECT * FROM UW_POLICY WHERE ID = " + newPolicyID);

        //            if (newPol != null)
        //            {
        //                szNewPOLICY_NO = newPol.policy_no;

        //                var m_ErrorApplyProduct = ProductHelper.ApplyProduct(
        //                                                    policyDto,
        //                                                      m_ProductInfo,
        //                                                      true,
        //                                                      true,
        //                                                      //false,
        //                                                      //false,
        //                                                      //newRec.PolicyRootID,
        //                                                      false,
        //                                                      //false,
        //                                                      oSubLineInfo.RulePoliciesEndAtNoon,
        //                                                      newPol.id);

        //                lstComputationsRules = oServiceUW.fnGetComputationData(m_ConnectionString, policyDto.policy.computation_id);

        //                decimal nNet = 0m;
        //                decimal nTotal = 0m;

        //                var AddedPolicyCovers = oContextWrite.UW_PolicyCovers.Where(w => w.PolicyID == policyDto.policy.id).ToList();

        //                if (m_ProductInfo.is_net == 1 && m_ProductInfo.is_fixed_total == 1)
        //                {
        //                    foreach (var item in AddedPolicyCovers)
        //                    {
        //                        nNet += item.NetPremium;
        //                        nTotal += item.TotalPremium;
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var item in AddedPolicyCovers)
        //                    {
        //                        if (policyDto.policy.is_cover_rate_on_net == 1)
        //                        {
        //                            nNet += item.NetPremium;
        //                        }
        //                        else
        //                        {
        //                            nTotal += item.TotalPremium;
        //                        }
        //                    }
        //                }

        //                decimal nSumInsured = 0m;
        //                foreach (var item in AddedPolicyCovers)
        //                {
        //                    if (item.isAddedToSumInsured)
        //                    {
        //                        nSumInsured += item.SumInsured;
        //                    }
        //                }

        //                this.CreatePolicyComputation(ref oContextWrite,
        //                                                policyDto.policy.id,
        //                                                policyDto.policy.computation_id,
        //                                                nNet,
        //                                                nTotal,
        //                                                m_ProductInfo.is_net,
        //                                                false,
        //                                                0,
        //                                                m_ProductInfo.is_detailed_computation,
        //                                                false,
        //                                                policyDto.policy.date_effective,
        //                                                false,
        //                                                oSubLineInfo);

        //                policyDto.policy.sum_insured = nSumInsured;
        //                oContextWrite.SaveChanges();

        //                this.Commissions_PrepareCommissions(ref oContextWrite,
        //                                                    policyDto.policy.id,
        //                                                    false);

        //                oContextWrite.SaveChanges();

        //                #endregion Finalizing the policy

        //                var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //                var deviceInfo = new System.Collections.Hashtable();
        //                var typeReportSource = new Telerik.Reporting.TypeReportSource();

        //                #region preparing the receipt Layout

        //                var szErrors = "";
        //                var drCur = DatabaseCommon.GetDataReader(m_ConnectionString,
        //                                                         "SELECT CUR_CODE FROM CORE_CURRENCY WHERE CurID = " +
        //                                                         newPol.premium_cur_id);
        //                if (drCur != null && drCur.HasRows)
        //                {
        //                    drCur.Read();

        //                    decimal NewReceiptID = mobileAppHelper.InsertNewReceipt(szNewPOLICY_NO,
        //                                                                            nTotal,
        //                                                                            drCur["CUR_CODE"].ToString(),
        //                                                                            ref szErrors,
        //                                                                            policyDto.policy.print_name,
        //                                                                            false);

        //                    if (NewReceiptID != -1)
        //                    {
        //                        //typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.Ledger.UW_RV, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //                        //typeReportSource.Parameters.Add("pTrsID", NewReceiptNo);
        //                        //typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));
        //                        //typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //                        //Telerik.Reporting.Processing.RenderingResult resultRV = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //                        //string fileNameRV = resultRV.DocumentName + "_" + NewReceiptNo + "." + resultRV.Extension;
        //                        //string filePathRV = System.IO.Path.Combine(path, fileNameRV);

        //                        //using (System.IO.FileStream fs = new System.IO.FileStream(filePathRV, System.IO.FileMode.Create))
        //                        //{
        //                        //    fs.Write(resultRV.DocumentBytes, 0, resultRV.DocumentBytes.Length);
        //                        //}

        //                        //// this is new to return policy & rv 
        //                        ////ms.Write(resultRV.DocumentBytes, 0, resultRV.DocumentBytes.Length);
        //                        //ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);

        //                        ////msRV.CopyTo(msPOL); 
        //                        ////using (var file1 = File.OpenRead(filePathPOL)) file1.CopyTo(ms);
        //                        ////using (var file2 = File.OpenRead(filePathRV)) file2.CopyTo(ms);

        //                        //ms.Position = 0;
        //                        ////WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
        //                        ////WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-disposition", "inline; filename=" + fileNamePOL);

        //                        //// this is new to return policy & rv 
        //                        /// 

        //                        #region preparing the policy layout

        //                        typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.UW.PolicyTravelApp, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //                        typeReportSource.Parameters.Add("pPolicyID", newPol.id);
        //                        typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //                        typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
        //                        typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));


        //                        Telerik.Reporting.Processing.RenderingResult resultPOL = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //                        string fileNamePOL = resultPOL.DocumentName + "_" + newPol.id + "." + resultPOL.Extension;
        //                        string path = m_PdfFilesPath;
        //                        string filePathPOL = System.IO.Path.Combine(path, fileNamePOL);

        //                        try
        //                        {
        //                            using (System.IO.FileStream fs = new System.IO.FileStream(filePathPOL, System.IO.FileMode.Create))
        //                            {
        //                                fs.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                        }

        //                        #endregion preparing the policy layout

        //                        ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //                        ms.Position = 0;

        //                        return ms;
        //                    }
        //                }
        //                else
        //                {
        //                    return ms;
        //                }

        //                #endregion preparing the receipt ;ayout

        //                // this is working
        //                //MemoryStream ms = new MemoryStream();
        //                //ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //                //ms.Position = 0;
        //                //WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
        //                //WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-disposition", "inline; filename=" + fileNamePOL);
        //                //return ms; 
        //                // this is working
        //            }

        //            return ms;
        //        }
        //        catch (Exception ex)
        //        {
        //            MemoryStream ms = new MemoryStream();
        //            return ms;
        //        }
        //    }

        //}

        #endregion Travel

        #region Relay

        /// <summary>
        /// get relay policy price
        /// </summary>
        /// <param name="ProductCode"></param>
        /// <param name="DOB"></param>
        /// <returns></returns>
        [HttpPost("RelayGetPrice")]
        //public decimal RelayGetPrice([FromBody]JObject _objData)
        //{
        //    //string _ProductCode, DateTime _DOB
        //    var _productCode = _objData["ProductCode"].ToString();
        //    var _dob = Convert.ToDateTime(_objData["DOB"].ToString());

        //    m_Age = DateTime.Today.Year - _dob.Year;
        //    m_ProductInfo = dbConn.QueryFirstOrDefault<UW_PROD>("SELECT * FROM UW_PROD WHERE PROD_CODE = '" + _productCode + "'", m_CompanyID);

        //    MobileAppHelper mobileAppHelper = new MobileAppHelper();
        //    decimal nThePrice = mobileAppHelper.GetProductPrice();

        //    return nThePrice;
        //}

        ///// <summary>
        /// get replay proposal info, it is needed to convert a proposal into a new policy
        /// </summary>
        /// <param name="ProposalID"></param>
        /// <returns></returns>
        //[HttpPost("RelayGetProposalInfo")]
        //public ProposalData RelayGetProposalInfo([FromBody]JObject _objData)
        //{
        //    //Int64 _ProposalID
        //    ProposalData prpInfo = new ProposalData();

        //    var dr = DatabaseCommon.GetDataReader(m_ConnectionString,
        //            " SELECT " +
        //            " FirstName, " +
        //            " MiddleName, " +
        //            " LastName, " +
        //            " ProductCode, " +
        //            " TotalPremium, " +
        //            " CUR_CODE, " +
        //            " DateIssue, " +
        //            " ContractStatusID, " +
        //            " (SELECT TheValue  " +
        //            " FROM UW_PolicyInterestProperties  " +
        //            " LEFT JOIN OB_ObjectProperties On UW_PolicyInterestProperties.ObjectPropertyID = OB_ObjectProperties.ObjectPropertyID  " +
        //            " LEFT JOIN UW_POLICY ChildCert On UW_PolicyInterestProperties.PolicyID = ChildCert.PolicyID" +
        //            " WHERE ChildCert.ParentPolicyID = UW_POLICY.PolicyID " +
        //            " AND OB_ObjectProperties.PropertyID = 553) AS DOB " +
        //            " FROM UW_POLICY " +
        //            " LEFT JOIN vProfiles ON UW_POLICY.InsuredProfileID = vProfiles.ProfileID " +
        //            " LEFT JOIN UW_Products ON UW_POLICY.ProductID = UW_Products.ProductID " +
        //            " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
        //            " WHERE PolicyID = " + _ProposalID);
        //    //" AND OB_ObjectProperties.PropertyID = 17) AS Age " +

        //    if (dr != null && dr.HasRows)
        //    {
        //        dr.Read();

        //        prpInfo.proposal_id = _ProposalID;
        //        prpInfo.product_code = dr["ProductCode"].ToString();
        //        prpInfo.proposal_date = (DateTime)dr["DateIssue"];
        //        prpInfo.dob = Convert.IsDBNull(dr["DOB"]) ? Convert.ToDateTime("2017/01/01") : Convert.ToDateTime(dr["DOB"]);
        //        prpInfo.first_name = dr["FirstName"].ToString();
        //        prpInfo.middle_name = dr["MiddleName"].ToString();
        //        prpInfo.last_name = dr["LastName"].ToString();
        //        prpInfo.balance = (decimal)dr["TotalPremium"];
        //        prpInfo.cur_code = dr["CUR_CODE"].ToString();
        //        prpInfo.status = Convert.IsDBNull(dr["ContractStatusID"]) ? -1 : (int)dr["ContractStatusID"];
        //    }
        //    else
        //    {
        //        prpInfo.proposal_id = -1;
        //        prpInfo.product_code = "";
        //        prpInfo.proposal_date = Convert.ToDateTime("2017/01/01");
        //        prpInfo.dob = Convert.ToDateTime("2017/01/01");
        //        prpInfo.first_name = "NOT FOUND";
        //        prpInfo.middle_name = "";
        //        prpInfo.last_name = "";
        //        prpInfo.balance = 0;
        //        prpInfo.cur_code = "";
        //        prpInfo.status = -1;
        //    }
        //    return prpInfo;
        //}

        //[HttpPost("RelayCreatePolicy")]
        //public Stream RelayCreatePolicy([FromBody]JObject _objData)
        //{
        //    var iPREMIUM_CUR_ID = 0;
        //    var nTotal = 0M;
        //    var szNewPOLICY_NO = "";
        //    var iNewPolicyID = 0;

        //    var _productCode = _objData["ProductCode"].ToString();
        //    var _DOB = Convert.ToDateTime(_objData["DOB"].ToString());
        //    var _FirstName = _objData["FirstName"].ToString();
        //    var _MiddleName = _objData["MiddleName"].ToString();
        //    var _LastName = _objData["LastName"].ToString();
        //    var _MobileCountry = _objData["MobileCountry"].ToString();
        //    var _MobileArea = _objData["MobileArea"].ToString();
        //    var _MobileNo = _objData["MobileNo"].ToString();
        //    var _Email = _objData["_Email"].ToString();
        //    var _TrsRefNo = _objData["TrsRefNo"].ToString();

        //    CreateRelayPolicyOrProposal((int)Enums.ContractType.Policy,
        //                                _productCode,
        //                                _DOB,
        //                                _FirstName, _MiddleName, _LastName,
        //                                ref iNewPolicyID,
        //                                ref szNewPOLICY_NO,
        //                                ref iPREMIUM_CUR_ID,
        //                                ref nTotal, 
        //                                _MobileCountry,
        //                                _MobileArea,
        //                                _MobileNo,
        //                                _Email,
        //                                _TrsRefNo);


        //    #region preparing the receipt & Layout

        //    MemoryStream ms = new MemoryStream();

        //    var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //    var deviceInfo = new System.Collections.Hashtable();
        //    var typeReportSource = new Telerik.Reporting.TypeReportSource();

        //    var szErrors = "";
        //    var drCur = DatabaseCommon.GetDataReader(m_ConnectionString,
        //                                             "SELECT CUR_CODE FROM CORE_CURRENCY WHERE CurID = " + iPREMIUM_CUR_ID);
        //    if (drCur != null && drCur.HasRows)
        //    {
        //        drCur.Read();

        //        var drPolicy = DatabaseCommon.GetDataReader(m_ConnectionString, "SELECT TotalPremium, PRINT_NAME FROM UW_POLICY WHERE PolicyID = " + iNewPolicyID);
        //        if (drPolicy.HasRows)
        //        {
        //            drPolicy.Read();
        //            nTotal = Convert.ToDecimal(drPolicy["TotalPremium"]);
        //        }

        //        decimal NewReceiptID = MobileAppHelper.InsertNewReceipt(szNewPOLICY_NO, nTotal, drCur["CUR_CODE"].ToString(), ref szErrors, _FirstName + " " + _MiddleName + " " + _LastName, false);

        //        if (NewReceiptID != -1)
        //        {
        //            #region preparing the policy layout

        //            typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.UW.PolicyMedicalApp, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //            typeReportSource.Parameters.Add("pPolicyID", iNewPolicyID);
        //            typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //            typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
        //            typeReportSource.Parameters.Add("pWording", Pixel.Core.Lib.NumberToWords.ToWordsEnglish((double)nTotal));


        //            Telerik.Reporting.Processing.RenderingResult resultPOL = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //            string fileNamePOL = resultPOL.DocumentName + "_" + iNewPolicyID + "." + resultPOL.Extension;
        //            string path = m_PdfFilesPath;
        //            string filePathPOL = System.IO.Path.Combine(path, fileNamePOL);

        //            using (System.IO.FileStream fs = new System.IO.FileStream(filePathPOL, System.IO.FileMode.Create))
        //            {
        //                fs.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //            }

        //            #endregion preparing the policy layout

        //            ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //            ms.Position = 0;
        //            return ms;
        //        }
        //        else
        //        {
        //            return ms;
        //        }
        //    }
        //    else
        //    {
        //        return ms;
        //    }

        //    #endregion preparing the receipt& Layout


        //    //typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.UW.PolicyMedicalApp, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //    //typeReportSource.Parameters.Add("pPolicyID", iNewPolicyID);
        //    //typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //    //Telerik.Reporting.Processing.RenderingResult resultPOL = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);


        //    //string fileName = resultPOL.DocumentName + "_" + iNewPolicyID + "." + resultPOL.Extension;
        //    //string path = m_PdfFilesPath;
        //    //string filePath = System.IO.Path.Combine(path, fileName);

        //    //using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
        //    //{
        //    //    fs.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //    //}

        //    //#endregion preparing the policy layout

        //    //#region preparing the receipt layout

        //    //var szErrors = "";
        //    //var drCur = DatabaseCommon.GetDataReader(m_ConnectionString,
        //    //                                            "SELECT CUR_CODE FROM CORE_CURRENCY WHERE CurID = " +
        //    //                                            iPREMIUM_CUR_ID);
        //    //if (drCur != null && drCur.HasRows)
        //    //{
        //    //    drCur.Read();

        //    //    decimal NewReceiptID = this.InsertNewReceipt(szNewPOLICY_NO, nTotal, drCur["CUR_CODE"].ToString(), ref szErrors);

        //    //    if (NewReceiptID != -1)
        //    //    {
        //    //        //typeReportSource.TypeName = "Reports.UW_RV, Pixel.Iris4.AppServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null";
        //    //        typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.Ledger.UW_RV, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //    //        typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
        //    //        typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));
        //    //        typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //    //        Telerik.Reporting.Processing.RenderingResult result2 = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //    //        string fileNameRV = result2.DocumentName + "_" + NewReceiptID.ToString() + "." + result2.Extension;
        //    //        //string path2 = m_PdfFilesPath;
        //    //        string filePathRV = System.IO.Path.Combine(path, fileNameRV);

        //    //        using (System.IO.FileStream fs = new System.IO.FileStream(filePathRV, System.IO.FileMode.Create))
        //    //        {
        //    //            fs.Write(result2.DocumentBytes, 0, result2.DocumentBytes.Length);
        //    //        }
        //    //    }
        //    //}

        //    //#endregion preparing the receipt ;ayout

        //    //MemoryStream ms = new MemoryStream();
        //    //ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //    //ms.Position = 0;
        //    //WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
        //    //WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-disposition", "inline; filename=" + fileName);

        //    //return ms;
        //}

        //[HttpPost("RelayCreateProposal")]
        //public int RelayCreateProposal([FromBody]JObject _objData)
        //{
        //    var _productCode = _objData["ProductCode"].ToString();
        //    var _DOB = Convert.ToDateTime(_objData["DOB"].ToString());
        //    var _FirstName = _objData["FirstName"].ToString();
        //    var _MiddleName = _objData["MiddleName"].ToString();
        //    var _LastName = _objData["LastName"].ToString();
        //    var _MobileCountry = _objData["MobileCountry"].ToString();
        //    var _MobileArea = _objData["MobileArea"].ToString();
        //    var _MobileNo = _objData["MobileNo"].ToString();
        //    var _Email = _objData["_Email"].ToString();
        //    var _TrsRefNo = _objData["TrsRefNo"].ToString();

        //    var iPREMIUM_CUR_ID = 0;
        //    var nTotal = 0M;
        //    var szNewProposal = "";
        //    var iNewProposalID = 0;

        //    CreateRelayPolicyOrProposal((int)Enums.ContractType.Proposal,
        //                                _productCode,
        //                                _DOB,
        //                                _FirstName, _MiddleName, _LastName,
        //                                ref iNewProposalID,
        //                                ref szNewProposal,
        //                                ref iPREMIUM_CUR_ID,
        //                                ref nTotal,
        //                                _MobileCountry,
        //                                _MobileArea,
        //                                _MobileNo,
        //                                _Email, "");

        //    return iNewProposalID;
        //}

        //private void CreateRelayPolicyOrProposal(
        //    int _CONTRACT_TYPE_ID,
        //    string _ProductCode,
        //    DateTime _DOB,
        //    string _FirstName, string _MiddleName, string _LastName,
        //    ref int _NewPolicyID,
        //    ref string _NewPOLICY_NO,
        //    ref int _PREMIUM_CUR_ID,
        //    ref decimal _Total,
        //    //string _CountryCode,
        //    string _MobileCountry,
        //    string _MobileArea,
        //    string _MobileNo,
        //    string _Email,
        //    string _TrsRefNo)
        //{
        //    var szNewProposal = "";

        //    m_Age = DateTime.Today.Year - _DOB.Year;
        //    m_DOB = _DOB;

        //    using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
        //    {
        //        dbConn.ConnectionString = GlobalVariables.ConnectionString;
        //        dbConn.Open();

        //        m_ProductInfo = dbConn.QueryFirstOrDefault<UW_PROD>("SELECT * FROM UW_PROD WHERE PROD_CODE = '" + _ProductCode + "'", m_CompanyID);

        //        var oSubLineInfo = m_ServiceUW.fnGetSubLineInfo(m_ConnectionString, m_ProductInfo.cob_id, false, Convert.ToInt16(DateTime.Today.Year), true).FirstOrDefault();

        //        var oCurrencyRates = Helper.GetCurrencyRates(m_ConnectionString, DateTime.Today, m_ProductInfo.default_cur_id);
        //        List<Pixel.Iris4.Models.Web.ListComputationRules> lstComputationsRules = new List<Pixel.Iris4.Models.Web.ListComputationRules>();

        //        this.m_ForcedIssueDate = null;
        //        //this.m_ForcedIssueDateBranch = null;

        //        this.m_ForcedIssueDate = m_ServiceSys.fnGetForcedIssueDateByBranch(m_ConnectionString,
        //                                                    m_CompanyID,
        //                                                    this.m_BranchID);// , (result) =>

        //        // Create the insured
        //        var iInsuredID = 0;
        //        var iAddressID = 0;

        //        MobileAppHelper mobileAppHelper = new MobileAppHelper();
        //        mobileAppHelper.AddNewInsured(
        //                        _FirstName, _MiddleName, _LastName,
        //                        _DOB,
        //                        //_CountryCode, 
        //                        _MobileCountry,
        //                        _MobileArea,
        //                        _MobileNo,
        //                        _Email,
        //                        ref iInsuredID,
        //                        //ref iAddressID,
        //                        m_CompanyID,
        //                        m_BrokerID,
        //                        m_UserID);


        //        PolicyDto policyDto = new PolicyDto();
        //        //reating the policy root
        //        policyDto.root.company_id = m_CompanyID;
        //        policyDto.root.branch_id = m_BranchID;
        //        policyDto.root.lob_id = m_ProductInfo.lob_id;
        //        policyDto.root.object_id = m_ProductInfo.object_id;
        //        policyDto.root.is_group = 1;
        //        policyDto.root.ri_managed_by = (int)Enums.RiManagement.Policy;


        //        #region Creating the policy

        //        policyDto.policy.root_id = policyDto.root.id;
        //        policyDto.policy.contract_type_id = _CONTRACT_TYPE_ID;
        //        //IRIS5
        //        //if (m_CompanyID == (int)Enums.ContractType.Proposal)
        //        //{
        //        //    policyDto.policy.DateProposal = DateTime.Today;
        //        //}

        //        if (m_ForcedIssueDate != null)
        //        {
        //            policyDto.policy.date_issue = Convert.ToDateTime(m_ForcedIssueDate);
        //        }
        //        else
        //        {
        //            policyDto.policy.date_issue = DateTime.Today;
        //        }

        //        policyDto.policy.date_effective = DateTime.Today;
        //        if (m_ProductInfo.is_period_per_annum == 1)
        //        {
        //            policyDto.policy.date_expiry = DateTime.Today.AddYears(1);
        //        }
        //        else
        //        {
        //            policyDto.policy.date_expiry = DateTime.Today.AddDays(m_ProductInfo.period_days);
        //        }

        //        if (!oSubLineInfo.RulePoliciesEndAtNoon)
        //        {
        //            policyDto.policy.date_expiry = Convert.ToDateTime(policyDto.policy.date_expiry).AddDays(-1);
        //        }

        //        policyDto.policy.uw_year = policyDto.policy.date_effective.Year;
        //        policyDto.policy.computation_id = m_ProductInfo.computation_id;
        //        policyDto.policy.prod_id = m_ProductInfo.id;
        //        policyDto.policy.object_id = m_ProductInfo.object_id;
        //        policyDto.policy.cob_id = m_ProductInfo.cob_id;
        //        policyDto.policy.is_cover_rate_on_net = m_ProductInfo.is_net;
        //        policyDto.policy.sum_insured_cur_id = m_ProductInfo.default_cur_id;
        //        policyDto.policy.premium_cur_id = m_ProductInfo.default_cur_id;
        //        policyDto.policy.base_cur_rate = oCurrencyRates.base_cur_rate;
        //        policyDto.policy.counter_cur_rate = oCurrencyRates.counter_cur_rate;
        //        policyDto.policy.created_on = DateTime.Now;
        //        policyDto.policy.created_by = m_UserID;
        //        policyDto.policy.print_copies = 2;
        //        policyDto.policy.insured_id = iInsuredID;
        //        policyDto.policy.broker_id = m_BrokerID;
        //        policyDto.policy.client_id = iInsuredID;
        //        //policyDto.policy.SubBrokerID = m_SubBrokerID;
        //        policyDto.policy.account_id = m_BrokerAccountID;
        //        policyDto.policy.print_name = _FirstName + " " + _MiddleName + " " + _LastName;
        //        policyDto.policy.language_id = 0;
        //        if (iAddressID > 0)
        //        {
        //            policyDto.policy.residency_address_id = iAddressID;
        //            policyDto.policy.billing_address_id = iAddressID;
        //        }
        //        if (_CONTRACT_TYPE_ID == (int)Enums.ContractType.Proposal)
        //        {
        //            //policyDto.policy.STATUS = (int)Enums.ProposalStatus.Request;
        //        }


        //        var ePolProfile = new UW_POLICY_PROFILE();
        //        ePolProfile.policy_id = policyDto.policy.id;
        //        ePolProfile.profile_type_id = 14;
        //        ePolProfile.profile_id = m_AccExecutiveID;
        //        policyDto.policy_profile.Add(ePolProfile);

        //        this.PrepareProductTablesToCreatePolicy(m_ProductInfo,
        //                                                false,
        //                                                -1,
        //                                                policyDto.policy.date_effective.ToString("yyyy/MM/dd"),
        //                                                true,
        //                                                0);

        //        for (int i = 0; i < oDtCovers.Rows.Count; i++)
        //        {
        //            var ePolCover = new UW_POLICY_COVER();
        //            ePolCover.flag = "A";
        //            ePolCover.policy_id = policyDto.policy.id;
        //            ePolCover.cover_id = Convert.ToInt32(oDtCovers.Rows[i]["CoverID"]);
        //            ePolCover.effective_date = policyDto.policy.date_effective;
        //            ePolCover.expiry_date = policyDto.policy.date_expiry;
        //            for (int k = 0; k < oDtSublineCovers.Rows.Count; k++)
        //            {
        //                if (Convert.ToInt32(oDtSublineCovers.Rows[k]["CoverID"]) == ePolCover.cover_id)
        //                {
        //                    ePolCover.is_added_to_sum_insured = Convert.ToBoolean(oDtSublineCovers.Rows[k]["isAddedToSumInsured"]);
        //                    break;
        //                }
        //            }

        //            policyDto.policy_cover.Add(ePolCover);
        //        }

        //         for (int i = 0; i < oDtObjProperties.Rows.Count; i++)
        //        {
        //            var ePolInterest = new UW_POLICY_PROPERTY();
        //            ePolInterest.policy_id = policyDto.policy.id;
        //            ePolInterest.object_property_id = Convert.ToInt32(oDtObjProperties.Rows[i]["ObjectPropertyID"]);
        //            if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 17)
        //            {
        //                ePolInterest.the_value = m_Age.ToString();
        //            }
        //            else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 553)
        //            {
        //                ePolInterest.the_value = m_DOB.ToString();
        //            }
        //            //////else if (Convert.ToInt32(oDtDimensions.Rows[i]["PropertyID"]) == 787)
        //            //////{
        //            //////    ePolInterest.TheValue = m_TravelPeriod.ToString();
        //            //////}
        //            policyDto.policy_property.Add(ePolInterest);
        //        }

        //        //// Adding the DOB
        //        //try
        //        //{
        //        //    var ePolInterest2 = new Pixel.Iris4.Models.Web.UW_PolicyInterestProperties();
        //        //    ePolInterest2.PolicyID = policyDto.policy.PolicyID;
        //        //    ePolInterest2.ObjectPropertyID = 1464;
        //        //    ePolInterest2.TheValue = m_DOB.ToString("yyyy/MM/dd");

        //        //    oContextWrite.UW_PolicyInterestProperties.AddObject(ePolInterest2);
        //        //}
        //        //catch (Exception)
        //        //{
        //        //}
        //        #endregion Creating the policy

        //        try
        //        {
        //            var newPolicyID = PolicyHelper.SavePolicy(policyDto, m_UserID);

        //            var newPol = dbConn.QueryFirstOrDefault<UW_POLICY>("SELECT * FROM UW_POLICY WHERE ID = " + newPolicyID);

        //            if (newPol != null)
        //            {
        //                #region Finalizing the policy

        //                _NewPolicyID = newPol.id;
        //                if (_CONTRACT_TYPE_ID == (int)Enums.ContractType.Policy)
        //                {
        //                    _NewPOLICY_NO = newPol.policy_no;
        //                }
        //                else
        //                {
        //                    szNewProposal = newPol.proposal_no;
        //                }

        //                _PREMIUM_CUR_ID = newPol.premium_cur_id;
        //                _Total = newPol.grand_total;


        //                var m_ErrorApplyProduct = ProductHelper.ApplyProduct(
        //                                                  ref oContextWrite,
        //                                                  m_ProductInfo,
        //                                                  true,
        //                                                  true,
        //                                                  //false,
        //                                                  //false,
        //                                                  //newRec.PolicyRootID,
        //                                                  false,
        //                                                  //false,
        //                                                  oSubLineInfo.RulePoliciesEndAtNoon,
        //                                                  newPol.id);//,
        //                                                             //true);

        //                lstComputationsRules = oServiceUW.fnGetComputationData(m_ConnectionString, policyDto.policy.computation_id);

        //                decimal nNet = 0m;
        //                decimal nTotal = 0m;

        //                var AddedPolicyCovers = oContextWrite.UW_PolicyCovers.Where(w => w.PolicyID == policyDto.policy.id).ToList();

        //                if (m_ProductInfo.is_net == 1 && m_ProductInfo.is_fixed_total == 1)
        //                {
        //                    foreach (var item in AddedPolicyCovers)
        //                    {
        //                        nNet += item.NetPremium;
        //                        nTotal += item.TotalPremium;
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var item in AddedPolicyCovers)
        //                    {
        //                        if (policyDto.policy.is_cover_rate_on_net == 1)
        //                        {
        //                            nNet += item.NetPremium;
        //                        }
        //                        else
        //                        {
        //                            nTotal += item.TotalPremium;
        //                        }
        //                    }
        //                }

        //                decimal nSumInsured = 0m;
        //                foreach (var item in AddedPolicyCovers)
        //                {
        //                    if (item.isAddedToSumInsured)
        //                    {
        //                        nSumInsured += item.SumInsured;
        //                    }
        //                }

        //                this.CreatePolicyComputation(ref oContextWrite,
        //                                                policyDto.policy.id,
        //                                                policyDto.policy.computation_id,
        //                                                nNet,
        //                                                nTotal,
        //                                                m_ProductInfo.is_net,
        //                                                false,
        //                                                0,
        //                                                m_ProductInfo.is_detailed_computation,
        //                                                false,
        //                                                policyDto.policy.date_effective,
        //                                                false,
        //                                                oSubLineInfo);

        //                policyDto.policy.sum_insured = nSumInsured;
        //                oContextWrite.SaveChanges();

        //                this.Commissions_PrepareCommissions(ref oContextWrite,
        //                                                    policyDto.policy.id,
        //                                                    false);

        //                oContextWrite.SaveChanges();

        //                #endregion Finalizing the policy

        //                #region Creating the Certificate

        //                var eCertificate = new UW_POLICY();
        //                eCertificate.root_id = ePolicyRoot.PolicyRootID;
        //                eCertificate.parent_policy_id = policyDto.policy.id;
        //                eCertificate.contract_type_id = (int)Enums.ContractType.Certificate;
        //                if (_CONTRACT_TYPE_ID == (int)Enums.ContractType.Policy)
        //                {
        //                    eCertificate.policy_no = _NewPOLICY_NO;

        //                }
        //                else
        //                {
        //                    eCertificate.proposal_no = szNewProposal;
        //                }
        //                eCertificate.date_issue = policyDto.policy.date_issue;
        //                eCertificate.date_effective = policyDto.policy.date_effective;
        //                eCertificate.date_expiry = policyDto.policy.date_expiry;
        //                eCertificate.uw_year = policyDto.policy.uw_year;
        //                eCertificate.computation_id = m_ProductInfo.computation_id;
        //                eCertificate.prod_id = m_ProductInfo.id;
        //                eCertificate.object_id = m_ProductInfo.object_id;
        //                eCertificate.cob_id = m_ProductInfo.cob_id;
        //                eCertificate.is_cover_rate_on_net = m_ProductInfo.is_net;
        //                eCertificate.sum_insured_cur_id = m_ProductInfo.default_cur_id;
        //                eCertificate.premium_cur_id = m_ProductInfo.default_cur_id;
        //                eCertificate.base_cur_rate = oCurrencyRates.BaseCurRate;
        //                eCertificate.counter_cur_rate = oCurrencyRates.CounterCurRate;
        //                eCertificate.created_on = DateTime.Now;
        //                eCertificate.created_by = m_UserID;
        //                eCertificate.print_copies = 2;
        //                eCertificate.insured_id = iInsuredID;
        //                eCertificate.principal_id = iInsuredID;
        //                eCertificate.broker_id = m_BrokerID;
        //                //policyDto.policy.SubBrokerID = m_SubBrokerID;
        //                eCertificate.account_id = m_BrokerAccountID;
        //                eCertificate.print_name = _FirstName + " " + _MiddleName + " " + _LastName;
        //                eCertificate.language_id = 0;
        //                eCertificate.sum_insured = nSumInsured;

        //                oContextWrite.UW_POLICY.AddObject(eCertificate);

        //                var lstParentCovers = oContextWrite.UW_PolicyCovers.Where(w => w.PolicyID == policyDto.policy.id).ToList();
        //                foreach (var item in lstParentCovers)
        //                {
        //                    var ePolCover = new UW_POLICY_COVER();
        //                    ePolCover.flag = "A";
        //                    ePolCover.policy_id = eCertificate.id;
        //                    ePolCover.cover_id = item.CoverID;
        //                    ePolCover.net_premium = item.NetPremium;
        //                    ePolCover.total_premium = item.TotalPremium;
        //                    ePolCover.yearly_net_premium = item.YearlyNetPremium;
        //                    ePolCover.effective_date = item.EffectiveDate;
        //                    ePolCover.expiry_date = item.ExpiryDate;
        //                    ePolCover.sum_insured = item.SumInsured;

        //                    oContextWrite.UW_PolicyCovers.AddObject(ePolCover);
        //                }
        //                //for (int i = 0; i < oDtCovers.Rows.Count; i++)
        //                //{
        //                //    var ePolCover = new Pixel.Iris4.Models.Web.UW_PolicyCovers();
        //                //    ePolCover.Flag = "A";
        //                //    ePolCover.PolicyID = eCertificate.PolicyID;
        //                //    ePolCover.CoverID = Convert.ToInt32(oDtCovers.Rows[i]["CoverID"]);

        //                //    for (int k = 0; k < oDtSublineCovers.Rows.Count; k++)
        //                //    {
        //                //        if (Convert.ToInt32(oDtSublineCovers.Rows[k]["CoverID"]) == ePolCover.CoverID)
        //                //        {
        //                //            ePolCover.isAddedToSumInsured = Convert.ToBoolean(oDtSublineCovers.Rows[k]["isAddedToSumInsured"]);
        //                //            break;
        //                //        }
        //                //    }

        //                //    oContextWrite.UW_PolicyCovers.AddObject(ePolCover);
        //                //}

        //                //var lstParentInterest = oContextWrite.UW_PolicyInterestProperties.Where(w => w.PolicyID == policyDto.policy.PolicyID).ToList();
        //                //foreach (var item in lstParentInterest)
        //                //{
        //                //    var ePolInterest = new Pixel.Iris4.Models.Web.UW_PolicyInterestProperties();
        //                //    ePolInterest.PolicyID = eCertificate.PolicyID;
        //                //    ePolInterest.ObjectPropertyID = item.ObjectPropertyID;
        //                //    ePolInterest.Sorting = item.Sorting;
        //                //    ePolInterest.TheValue = item.TheValue;
        //                //    oContextWrite.UW_PolicyInterestProperties.AddObject(ePolInterest);
        //                //}

        //                //for (int i = 0; i < oDtDimensions.Rows.Count; i++)
        //                //{
        //                //    var ePolInterest = new Pixel.Iris4.Models.Web.UW_PolicyInterestProperties();
        //                //    ePolInterest.PolicyID = eCertificate.PolicyID;
        //                //    ePolInterest.ObjectPropertyID = Convert.ToInt32(oDtDimensions.Rows[i]["ObjectPropertyID"]);
        //                //    if (Convert.ToInt32(oDtDimensions.Rows[i]["PropertyID"]) == 17)
        //                //    {
        //                //        ePolInterest.TheValue = m_Age.ToString();
        //                //    }
        //                //    else if (Convert.ToInt32(oDtDimensions.Rows[i]["PropertyID"]) == 787)
        //                //    {
        //                //        ePolInterest.TheValue = m_TravelPeriod.ToString();
        //                //    }
        //                //    oContextWrite.UW_PolicyInterestProperties.AddObject(ePolInterest);
        //                //}

        //                for (int i = 0; i < oDtObjProperties.Rows.Count; i++)
        //                {
        //                    var ePolInterest = new UW_POLICY_PROPERTY();
        //                    ePolInterest.policy_id = eCertificate.id;
        //                    ePolInterest.object_property_id = Convert.ToInt32(oDtObjProperties.Rows[i]["ObjectPropertyID"]);
        //                    if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 17)
        //                    {
        //                        ePolInterest.the_value = m_Age.ToString();
        //                    }
        //                    else if (Convert.ToInt32(oDtObjProperties.Rows[i]["PropertyID"]) == 553)
        //                    {
        //                        ePolInterest.the_value = m_DOB.ToString();
        //                    }
        //                    else
        //                    {
        //                        ePolInterest.the_value = "";
        //                    }

        //                    oContextWrite.UW_PolicyInterestProperties.AddObject(ePolInterest);
        //                }


        //                var lstParentComputation = oContextWrite.UW_PolicyComputations.Where(w => w.PolicyID == policyDto.policy.id).ToList();
        //                foreach (var item in lstParentComputation)
        //                {
        //                    var ePolComput = new UW_POLICY_COMPUTATION();
        //                    ePolComput.policy_id = eCertificate.id;
        //                    //ePolComput.ComputationFractionID = item.ComputationFractionID;
        //                    ePolComput.computation_item_id = item.id;
        //                    ePolComput.amount = item.Amount;
        //                    ePolComput.base_amt = item.base_amt;
        //                    oContextWrite.UW_PolicyComputations.AddObject(ePolComput);
        //                }

        //                var lstParentCom = oContextWrite.UW_PolicyCommissions.Where(w => w.PolicyID == policyDto.policy.id).ToList();
        //                foreach (var item in lstParentCom)
        //                {
        //                    var ePolCommiss = new UW_POLICY_COMMISSION();
        //                    ePolCommiss.policy_id = eCertificate.id;
        //                    //ePolCommiss.ComputationFractionID = item.ComputationFractionID;
        //                    ePolCommiss.commission_item_id = item.commission_item_id;
        //                    ePolCommiss.pct = item.pct;
        //                    ePolCommiss.pct_basis_fraction_id = item.pct_basis_fraction_id;
        //                    ePolCommiss.pct_basis_item_id = item.pct_basis_item_id;
        //                    ePolCommiss.amount = item.Amount;
        //                    ePolCommiss.tax_pct = item.TaxPct;
        //                    ePolCommiss.tax_amt = item.TaxAmount;
        //                    ePolCommiss.profile_type_id = item.ProfileTypeID;
        //                    ePolCommiss.profile_id = item.ProfileID;
        //                    ePolCommiss.profile_account_id = item.ProfileAccountID;

        //                    oContextWrite.UW_PolicyCommissions.AddObject(ePolCommiss);
        //                }
        //                oContextWrite.SaveChanges();

        //                //var tmpContextRead = new Pixel.Iris4.Models.Web.Iris4EntitiesUW(
        //                //                           EntityConnectionProvider.UWConnectionString(m_ConnectionString));

        //                //var newCert = tmpContextRead.UW_POLICY.FirstOrDefault(w => w.PolicyID == eCertificate.PolicyID);

        //                //if (newCert != null)
        //                //{
        //                //    szNewPOLICY_NO = newPol.POLICY_NO;

        //                //    //var m_ErrorApplyProduct = ApplyProduct(
        //                //    //                                  ref oContextWrite,
        //                //    //                                  m_ProductInfo,
        //                //    //                                  true,
        //                //    //                                  true,
        //                //    //    //false,
        //                //    //    //false,
        //                //    //    //newRec.PolicyRootID,
        //                //    //                                  false,
        //                //    //    //false,
        //                //    //                                  oSubLineInfo.RulePoliciesEndAtNoon,
        //                //    //                                  newPol.PolicyID);//,
        //                //    ////true);

        //                //    //lstComputationsRules = oServiceUW.fnGetComputationData(m_ConnectionString, policyDto.policy.ComputationID);

        //                //    //decimal nNet = 0m;
        //                //    //decimal nTotal = 0m;

        //                //    //var AddedPolicyCovers = oContextWrite.UW_PolicyCovers.Where(w => w.PolicyID == policyDto.policy.PolicyID).ToList();

        //                //    //if (m_ProductInfo.isNet && m_ProductInfo.isFixedTotal)
        //                //    //{
        //                //    //    foreach (var item in AddedPolicyCovers)
        //                //    //    {
        //                //    //        nNet += item.NetPremium;
        //                //    //        nTotal += item.TotalPremium;
        //                //    //    }
        //                //    //}
        //                //    //else
        //                //    //{
        //                //    //    foreach (var item in AddedPolicyCovers)
        //                //    //    {
        //                //    //        if (policyDto.policy.isCoverRateOnNet)
        //                //    //        {
        //                //    //            nNet += item.NetPremium;
        //                //    //        }
        //                //    //        else
        //                //    //        {
        //                //    //            nTotal += item.TotalPremium;
        //                //    //        }
        //                //    //    }
        //                //    //}

        //                //    //this.CreatePolicyComputation(ref oContextWrite,
        //                //    //                                policyDto.policy.PolicyID,
        //                //    //                                policyDto.policy.ComputationID,
        //                //    //                                nNet,
        //                //    //                                nTotal,
        //                //    //                                m_ProductInfo.isNet,
        //                //    //                                false,
        //                //    //                                0,
        //                //    //                                m_ProductInfo.isDetailedComputation,
        //                //    //                                false,
        //                //    //                                policyDto.policy.DateEffective,
        //                //    //                                false,
        //                //    //                                oSubLineInfo);

        //                //    oContextWrite.SaveChanges();

        //                //    //this.Commissions_PrepareCommissions(ref oContextWrite,
        //                //    //                                    policyDto.policy.PolicyID,
        //                //    //                                    false);

        //                //    oContextWrite.SaveChanges();

        //                //}

        //                #endregion Creating the Certificate

        //                //return 1;
        //            }
        //            else
        //            {
        //                //return 0;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //return 0;
        //        }
        //    }

        //}

        //[HttpPost("RelayConvertProposalIntoPolicy")]
        //public Stream RelayConvertProposalIntoPolicy([FromBody]JObject _objData)
        //{
        //    int _ProposalID;
        //    MemoryStream ms = new MemoryStream();

        //    var dr = DatabaseCommon.GetDataReader(m_ConnectionString,
        //                                          "SELECT ID, ContractStatusID " +
        //                                          "FROM UW_POLICY " +
        //                                          "WHERE ID = " + _ProposalID);
        //    if (dr == null || !dr.HasRows)
        //    {
        //        return ms;
        //    }

        //    dr.Read();

        //    int iProposalID = Convert.ToInt32(dr["ID"]);

        //    //if (Convert.ToInt32(dr["ContractStatusID"]) == (int)Enums.ContractStatuses.Proposal)
        //    if (Convert.ToInt32(dr["ContractStatusID"]) != (int)Enums.ProposalStatus.Rejected)
        //    {
        //        DatabaseCommon.ExecuteQuery(m_ConnectionString,
        //                                "UPDATE UW_POLICY SET CONTRACT_TYPE_ID = 0 WHERE ID = " + iProposalID);

        //        dr = DatabaseCommon.GetDataReader(m_ConnectionString,
        //                                          "SELECT ID, POLICY_NO, PRINT_NAME, PREMIUM_CUR_ID, GRAND_TOTAL " +
        //                                          "FROM UW_POLICY " +
        //                                          "WHERE ID = " + _ProposalID);
        //        if (dr == null || !dr.HasRows)
        //        {
        //            return null;
        //        }
        //        dr.Read();

        //        int iPREMIUM_CUR_ID = Convert.ToInt32(dr["PREMIUM_CUR_ID"]);
        //        string szNewPOLICY_NO = dr["POLICY_NO"].ToString();
        //        decimal nTotal = (decimal)dr["GRAND_TOTAL"];
        //        string szPRINT_NAME = dr["PRINT_NAME"].ToString();

        //        #region preparing the receipt & Layout

        //        var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //        var deviceInfo = new System.Collections.Hashtable();
        //        var typeReportSource = new Telerik.Reporting.TypeReportSource();

        //        var szErrors = "";
        //        var drCur = DatabaseCommon.GetDataReader(m_ConnectionString,
        //                                                 "SELECT CUR_CODE FROM CORE_CURRENCY WHERE CurID = " + iPREMIUM_CUR_ID);
        //        if (drCur != null && drCur.HasRows)
        //        {
        //            drCur.Read();

        //            decimal NewReceiptID = MobileAppHelper.InsertNewReceipt(szNewPOLICY_NO, nTotal, drCur["CUR_CODE"].ToString(), ref szErrors, szPRINT_NAME, false);

        //            if (NewReceiptID != -1)
        //            {
        //                #region preparing the policy layout

        //                typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.UW.PolicyMedicalApp, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //                typeReportSource.Parameters.Add("pPolicyID", iProposalID);
        //                typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //                typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
        //                typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));


        //                Telerik.Reporting.Processing.RenderingResult resultPOL = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //                string fileNamePOL = resultPOL.DocumentName + "_" + iProposalID + "." + resultPOL.Extension;
        //                string path = m_PdfFilesPath;
        //                string filePathPOL = System.IO.Path.Combine(path, fileNamePOL);

        //                using (System.IO.FileStream fs = new System.IO.FileStream(filePathPOL, System.IO.FileMode.Create))
        //                {
        //                    fs.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //                }

        //                #endregion preparing the policy layout

        //                ms.Write(resultPOL.DocumentBytes, 0, resultPOL.DocumentBytes.Length);
        //                ms.Position = 0;
        //                return ms;
        //            }
        //            else
        //            {
        //                return ms;
        //            }
        //        }
        //        else
        //        {
        //            return ms;
        //        }

        //        #endregion preparing the receipt& Layout


        //        ////typeReportSource.TypeName = "Reports.UW_RV, Pixel.Iris4.AppServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null";
        //        //typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.UW.PolicyMedicalApp, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //        //typeReportSource.Parameters.Add("pPolicyID", iProposalID);
        //        //typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);
        //        //    //typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));

        //        //    //typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
        //        //    //typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));

        //        //    Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //        //string fileName = result.DocumentName + "_" + iProposalID + "." + result.Extension;
        //        //string path = m_PdfFilesPath;
        //        //string filePath = System.IO.Path.Combine(path, fileName);

        //        //using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
        //        //{
        //        //    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
        //        //}

        //        //#endregion preparing the policy layout

        //        //#region preparing the receipt layout

        //        //var szErrors = "";
        //        //var drCur = DatabaseCommon.GetDataReader(m_ConnectionString,
        //        //                                            "SELECT CUR_CODE FROM CORE_CURRENCY WHERE CurID = " +
        //        //                                            iPREMIUM_CUR_ID);
        //        //if (drCur != null && drCur.HasRows)
        //        //{
        //        //    drCur.Read();

        //        //    var NewReceiptID = this.InsertNewReceipt(szNewPOLICY_NO, nTotal, drCur["CUR_CODE"].ToString(), ref szErrors);

        //        //    if (NewReceiptID != -1)
        //        //    {
        //        //        //var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
        //        //        //var deviceInfo = new System.Collections.Hashtable();
        //        //        //var typeReportSource = new Telerik.Reporting.TypeReportSource();

        //        //        //typeReportSource.TypeName = "Reports.UW_RV, Pixel.Iris4.AppServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null";
        //        //        typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.Ledger.UW_RV, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        //        //        typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
        //        //        typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)nTotal));
        //        //        typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

        //        //        Telerik.Reporting.Processing.RenderingResult result2 = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

        //        //        string fileName2 = result2.DocumentName + "_" + NewReceiptID.ToString() + "." + result2.Extension;
        //        //        //string path = System.IO.Path.GetTempPath(); // "c:\\MobileAppPDF\\";
        //        //        string path2 = m_PdfFilesPath;
        //        //        string filePath2 = System.IO.Path.Combine(path2, fileName2);

        //        //        using (System.IO.FileStream fs = new System.IO.FileStream(filePath2, System.IO.FileMode.Create))
        //        //        {
        //        //            fs.Write(result2.DocumentBytes, 0, result2.DocumentBytes.Length);
        //        //        }

        //        //        //return NewReceiptNo;
        //        //    }
        //        //    else
        //        //    {
        //        //        //return NewReceiptNo;
        //        //    }
        //        //}

        //        //MemoryStream ms = new MemoryStream();
        //        //ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
        //        //ms.Position = 0;
        //        //WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
        //        //WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-disposition", "inline; filename=" + fileName);

        //        //return ms;
        //        //}
        //        //else
        //        //{
        //        //    MemoryStream ms = new MemoryStream();
        //        //    return ms;
        //        //}
        //    }
        //    return ms;
        //}

        #endregion Relay

        [HttpPost("GetPolicyValidity")]
        public int GetPolicyValidity([FromBody]JObject _objData)
        {
            var szSql = "SELECT TOP 1 UW_POLICY.ROOT_ID " +
                        " FROM UW_POLICY " +
                        " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                        " WHERE UW_ROOT.COMPANY_ID = " + appOptions.CompanyID +
                        " AND CONTRACT_TYPE_ID = 0 " +
                        " AND POLICY_NO = '" + _objData["PolicyNo"].ToString() + "'";

            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = GlobalVariables.ConnectionString;
                dbConn.Open();

                var dr = dbConn.QueryFirstOrDefault<int?>(szSql);
                if (dr == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

        }
    }
}