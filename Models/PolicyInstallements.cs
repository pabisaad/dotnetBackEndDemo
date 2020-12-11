using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pixel.IRIS5.API.Mobile.Models
{
    public class PolicyInstallements
    {
        public int ID { get; set; }
        public int LINKED_TO_INST_ID { get; set; }
        public int MVMT_ID { get; set; }
        public string INST_NO { get; set; }
        public DateTime DUE_DATE { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal VAT_AMT { get; set; }
        public decimal PAID_AMT { get; set; }
        public decimal INTERESET_AMT { get; set; }
        public decimal PDC_AMT { get; set; }
        public int GRACE_PERIOD { get; set; }
        public int IS_POSTED { get; set; }

    }
}
