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
using Pixel.IRIS5.API.Web.Controllers.UW;
using Pixel.IRIS5.Models;
using Pixel.IRIS5.Shared;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    [Route("api/[controller]")]
    public class AppCollectionController : Controller
    {
        [HttpPost("GetPolicyBalance")]
        public PolicyBalance GetPolicyBalance([FromBody]JObject _objData)
        {
            var _policyNo = _objData["PolicyNo"].ToString();

            PolicyBalance policyData = new PolicyBalance();

            //lst.Add(new PolicyData { root_id = -1, balance = 0, cur_code = "" });

            var szSql = "SELECT TOP 1 UW_POLICY.ROOT_ID, POLICY_NO, CUR_CODE " +
                        " FROM UW_POLICY " +
                        " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                        " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
                        " WHERE UW_ROOT.COMPANY_ID = " + appOptions.CompanyID + //m_BrokingCompanyID +
                        " AND (CONTRACT_TYPE_ID = 0 OR CONTRACT_TYPE_ID = 11 ) " +
                        " AND POLICY_NO = '" + _policyNo + "'" +
                        " ORDER BY UW_POLICY.ID DESC ";

            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = GlobalVariables.ConnectionString;//  connectionSQL;
                dbConn.Open();

                var dr = dbConn.QueryFirstOrDefault<PolicyBalance>(szSql);
                //if (dr == null || !dr.HasRows)
                //{
                //    szSql = "SELECT TOP 1 UW_POLICY.ROOT_ID, POLICY_NO, CUR_CODE " +
                //          " FROM UW_POLICY " +
                //          " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                //          " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
                //          " WHERE UW_ROOT.COMPANY_ID = CASE WHEN SUBSTRING(POLICY_NO,1,2) = 'LF' THEN " + m_LifeCompanyID + " ELSE " + m_CompanyID + " END " +
                //          " AND (CONTRACT_TYPE_ID = 0 OR CONTRACT_TYPE_ID = 11) " +
                //          " AND POLICY_NO = '" + _policyNo + "'" +
                //          " ORDER BY UW_POLICY.ID DESC ";

                //    dr = DatabaseCommon.GetDataReader(m_ConnectionString, szSql);
                //    if (dr == null || !dr.HasRows)
                //    {
                //        return lst;
                //    }
                //}

                //dr.Read();

                MobileAppHelper mobileAppHelper = new MobileAppHelper();
                decimal nBalance = mobileAppHelper.GetPolicyUnpaidBalance(dr.root_id, 0);

                policyData.root_id = dr.root_id;
                policyData.policy_no = dr.policy_no;
                policyData.cur_code = dr.cur_code;
                policyData.balance = nBalance;

                return policyData;
            }
        }

        [HttpPost("CollectMultiplePolicies")]
        public Stream CollectMultiplePolicies([FromBody]JObject _objData)
        {
            string _CollectedPolicies = _objData["CollectedPolicies"].ToString();

            //string path = m_PdfFilesPath;
            //string fileName = "";
            String[] myArray = _CollectedPolicies.Split('|');

            // MemoryStream[] ms_array = new MemoryStream[myArray.Length];

            string szMessage = "";

            //int i = 0;
            var szReceiptIDs = "";
            foreach (var substring in myArray)
            {
                String[] values = substring.Split(',');

                MobileAppHelper mobileAppHelper = new MobileAppHelper();
                decimal NewReceiptID = mobileAppHelper.InsertNewReceipt(values[0].ToString(),
                                                        Convert.ToDecimal(values[1]),
                                                        values[2].ToString(),
                                                        ref szMessage,
                                                        "",
                                                        true);
                if (NewReceiptID != -1)
                {
                    if (szReceiptIDs == "")
                    {
                        szReceiptIDs += NewReceiptID;
                    }
                    else
                    {
                        szReceiptIDs += "," + NewReceiptID;
                    }

                    //var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
                    //var deviceInfo = new System.Collections.Hashtable();
                    //var typeReportSource = new Telerik.Reporting.TypeReportSource();

                    //typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.Ledger.UW_RV, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    //typeReportSource.Parameters.Add("pTrsID", NewReceiptID);
                    //typeReportSource.Parameters.Add("pWording", Pixel.Silverlight.Libs.NumberToWords.ToWordsEnglish((double)Convert.ToDecimal(values[1])));
                    //typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

                    //Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

                    //fileName = result.DocumentName + "_" + NewReceiptID.ToString() + "." + result.Extension;
                    //string filePath = System.IO.Path.Combine(path, fileName);

                    //using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    //{
                    //    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);                         
                    //}

                    //MemoryStream ms = new MemoryStream();
                    //ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                    //ms.Position = 0;

                    //ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);

                    //ms_array[i] = ms;
                    //i += 1;
                }
            }
            //return ms_array;





            //TechnicalStatement report = new TechnicalStatement();
            //report.Parameters["Prm"].Value = _objData["Prm"].ToString();
            ////report.Parameters["Layout"].Value = _objData["Layout"].ToString();

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    report.ExportToPdf(ms);

            //    dynamic objToReturn = new System.Dynamic.ExpandoObject();
            //    objToReturn.byteArray = ms.ToArray();
            //    return objToReturn;
            //}




            //var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            //var deviceInfo = new System.Collections.Hashtable();
            //var typeReportSource = new Telerik.Reporting.TypeReportSource();

            //typeReportSource.TypeName = "Pixel.Iris4.LayoutsLibAssurex.Ledger.UW_RVMultiple, Pixel.Iris4.LayoutsLibAssurex, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            //typeReportSource.Parameters.Add("pTrsIDs", szReceiptIDs);
            //typeReportSource.Parameters.Add("pConnectionString", m_ConnectionString);

            //Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", typeReportSource, deviceInfo);

            //fileName = result.DocumentName + "_" + Guid.NewGuid() + "." + result.Extension;
            //string filePath = System.IO.Path.Combine(path, fileName);

            //using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            //{
            //    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            //}

            MemoryStream ms = new MemoryStream();
            //ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
            //ms.Position = 0;

            return ms;
        }

        [HttpPost("CollectSinglePolicy")]
        //public Task<byte[]> CollectSinglePolicy([FromBody]JObject _objData)
        public Task<Stream> CollectSinglePolicy([FromBody]JObject _objData)
        {
            string szMessage = "";
            MobileAppHelper mobileAppHelper = new MobileAppHelper();
            int NewReceiptID = mobileAppHelper.InsertNewReceipt(_objData["PolicyNo"].ToString(),
                                                                Convert.ToDecimal(_objData["PaidAmount"].ToString()),
                                                                _objData["CurCode"].ToString(),
                                                                ref szMessage,
                                                                "",
                                                                true);

            if (NewReceiptID != -1)
            {
                var result = GetReceiptAsPdf(NewReceiptID);
                return result;

                //HttpClient httpClient = new HttpClient();

                //var stringContent = new FormUrlEncodedContent(new[]
                //{
                //    new KeyValuePair<string, string>("ID", NewReceiptID.ToString())//,
                //    //new KeyValuePair<string, string>("field2", "value2"),
                //});

                ////HttpResponseMessage response = await httpClient.PostAsync("http://localhost:5006/api/rp_rv/GetPdf", stringContent);
                //HttpResponseMessage response = httpClient.PostAsync("http://localhost:5006/api/rp_rv/GetPdf", stringContent).Result;
                //response.EnsureSuccessStatusCode();
                ////var responseBody = await response.Content.ReadAsByteArrayAsync();
                //var responseBody = await response.Content.ReadAsStreamAsync();

                ////HttpResponseMessage response = await httpClient.PostAsync("http://localhost:5006/api/rp_rv/GetPdf", stringContent);
                ////var responseBody = await response.Content.ReadAsStreamAsync();

                //return responseBody;

                ////MemoryStream ms = new MemoryStream();
                ////ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                ////ms.Position = 0;

                ////return ms;
            }
            else
            {
                return null;
            }
        }

        //async Task<byte[]> GetReceiptAsPdf(int _receiptId)
        async Task<Stream> GetReceiptAsPdf(int _receiptId)
        {
            HttpClient httpClient = new HttpClient();

            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ID", _receiptId.ToString())//,
                //new KeyValuePair<string, string>("field2", "value2"),
            });

            HttpResponseMessage response = await httpClient.PostAsync(appOptions.layoutUrl + "rp_rv/GetPdf", stringContent);
            response.EnsureSuccessStatusCode();
            //var responseBody = await response.Content.ReadAsByteArrayAsync();
            var responseBody = await response.Content.ReadAsStreamAsync();

            return responseBody;
        }

    }
}