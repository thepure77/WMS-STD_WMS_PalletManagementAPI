using System;
using System.Collections.Generic;
using System.Text;

namespace PalletmanageBusiness.Reports
{
    public class ReportExportLentPalletViewModel
    {
        public string lentpallet_Id { get; set; }

        public string documentType_Name { get; set; }

        public string vender_Id { get; set; }

        public string vender_Name { get; set; }

        public DateTime? lentpalletDate { get; set; }

        public string lentpalletDate_text { get; set; }

        public string ref_No2 { get; set; }       

        public int lentPallet_QtyLent { get; set; }

        public int lentPallet_QtyReturnGood { get; set; }

        public int lentPallet_QtyReturnDmg { get; set; }

        public int lentPallet_QtyTotal { get; set; }

        public string vehicleType_Name { get; set; }

        public string lentpallet_license { get; set; }

        public string ref_No1 { get; set; }

        public string status_Name { get; set; }

        public string create_By { get; set; }

        public DateTime? createDate { get; set; }

        public string createDate_text { get; set; }

        public string update_By { get; set; }

        public DateTime? update_Date { get; set; }

        public string updateDate_text { get; set; }

        public string document_status { get; set; }

        public string report_date_to { get; set; }

        public string report_date { get; set; }

        public string cancel_By { get; set; }

        public DateTime? cancel_Date { get; set; }

        public string cancelDate_text { get; set; }
    }
}