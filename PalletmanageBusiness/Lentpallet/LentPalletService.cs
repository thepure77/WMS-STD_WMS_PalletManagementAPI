using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Business.Services;
using DataAccess.Models.Master.Table;
using Newtonsoft.Json;
using DataAccess.Models.Master.View;
using BinBalanceDataAccess.Models;
using DataAccess.Models.Transfer.Table;
using Business.Library;
using PalletmanageBusiness.ModelConfig;
using System.Data;
using MasterDataBusiness.ViewModels;
using Microsoft.Extensions.Configuration;
using System.IO;
using PalletmanageBusiness.BusinessUnit;
using DataAccess.Models.Outbound.Table;
using PalletmanageBusiness.AutoNumber;
using PalletmanageBusiness.pallet;
using Common.Utils;
using Business.Models;
using Business.Commons;
using PalletmanageBusiness.Reports;
using AspNetCore.Reporting;
using PalletmanageBusiness.Libs;

namespace MasterDataBusiness
{
    public class LentPalletService
    {
        private MasterDbContext db;
        private OutboundDbContext dbOb;

        public LentPalletService()
        {
            db = new MasterDbContext();
            dbOb = new OutboundDbContext();
        }

        public LentPalletService(MasterDbContext db,OutboundDbContext dbOb)
        {
            this.db = db;
            this.dbOb = dbOb;
        }


        public actionResultLentPalletResultViewModel Filter(PalletFilterViewModel data)
        {
            var result = new actionResultLentPalletResultViewModel();
            var list_data = new List<LentPalletResultViewModel>();
            DateTime dateStart = DateTime.Now.toString().toBetweenDate().start;
            DateTime dateEnd = DateTime.Now.toString().toBetweenDate().end;
            try
            {
                var model = dbOb.im_LentPallet_loc.AsQueryable();

                if (!string.IsNullOrEmpty(data.vender_Index.ToString()) && data.vender_Index.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    model = model.Where(c => c.Vender_Index == data.vender_Index);
                }
                if (!string.IsNullOrEmpty(data.start_Date.ToString()) && !string.IsNullOrEmpty(data.end_Date.ToString()))
                {
                    dateStart = data.start_Date.toBetweenDate().start;
                    dateEnd = data.end_Date.toBetweenDate().end;

                    model = model.Where(c =>  c.LentPallet_Date >= dateStart && c.LentPallet_Date <= dateEnd);
                }
                if (!string.IsNullOrEmpty(data.document_No))
                {
                    model = model.Where(c => data.document_No.Contains(c.LentPallet_Id.Replace("\r\n", "")));
                }

                var TotalRow = new List<im_LentPallet_loc>();
                TotalRow = model.ToList();
               

                if (data.is_sum)
                {
                    var queryResult = model.OrderBy(o => o.LentPallet_Id).ToList();
                    var querySumResult = queryResult.Where( c => c.Document_status != 0 && !c.IsDelete).GroupBy(c => new { c.Vender_Id, c.Vender_Name })
                                              .Select(x => new
                                              {
                                                  Vender_Id = x.Key.Vender_Id,
                                                  Vender_Name = x.Key.Vender_Name,
                                                  LentPallet_Date = x.Max(c => c.LentPallet_Date),
                                                  LentPallet_QtyLent = x.Sum(c => c.LentPallet_QtyLent),
                                                  LentPallet_QtyReturnGood = x.Sum(c => c.LentPallet_QtyReturnGood),
                                                  LentPallet_QtyReturnDmg = x.Sum(c => c.LentPallet_QtyReturnDmg)
                                              }).ToList();
                    foreach (var item in querySumResult)
                    {
                        var impallet = new LentPalletResultViewModel();
                        impallet.vender_Id = item.Vender_Id;
                        impallet.vender_Name = item.Vender_Name;
                        impallet.lentpalletDate_text = Convert.ToDateTime(item.LentPallet_Date).ToString("dd/MM/yyyy HH:mm");
                        impallet.lentPallet_QtyLent = item.LentPallet_QtyLent;
                        impallet.lentPallet_QtyReturnDmg = item.LentPallet_QtyReturnDmg;
                        impallet.lentPallet_QtyReturnGood = item.LentPallet_QtyReturnGood;
                        impallet.lentPallet_QtyTotal = item.LentPallet_QtyLent - (item.LentPallet_QtyReturnGood + item.LentPallet_QtyReturnDmg);
                        list_data.Add(impallet);
                    }
                }
                else
                {
                    var statusModels = new List<int?>();

                    if (data.status != null)
                    {
                        if (data.status.Count > 0)
                        {
                            foreach (var item in data.status)
                            {
                                statusModels.Add(item.value);
                            }
                            model = model.Where(c => statusModels.Contains(c.Document_status));
                        }
                    }

                    var queryResult = model.OrderBy(o => o.LentPallet_Id).ToList();

                    foreach (var item in queryResult)
                    {
                        var lentimpallet = new LentPalletResultViewModel();
                        lentimpallet.lentPallet_Index = item.LentPallet_Index;
                        lentimpallet.lentPallet_Id = item.LentPallet_Id;
                        lentimpallet.documentType_Name = item.DocumentType_Name;
                        lentimpallet.vender_Name = item.Vender_Name;
                        lentimpallet.vender_Id = item.Vender_Id;
                        lentimpallet.vender_Index = item.Vender_Index;
                        lentimpallet.createDate_text = Convert.ToDateTime(item.Create_Date).ToString("dd/MM/yyyy HH:mm");
                        lentimpallet.lentpalletDate_text = Convert.ToDateTime(item.LentPallet_Date).ToString("dd/MM/yyyy HH:mm");
                        lentimpallet.lentPallet_QtyLent = item.LentPallet_QtyLent;
                        lentimpallet.lentPallet_QtyReturnGood = item.LentPallet_QtyReturnGood;
                        lentimpallet.lentPallet_QtyReturnDmg = item.LentPallet_QtyReturnDmg;
                        lentimpallet.lentPallet_QtyTotal = item.LentPallet_QtyReturnGood + item.LentPallet_QtyReturnDmg;
                        lentimpallet.ref_No1 = item.Ref_No1;
                        lentimpallet.ref_No2 = item.Ref_No2;
                        if (item.Update_Date != null)
                        {
                            lentimpallet.updateDate_text = Convert.ToDateTime(item.Update_Date).ToString("dd/MM/yyyy HH:mm");
                            lentimpallet.update_By = item.Update_By;
                        }
                        if (item.Cancel_Date != null)
                        {
                            lentimpallet.cancelDate_text = Convert.ToDateTime(item.Cancel_Date).ToString("dd/MM/yyyy HH:mm");
                            lentimpallet.cancel_By = item.Cancel_By;
                        }
                        lentimpallet.create_By = item.Create_By;
                        lentimpallet.createDate = Convert.ToDateTime(item.Create_Date).ToString("yyyyMMdd");
                        lentimpallet.createTime_text = Convert.ToDateTime(item.Create_Date).ToString("HH:mm");
                        lentimpallet.lentPallet_Date = Convert.ToDateTime(item.LentPallet_Date).ToString("yyyyMMdd");
                        lentimpallet.lentpallettime_text = Convert.ToDateTime(item.LentPallet_Date).ToString("HH:mm");
                        lentimpallet.lentPallet_license = item.LentPallet_license;
                        lentimpallet.vehicleType_Name = item.VehicleType_Name;
                        lentimpallet.vehicle_Index = item.Vehicle_Index;
                        lentimpallet.vehicleType_Id = item.VehicleType_Id;

                        if (item.Vehicle_Index != null)
                        {
                            var data_ = new VehicleTypeViewModel();
                            var service_Dropdown = new DropdownService();
                            var data_vehicleType = service_Dropdown.vehicleTypedropdown(data_);
                            lentimpallet.vehicleType_dropdown = data_vehicleType.FirstOrDefault(c => c.vehicleType_Index == item.Vehicle_Index);
                        }

                        if (item.DocumentType_Index != null)
                        {
                            var data_ = new DocumentTypeViewModel();
                            var service_Dropdown = new DropdownService();
                            var data_documentType = service_Dropdown.documentTypefilter(data_);
                            lentimpallet.documentType_dropdown = data_documentType.FirstOrDefault(c => c.documentType_Index == item.DocumentType_Index);
                        }

                        if (item.Document_status == 0)
                        {
                            lentimpallet.status_Name = "ยกเลิก";
                        }
                        else if (item.Document_status == 1)
                        {
                            lentimpallet.status_Name = "รอยืนยัน";
                        }
                        else if (item.Document_status == 2)
                        {
                            lentimpallet.status_Name = "แก้ไข";
                        }

                        list_data.Add(lentimpallet);
                    }
                }
                var count = TotalRow.Count;

                var actionResultLentPalletViewModel = new actionResultLentPalletResultViewModel();
                actionResultLentPalletViewModel.items = list_data.ToList();
                actionResultLentPalletViewModel.pagination = new Pagination() { TotalRow = count, CurrentPage = data.CurrentPage, PerPage = data.PerPage };
                
                result = actionResultLentPalletViewModel;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region SaveAdd and UpdateAddLentpallets
        public Result SaveLentPallet(LentPalletViewModel data)
        {
            var Result = new Result();
            try
            {
                im_LentPallet_loc model = dbOb.im_LentPallet_loc.Find(data.lentPallet_Index);
                Guid lentpallet_Index;
                string UserBy;
                var date_ = DateTime.Now;
                DateTime UserDate = date_.Date + new TimeSpan(date_.Hour, date_.Minute, 0);
                //DateTime UserDate = DateTime.Now;

                if (model is null)
                {
                    lentpallet_Index = Guid.NewGuid();
                    UserBy = data.create_By;

                    var result = new List<GenDocumentTypeViewModel>();
                    var filterModel = new GenDocumentTypeViewModel();
                    filterModel.documentType_Index = data.documentType_dropdown.documentType_Index;
                    filterModel.documentType_Id = data.documentType_dropdown.documentType_Id;
                    filterModel.documentType_Name = data.documentType_dropdown.documentType_Name;
                    filterModel.format_Document = data.documentType_dropdown.format_Document;
                    filterModel.format_Running = data.documentType_dropdown.format_Running;
                    filterModel.format_Date = data.documentType_dropdown.format_Date;
                    filterModel.isResetByDay = data.documentType_dropdown.isResetByDay;
                    filterModel.isResetByMonth = data.documentType_dropdown.isResetByMonth;
                    filterModel.isResetByYear = data.documentType_dropdown.isResetByYear;
                    filterModel.format_Text = data.documentType_dropdown.format_Text;
                    result.Add(filterModel);

                    ////GetConfig
                    var genDoc = new AutoNumberService(db);
                    string DocNo = "";
                   // DateTime DocumentDate = (DateTime)data.createDate.toDate();
                    DocNo = genDoc.genAutoDocmentNumber(result, UserDate);
                    data.lentPallet_Id = DocNo;

                    ///update lentpallet_loc
                    im_LentPallet_loc im_LentPallet_loc = new im_LentPallet_loc();
                    im_LentPallet_loc.LentPallet_Index = lentpallet_Index;
                    im_LentPallet_loc.LentPallet_Id = data.lentPallet_Id;

                    if (!string.IsNullOrEmpty(data.lentPallet_Date))
                    {
                        string[] time = null;
                        time = data.lentpallettime_text.Split(':');
                        var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                        var lentpallet_date_db = (data.lentPallet_Date + _time).toDatetime();
                        im_LentPallet_loc.LentPallet_Date = (DateTime)lentpallet_date_db;
                    }

                    im_LentPallet_loc.Vender_Index = data.vender_Index;
                    im_LentPallet_loc.Vender_Id = data.vender_Id;
                    im_LentPallet_loc.Vender_Name = data.vender_Name;
                    im_LentPallet_loc.DocumentType_Index = data.documentType_dropdown.documentType_Index;
                    im_LentPallet_loc.DocumentType_Id = data.documentType_dropdown.documentType_Id;
                    im_LentPallet_loc.DocumentType_Name = data.documentType_dropdown.documentType_Name;
                    im_LentPallet_loc.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                    im_LentPallet_loc.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                    im_LentPallet_loc.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                    im_LentPallet_loc.LentPallet_license = data.lentPallet_license;
                    im_LentPallet_loc.LentPallet_QtyLent = data.lentPallet_QtyLent;
                    im_LentPallet_loc.LentPallet_QtyReturnGood = data.lentPallet_QtyReturnGood;
                    im_LentPallet_loc.LentPallet_QtyReturnDmg = data.lentPallet_QtyReturnDmg;
                    im_LentPallet_loc.Document_status = 1;
                    im_LentPallet_loc.Ref_No1 = data.ref_No1;
                    im_LentPallet_loc.Ref_No2 = data.ref_No2; //ปีพาเลท
                    im_LentPallet_loc.IsActive = true;
                    im_LentPallet_loc.IsDelete = false;
                    im_LentPallet_loc.IsSystem = false;
                    //im_LentPallet.status_Id = 1;
                    im_LentPallet_loc.Create_By = UserBy;
                    im_LentPallet_loc.Create_Date = UserDate; 
                    //if (!string.IsNullOrEmpty(data.createDate))
                    //{
                    //    string[] time = null;
                    //    time = data.createTime_text.Split(':');
                    //    var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                    //    var create_date_db = (data.createDate + _time).toDatetime();
                    //    im_LentPallet_loc.Create_Date = create_date_db;
                    //}
                    dbOb.im_LentPallet_loc.Add(im_LentPallet_loc);


                    ///insert lentpallet
                    im_LentPallet im_LentPallet = new im_LentPallet();
                    im_LentPallet.LentPallet_Index = lentpallet_Index;
                    im_LentPallet.LentPallet_Id = data.lentPallet_Id;

                    if (!string.IsNullOrEmpty(data.lentPallet_Date))
                    {
                        string[] time = null;
                        time = data.lentpallettime_text.Split(':');
                        var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                        var lentpallet_date_db = (data.lentPallet_Date + _time).toDatetime();
                        im_LentPallet.LentPallet_Date = (DateTime)lentpallet_date_db;
                    }

                    im_LentPallet.Vender_Index = data.vender_Index;
                    im_LentPallet.Vender_Id = data.vender_Id;
                    im_LentPallet.Vender_Name = data.vender_Name;
                    im_LentPallet.DocumentType_Index = data.documentType_dropdown.documentType_Index;
                    im_LentPallet.DocumentType_Id = data.documentType_dropdown.documentType_Id;
                    im_LentPallet.DocumentType_Name = data.documentType_dropdown.documentType_Name;
                    im_LentPallet.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                    im_LentPallet.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                    im_LentPallet.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                    im_LentPallet.LentPallet_license = data.lentPallet_license;
                    im_LentPallet.LentPallet_QtyLent = data.lentPallet_QtyLent;
                    im_LentPallet.LentPallet_QtyReturnGood = data.lentPallet_QtyReturnGood;
                    im_LentPallet.LentPallet_QtyReturnDmg = data.lentPallet_QtyReturnDmg;                 
                    im_LentPallet.Document_status = 1;
                    im_LentPallet.Ref_No1 = data.ref_No1;
                    im_LentPallet.Ref_No2 = data.ref_No2; //ปีพาเลท
                    im_LentPallet.IsActive = true;
                    im_LentPallet.IsDelete = false;
                    im_LentPallet.IsSystem = false;
                    //im_LentPallet.status_Id = 1;
                    im_LentPallet.Create_By = UserBy;
                    im_LentPallet.Create_Date = UserDate;
                    //if (!string.IsNullOrEmpty(data.createDate))
                    //{
                    //    string[] time = null;
                    //    time = data.createTime_text.Split(':');
                    //    var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                    //    var create_date_db = (data.createDate + _time).toDatetime();
                    //    im_LentPallet.Create_Date = create_date_db;
                    //}
                    dbOb.im_LentPallet.Add(im_LentPallet);
                }
                else
                {
                    lentpallet_Index = data.lentPallet_Index;
                    UserBy = data.update_By;

                    if (!string.IsNullOrEmpty(data.lentPallet_Date))
                    {
                        string[] time = null;
                        time = data.lentpallettime_text.Split(':');
                        var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                        var lentpallet_date_db = (data.lentPallet_Date + _time).toDatetime();
                        model.LentPallet_Date = (DateTime)lentpallet_date_db;
                    }
                    model.Vender_Index = data.vender_Index;
                    model.Vender_Id = data.vender_Id;
                    model.Vender_Name = data.vender_Name;
                    model.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                    model.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                    model.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                    model.LentPallet_license = data.lentPallet_license;
                    model.LentPallet_QtyLent = data.lentPallet_QtyLent;
                    model.LentPallet_QtyReturnGood = data.lentPallet_QtyReturnGood;
                    model.LentPallet_QtyReturnDmg = data.lentPallet_QtyReturnDmg;
                    model.Document_status = 2;
                    model.Ref_No1 = data.ref_No1;
                    model.Ref_No2 = data.ref_No2;
                    model.IsActive = true;
                    model.IsDelete = false;
                    model.IsSystem = false;
                    //model.status_Id = 2;
                    model.Update_By = data.update_By;
                    model.Update_Date = UserDate;

                    dbOb.im_LentPallet_loc.Update(model);

                    im_LentPallet model_ = dbOb.im_LentPallet.Find(data.lentPallet_Index);
                    if (model_ != null)
                    {
                        if (!string.IsNullOrEmpty(data.lentPallet_Date))
                        {
                            string[] time = null;
                            time = data.lentpallettime_text.Split(':');
                            var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                            var lentpallet_date_db = (data.lentPallet_Date + _time).toDatetime();
                            model_.LentPallet_Date = (DateTime)lentpallet_date_db;
                        }
                        model_.Vender_Index = data.vender_Index;
                        model_.Vender_Id = data.vender_Id;
                        model_.Vender_Name = data.vender_Name;
                        model_.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                        model_.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                        model_.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                        model_.LentPallet_license = data.lentPallet_license;
                        model_.LentPallet_QtyLent = data.lentPallet_QtyLent;
                        model_.LentPallet_QtyReturnGood = data.lentPallet_QtyReturnGood;
                        model_.LentPallet_QtyReturnDmg = data.lentPallet_QtyReturnDmg;
                        model_.Document_status = 2;
                        model_.Ref_No1 = data.ref_No1;
                        model_.Ref_No2 = data.ref_No2;
                        model_.IsActive = true;
                        model_.IsDelete = false;
                        model_.IsSystem = false;
                        //model.status_Id = 2;
                        model_.Update_By = data.update_By;
                        model_.Update_Date = UserDate;

                        dbOb.im_LentPallet.Update(model_);
                    }

                }
                var MyTransaction = dbOb.Database.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    dbOb.SaveChanges();
                    MyTransaction.Commit();
                }
                catch (Exception saveEx)
                {
                    MyTransaction.Rollback();
                    throw saveEx;
                }

                Result.code = 200;
                Result.no = data.lentPallet_Id;
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Deletepallets
        public Result DeleteLentPallet(LentPalletViewModel data)
        {
            var Result = new Result();
            try
            {
                im_LentPallet_loc model = dbOb.im_LentPallet_loc.Find(data.lentPallet_Index);
                string UserBy;
                var date_ = DateTime.Now;
                DateTime UserDate = date_.Date + new TimeSpan(date_.Hour, date_.Minute, 0);
                //DateTime UserDate = DateTime.Now;
                if (model != null)
                {
                    UserBy = data.update_By;                   
                    model.Document_status = 0;                   
                    model.IsDelete = true;                  
                    //model.status_Id = 0;
                    //model.Update_By = data.cancel_By;
                    //model.Update_Date = UserDate;
                    model.Cancel_By = data.cancel_By;
                    model.Cancel_Date = UserDate;

                    dbOb.im_LentPallet_loc.Update(model);

                    ///update im_lentpallet
                    im_LentPallet model_ = dbOb.im_LentPallet.Find(data.lentPallet_Index);
                    if (model_ != null)
                    {
                        UserBy = data.update_By;
                        model_.Document_status = 0;
                        model_.IsDelete = true;
                        //model.status_Id = 0;
                        //model_.Update_By = data.cancel_By;
                        //model_.Update_Date = UserDate;
                        model_.Cancel_By = data.cancel_By;
                        model_.Cancel_Date = UserDate;

                        dbOb.im_LentPallet.Update(model_);
                    }

                    var MyTransaction = dbOb.Database.BeginTransaction(IsolationLevel.Serializable);
                    try
                    {
                        dbOb.SaveChanges();
                        MyTransaction.Commit();
                    }
                    catch (Exception saveEx)
                    {
                        MyTransaction.Rollback();
                        throw saveEx;
                    }

                    Result.code = 200;
                }                
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region printLentPallet
        public dynamic printLentPallet(LentPalletResultViewModel data, string rootPath = "")
        {
            //var Master_DBContext = new MasterDataDbContext();
            //var temp_Master_DBContext = new temp_MasterDataDbContext();
            //var culture = new System.Globalization.CultureInfo("en-US");
            //String State = "Start";
            //String msglog = "";
            //var olog = new logtxt();
            var result = new List<ReportPrintPalletViewModel>();
            try
            {
                var item = new ReportPrintPalletViewModel();
                item.docket_no = data.lentPallet_Id;
                item.company_name = data.vender_Name;
                item.truck_registration = data.lentPallet_license;
                item.palletDate_text = !string.IsNullOrEmpty(data.lentpalletDate_text) ? data.lentpalletDate_text : "";
                item.pallet_year = data.ref_No2;
                item.return_qtygood = data.lentPallet_QtyReturnGood;
                item.return_qtydmg = data.lentPallet_QtyReturnDmg;
                item.return_qtytotal = (data.lentPallet_QtyReturnGood + data.lentPallet_QtyReturnDmg ) - data.lentPallet_QtyLent;
                item.return_qty = data.lentPallet_QtyLent;
                item.remark = data.ref_No1;
                if (data.vehicleType_Name == "4 ล้อ")
                {
                    item.truck_4w = "X";
                    item.truck_type = "";
                }
                else if (data.vehicleType_Name == "6 ล้อ")
                {
                    item.truck_6w = "X";
                    item.truck_type = "";
                }
                else if (data.vehicleType_Name == "10 ล้อ")
                {
                    item.truck_10w = "X";
                    item.truck_type = "";
                }
                else
                {
                    item.truck_other = "X";
                    item.truck_type = data.vehicleType_Name;
                }
                result.Add(item);

                rootPath = rootPath.Replace("\\PalletmanageAPI", "");
                var reportPath = rootPath + new AppSettingConfig().GetUrl("ReportPrintPallet");
                LocalReport report = new LocalReport(reportPath);
                report.AddDataSource("DataSet1", result);

                System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                string fileName = "";
                string fullPath = "";
                fileName = "tmpReport" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var renderedBytes = report.Execute(RenderType.Pdf);

                Utils objReport = new Utils();
                fullPath = objReport.saveReport(renderedBytes.MainStream, fileName + ".pdf", rootPath);
                var saveLocation = objReport.PhysicalPath(fileName + ".pdf", rootPath);
                return saveLocation;
            }
            catch (Exception ex)
            {
                //olog.logging("ReportGIByShipmentNoAndProductId", ex.Message);
                throw ex;
            }
        }
        #endregion

        #region exportExcel
        public string ExportExcel(PalletFilterViewModel data, string rootPath = "")
        {
            var culture = new System.Globalization.CultureInfo("en-US");
            //String State = "Start";
            //String msglog = "";
            //var olog = new logtxt();
            var result = new List<ReportExportLentPalletViewModel>();
            try
            {
                DateTime dateStart = DateTime.Now.toString().toBetweenDate().start;
                DateTime dateEnd = DateTime.Now.toString().toBetweenDate().end;

                var model = dbOb.im_LentPallet_loc.AsQueryable();

                if (!string.IsNullOrEmpty(data.vender_Index.ToString()) && data.vender_Index.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    model = model.Where(c => c.Vender_Index == data.vender_Index);
                }
                if (!string.IsNullOrEmpty(data.start_Date.ToString()) && !string.IsNullOrEmpty(data.end_Date.ToString()))
                {
                    dateStart = data.start_Date.toBetweenDate().start;
                    dateEnd = data.end_Date.toBetweenDate().end;
                    model = model.Where(c => c.LentPallet_Date >= dateStart && c.LentPallet_Date <= dateEnd);
                }
                if (!string.IsNullOrEmpty(data.document_No))
                {
                    model = model.Where(c => data.document_No.Contains(c.LentPallet_Id.Replace("\r\n", "")));
                }

                var statusModels = new List<int?>();

                if (data.status != null)
                {
                    if (data.status.Count > 0)
                    {
                        foreach (var item in data.status)
                        {
                            statusModels.Add(item.value);
                        }
                        model = model.Where(c => statusModels.Contains(c.Document_status));
                    }
                }

                var resultquery = model.ToList();

                if (resultquery.Count == 0)
                {
                    var resultItem = new ReportExportLentPalletViewModel();
                    var startDate = DateTime.ParseExact(data.start_Date.Substring(0, 8), "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", culture);

                    var endDate = DateTime.ParseExact(data.end_Date.Substring(0, 8), "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy", culture);

                    resultItem.report_date = startDate;
                    resultItem.report_date_to = endDate;
                    result.Add(resultItem);
                }
                else
                {
                    foreach (var item in resultquery)
                    {
                        var impallet = new ReportExportLentPalletViewModel();
                        impallet.lentpallet_Id = item.LentPallet_Id;
                        impallet.vender_Name = item.Vender_Name;
                        impallet.vender_Id = item.Vender_Id;
                        //impallet.createDate_text = Convert.ToDateTime(item.Create_Date).ToString("dd/MM/yyyy HH:mm");
                        //impallet.palletDate_text = Convert.ToDateTime(item.Pallet_Date).ToString("dd/MM/yyyy HH:mm");
                        impallet.lentPallet_QtyLent = item.LentPallet_QtyLent;
                        impallet.lentPallet_QtyReturnGood = item.LentPallet_QtyReturnGood;
                        impallet.lentPallet_QtyReturnDmg = item.LentPallet_QtyReturnDmg;
                        impallet.lentPallet_QtyTotal = impallet.lentPallet_QtyReturnGood + impallet.lentPallet_QtyReturnDmg;
                        impallet.ref_No1 = !string.IsNullOrEmpty(item.Ref_No1) ? item.Ref_No1 : "NULL";
                        impallet.ref_No2 = !string.IsNullOrEmpty(item.Ref_No2) ? item.Ref_No2 : "NULL";
                        if (item.Update_Date != null)
                        {
                            impallet.update_Date = Convert.ToDateTime(item.Update_Date);
                            impallet.update_By = item.Update_By;
                            impallet.updateDate_text = Convert.ToDateTime(item.Update_Date).ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                        {
                            impallet.update_By = "NULL";
                            impallet.update_Date = null;
                            impallet.updateDate_text = "NULL";
                        }

                        if (item.Create_Date != null)
                        {
                            impallet.createDate = Convert.ToDateTime(item.Create_Date);
                            impallet.create_By = item.Create_By;
                            //impallet.createDate_text = Convert.ToDateTime(item.Create_Date).ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                        {
                            impallet.create_By = "NULL";
                            impallet.createDate = null;
                            //impallet.createDate_text = "NULL";
                        }

                        if (item.Cancel_Date != null)
                        {
                            impallet.cancel_Date = Convert.ToDateTime(item.Cancel_Date);
                            impallet.cancel_By = item.Cancel_By;
                            impallet.cancelDate_text = Convert.ToDateTime(item.Cancel_Date).ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                        {
                            impallet.cancel_By = "NULL";
                            impallet.cancelDate_text = "NULL";
                        }

                        if (item.LentPallet_Date != null)
                        {
                            impallet.lentpalletDate = Convert.ToDateTime(item.LentPallet_Date);
                            //impallet.lentpalletDate_text = Convert.ToDateTime(item.LentPallet_Date).ToString("yyyy-MM-dd HH:mm");
                        }
                        else
                        {
                            impallet.lentpalletDate = null;
                            //impallet.lentpalletDate_text = "NULL";
                        }
                        impallet.lentpallet_license = item.LentPallet_license;
                        impallet.vehicleType_Name = item.VehicleType_Name;

                        if (item.Document_status == 0)
                            impallet.status_Name = "ยกเลิก";
                        else if (item.Document_status == 1)
                            impallet.status_Name = "รอยืนยัน";
                        else if (item.Document_status == 2)
                            impallet.status_Name = "แก้ไข";

                        result.Add(impallet);
                    }
                }

                rootPath = rootPath.Replace("\\PalletmanageAPI", "");
                var reportPath = rootPath + new AppSettingConfig().GetUrl("ReportExportLentPallet");

                LocalReport report = new LocalReport(reportPath);
                report.AddDataSource("DataSet1", result);

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                string fileName = "";
                string fullPath = "";
                fileName = "tmpReport";

                Utils objReport = new Utils();
                var renderedBytes = report.Execute(RenderType.Excel);
                fullPath = objReport.saveReport(renderedBytes.MainStream, fileName + ".xls", rootPath);
                var saveLocation = objReport.PhysicalPath(fileName + ".xls", rootPath);
                return saveLocation;
            }
            catch (Exception ex)
            {
                //olog.logging("ReportGIByShipmentDateAndBusinessUnit", ex.Message);
                throw ex;
            }

        }
        #endregion


    }
}
