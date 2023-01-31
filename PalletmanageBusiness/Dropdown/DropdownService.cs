using Business.Models;
using DataAccess;
using MasterDataBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterDataBusiness
{
    public class DropdownService
    {

        private MasterDbContext db;

        public DropdownService()
        {
            db = new MasterDbContext();
        }

        public DropdownService(MasterDbContext db)
        {
            this.db = db;
        }

        public List<VehicleTypeViewModel> vehicleTypedropdown(VehicleTypeViewModel data)
        {
            try
            {
                var result = new List<VehicleTypeViewModel>();
                var query = db.MS_VehicleType.Where(c => c.IsActive == 1 && c.IsDelete == 0 && (c.VehicleType_Id == "V001" || c.VehicleType_Id == "V002" || c.VehicleType_Id == "V003")).OrderBy(o => o.VehicleType_Id).ToList();               
               
                foreach (var item in query)
                {
                    var resultItem = new VehicleTypeViewModel();

                    resultItem.vehicleType_Index = item.VehicleType_Index;
                    resultItem.vehicleType_Id = item.VehicleType_Id;
                    resultItem.vehicleType_Name = item.VehicleType_Name;

                    result.Add(resultItem);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DocumentTypeViewModel> documentTypefilter(DocumentTypeViewModel data)
        {
           
            String msglog = "";
            try
            {
                //string[] document_id = null;
                //document_id = data.format_Text.Split(',');

                var result = new List<DocumentTypeViewModel>();

                var query = db.Ms_DocumentType.Where(c => c.IsActive == 1 && c.IsDelete == 0).AsQueryable();
                if (data.process_Index != new Guid("00000000-0000-0000-0000-000000000000".ToString()))
                {
                    msglog = "Process_Index";
                    query = query.Where(c => c.Process_Index == data.process_Index);
                }

                //if (!string.IsNullOrEmpty(data.format_Text))
                //{
                //    msglog = "DocumentType_Id";
                //    query = query.Where(c => document_id.Contains(c.Format_Text));
                //}

                //if (!string.IsNullOrEmpty(data.documentType_Name))
                //{
                //    msglog = "DocumentType_Name";
                //    query = query.Where(c => c.DocumentType_Name == data.documentType_Name);
                //}
                //if (data.process_Index != new Guid("00000000-0000-0000-0000-000000000000".ToString()))
                //{
                //    msglog = "Process_Index";
                //    query = query.Where(c =>  aa.Contains(c.Process_Index));
                //}
                if (data.documentType_Index != null && data.documentType_Index != new Guid("00000000-0000-0000-0000-000000000000".ToString()))
                {
                    msglog = "DocumentType_Index";
                    query = query.Where(c => c.DocumentType_Index == data.documentType_Index);
                }

                var queryResult = query.OrderBy(o => o.DocumentType_Id).ToList();

                foreach (var item in queryResult)
                {
                    msglog = "by values";
                    var resultItem = new DocumentTypeViewModel();
                    resultItem.documentType_Index = item.DocumentType_Index;
                    resultItem.documentType_Id = item.DocumentType_Id;
                    resultItem.documentType_Name = item.DocumentType_Name;
                    resultItem.format_Date = item.Format_Date;
                    resultItem.format_Document = item.Format_Document;
                    resultItem.format_Running = item.Format_Running;
                    resultItem.format_Text = item.Format_Text;
                    resultItem.isResetByDay = item.IsResetByDay;
                    resultItem.isResetByMonth = item.IsResetByMonth;
                    resultItem.isResetByYear = item.IsResetByYear;
                    resultItem.process_Index = item.Process_Index;
                    resultItem.create_Date = item.Create_Date;

                    result.Add(resultItem);
                }
                return result;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ItemListViewModel> vendordropdown(ItemListViewModel data)
        {
            try
            {
                using (var context = new MasterDbContext())
                {
                    var query = context.MS_Vendor.Take(1);
                    if (data.key == "-")
                    {
                    }
                    else if (!string.IsNullOrEmpty(data.key))
                    {
                        query = context.MS_Vendor.Where(c => c.IsActive == 1 && c.IsDelete == 0 && c.Vendor_Id.Contains(data.key) || c.Vendor_Name.Contains(data.key));
                    }
                    
                    var result = query.Select(c => new { c.Vendor_Index, c.Vendor_Id, c.Vendor_Name }).Distinct().Take(10).ToList();
                    var items = new List<ItemListViewModel>();                   
                    foreach (var item in result)
                    {
                        var resultItem = new ItemListViewModel();
                        resultItem.index = item.Vendor_Index;
                        resultItem.id = item.Vendor_Id;
                        resultItem.name = item.Vendor_Name;
                        resultItem.value1 = item.Vendor_Id + " - " + item.Vendor_Name;
                        items.Add(resultItem);
                    }
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
