using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Outbound.Table
{
    public partial class im_LentPallet_loc
    {
        [Key]
        public Guid LentPallet_Index { get; set; }

        [StringLength(50)]
        public string LentPallet_Id { get; set; }

        public DateTime LentPallet_Date { get; set; }

        public Guid Vender_Index { get; set; }

        [StringLength(50)]
        public string Vender_Id { get; set; }

        [StringLength(200)]
        public string Vender_Name { get; set; }

        public Guid DocumentType_Index { get; set; }

        [StringLength(50)]
        public string DocumentType_Id { get; set; }

        [StringLength(200)]
        public string DocumentType_Name { get; set; }

        public Guid Vehicle_Index { get; set; }

        [StringLength(50)]
        public string VehicleType_Id { get; set; }

        [StringLength(200)]
        public string VehicleType_Name { get; set; }

        [StringLength(200)]
        public string LentPallet_license { get; set; }

        public int LentPallet_QtyLent { get; set; }      

        public int LentPallet_QtyReturnGood { get; set; }

        public int LentPallet_QtyReturnDmg { get; set; }       

        [StringLength(200)]
        public string Ref_No1 { get; set; }

        [StringLength(200)]
        public string Ref_No2 { get; set; }

        [StringLength(200)]
        public string Ref_No3 { get; set; }

        [StringLength(200)]
        public string Ref_No4 { get; set; }

        [StringLength(200)]
        public string Ref_No5 { get; set; }

        [StringLength(200)]
        public string UDF_1 { get; set; }

        [StringLength(200)]
        public string UDF_2 { get; set; }

        [StringLength(200)]
        public string UDF_3 { get; set; }

        [StringLength(200)]
        public string UDF_4 { get; set; }

        [StringLength(200)]
        public string UDF_5 { get; set; }
  
        public int Document_status { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        public bool IsSystem { get; set; }

        public int? status_Id { get; set; }

        public DateTime? Create_Date { get; set; }

        [StringLength(50)]
        public string Create_By { get; set; }

        public DateTime? Update_Date { get; set; }

        [StringLength(50)]
        public string Update_By { get; set; }

        public DateTime? Cancel_Date { get; set; }

        [StringLength(50)]
        public string Cancel_By { get; set; }
    }
}
