using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Outbound.Table
{
    public partial class im_Pallet
    {
        [Key]
        public Guid Pallet_Index { get; set; }

        [StringLength(50)]
        public string Pallet_Id { get; set; }

        public DateTime Pallet_Date { get; set; }

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
        public string Pallet_license { get; set; }

        public int Pallet_QtyPlan { get; set; }

        public int Pallet_QtyReceive { get; set; }

        public int Pallet_QtyReturnGood { get; set; }

        public int Pallet_QtyReturnDmg { get; set; }

        public int Pallet_QtyTotal { get; set; }       

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
