using MasterDataBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Business.Models
{
    public class OnhandSummaryViewModel
    {
        public string start_Date { get; set; }
        public string end_Date { get; set; }
        public string types { get; set; }
        public int? printExcel { get; set; }
        public List<ItemListViewModel> listVendor { get; set; }
    }

    public class ResultOnhandSummaryViewModel
    {
        public ResultDataViewModel pttor_data { get; set; }
        public ResultDataViewModel supplier_data { get; set; }
        public int? sumdmg_or { get; set; }
        public int? sumdmg_supplier { get; set; }
    }
    public class ResultDataViewModel
    {
        public List<string> header { get; set; }
        public DataTable detail { get; set; }
    }
}
