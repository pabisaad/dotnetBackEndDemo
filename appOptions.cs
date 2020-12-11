namespace Pixel.IRIS5.API.Mobile
{
    public static class appOptions
    {
        public static string Database { get; set; }
        public static string ConnectionString { get; set; }

        public static int CompanyID { get; set; }
        public static int UserID { get; set; }
        public static int BranchID { get; set; }
        public static int AccExecutiveID { get; set; }
        public static int BaseCurID { get; set; }
        public static int CounterCurID { get; set; }
        public static int BrokerID { get; set; }
        public static int BrokerAccountID { get; set; }
        public static int BrokingCompanyID { get; set; }
        public static int BrokingReceipt_DB_AccountID { get; set; }
        public static int BrokingReceipt_DefaultModeOfPayment { get; set; }
        public static int BrokingReceipt_DefaultModeOfPayment_Life { get; set; }

        public static int LifeCompanyID { get; set; }
        public static int LifeReceipt_DB_AccountID { get; set; }
        public static int LifeReceipt_DefaultModeOfPayment { get; set; }

        public static int Receipt_DB_AccountID { get; set; }
        public static int Receipt_DefaultModeOfPayment { get; set; }
        public static int Receipt_JournalTypeID { get; set; }

        public static bool UsesSharedSettings { get; set; }
        public static bool UwCalculateAccountExecutiveCommissionAtPolicyLevel { get; set; }

        public static string layoutUrl { get; set; }

        public static int IssuigDateBasis { get; set; }

    }
}
