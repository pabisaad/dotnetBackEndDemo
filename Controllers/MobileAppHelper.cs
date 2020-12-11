using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Pixel.Core.Lib;
using Pixel.IRIS5.API.Ledger.Controllers;
using Pixel.IRIS5.API.Web.Controllers.Profile;
using Pixel.IRIS5.API.Web.Controllers.UW;
using Pixel.IRIS5.Models;
using Pixel.IRIS5.Shared;
using static Pixel.IRIS5.Shared.Structures;

namespace Pixel.IRIS5.API.Mobile.Controllers
{
    public class MobileAppHelper
    { 
        public int InsertNewReceipt(string _policyNo,
                                        decimal _paidAmt,
                                        string _curCode,
                                        ref string _ErrorMessage,
                                        string _printName,
                                        bool isCheckForPolicyCur)
        {
            TrsDto trsDto = new TrsDto();

            int iCurID;
            int iPolicyCurID;
            decimal nPolicyPaidAmt = _paidAmt;

            using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
            {
                dbConn.ConnectionString = appOptions.ConnectionString;
                dbConn.Open();

                var szSqlCur = "SELECT ID FROM CORE_CURRENCY WHERE CUR_CODE = '" + _curCode + "'";
                iCurID = dbConn.QueryFirstOrDefault<int>(szSqlCur);
                if (iCurID == 0)
                {
                    return -1;
                }
                else
                {
                    iPolicyCurID = iCurID;
                }

                var oCurrencyRates = Pixel.IRIS5.Shared.Helper.GetCurrencyRates(appOptions.ConnectionString, DateTime.Today, iCurID);

                if (isCheckForPolicyCur)
                {
                    // var szSqlPolicyCur =
                    // " SELECT PREMIUM_CUR_ID " +
                    //" FROM UW_POLICY " +
                    //" LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                    //" WHERE CONTRACT_TYPE_ID = 0 " +
                    //" AND POLICY_NO = '" + _POLICY_NO + "'" +
                    //" AND UW_ROOT.COMPANY_ID = " + m_BrokingCompanyID;

                    // iPolicyCurID = dbConn.QueryFirstOrDefault<int>(szSqlPolicyCur);
                    // if (iPolicyCurID > 0)
                    // {
                    //     if (iPolicyCurID != iCurID)
                    //     {
                    //         oCurrencyRates = Helper.GetCurrencyRates(GlobalOptions.ConnectionString, DateTime.Today, (int)iPolicyCurID);
                    //         nPolicyPaidAmt = Helper.ConvertAmount(GlobalOptions.ConnectionString, _PAID_AMT, iCurID, DateTime.Today, (int)iPolicyCurID, 2);
                    //     }
                    // }
                    // else
                    // {
                    var szSqlPolicyCur =
                            "SELECT PREMIUM_CUR_ID " +
                            " FROM UW_POLICY " +
                            " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                            " WHERE CONTRACT_TYPE_ID = 0 " +
                            " AND POLICY_NO = '" + _policyNo + "'" +
                            " AND UW_ROOT.COMPANY_ID = " + appOptions.CompanyID;
                    //                    " AND UW_ROOT.COMPANY_ID = CASE WHEN SUBSTRING(POLICY_NO,1,2) = 'LF' THEN " + m_LifeCompanyID + " ELSE " + m_CompanyID + " END ";

                    iPolicyCurID = dbConn.QueryFirstOrDefault<int>(szSqlPolicyCur);
                    if (iPolicyCurID > 0)
                    {
                        if (iPolicyCurID != iCurID)
                        {
                            oCurrencyRates = Shared.Helper.GetCurrencyRates(appOptions.ConnectionString, DateTime.Today, (int)iPolicyCurID);
                            nPolicyPaidAmt = Shared.Helper.ConvertAmount(appOptions.ConnectionString, _paidAmt, iCurID, DateTime.Today, (int)iPolicyCurID, 2);
                        }
                    }
                    //}
                }

                //var szSql =
                //      "SELECT " +
                //      " RP_MVMT.COMPANY_ID, PRINT_NAME, " +
                //      " RP_MVMT.ID," +
                //      " -1 AS INST_ID, " +
                //      " RP_MVMT.AMOUNT - RP_MVMT.PAID_AMT AS DUE_AMT," +
                //      " PROFILE_TYPE_ID, " +
                //      " PROFILE_ID, " +
                //      " RP_MVMT.ACCOUNT_ID, " +
                //      " GL_ACCOUNT.ACCOUNT_DESC AS ATTENTION, " +
                //      " RP_MVMT.DUE_DATE," +
                //      " CUR_CODE " +
                //      " FROM RP_MVMT  " +
                //      " LEFT JOIN UW_POLICY ON RP_MVMT.LINK_ID = UW_POLICY.ID " +
                //      " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                //      " LEFT JOIN GL_ACCOUNT ON RP_MVMT.ACCOUNT_ID = GL_ACCOUNT.ID " +
                //      " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
                //      " WHERE UW_ROOT.COMPANY_ID = " + m_BrokingCompanyID +
                //      " AND LINK_TYPE_ID = 1 " +
                //      " AND IS_SCHEDULED = 0 " +
                //      " AND POLICY_NO = '" + _POLICY_NO + "'" +
                //      // " AND RP_MVMT.AMOUNT > RP_MVMT.PAID_AMT " +
                //      " UNION ALL  " +
                //      " SELECT " +
                //      " RP_MVMT.COMPANY_ID, PRINT_NAME, " +
                //      " RP_MVMT_INST.MVMT_ID, " +
                //      " INST_ID, " +
                //      " RP_MVMT_INST.AMOUNT - RP_MVMT_INST.PAID_AMT AS DUE_AMT," +
                //      " PROFILE_TYPE_ID, " +
                //      " PROFILE_ID, " +
                //      " RP_MVMT.ACCOUNT_ID, " +
                //      " GL_ACCOUNT.ACCOUNT_DESC AS ATTENTION, " +
                //      " RP_MVMT.DUE_DATE," +
                //      " CUR_CODE " +
                //      " FROM RP_MVMT_INST " +
                //      " LEFT JOIN RP_MVMT ON RP_MVMT_INST.MVMT_ID = RP_MVMT.ID " +
                //      " LEFT JOIN UW_POLICY ON RP_MVMT.LINK_ID = UW_POLICY.ID " +
                //      " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                //      " LEFT JOIN GL_ACCOUNT ON RP_MVMT.ACCOUNT_ID = GL_ACCOUNT.ID " +
                //      " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
                //      " WHERE UW_ROOT.COMPANY_ID = " + m_BrokingCompanyID +
                //      " AND RP_MVMT.LINK_TYPE_ID = 1 " +
                //      " AND POLICY_NO = '" + _POLICY_NO + "'"; 

                //var drReceivables = dbConn.Query<RpDto>(szSql).ToList();
                //if (drReceivables == null || drReceivables.Count() == 0)
                //{
                var szSql =
                    " SELECT " +
                    " RP_MVMT.COMPANY_ID, " +
                    " PRINT_NAME, " +
                    " RP_MVMT.ID AS MVMT_ID," +
                    " NULL AS INST_ID, " +
                    " RP_MVMT.AMOUNT - RP_MVMT.PAID_AMT AS DUE_AMT," +
                    " PROFILE_TYPE_ID, " +
                    " PROFILE_ID, " +
                    " RP_MVMT.ACCOUNT_ID, " +
                    " GL_ACCOUNT.ACCOUNT_DESC AS ATTENTION, " +
                    " RP_MVMT.DUE_DATE," +
                    " CUR_CODE " +
                    " FROM RP_MVMT  " +
                    " LEFT JOIN UW_POLICY ON RP_MVMT.LINK_ID = UW_POLICY.ID " +
                    " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                    " LEFT JOIN GL_ACCOUNT ON RP_MVMT.ACCOUNT_ID = GL_ACCOUNT.ID " +
                    " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
                    //" WHERE UW_ROOT.COMPANY_ID = CASE WHEN SUBSTRING(POLICY_NO,1,2) = 'LF' THEN " + m_LifeCompanyID + " ELSE " + m_CompanyID + " END " +
                    " WHERE UW_ROOT.COMPANY_ID = " + appOptions.CompanyID +
                    " AND LINK_TYPE_ID = 1 " +
                    " AND IS_SCHEDULED = 0 " +
                    " AND POLICY_NO = '" + _policyNo + "'" +
                    " UNION ALL  " +
                    " SELECT " +
                    " RP_MVMT.COMPANY_ID, " +
                    " PRINT_NAME, " +
                    " RP_MVMT_INST.MVMT_ID, " +
                    " RP_MVMT_INST.ID AS INST_ID, " +
                    " RP_MVMT_INST.AMOUNT - RP_MVMT_INST.PAID_AMT AS DUE_AMT," +
                    " PROFILE_TYPE_ID, " +
                    " PROFILE_ID, " +
                    " RP_MVMT.ACCOUNT_ID, " +
                    " GL_ACCOUNT.ACCOUNT_DESC AS ATTENTION, " +
                    " RP_MVMT.DUE_DATE," +
                    " CUR_CODE " +
                    " FROM RP_MVMT_INST " +
                    " LEFT JOIN RP_MVMT ON RP_MVMT_INST.MVMT_ID = RP_MVMT.ID " +
                    " LEFT JOIN UW_POLICY ON RP_MVMT.LINK_ID = UW_POLICY.ID " +
                    " LEFT JOIN UW_ROOT ON UW_POLICY.ROOT_ID = UW_ROOT.ID " +
                    " LEFT JOIN GL_ACCOUNT ON RP_MVMT.ACCOUNT_ID = GL_ACCOUNT.ID " +
                    " LEFT JOIN CORE_CURRENCY ON UW_POLICY.PREMIUM_CUR_ID = CORE_CURRENCY.ID " +
                    //" WHERE UW_ROOT.COMPANY_ID = CASE WHEN SUBSTRING(POLICY_NO,1,2) = 'LF' THEN " + m_LifeCompanyID + " ELSE " + m_CompanyID + " END " +
                    " WHERE UW_ROOT.COMPANY_ID = " + appOptions.CompanyID +
                    " AND RP_MVMT.LINK_TYPE_ID = 1 " +
                    " AND POLICY_NO = '" + _policyNo + "'";

                var drReceivables = dbConn.Query<RpDto>(szSql).ToList();
                if (drReceivables == null || drReceivables.Count() == 0)
                {
                    return -1;
                }
                //}

                //drReceivables.Read();
                //var icompany_id = drReceivables.ElementAt(0).company_id;
                var iPROFILE_TYPE_ID = drReceivables.ElementAt(0).profile_type_id;
                var iPROFILE_ID = drReceivables.ElementAt(0).profile_id;
                var iACCOUNT_ID = drReceivables.ElementAt(0).account_id;
                var szATTENTION = drReceivables.ElementAt(0).print_name;
                var nMvmtId = drReceivables.ElementAt(0).mvmt_id;
                int? nInstId = drReceivables.ElementAt(0).inst_id;
                var nDueAmt = drReceivables.ElementAt(0).due_amt;
                var dDueDate = drReceivables.ElementAt(0).due_date;

                //System.Data.DataRow[] oDrArray = null;

                GL_TRS eTrs = new GL_TRS();
                GL_TRS_LINE etrsLine;
                GL_TRS_ALLOCAT etrsAllocat;
                GL_TRS_PAYMENT etrsPayment;

                var fiscalPeriod = dbConn.QueryFirstOrDefault<CORE_PERIOD>("SELECT TOP 1 CORE_PERIOD.* " +
                    " FROM CORE_PERIOD " +
                    " WHERE COMPANY_ID = " + appOptions.CompanyID +
                    " AND START_DATE <='" + DateTime.Today.ToString("yyyy/MM/dd") + "'" +
                    " AND END_DATE >='" + DateTime.Today.ToString("yyyy/MM/dd") + "'");

                if (fiscalPeriod == null)
                {
                    return -1;
                }
                //SetDefaultValues_GL_TRS(_WcfOptions, _UserID, ref eTransactions);
                Shared.Helper.SetDefaultValues_GL_TRS(appOptions.CompanyID,
                                               ref eTrs,
                                               appOptions.UserID,
                                               appOptions.BranchID,
                                               fiscalPeriod.id,
                                               fiscalPeriod.end_date);

                trsDto.trs = eTrs;
                trsDto.trs.id = IdGenerator.GetNextTempId();

                //if (icompany_id == 23)
                //{
                //    trsDto.trs.company_id = m_LifeCompanyID;
                //    trsDto.trs.period_id = m_LifePeriodID;
                //}
                //else if (icompany_id == 21)
                //{
                //    trsDto.trs.company_id = m_CompanyID;
                //    trsDto.trs.period_id = m_PeriodID;
                //}
                //else if (icompany_id == 22)
                //{
                //    trsDto.trs.company_id = m_BrokingCompanyID;
                //    trsDto.trs.period_id = m_BrokingPeriodID;
                //}
                trsDto.trs.branch_id = appOptions.BranchID;
                trsDto.trs.trs_date = DateTime.Today < fiscalPeriod.end_date ? DateTime.Today : fiscalPeriod.end_date;
                trsDto.trs.is_posted = 1;
                trsDto.trs.created_by = appOptions.UserID;
                trsDto.trs.journal_type_id = appOptions.Receipt_JournalTypeID;
                trsDto.trs.trs_date = DateTime.Today;
                trsDto.trs.origin_type = "MobileApp";
                trsDto.trs.account_profile_type_id = iPROFILE_TYPE_ID;
                trsDto.trs.account_profile_id = iPROFILE_ID;
                trsDto.trs.account_id = iACCOUNT_ID;
                trsDto.trs.trs_type = "MA";
                if (_printName != "")
                {
                    trsDto.trs.attention = _printName;
                }
                else
                {
                    trsDto.trs.attention = szATTENTION;
                }
                trsDto.trs.amount = _paidAmt;
                trsDto.trs.cur_id = iCurID;
                trsDto.trs.base_cur_rate = oCurrencyRates.base_cur_rate;
                trsDto.trs.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                trsDto.trs.being = "PAY/POL " + _policyNo;

                // TRS Lines
                etrsLine = new GL_TRS_LINE();
                etrsLine.created_by = appOptions.UserID;
                etrsLine.trs_id = trsDto.trs.id;
                etrsLine.branch_id = appOptions.BranchID;
                ////if (_WcfOptions.PostingByBranch)
                ////{
                ////    eTransactionLines.BranchID = m_BranchID;
                ////}   
                //if (icompany_id == 23)
                //{
                //    etrsLine.account_id = m_LifeReceipt_DB_AccountID;
                //}
                //else if (icompany_id == 21)
                //{
                etrsLine.account_id = appOptions.Receipt_DB_AccountID;
                //}
                //else if (icompany_id == 22)
                //{
                //    etrsLine.account_id = m_BrokingReceipt_DB_AccountID;
                //}

                ////eTransactionLines.CostCenterTypeID = (int)Pixel.Iris4.Settings.Enums.CostCenterTypes.SubLines;
                ////eTransactionLines.CostCenterID = (int)_DataRow["SubLineID"];
                etrsLine.db_cr = (_paidAmt > 0 ? "D" : "C");
                etrsLine.cur_id = iCurID;
                etrsLine.base_cur_rate = oCurrencyRates.base_cur_rate;
                etrsLine.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                etrsLine.amount = System.Math.Abs(_paidAmt);
                etrsLine.base_amt = System.Math.Abs(Math.Round(_paidAmt * oCurrencyRates.base_cur_rate, 2));
                etrsLine.counter_amt = System.Math.Abs(Math.Round(_paidAmt / oCurrencyRates.counter_cur_rate, 2));
                etrsLine.wording = "PAY/POL " + _policyNo;
                etrsLine.is_payment_mode = 1;
                trsDto.trs_line.Add(etrsLine);

                // Broker or Client Account
                etrsLine = new GL_TRS_LINE();
                etrsLine.branch_id = appOptions.BranchID;
                etrsLine.created_by = appOptions.UserID;
                etrsLine.trs_id = trsDto.trs.id;
                //if (_WcfOptions.PostingByBranch)
                //{
                //    eTransactionLines.BranchID = m_BranchID;
                //}
                etrsLine.account_id = iACCOUNT_ID;
                //eTransactionLines.CostCenterTypeID = (int)Pixel.Iris4.Settings.Enums.CostCenterTypes.SubLines;
                //eTransactionLines.CostCenterID = (int)_DataRow["SubLineID"];
                etrsLine.db_cr = (_paidAmt > 0 ? "C" : "D");
                etrsLine.cur_id = iPolicyCurID;
                etrsLine.base_cur_rate = oCurrencyRates.base_cur_rate;
                etrsLine.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                etrsLine.amount = System.Math.Abs(nPolicyPaidAmt);// System.Math.Abs(_PAID_AMT);
                etrsLine.base_amt = System.Math.Abs(Math.Round(nPolicyPaidAmt * oCurrencyRates.base_cur_rate, 2)); //_PAID_AMT
                etrsLine.counter_amt = System.Math.Abs(Math.Round(nPolicyPaidAmt / oCurrencyRates.counter_cur_rate, 2)); //_PAID_AMT
                etrsLine.wording = "PAY/POL " + _policyNo;
                trsDto.trs_line.Add(etrsLine);

                // Settlements
                if (nPolicyPaidAmt <= nDueAmt)
                {
                    etrsAllocat = new GL_TRS_ALLOCAT();
                    etrsAllocat.trs_id = trsDto.trs.id;
                    etrsAllocat.mvmt_id = nMvmtId;
                    etrsAllocat.inst_id = nInstId;
                    etrsAllocat.db_cr = (_paidAmt > 0 ? "C" : "D");
                    etrsAllocat.due_date = dDueDate;
                    etrsAllocat.due_amt = nDueAmt;// _PAID_AMT;
                    etrsAllocat.paid_amt = nPolicyPaidAmt;// _PAID_AMT;
                    etrsAllocat.equiv_amt = _paidAmt;
                    etrsAllocat.cur_id = iPolicyCurID;
                    etrsAllocat.base_cur_rate = oCurrencyRates.base_cur_rate;
                    etrsAllocat.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                    trsDto.trs_allocat.Add(etrsAllocat);
                }
                else
                {
                    //decimal nRemainingAmount = _PAID_AMT - nDueAmt;
                    decimal nRemainingAmount = nPolicyPaidAmt;// -nDueAmt;

                    etrsAllocat = new GL_TRS_ALLOCAT();
                    etrsAllocat.trs_id = trsDto.trs.id;
                    etrsAllocat.mvmt_id = nMvmtId;
                    etrsAllocat.inst_id = nInstId;
                    etrsAllocat.db_cr = (_paidAmt > 0 ? "C" : "D");
                    etrsAllocat.due_date = dDueDate;
                    etrsAllocat.cur_id = iPolicyCurID;
                    etrsAllocat.due_amt = nDueAmt;
                    etrsAllocat.paid_amt = nDueAmt;
                    if (iCurID == iPolicyCurID)
                    {
                        etrsAllocat.equiv_amt = nDueAmt;
                    }
                    else
                    {
                        etrsAllocat.equiv_amt = Shared.Helper.ConvertAmount(appOptions.ConnectionString, nDueAmt, iPolicyCurID, DateTime.Today, iCurID, 2);
                    }

                    etrsAllocat.base_cur_rate = oCurrencyRates.base_cur_rate;
                    etrsAllocat.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                    trsDto.trs_allocat.Add(etrsAllocat);

                    nRemainingAmount -= etrsAllocat.paid_amt;

                    for (int i = 1; i < drReceivables.Count(); i++)
                    {
                        if (nRemainingAmount > 0)
                        {
                            nMvmtId = drReceivables.ElementAt(i).mvmt_id;
                            nInstId = drReceivables.ElementAt(i).inst_id;
                            nDueAmt = drReceivables.ElementAt(i).due_amt;
                            dDueDate = drReceivables.ElementAt(i).due_date;

                            etrsAllocat = new GL_TRS_ALLOCAT();
                            etrsAllocat.trs_id = trsDto.trs.id;
                            etrsAllocat.mvmt_id = nMvmtId;
                            etrsAllocat.inst_id = nInstId;
                            etrsAllocat.due_date = dDueDate;
                            etrsAllocat.db_cr = (_paidAmt > 0 ? "C" : "D");
                            etrsAllocat.cur_id = iCurID;
                            if (nRemainingAmount > nDueAmt)
                            {
                                etrsAllocat.due_amt = nDueAmt;
                                etrsAllocat.paid_amt = nDueAmt;
                                etrsAllocat.equiv_amt = nDueAmt;
                            }
                            else
                            {
                                etrsAllocat.due_amt = nRemainingAmount;
                                etrsAllocat.paid_amt = nRemainingAmount;
                                etrsAllocat.equiv_amt = nRemainingAmount;
                            }
                            etrsAllocat.base_cur_rate = oCurrencyRates.base_cur_rate;
                            etrsAllocat.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                            trsDto.trs_allocat.Add(etrsAllocat);

                            nRemainingAmount -= etrsAllocat.paid_amt;
                        }
                    }

                    if (nRemainingAmount > 0)
                    {
                        // then we need to set the amount on any entry as OVER PAID

                        etrsAllocat = new GL_TRS_ALLOCAT();
                        etrsAllocat.trs_id = trsDto.trs.id;
                        etrsAllocat.mvmt_id = nMvmtId;
                        etrsAllocat.inst_id = nInstId;
                        etrsAllocat.db_cr = (_paidAmt > 0 ? "C" : "D");
                        etrsAllocat.due_date = dDueDate;
                        etrsAllocat.due_amt = 0;
                        etrsAllocat.paid_amt = nRemainingAmount;
                        if (iCurID == iPolicyCurID)
                        {
                            etrsAllocat.equiv_amt = nRemainingAmount;
                        }
                        else
                        {
                            etrsAllocat.equiv_amt = Shared.Helper.ConvertAmount(appOptions.ConnectionString, nRemainingAmount, iPolicyCurID, DateTime.Today, iCurID, 2);
                        }
                        etrsAllocat.cur_id = iPolicyCurID;
                        etrsAllocat.base_cur_rate = oCurrencyRates.base_cur_rate;
                        etrsAllocat.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                        trsDto.trs_allocat.Add(etrsAllocat);
                    }
                }

                // Payment Mode
                etrsPayment = new GL_TRS_PAYMENT();
                etrsPayment.created_by = appOptions.UserID;
                etrsPayment.trs_id = trsDto.trs.id;
                //if (icompany_id == 23)
                //{
                //    etrsPayment.payment_mode_id = m_LifeReceipt_DefaultModeOfPayment;
                //}
                //else if (icompany_id == 21)
                //{
                etrsPayment.payment_mode_id = appOptions.Receipt_DefaultModeOfPayment;
                //}
                //else if (icompany_id == 22)
                //{
                //    if (_POLICY_NO.Substring(0, 2) == "LF")
                //    {
                //        etrsPayment.payment_mode_id = m_BrokingReceipt_DefaultModeOfPayment_Life;
                //    }
                //    else
                //    {
                //        etrsPayment.payment_mode_id = m_BrokingReceipt_DefaultModeOfPayment;
                //    }
                //}
                etrsPayment.cur_id = iCurID;
                etrsPayment.base_cur_rate = oCurrencyRates.base_cur_rate;
                etrsPayment.counter_cur_rate = oCurrencyRates.counter_cur_rate;
                etrsPayment.amount = Math.Abs(_paidAmt);
                etrsPayment.equiv_amt = Math.Abs(_paidAmt);
                etrsPayment.value_date = DateTime.Today;
                trsDto.trs_payment.Add(etrsPayment);

                //// >>> Testing for difference, then Insert rows for the exchange difference
                //this.Posting_CheckJvBalance(ref _WcfOptions, m_UserID, ref _ContextLedgerWrite, _iAccExchangeDiffDebit, _iAccExchangeDiffCredit, (DateTime)_DataRow["DateIssue"], eTransactions.TrsID, ref oCurrencyRates);

                // >>> Update the database
                // try
                // {
                TrsHelper trsHelper = new TrsHelper();
                var savResult = trsHelper.SaveTrs(trsDto, appOptions.UserID);

                if (Pixel.Core.Lib.CoreHelper.IsNumeric(savResult))
                {
                    return Convert.ToInt32(savResult);
                }
                else
                {
                    return -1;
                }

                // }
                // catch (Exception ex)
                // {
                //  _ErrorMessage = ex.Message;
                // return -1;
                // }
            } // dbConn         
        }
        
        public void AddNewInsured(
                                    string _FirstName,
                                    string _MiddleName,
                                    string _LastName,
                                    DateTime? _dob,
                                    //string _CountryCode,
                                    string _MobileCountry,
                                    string _MobileArea,
                                    string _MobileNo,
                                    string _Email,
                                    ref int _InsuredID
                                    //ref int _AddressID,
                                    )
        {
            ProfileDto profileDto = new ProfileDto();

            // PF_Members
            //if (!Convert.IsDBNull(item["DateOfBirth"]) && item["DateOfBirth"].ToString() != string.Empty)
            //{
            //    szDOB = DateTime.Parse( item["DateOfBirth"].ToString ()).Year + "/" + DateTime.Parse( item["DateOfBirth"].ToString()).Month + "/" + DateTime.Parse( item["DateOfBirth"].ToString()).Day;
            //}
            //if (!Convert.IsDBNull(item["Title"]) && item["Title"].ToString() != string.Empty)
            //{
            //    eMember.TitleID = (int)(oContextProfile.PF_Titles.FirstOrDefault((e1) => e1.TitleDesc.ToUpper() == item["Title"].ToString().Trim().Replace("'", "''").ToUpper()).TitleID);
            //}
            profileDto.contact.first_name = _FirstName;
            profileDto.contact.middle_name = _MiddleName;
            profileDto.contact.last_name = _LastName;
            profileDto.contact.print_name = _FirstName + " " + _MiddleName + " " + _LastName;
            profileDto.contact.contact_type = (int)Enums.ContactType.Individual;
            profileDto.contact.dob = _dob;
            profileDto.contact.created_by = appOptions.UserID;
            profileDto.contact.created_on = DateTime.Today;
            //if (szDOB != string.Empty)
            //{
            //    eMember.DateOfBirth = DateTime.Parse(szDOB);
            //}
            //if (!Convert.IsDBNull(item["Gender"]) && item["Gender"].ToString() != string.Empty)
            //{
            //    var g = Pixel.Iris4.Settings.Options.Instance.ListSysTypes.FirstOrDefault(e1 => e1.TypeTable == "Gender" && e1.TypeDesc == item["Gender"].ToString().Trim().Replace("'", "''"));
            //    if (g != null) eMember.GenderID = g.TypeID;
            //}
            //if (!Convert.IsDBNull(item["MaritalStatus"]) && item["MaritalStatus"].ToString() != string.Empty)
            //{
            //    var m = Pixel.Iris4.Settings.Options.Instance.ListSysTypes.FirstOrDefault(e1 => e1.TypeTable == "MaritalStatus" && e1.TypeDesc == item["MaritalStatus"].ToString().Trim().Replace("'", "''"));
            //    if (m != null) eMember.GenderID = m.TypeID;
            //}
            //if (!Convert.IsDBNull(item["Profession"]) && item["Profession"].ToString() != string.Empty)
            //{
            //    var p = oContextProfile.PF_Professions.FirstOrDefault(e1 => e1.ProfessionDesc.ToUpper() == item["Profession"].ToString().Trim().Replace("'", "''"));
            //    if (p != null) eMember.ProfessionID  = p.ProfessionID;
            //}

            profileDto.profile.contact_id = profileDto.contact.id;
            profileDto.profile.profile_type_id = 2;
            profileDto.profile.is_accountable = 0;
            profileDto.profile.is_taxable = 0;
            profileDto.profile.source_channel = 163; // mobile app
            profileDto.profile.is_direct = 0;
            profileDto.profile.email1 = _Email;
            profileDto.profile.status_id = (int)Enums.ProfileStatus.Pending;
            profileDto.profile.created_by = appOptions.UserID;
            profileDto.profile.created_on = DateTime.Today;


            var profileRelation = new PF_PROFILE_RELATION();
            profileRelation.main_profile_id = appOptions.BrokerID;
            profileRelation.profile_id = profileDto.profile.id;
            profileRelation.relation_id = (int)Enums.SystemRelations.Broker;
            profileRelation.relation_date = DateTime.Today;
            profileRelation.is_inactive = 0;
            profileDto.profile_relation.Add(profileRelation);

            if (_MobileNo != "")
            {
                var ePhone = new PF_PROFILE_PHONE();
                ePhone.profile_id = profileDto.profile.id;
                ePhone.phone_type_id = (int)Enums.PhoneType.Mobile;
                ePhone.country_code = _MobileCountry;
                ePhone.area_code = _MobileArea;
                ePhone.local_number = _MobileNo;
                ePhone.is_default = 1;
                ePhone.is_sms_technical = 1;
                ePhone.created_by = appOptions.UserID;
                profileDto.profile_phone.Add(ePhone);
            }
             
            List<CORE_ADDRESS> list_fake_address = new List<CORE_ADDRESS>();

            try
            {
                ProfileHelper profileHelper = new ProfileHelper(null, null);
                var result = profileHelper.SaveProfile(profileDto, appOptions.CompanyID, 0, appOptions.UserID, list_fake_address, null, null, false, null);

                if (Pixel.Core.Lib.CoreHelper.IsNumeric(result))
                {
                    _InsuredID = Convert.ToInt32(result);
                }

                //if (_MobileNo != "" || _Email != "")
                //{
                //    var eAddress = new CORE_ADDRESS();
                //    eAddress.is_default = 1;
                //    eAddress.is_billing = 1;
                //    eAddress.profile_id = Convert.ToInt32(result);
                //    //eAddress.isCommunicatorEnabled = true;
                //    eAddress.created_by = _userId;
                //    var countryId = _dbConn.QueryFirstOrDefault<int>("SELECT ID FROM CORE_COUNTRY WHERE COUNTRY_CODE = '" + _CountryCode + "'");
                //    if (countryId > 0)
                //    { 
                //        eAddress.country_id = countryId;
                //    }
                //    _ContextProfileWrite.PF_Addresses.AddObject(eAddress);

                //    try
                //    {
                //        _ContextProfileWrite.SaveChanges();
                //        _AddressID = eAddress.id;
                //    }
                //    catch (Exception)
                //    {
                //        _AddressID = -1; 
                //    }
                //} 
            }
            catch (Exception)
            {
                _InsuredID = -1;
            }
        }

        //public void Commissions_PrepareCommissions(ref Pixel.Iris4.Models.Web.Iris4EntitiesUW _ContextUW,
        //                                          int _PolicyID,
        //                                          bool _ProductIsCommissionByCover)
        //{
        //    var ePolicy = _ContextUW.UW_POLICY.FirstOrDefault(w => w.PolicyID == _PolicyID);
        //    var ePolicyRoot = _ContextUW.UW_ROOT.FirstOrDefault(w => w.PolicyRootID == ePolicy.PolicyRootID);

        //    if (ePolicy == null || ePolicy.BrokerPROFILE_ID == 0) return;


        //    bool bIsRefund = ePolicy.TotalPremium < 0;


        //    CommissionService oCommissionService2 = new CommissionService();
        //    oCommissionService2.p_ContextUW = _ContextUW;
        //    oCommissionService2.p_PolicyID = _PolicyID;
        //    //12-09-2016
        //    //oCommissionService2.p_WcfOptions = m_WcfOptions;
        //    oCommissionService2.p_COMPANY_ID = m_COMPANY_ID;
        //    oCommissionService2.p_UsesSharedSettings = m_UsesSharedSettings;
        //    oCommissionService2.p_UwCalculateAccountExecutiveCommissionAtPolicyLevel =
        //        m_UwCalculateAccountExecutiveCommissionAtPolicyLevel;
        //    //12-09-2016

        //    //oCommissionService2.p_LstCurrencyRateLines

        //    oCommissionService2.CreateCommissions_Policy(m_ConnectionString,
        //                                                 ePolicy,
        //                                                 ePolicyRoot.LineID,
        //                                                 ePolicyRoot.isAcceptance,
        //                                                (Convert.IsDBNull(ePolicyRoot.OldRootID) ? Pixel.Iris4.Settings.Enums.CommissionType.NewBusiness : Pixel.Iris4.Settings.Enums.CommissionType.RenewedBusiness),
        //                                                 bIsRefund);

        //}

        //public void CreatePolicyComputation(
        //       ref Pixel.Iris4.Models.Web.Iris4EntitiesUW _ContextUW,
        //       int _PolicyID,
        //       int _ComputationID,
        //       decimal _NetPremium,
        //       decimal _TotalPremium,
        //       bool _isBasedOnNet,
        //       bool _bCreateZeroComputation,
        //       decimal _DiscountOnTotal,
        //       bool _UsesDetailedComputation,
        //       bool _isAcceptance,
        //       DateTime _ParentDate,
        //       bool _isUsesReinsuranceRates,
        //       UW_COB _SubLineInfo)
        //{
        //    List<Pixel.Iris4.Models.Web.UW_PolicyComputations> oListPolicyComputations = _ContextUW.UW_PolicyComputations.Where(w => w.PolicyID == _PolicyID).ToList();
        //    Pixel.Iris4.Models.Web.UW_POLICY ePolicy;

        //    ePolicy = _ContextUW.UW_POLICY.FirstOrDefault(w => w.PolicyID == _PolicyID);

        //    foreach (var itemPolicyComputation in oListPolicyComputations)
        //    {
        //        _ContextUW.UW_PolicyComputations.DeleteObject(itemPolicyComputation);
        //    }

        //    Pixel.Iris4.Models.Web.Services.Iris4DomainServiceUW oServiceUW = new Models.Web.Services.Iris4DomainServiceUW();

        //    var oListComputationRules = new List<Models.Web.ListComputationRules>();

        //    oListComputationRules = oServiceUW.fnGetComputationData(m_ConnectionString, _ComputationID).ToList();

        //    decimal nValue = 0M;

        //    Pixel.Iris4.Models.Web.UW_PolicyComputations ePolicyComputation;

        //    foreach (var itemComputationRule in oListComputationRules)
        //    {
        //        ePolicyComputation = new Pixel.Iris4.Models.Web.UW_PolicyComputations();

        //        ePolicyComputation.PolicyID = ePolicy.PolicyID;
        //        //ePolicyComputation.RuleSorting = itemComputationRule.RuleSorting;
        //        ePolicyComputation.ComputationRuleID = itemComputationRule.ComputationRuleID;
        //        ePolicyComputation.ComputationFractionID = itemComputationRule.ComputationFractionID;

        //        // CHANGED BY TONY ON 15-OCT-2014
        //        if (ePolicy.CONTRACT_TYPE_ID != (int)Pixel.Iris4.Settings.Enums.ContractType.Certificate ||
        //             (ePolicy.CONTRACT_TYPE_ID == (int)Pixel.Iris4.Settings.Enums.ContractType.Certificate &&
        //              !itemComputationRule.isAppliedOncePerGroup))
        //        //if (ePolicy.CONTRACT_TYPE_ID == (int)Pixel.Iris4.Settings.Enums.ContractType.Certificate ||
        //        //    ePolicy.CONTRACT_TYPE_ID == (int)Pixel.Iris4.Settings.Enums.ContractType.Shipment)
        //        //{
        //        //    ePolicyComputation.Amount = 0;
        //        //    ePolicyComputation.base_amt = 0;
        //        //}
        //        //else
        //        {
        //            if (Pixel.Silverlight.Libs.SimulateVB.IsNumeric(itemComputationRule.Formula))
        //            {
        //                if (Convert.IsDBNull(itemComputationRule.CurID))
        //                {
        //                    //// MessageBox.Show("Currency is not specified for component : " + oDsComputationRules.UW_ComputationRules.Rows(j)["ComputationRuleDesc"], string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                }
        //                else
        //                {
        //                    //if (itemComputationRule.isPercopy == true)
        //                    //{
        //                    //    nValue = Convert.ToDecimal(itemComputationRule.Formula) * 
        //                    //                                      oPolicy.PrintCopies;
        //                    //}
        //                    //else
        //                    //{
        //                    nValue = Convert.ToDecimal(itemComputationRule.Formula);
        //                    //}

        //                    if (Convert.ToInt32(itemComputationRule.CurID) == m_BaseCurID)
        //                    {
        //                        ePolicyComputation.base_amt = nValue;
        //                    }
        //                    else if (Convert.ToInt32(itemComputationRule.CurID) == m_CounterCurID)
        //                    {
        //                        ePolicyComputation.base_amt = Helpers.ConvertAmount(m_ConnectionString, nValue, Convert.ToInt32(itemComputationRule.CurID),
        //                                                                   Convert.ToDateTime(ePolicy.DateIssue), m_BaseCurID);
        //                    }
        //                    if (Convert.ToInt32(itemComputationRule.CurID) != ePolicy.PREMIUM_CUR_ID)
        //                    {
        //                        nValue = Helpers.ConvertAmount(m_ConnectionString, nValue, Convert.ToInt32(itemComputationRule.CurID),
        //                                                       Convert.ToDateTime(ePolicy.DateIssue), ePolicy.PREMIUM_CUR_ID);
        //                    }
        //                    ePolicyComputation.Amount = nValue;
        //                }
        //            }
        //            else
        //            {
        //                if (itemComputationRule.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net)
        //                {
        //                    ePolicyComputation.Amount = _NetPremium;
        //                }
        //                else if (itemComputationRule.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                {
        //                    ePolicyComputation.Amount = _TotalPremium;
        //                }
        //            }
        //        }

        //        _ContextUW.UW_PolicyComputations.AddObject(ePolicyComputation);
        //    }

        //    if (_UsesDetailedComputation)
        //    {
        //        var oQueryPolicyCoverComputations =
        //       (
        //           from a in _ContextUW.UW_PolicyCoverComputations
        //           join b in _ContextUW.UW_PolicyCovers on a.PolicyCoverID equals b.PolicyCoverID
        //           where b.PolicyID == _PolicyID
        //           select new { SysID = a.SysID }
        //        );

        //        Pixel.Iris4.Models.Web.UW_PolicyCoverComputations oPolicyCoverComputation;

        //        foreach (var itemComputationRule in oListComputationRules)
        //        {
        //            if ((itemComputationRule.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net) ||
        //                (itemComputationRule.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Charges) ||
        //                (itemComputationRule.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Cost))
        //            {
        //                if (itemComputationRule.isAppliedOncePerGroup == true)
        //                {
        //                    foreach (var itemPolicyCoverComputation in oQueryPolicyCoverComputations.ToList())
        //                    {
        //                        oPolicyCoverComputation = _ContextUW.UW_PolicyCoverComputations.FirstOrDefault(w => w.SysID == itemPolicyCoverComputation.SysID);
        //                        if (oPolicyCoverComputation.EntityState != EntityState.Deleted)
        //                        {
        //                            if (oPolicyCoverComputation.ComputationFractionID == itemComputationRule.ComputationFractionID)
        //                            {
        //                                oPolicyCoverComputation.Amount = 0;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    var AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();

        //    if (_bCreateZeroComputation)
        //    {
        //        foreach (var itemPolicyComputation in AddedPolicyComputations)
        //        {
        //            itemPolicyComputation.Amount = 0;
        //        }
        //    }
        //    else
        //    {
        //        ComputationService oComputationService = new ComputationService();
        //        oComputationService.p_ContextUW = _ContextUW;
        //        oComputationService.p_PolicyID = _PolicyID;
        //        oComputationService.p_BaseCurID = m_BaseCurID;
        //        oComputationService.p_CounterCurID = m_CounterCurID;

        //        oComputationService.p_ListComputationRules = oListComputationRules;

        //        #region Detailed Computation

        //        if (_UsesDetailedComputation)
        //        {
        //            decimal nAmt = 0M;

        //            var oQueryPolicyCoverComputations =
        //            (
        //                from a in _ContextUW.UW_PolicyCoverComputations
        //                join b in _ContextUW.UW_PolicyCovers on a.PolicyCoverID equals b.PolicyCoverID
        //                where b.PolicyID == _PolicyID
        //                select new
        //                {
        //                    SysID = a.SysID,
        //                    ComputationFractionID = a.ComputationFractionID,
        //                    Amount = a.Amount
        //                }
        //            );

        //            foreach (var itemPolicyComputation in AddedPolicyComputations)
        //            {
        //                if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net ||
        //                    itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.ServicePremium ||
        //                    itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Charges)
        //                {
        //                    nAmt = 0M;
        //                    //todo tony remove the rem
        //                    if (oQueryPolicyCoverComputations != null)
        //                    {
        //                        var lstPolicyCoverComputations = oQueryPolicyCoverComputations
        //                            .Where(w => w.ComputationFractionID == itemPolicyComputation.ComputationFractionID).ToList();

        //                        if (lstPolicyCoverComputations != null)
        //                        {
        //                            foreach (var itemPolicyCoverComputation in lstPolicyCoverComputations)
        //                            {
        //                                nAmt += itemPolicyCoverComputation.Amount;
        //                            }
        //                        }
        //                    }

        //                    itemPolicyComputation.Amount = nAmt;
        //                    if (ePolicy.PREMIUM_CUR_ID == m_BaseCurID)
        //                    {
        //                        itemPolicyComputation.base_amt = nAmt;
        //                    }
        //                    else if (ePolicy.PREMIUM_CUR_ID == m_CounterCurID)
        //                    {
        //                        itemPolicyComputation.base_amt = Helpers.ConvertAmount(m_ConnectionString, nAmt,
        //                                                                                 ePolicy.PREMIUM_CUR_ID,
        //                                                                                 ePolicy.DateIssue,
        //                                                                                 m_BaseCurID);
        //                    }

        //                }
        //            }

        //            if (_ParentDate != null)
        //            {
        //                oComputationService.p_CurrentDate = Convert.ToDateTime(ePolicy.DateIssue);
        //                oComputationService.p_ParentDate = _ParentDate;
        //            }

        //            //TODO:JACKY 29/06/2012
        //            oComputationService.PerformCalculationExceptNetCharges(ePolicy.PREMIUM_CUR_ID, _isAcceptance);
        //            //END TODO:JACKY 29/06/2012

        //            AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();

        //            //AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(
        //            //                                            e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>().Where(
        //            //                                                w => w.EntityState != EntityState.Detached);

        //            //Added by tony on 02-06-08 to ensure rounding of total
        //            decimal Amount = 0;

        //            foreach (var itemPolicyComputation in AddedPolicyComputations)
        //            {
        //                if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                {
        //                    Amount = Convert.ToInt32(itemPolicyComputation.Amount);
        //                    RoundTotalPremium(ref Amount, _SubLineInfo);
        //                    itemPolicyComputation.Amount = Amount;

        //                    oComputationService.PremiumBalancer(itemPolicyComputation.Amount, ePolicy.PREMIUM_CUR_ID, _isAcceptance,
        //                                                        false,
        //                                                        ePolicy.CONTRACT_TYPE_ID == 2);

        //                    AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();
        //                    //AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(
        //                    //                                           e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>().Where(
        //                    //                                               w => w.EntityState != EntityState.Detached);
        //                }
        //            }
        //        }

        //        #endregion Detailed Computation

        //        #region regular computation

        //        else
        //        {
        //            //uses regular (non-detailed) computation
        //            if (_isBasedOnNet)
        //            {
        //                foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                {
        //                    if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net)
        //                    {
        //                        itemPolicyComputation.Amount = _NetPremium;
        //                        break;
        //                    }
        //                }

        //                if (m_ProductInfo != null && (m_ProductInfo.isFixedTotal || m_ProductInfo.isNet == false))
        //                {
        //                    oComputationService.PerformCalculationOnAllRowsOfTheGrid(-1, -1,
        //                                                                             ePolicy.PREMIUM_CUR_ID,
        //                                                                             _isAcceptance,
        //                                                                             true,
        //                                                                             ePolicy.CONTRACT_TYPE_ID == 2);
        //                }
        //                else
        //                {
        //                    oComputationService.PerformCalculationOnAllRowsOfTheGrid(-1, -1,
        //                                                                             ePolicy.PREMIUM_CUR_ID,
        //                                                                             _isAcceptance,
        //                                                                             false,
        //                                                                             ePolicy.CONTRACT_TYPE_ID == 2);
        //                }

        //                AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();

        //                if (_DiscountOnTotal != 0M)
        //                {
        //                    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                    {
        //                        if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net)
        //                        {
        //                            itemPolicyComputation.Amount = 0;

        //                            oComputationService.PerformCalculationOnAllRowsOfTheGrid(-1, -1, ePolicy.PREMIUM_CUR_ID, _isAcceptance, false,
        //                                                                                     ePolicy.CONTRACT_TYPE_ID == 2);
        //                            AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();
        //                            break;
        //                        }
        //                    }

        //                    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                    {
        //                        if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                        {
        //                            itemPolicyComputation.Amount = Convert.ToDecimal(itemPolicyComputation.Amount) + Convert.ToDecimal(itemPolicyComputation.Amount) * _DiscountOnTotal / 100; ;

        //                            oComputationService.ComputeNetPremium(ePolicy.PREMIUM_CUR_ID, _TotalPremium, _isAcceptance);

        //                            AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();

        //                            oComputationService.PremiumBalancer(_TotalPremium, ePolicy.PREMIUM_CUR_ID, _isAcceptance,
        //                                                                false,
        //                                                                ePolicy.CONTRACT_TYPE_ID == 2);

        //                            AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();
        //                            break;
        //                        }
        //                    }
        //                }

        //                //Rounding the total amount
        //                if (_TotalPremium != 0M)
        //                {
        //                    decimal Amount = 0;

        //                    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                    {
        //                        if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                        {
        //                            Amount = Convert.ToDecimal(itemPolicyComputation.Amount);
        //                            RoundTotalPremium(ref Amount, _SubLineInfo);

        //                            itemPolicyComputation.Amount = Amount;
        //                            oComputationService.PremiumBalancer(itemPolicyComputation.Amount, ePolicy.PREMIUM_CUR_ID, _isAcceptance,
        //                                                                false,
        //                                                                ePolicy.CONTRACT_TYPE_ID == 2);

        //                            AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //based on total
        //                if (_DiscountOnTotal != 0M)
        //                {
        //                    _TotalPremium = _TotalPremium + (_DiscountOnTotal * _TotalPremium / 100);
        //                }

        //                foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                {
        //                    if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                    {
        //                        itemPolicyComputation.Amount = _TotalPremium;
        //                        break;
        //                    }
        //                }

        //                if (_ParentDate != null)
        //                {
        //                    oComputationService.p_CurrentDate = Convert.ToDateTime(ePolicy.DateIssue);
        //                    oComputationService.p_ParentDate = _ParentDate;
        //                }

        //                if (_isUsesReinsuranceRates)// && m_WcfOptions.BusinessType != (int)Pixel.Iris4.Settings.Enums.BusinessType.Brokerage)
        //                {
        //                    decimal nNet = 0M;

        //                    foreach (var itemPolicyCover in _ContextUW.UW_PolicyCovers.Where(w => w.PolicyID == _PolicyID).ToList())
        //                    {
        //                        nNet += itemPolicyCover.NetPremium;
        //                    }

        //                    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                    {
        //                        if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net)
        //                        {
        //                            itemPolicyComputation.Amount = nNet;
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    oComputationService.ComputeNetPremium(ePolicy.PREMIUM_CUR_ID, _TotalPremium, _isAcceptance);
        //                    AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();
        //                }

        //                oComputationService.PremiumBalancer(_TotalPremium,
        //                                                    ePolicy.PREMIUM_CUR_ID,
        //                                                    _isAcceptance,
        //                                                    false,
        //                                                    ePolicy.CONTRACT_TYPE_ID == 2);

        //                AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();
        //            }
        //        }

        //        #endregion regular computation

        //    }

        //    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //    {
        //        //Base and Counter Amounts
        //        if (ePolicy.PREMIUM_CUR_ID == m_BaseCurID)
        //        {
        //            if (itemPolicyComputation.base_amt == 0)
        //            {
        //                itemPolicyComputation.base_amt = itemPolicyComputation.Amount;
        //            }
        //        }
        //        else if (ePolicy.PREMIUM_CUR_ID == m_CounterCurID)
        //        {
        //            if (itemPolicyComputation.base_amt == 0)
        //            {
        //                itemPolicyComputation.base_amt = Math.Round(itemPolicyComputation.Amount * ePolicy.BaseCurRate, 2);
        //            }
        //        }
        //        else
        //        {
        //            if (itemPolicyComputation.base_amt == 0)
        //            {
        //                itemPolicyComputation.base_amt = Math.Round(itemPolicyComputation.Amount * ePolicy.BaseCurRate, 2);
        //            }
        //        }

        //        if (itemPolicyComputation.UW_ComputationsFractions.BaseCurRoundedTo != 0)
        //        {
        //            itemPolicyComputation.base_amt = Math.Ceiling(itemPolicyComputation.base_amt / Convert.ToInt32(itemPolicyComputation.UW_ComputationsFractions.BaseCurRoundedTo)) *
        //                                            Convert.ToInt32(itemPolicyComputation.UW_ComputationsFractions.BaseCurRoundedTo);

        //        }

        //    }

        //    decimal nbase_amt = 0M;
        //    decimal nBaseDiscountAmount = 0M;

        //    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //    {
        //        if (itemPolicyComputation.EntityState != EntityState.Deleted &&
        //            !(Convert.IsDBNull(itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID)))
        //        {
        //            switch (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID)
        //            {
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Surplus:
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Taxes:
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.ServicePremium:
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Stamps:
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Charges:
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net:
        //                    nbase_amt += itemPolicyComputation.base_amt;
        //                    break;
        //                case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Discount:
        //                    nBaseDiscountAmount += itemPolicyComputation.base_amt;
        //                    break;
        //            }
        //        }
        //    }

        //    foreach (var itemPolicyComputation in AddedPolicyComputations)
        //    {
        //        if (itemPolicyComputation.EntityState != EntityState.Deleted &&
        //            !(Convert.IsDBNull(itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID)))
        //        {
        //            if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //            {
        //                itemPolicyComputation.base_amt = nbase_amt;
        //            }
        //            else if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.GrandTotal)
        //            {
        //                itemPolicyComputation.base_amt = nbase_amt - nBaseDiscountAmount;
        //            }
        //        }
        //    }
        //}

        //private void ProcessComputation(Pixel.Iris4.Models.Web.UW_ROOT _ePolicyRoot,
        //                          Pixel.Iris4.Models.Web.UW_POLICY _ePolicy,
        //                          List<Pixel.Iris4.Models.Web.UW_PolicyComputations> _PolicyComputations,
        //                          List<Pixel.Iris4.Models.Web.UW_PolicyCoverComputations> _PolicyCoverComputations,
        //                          List<Pixel.Iris4.Models.Web.UW_PolicyCovers> _PolicyCovers,
        //                          ref Pixel.Iris4.Models.Web.Iris4EntitiesUW _ContextUW,
        //                          Pixel.Iris4.Models.Structures.ProductInfo _ProductInfo,
        //                          Pixel.Iris4.Models.Structures.SubLineInfo _SubLineInfo,
        //                          List<Pixel.Iris4.Models.Web.ListComputationRules> _ListComputationRules,
        //                          List<Pixel.Iris4.Models.Web.UW_ComputationRuleValues> _ComputationRulesValues)
        //{
        //    var AddedPolicyComputations = _ContextUW.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added).Select(e => e.Entity).OfType<Pixel.Iris4.Models.Web.UW_PolicyComputations>();

        //    if (_ePolicyRoot.isGroupPolicy)
        //    {
        //        // nothing to do
        //    }
        //    else
        //    {
        //        ComputationService oComputationService = new ComputationService();
        //        oComputationService.p_ContextUW = _ContextUW;
        //        oComputationService.p_PolicyID = _ePolicy.PolicyID;
        //        oComputationService.p_BaseCurID = m_BaseCurID;
        //        oComputationService.p_CounterCurID = m_CounterCurID;
        //        //oComputationService.p_WcfOptions = m_WcfOptions;

        //        oComputationService.p_ListComputationRules = _ListComputationRules;

        //        #region isDetailedComputation

        //        if (_ProductInfo.isDetailedComputation)
        //        {
        //            //Product uses Detailed Computations

        //            foreach (var itemPolicyComputation in AddedPolicyComputations)
        //            {
        //                if ((itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net) ||
        //                    (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.ServicePremium) ||
        //                    (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Charges))
        //                {
        //                    decimal nAmt = 0M;
        //                    foreach (var itemPolicyCoverComputation in _PolicyCoverComputations)
        //                    {
        //                        if (itemPolicyCoverComputation.ComputationFractionID == itemPolicyComputation.ComputationFractionID)
        //                        {
        //                            nAmt += itemPolicyCoverComputation.Amount;
        //                        }
        //                    }

        //                    itemPolicyComputation.Amount = nAmt;
        //                }
        //            }

        //            oComputationService.PerformCalculationExceptNetCharges(_ePolicy.PREMIUM_CUR_ID, _ePolicyRoot.isAcceptance);

        //            decimal Amount = 0;

        //            foreach (var itemPolicyComputation in AddedPolicyComputations)
        //            {
        //                if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                {
        //                    RoundTotalPremium(ref Amount, _SubLineInfo);

        //                    itemPolicyComputation.Amount = Amount;

        //                    oComputationService.PremiumBalancer(itemPolicyComputation.Amount, _ePolicy.PREMIUM_CUR_ID, _ePolicyRoot.isAcceptance,
        //                                                        false,
        //                                                        _ePolicy.CONTRACT_TYPE_ID == 2);
        //                }
        //            }
        //        }

        //        #endregion Detailed Computation

        //        #region regular computation

        //        else // regular computation
        //        {
        //            if (_ePolicy.isCoverRateOnNet)
        //            {
        //                //regular computation on Net
        //                decimal nNet = 0M;
        //                decimal nServiceNet = 0M;
        //                for (var i = 0; i < _PolicyCovers.Count; i++)
        //                {
        //                    if (Convert.ToBoolean(_PolicyCovers.ElementAt(i).isService))
        //                    {
        //                        nServiceNet += _PolicyCovers.ElementAt(i).NetPremium;
        //                    }
        //                    else
        //                    {
        //                        nNet += _PolicyCovers.ElementAt(i).NetPremium;
        //                    }
        //                }

        //                foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                {
        //                    switch (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID)
        //                    {
        //                        case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net:
        //                            itemPolicyComputation.Amount = nNet;
        //                            break;
        //                        case (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.ServicePremium:
        //                            itemPolicyComputation.Amount = nServiceNet;
        //                            break;
        //                    }
        //                }

        //                //12-09-2016 this.UpdateComponentsInGrid(false, ref _ContextUW, _ListComputationRules, _ComputationRulesValues, _ePolicy);

        //                oComputationService.PerformCalculationOnAllRowsOfTheGrid(-1, -1, _ePolicy.PREMIUM_CUR_ID,
        //                                                                         _ePolicyRoot.isAcceptance, false,
        //                                                                         _ePolicy.CONTRACT_TYPE_ID == 2);

        //                // This to ensure total premium will be rounded
        //                decimal Amount = 0;
        //                foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                {
        //                    if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                    {
        //                        RoundTotalPremium(ref Amount, _SubLineInfo);

        //                        itemPolicyComputation.Amount = Amount;

        //                        oComputationService.PremiumBalancer(itemPolicyComputation.Amount, _ePolicy.PREMIUM_CUR_ID, _ePolicyRoot.isAcceptance,
        //                                                            false,
        //                                                            _ePolicy.CONTRACT_TYPE_ID == 2);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //regular computation on total
        //                decimal nTotal = 0M;
        //                decimal nNetPremium = 0M;
        //                for (var i = 0; i < _PolicyCovers.Count; i++)
        //                {
        //                    nTotal += _PolicyCovers.ElementAt(i).TotalPremium;
        //                    nNetPremium += _PolicyCovers.ElementAt(i).NetPremium;
        //                }

        //                foreach (var itemPolicyComputation in AddedPolicyComputations)
        //                {
        //                    if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Total)
        //                    {
        //                        itemPolicyComputation.Amount = nTotal;
        //                    }
        //                    if (itemPolicyComputation.UW_ComputationsFractions.ComputationGroupID == (int)Pixel.Iris4.Settings.Enums.ComputationsGroups.Net)
        //                    {
        //                        if (_SubLineInfo.RuleUsesReinsuranceRatesAtPolicyCover)
        //                        {
        //                            itemPolicyComputation.Amount = nNetPremium;
        //                        }
        //                    }
        //                }

        //                // as testing: if CoverRates are on TOTAL to allow the system set the correct conditional cost 
        //                //12-09-2016 this.UpdateComponentsInGrid(false, ref _ContextUW, _ListComputationRules, _ComputationRulesValues, _ePolicy);

        //                //rounding of total premium

        //                RoundTotalPremium(ref nTotal, _SubLineInfo);

        //                if (_SubLineInfo.RuleUsesReinsuranceRatesAtPolicyCover)
        //                {
        //                    oComputationService.PremiumBalancer(nTotal, _ePolicy.PREMIUM_CUR_ID, _ePolicyRoot.isAcceptance,
        //                                                        false,
        //                                                        _ePolicy.CONTRACT_TYPE_ID == 2);
        //                }
        //                else
        //                {
        //                    oComputationService.ComputeNetPremium(_ePolicy.PREMIUM_CUR_ID, nTotal, _ePolicyRoot.isAcceptance);

        //                    oComputationService.PremiumBalancer(nTotal, _ePolicy.PREMIUM_CUR_ID, _ePolicyRoot.isAcceptance,
        //                                                        false,
        //                                                        _ePolicy.CONTRACT_TYPE_ID == 2);
        //                }
        //            }
        //        }

        //        #endregion regular computation

        //    }
        //}
         
        public decimal GetPolicyUnpaidBalance(int _rootId, int _closeOnNet)
        {
            decimal balance = 0M;
            try
            {
                using (var dbConn = Pixel.Core.Dapper.My.ConnectionFactory())
                {
                    dbConn.ConnectionString = appOptions.ConnectionString;
                    dbConn.Open();

                    var strQuery =
                        @"SELECT	
                                SUM(CASE WHEN DB_CR ='D' THEN abs(RP_MVMT.AMOUNT) ELSE 0 END) AS DEBIT ,
			                    SUM(CASE WHEN DB_CR ='C' THEN abs(RP_MVMT.AMOUNT) ELSE 0 END) AS CREDIT 
	                    FROM RP_MVMT
	                    LEFT JOIN UW_POLICY ON RP_MVMT.LINK_ID = UW_POLICY.ID
                        WHERE LINK_TYPE_ID = 1  
		                    AND UW_POLICY.ROOT_ID = @ROOT_ID
	                    GROUP BY UW_POLICY.ROOT_ID

	                    --COMMISSION
	                    UNION ALL 
	                    SELECT	SUM(CASE WHEN DB_CR ='D' THEN abs(RP_MVMT.AMOUNT) ELSE 0 END) AS DEBIT ,
			                    SUM(CASE WHEN DB_CR ='C' THEN abs(RP_MVMT.AMOUNT) ELSE 0 END) AS CREDIT 
	                    FROM RP_MVMT
                        LEFT JOIN UW_POLICY_COMMISSION ON RP_MVMT.LINK_ID = UW_POLICY_COMMISSION.ID
                        LEFT JOIN UW_POLICY ON UW_POLICY_COMMISSION.POLICY_ID = UW_POLICY.ID
                        WHERE (@CLOSE_ON_NET = 1 AND (LINK_TYPE_ID = 8 OR LINK_TYPE_ID = 10))
		                    AND UW_POLICY.ROOT_ID = @ROOT_ID
	                    GROUP BY UW_POLICY.ROOT_ID

	                    -- PAID PREMIUM
	                    UNION ALL
	                    SELECT SUM(CASE WHEN GL_TRS_ALLOCAT.DB_CR = 'D' THEN GL_TRS_ALLOCAT.PAID_AMT ELSE 0 END) AS DEBIT, 
		                       SUM(CASE WHEN GL_TRS_ALLOCAT.DB_CR = 'C' THEN GL_TRS_ALLOCAT.PAID_AMT ELSE 0 END) AS CREDIT 
                        FROM GL_TRS_ALLOCAT  
		                    LEFT JOIN GL_TRS ON GL_TRS_ALLOCAT.TRS_ID = GL_TRS.ID
		                    LEFT JOIN GL_JOURNAL_TYPE ON GL_TRS.JOURNAL_TYPE_ID = GL_JOURNAL_TYPE.ID
		                    LEFT JOIN RP_MVMT ON GL_TRS_ALLOCAT.MVMT_ID = RP_MVMT.ID  
		                    LEFT JOIN UW_POLICY ON RP_MVMT.LINK_ID = UW_POLICY.ID
                        WHERE LINK_TYPE_ID = 1 
		                    AND UW_POLICY.ROOT_ID = @ROOT_ID
	                    GROUP BY UW_POLICY.ROOT_ID
	
	                    -- PAID PREMIUM
	                    UNION ALL
	                    SELECT SUM(CASE WHEN GL_TRS_ALLOCAT.DB_CR = 'D' THEN GL_TRS_ALLOCAT.PAID_AMT ELSE 0 END) AS DEBIT, 
		                       SUM(CASE WHEN GL_TRS_ALLOCAT.DB_CR = 'C' THEN GL_TRS_ALLOCAT.PAID_AMT ELSE 0 END) AS CREDIT 
                        FROM GL_TRS_ALLOCAT  
		                    LEFT JOIN GL_TRS ON GL_TRS_ALLOCAT.TRS_ID = GL_TRS.ID
		                    LEFT JOIN GL_JOURNAL_TYPE ON GL_TRS.JOURNAL_TYPE_ID = GL_JOURNAL_TYPE.ID
		                    LEFT JOIN RP_MVMT ON GL_TRS_ALLOCAT.MVMT_ID = RP_MVMT.ID  
	                        LEFT JOIN UW_POLICY_COMMISSION ON RP_MVMT.LINK_ID = UW_POLICY_COMMISSION.ID
		                    LEFT JOIN UW_POLICY ON UW_POLICY_COMMISSION.POLICY_ID = UW_POLICY.ID
                        WHERE (@CLOSE_ON_NET = 1 AND (LINK_TYPE_ID = 1 OR LINK_TYPE_ID = 8 OR LINK_TYPE_ID = 10)) 
		                    AND UW_POLICY.ROOT_ID = @ROOT_ID
	                    GROUP BY UW_POLICY.ROOT_ID";

                    var result = dbConn.Query<DebitCreditDto>(strQuery,
                        new
                        {
                            ROOT_ID = _rootId,
                            CLOSE_ON_NET = _closeOnNet
                        }).ToList();

                    foreach (var item in result)
                    {
                        balance += (item.debit - item.credit);
                    }
                }

                return balance;
            }
            catch (Exception)
            {
                return balance;
            }
        }

    }

}