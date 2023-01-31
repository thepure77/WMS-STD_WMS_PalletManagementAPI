using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Business.Models;
using MasterDataBusiness.ViewModels;

namespace PalletmanageBusiness.BusinessUnit
{
    public class LentPalletViewModel
    {   
        public Guid lentPallet_Index { get; set; }

        public string lentPallet_Id { get; set; }

        public string lentPallet_Date { get; set; }

        public string lentpalletDate_text { get; set; }

        public string lentpallettime_text { get; set; }

        public Guid documentType_Index { get; set; }

        public string documentType_Id { get; set; }

        public string documentType_Name { get; set; }

        public int document_status { get; set; }

        public Guid vender_Index { get; set; }

        public string vender_Id { get; set; }

        public string vender_Name { get; set; }

        public Guid vehicle_Index { get; set; }

        public string vehicleType_Id { get; set; }

        public string vehicleType_Name { get; set; }

        public string lentPallet_license { get; set; }

        public int lentPallet_QtyLent { get; set; }

        public int lentPallet_QtyReturnGood { get; set; }

        public int lentPallet_QtyReturnDmg { get; set; }

        public string ref_No1 { get; set; }

        public string ref_No2 { get; set; }

        public string ref_No3 { get; set; }

        public string ref_No4 { get; set; }

        public string ref_No5 { get; set; }

        public string udf_1 { get; set; }

        public string udf_2 { get; set; }

        public string udf_3 { get; set; }

        public string udf_4 { get; set; }

        public string udf_5 { get; set; }

        public bool isActive { get; set; }

        public bool isDelete { get; set; }

        public bool isSystem { get; set; }

        public int? status_Id { get; set; }

        public string createDate { get; set; }

        public string createDate_text { get; set; }

        public string createTime_text { get; set; }

        public string create_By { get; set; }

        public DateTime? update_Date { get; set; }

        public string updateDate_text { get; set; }

        public string update_By { get; set; }

        public DateTime? cancel_Date { get; set; }

        public string cancelDate_text { get; set; }

        public string cancel_By { get; set; }

        public ItemListViewModel vender_auto { get; set; }
        public VehicleTypeViewModel vehicleType_dropdown { get; set; }
        public DocumentTypeViewModel documentType_dropdown { get; set; }
        public List<GenDocumentTypeViewModel> GenDocument { get; set; }
    }
}
