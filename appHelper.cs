using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Pixel.Core.Lib;
using static Pixel.IRIS5.Shared.Structures;
using Pixel.IRIS5.Models;
using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Pixel.IRIS5.API.Mobile
{
    public static class appHelper
    {
        public static void SetAppOptions(IConfiguration _configuration)
        {
            appOptions.Database = _configuration["ConnectionStrings:Database"];
            appOptions.ConnectionString = _configuration["ConnectionStrings:ConnectionSQL"];
            //if (usedDatabase == "SQL")
            //{
            //    GlobalVariables.ConnectionString = _configuration["ConnectionStrings:connectionSQL"];
            //}
            //else if (usedDatabase == "ORCL")
            //{
            //    GlobalVariables.ConnectionString = _configuration["ConnectionStrings:connectionORCL"];
            //}
            appOptions.UsesSharedSettings = Convert.ToBoolean(_configuration["Options:UsesSharedSettings"]);
            appOptions.UwCalculateAccountExecutiveCommissionAtPolicyLevel = Convert.ToBoolean(_configuration["Options:UwCalculateAccountExecutiveCommissionAtPolicyLevel"]);
            appOptions.CompanyID = Convert.ToInt32(_configuration["Options:CompanyID"]);
            appOptions.BranchID = Convert.ToInt32(_configuration["Options:BranchID"]);
            appOptions.BaseCurID = Convert.ToInt32(_configuration["Options:BaseCurID"]);
            appOptions.CounterCurID = Convert.ToInt32(_configuration["Options:CounterCurID"]);
            appOptions.UserID = Convert.ToInt32(_configuration["Options:UserID"]);
            appOptions.BrokerID = Convert.ToInt32(_configuration["Options:BrokerID"]);
            appOptions.BrokerAccountID = Convert.ToInt32(_configuration["Options:BrokerAccountID"]);
            appOptions.Receipt_DefaultModeOfPayment = Convert.ToInt32(_configuration["Options:Receipt_DefaultModeOfPayment"]);
            appOptions.Receipt_DB_AccountID = Convert.ToInt32(_configuration["Options:Receipt_DB_AccountID"]);
            appOptions.Receipt_JournalTypeID = Convert.ToInt32(_configuration["Options:Receipt_JournalTypeID"]);

            appOptions.BrokingCompanyID = Convert.ToInt32(_configuration["Options:BrokingCompanyID"]);
            appOptions.BrokingReceipt_DB_AccountID = Convert.ToInt32(_configuration["Options:BrokingReceipt_DB_AccountID"]);
            appOptions.BrokingReceipt_DefaultModeOfPayment = Convert.ToInt32(_configuration["Options:BrokingReceipt_DefaultModeOfPayment"]);
            appOptions.BrokingReceipt_DefaultModeOfPayment_Life = Convert.ToInt32(_configuration["Options:BrokingReceipt_DefaultModeOfPayment_Life"]);

            appOptions.LifeCompanyID = Convert.ToInt32(_configuration["Options:LifeCompanyID"]);
            appOptions.LifeReceipt_DB_AccountID = Convert.ToInt32(_configuration["Options:LifeReceipt_DB_AccountID"]);
            appOptions.LifeReceipt_DefaultModeOfPayment = Convert.ToInt32(_configuration["Options:LifeReceipt_DefaultModeOfPayment"]);

            appOptions.layoutUrl = Convert.ToString(_configuration["LayoutUrl"]);

            appOptions.IssuigDateBasis = Convert.ToInt32(_configuration["Options:IssuigDateBasis"]);

        }
    }
}
