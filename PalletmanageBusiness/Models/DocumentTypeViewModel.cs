using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models
{   
    public  class DocumentTypeViewModel
    {
        public Guid documentType_Index { get; set; }

        public Guid? process_Index { get; set; }
        
        [StringLength(50)]
        public string documentType_Id { get; set; }
        [StringLength(200)]
        public string process_Name { get; set; }

        [StringLength(200)]
        public string documentType_Name { get; set; }

        [StringLength(200)]
        public string format_Text { get; set; }

        [StringLength(200)]
        public string format_Date { get; set; }

        [StringLength(200)]
        public string format_Running { get; set; }

        [StringLength(200)]
        public string format_Document { get; set; }

        public int? isResetByYear { get; set; }

        public int? isResetByMonth { get; set; }

        public int? isResetByDay { get; set; }

        public int? isActive { get; set; }

        public int? isDelete { get; set; }

        public int? isSystem { get; set; }

        public int? status_Id { get; set; }

        
        [StringLength(200)]
        public string create_By { get; set; }

        public DateTime? create_Date { get; set; }

        [StringLength(200)]
        public string update_By { get; set; }

        public DateTime? update_Date { get; set; }

        [StringLength(200)]
        public string cancel_By { get; set; }

        public DateTime? cancel_Date { get; set; }

        public int chk { get; set; }

        public string key { get; set; }

        public string name { get; set; }
    }
}
