using System;
using System.Collections.Generic;
using System.Text;

namespace PalletmanageBusiness.Reports
{
    public class ReportPrintPalletViewModel
    {
        public string company_name { get; set; }
        public string docket_no { get; set; }
        public string truck_registration { get; set; }
        public string truck_type { get; set; }
        public string truck_4w { get; set; }
        public string truck_6w { get; set; }
        public string truck_10w { get; set; }
        public string truck_other { get; set; }
        public string remark { get; set; }
        public int return_qtygood { get; set; }
        public int return_qtydmg { get; set; }
        public int return_qtytotal { get; set; }
        public int return_qty { get; set; }
        public DateTime? pallet_Date { get; set; }
        public string  palletDate_text { get; set; }
        public string pallet_year { get; set; }
    }


}
