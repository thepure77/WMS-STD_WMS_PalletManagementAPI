using Business.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MasterDataBusiness.ViewModels
{
    public class PalletFilterViewModel : Pagination
    {
        [Key]
        public Guid? vender_Index { get; set; }

        public string start_Date { get; set; }

        public string end_Date { get; set; }

        public string document_No { get; set; }
        public string pallet_Id { get; set; }


        public bool is_sum { get; set; }
        public List<statusViewModel> status { get; set; }

        public class statusViewModel
        {
            public int value { get; set; }
            public string display { get; set; }
            public int seq { get; set; }
        }

    }
}
