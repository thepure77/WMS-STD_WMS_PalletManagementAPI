using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
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
using Business.Models;
using PalletmanageBusiness.AutoNumber;
using PalletmanageBusiness.pallet;
using PalletmanageBusiness.Reports;
using AspNetCore.Reporting;
using PalletmanageBusiness.Libs;
using Business.Commons;

namespace MasterDataBusiness
{
    public class PalletService
    {
        private MasterDbContext db;
        private OutboundDbContext dbOb;

        public PalletService()
        {
            db = new MasterDbContext();
            dbOb = new OutboundDbContext();
        }

        public PalletService(MasterDbContext db,OutboundDbContext dbOb)
        {
            this.db = db;
            this.dbOb = dbOb;
        }


        public actionResultPalletResultViewModel Filter(PalletFilterViewModel data)
        {
            //var result = new List<PalletResultViewModel>();
            var result = new actionResultPalletResultViewModel();
            var list_data = new List<PalletResultViewModel>();
            DateTime dateStart = DateTime.Now.toString().toBetweenDate().start;
            DateTime dateEnd = DateTime.Now.toString().toBetweenDate().end;
            try
            {
                var model = dbOb.im_Pallet_loc.AsQueryable();

                if (!string.IsNullOrEmpty(data.vender_Index.ToString()) && data.vender_Index.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    model = model.Where(c => c.Vender_Index == data.vender_Index);
                }
                if (!string.IsNullOrEmpty(data.start_Date.ToString()) && !string.IsNullOrEmpty(data.end_Date.ToString()))
                {
                    dateStart = data.start_Date.toBetweenDate().start;
                    dateEnd = data.end_Date.toBetweenDate().end;
                    model = model.Where(c => c.Pallet_Date >= dateStart && c.Pallet_Date <= dateEnd);
                }
                if (!string.IsNullOrEmpty(data.pallet_Id))
                {
                    model = model.Where(c => data.pallet_Id.Contains(c.Pallet_Id.Replace("\r\n", "")));
                }

                var TotalRow = new List<im_Pallet_loc>();
                TotalRow = model.ToList();
                
                if (data.is_sum)
                {
                    var queryResult = model.OrderBy(o => o.Pallet_Id).ToList();

                    var querySumResult = queryResult.Where( c => c.Document_status != 0 && !c.IsDelete).GroupBy(c => new { c.Vender_Id, c.Vender_Name })
                                              .Select(x => new
                                              {
                                                  Vender_Id = x.Key.Vender_Id,
                                                  Vender_Name = x.Key.Vender_Name,
                                                  Pallet_Date = x.Max(c => c.Pallet_Date),
                                                  Pallet_QtyPlan = x.Sum(c => c.Pallet_QtyPlan),
                                                  Pallet_QtyReceive = x.Where(c => c.DocumentType_Id != "P004" && c.DocumentType_Id != "P005").Sum(c => c.Pallet_QtyReceive),
                                                  Pallet_QtyReturnGood = x.Sum(c => c.Pallet_QtyReturnGood),
                                                  Pallet_QtyReturnDmg = x.Where(c => c.DocumentType_Id != "P004" && c.DocumentType_Id != "P005").Sum(c => c.Pallet_QtyReturnDmg),
                                                  Pallet_QtyTotal = x.Where(c => c.DocumentType_Id != "P004" && c.DocumentType_Id != "P005").Sum(c => c.Pallet_QtyTotal),
                                                  Pallet_Qtybalance_receive = x.Where( c => c.DocumentType_Id == "P002").Sum(c => c.Pallet_QtyReceive)
                                              }).ToList();
               
                    foreach (var item in querySumResult)
                    {
                        var impallet = new PalletResultViewModel();
                        impallet.vender_Id = item.Vender_Id;
                        impallet.vender_Name = item.Vender_Name;
                        impallet.palletDate_text = item.Pallet_Date.ToString("dd/MM/yyyy HH:mm");
                        impallet.pallet_QtyReceive = item.Pallet_QtyReceive;
                        impallet.pallet_QtyReturnGood = item.Pallet_QtyReturnGood;
                        //if (item.Pallet_QtyReturnDmg >= item.Pallet_QtyReturnGood)
                        //{
                            impallet.pallet_QtyReturnDmg = item.Pallet_QtyReturnDmg - item.Pallet_Qtybalance_receive;
                        //}
                        //else if (item.Pallet_QtyReturnDmg > item.Pallet_QtyReturnGood)
                        //{
                        //    impallet.pallet_QtyReturnDmg = item.Pallet_QtyReturnDmg;
                        //}
                        // impallet.pallet_QtyTotal = impallet.pallet_QtyReceive  + impallet.pallet_QtyReturnDmg;
                        impallet.pallet_QtyTotal = impallet.pallet_QtyReceive + impallet.pallet_QtyReturnDmg;
                        impallet.pallet_QtyPlan = item.Pallet_QtyPlan;
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

                    var queryResult = model.OrderBy(o => o.Pallet_Id).ToList();

                    foreach (var item in queryResult)
                    {
                        var impallet = new PalletResultViewModel();
                        impallet.pallet_Index = item.Pallet_Index;
                        impallet.pallet_Id = item.Pallet_Id;
                        impallet.documentType_Name = item.DocumentType_Name;
                        impallet.vender_Name = item.Vender_Name;
                        impallet.vender_Id = item.Vender_Id;
                        impallet.vender_Index = item.Vender_Index;
                        impallet.createDate_text = Convert.ToDateTime(item.Create_Date).ToString("dd/MM/yyyy HH:mm");
                        impallet.palletDate_text = Convert.ToDateTime(item.Pallet_Date).ToString("dd/MM/yyyy HH:mm");
                        impallet.pallettime_temp = Convert.ToDateTime(item.Pallet_Date);
                        impallet.pallet_QtyPlan = item.Pallet_QtyPlan;
                        impallet.pallet_QtyReceive = item.Pallet_QtyReceive;
                        impallet.pallet_QtyReturnGood = item.Pallet_QtyReturnGood;
                        impallet.pallet_QtyReturnDmg = item.Pallet_QtyReturnDmg;
                        impallet.pallet_QtyTotal = item.Pallet_QtyTotal;
                        impallet.ref_No1 = item.Ref_No1;
                        impallet.ref_No2 = item.Ref_No2;
                        if (item.Update_Date != null)
                        {
                            impallet.updateDate_text = Convert.ToDateTime(item.Update_Date).ToString("dd/MM/yyyy HH:mm");
                            impallet.update_By = item.Update_By;
                        }
                        if (item.Cancel_Date != null)
                        {
                            impallet.cancelDate_text = Convert.ToDateTime(item.Cancel_Date).ToString("dd/MM/yyyy HH:mm");
                            impallet.cancel_By = item.Update_By;
                        }
                        impallet.create_By = item.Create_By;
                        impallet.createDate = Convert.ToDateTime(item.Create_Date).ToString("yyyyMMdd");
                        impallet.createTime_text = Convert.ToDateTime(item.Create_Date).ToString("HH:mm");
                        impallet.palletDate = Convert.ToDateTime(item.Pallet_Date).ToString("yyyyMMdd");
                        impallet.pallettime_text = Convert.ToDateTime(item.Pallet_Date).ToString("HH:mm");
                        impallet.pallet_license = item.Pallet_license;
                        impallet.vehicleType_Name = item.VehicleType_Name;
                        impallet.vehicle_Index = item.Vehicle_Index;
                        impallet.vehicleType_Id = item.VehicleType_Id;

                        if (item.Vehicle_Index != null)
                        {
                            var data_ = new VehicleTypeViewModel();
                            var service_Dropdown = new DropdownService();
                            var data_vehicleType = service_Dropdown.vehicleTypedropdown(data_);
                            impallet.vehicleType_dropdown = data_vehicleType.FirstOrDefault(c => c.vehicleType_Index == item.Vehicle_Index);
                        }

                        if (item.DocumentType_Index != null)
                        {
                            var data_ = new DocumentTypeViewModel();
                            var service_Dropdown = new DropdownService();
                            var data_documentType = service_Dropdown.documentTypefilter(data_);
                            impallet.documentType_dropdown = data_documentType.FirstOrDefault(c => c.documentType_Index == item.DocumentType_Index);
                        }

                        if (item.Document_status == 0)
                        {
                            impallet.status_Name = "ยกเลิก";
                        }
                        else if (item.Document_status == 1)
                        {
                            impallet.status_Name = "รอยืนยัน";
                        }
                        else if (item.Document_status == 2)
                        {
                            impallet.status_Name = "แก้ไข";
                        }


                        list_data.Add(impallet);
                    }
                }
                var count = TotalRow.Count;

                var actionResultPalletViewModel = new actionResultPalletResultViewModel();
                actionResultPalletViewModel.items = list_data.ToList();
                actionResultPalletViewModel.pagination = new Pagination() { TotalRow = count, CurrentPage = data.CurrentPage, PerPage = data.PerPage };
                result = actionResultPalletViewModel;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region SaveAdd and UpdateAddpallets
        public Result SaveAddPallet(PalletViewModel data)
        {  
            var Result = new Result();
            try
            {
                //im_Pallet model = dbOb.im_Pallet.Find(data.pallet_Index);
                im_Pallet_loc model = dbOb.im_Pallet_loc.Find(data.pallet_Index);
                Guid pallet_Index;
                string UserBy;
                var date_ = DateTime.Now;
                DateTime UserDate = date_.Date + new TimeSpan(date_.Hour, date_.Minute, 0);
                //DateTime UserDate = DateTime.Now;

                if (model is null)
                {
                    pallet_Index = Guid.NewGuid();
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
                    //DateTime DocumentDate =  DateTime.Now;
                    DocNo = genDoc.genAutoDocmentNumber(result, UserDate);
                    data.pallet_Id = DocNo;

                    //// insert im_pallet_loc
                    im_Pallet_loc im_Pallet_loc = new im_Pallet_loc();
                    im_Pallet_loc.Pallet_Index = pallet_Index;
                    im_Pallet_loc.Pallet_Id = data.pallet_Id;

                    if (!string.IsNullOrEmpty(data.palletDate))
                    {
                        string[] time = null;
                        time = data.pallettime_text.Split(':');
                        var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                        var pallet_date_db = (data.palletDate + _time).toDatetime();
                        im_Pallet_loc.Pallet_Date = (DateTime)pallet_date_db;
                    }

                    im_Pallet_loc.Vender_Index = data.vender_Index;
                    im_Pallet_loc.Vender_Id = data.vender_Id;
                    im_Pallet_loc.Vender_Name = data.vender_Name;
                    im_Pallet_loc.DocumentType_Index = data.documentType_dropdown.documentType_Index;
                    im_Pallet_loc.DocumentType_Id = data.documentType_dropdown.documentType_Id;
                    im_Pallet_loc.DocumentType_Name = data.documentType_dropdown.documentType_Name;
                    im_Pallet_loc.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                    im_Pallet_loc.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                    im_Pallet_loc.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                    im_Pallet_loc.Pallet_license = data.pallet_license;
                    im_Pallet_loc.Pallet_QtyPlan = data.pallet_QtyPlan;
                    im_Pallet_loc.Pallet_QtyReceive = data.pallet_QtyReceive;
                    //im_Pallet_loc.Pallet_QtyReturnGood = data.pallet_QtyReturnGood;
                    im_Pallet_loc.Pallet_QtyReturnGood = 0;
                    im_Pallet_loc.Pallet_QtyReturnDmg = data.pallet_QtyReturnDmg;
                    im_Pallet_loc.Pallet_QtyTotal = data.pallet_QtyTotal;
                    im_Pallet_loc.Document_status = 1;
                    im_Pallet_loc.Ref_No1 = data.ref_No1;
                    im_Pallet_loc.Ref_No2 = data.ref_No2;///ปีพาเลท
                    im_Pallet_loc.IsActive = true;
                    im_Pallet_loc.IsDelete = false;
                    im_Pallet_loc.IsSystem = false;
                    //im_Pallet.status_Id = 1;
                    im_Pallet_loc.Create_By = UserBy;
                    im_Pallet_loc.Create_Date = UserDate;
                    //if (!string.IsNullOrEmpty(data.createDate))
                    //{
                    //    string[] time = null;
                    //    time = data.createTime_text.Split(':');
                    //    var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                    //    var create_date_db = (data.createDate + _time).toDatetime();
                    //    im_Pallet_loc.Create_Date = create_date_db;
                    //}
                    dbOb.im_Pallet_loc.Add(im_Pallet_loc);



                    //// insert im_pallet
                    im_Pallet im_Pallet = new im_Pallet();
                    im_Pallet.Pallet_Index = pallet_Index;
                    im_Pallet.Pallet_Id = data.pallet_Id;

                    if (!string.IsNullOrEmpty(data.palletDate))
                    {
                        string[] time = null;
                        time = data.pallettime_text.Split(':');
                        var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                        var pallet_date_db = (data.palletDate + _time).toDatetime();
                        im_Pallet.Pallet_Date = (DateTime)pallet_date_db;
                    }  
                    
                    im_Pallet.Vender_Index = data.vender_Index;
                    im_Pallet.Vender_Id = data.vender_Id;
                    im_Pallet.Vender_Name = data.vender_Name;
                    im_Pallet.DocumentType_Index = data.documentType_dropdown.documentType_Index;
                    im_Pallet.DocumentType_Id = data.documentType_dropdown.documentType_Id;
                    im_Pallet.DocumentType_Name = data.documentType_dropdown.documentType_Name;
                    im_Pallet.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                    im_Pallet.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                    im_Pallet.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                    im_Pallet.Pallet_license = data.pallet_license;
                    im_Pallet.Pallet_QtyPlan = data.pallet_QtyPlan;
                    im_Pallet.Pallet_QtyReceive = data.pallet_QtyReceive;
                    //im_Pallet.Pallet_QtyReturnGood = data.pallet_QtyReturnGood;
                    im_Pallet.Pallet_QtyReturnGood = 0;
                    im_Pallet.Pallet_QtyReturnDmg = data.pallet_QtyReturnDmg;
                    im_Pallet.Pallet_QtyTotal = data.pallet_QtyTotal;
                    im_Pallet.Document_status = 1;
                    im_Pallet.Ref_No1 = data.ref_No1;
                    im_Pallet.Ref_No2 = data.ref_No2; ///ปีพาเลท
                    im_Pallet.IsActive = true;
                    im_Pallet.IsDelete = false;
                    im_Pallet.IsSystem = false;
                    //im_Pallet.status_Id = 1;
                    im_Pallet.Create_By = UserBy;
                    im_Pallet.Create_Date = UserDate;
                    //if (!string.IsNullOrEmpty(data.createDate))
                    //{
                    //    string[] time = null;
                    //    time = data.createTime_text.Split(':');
                    //    var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                    //    var create_date_db = (data.createDate + _time).toDatetime();
                    //    im_Pallet.Create_Date = create_date_db;
                    //}
                    dbOb.im_Pallet.Add(im_Pallet);
                }
                else
                {
                    //// update im_Pallet_loc
                    pallet_Index = data.pallet_Index;
                    UserBy = data.update_By;

                    if (!string.IsNullOrEmpty(data.palletDate))
                    {
                        string[] time = null;
                        time = data.pallettime_text.Split(':');
                        var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                        var pallet_date_db = (data.palletDate + _time).toDatetime();
                        model.Pallet_Date = (DateTime)pallet_date_db;
                    }                   
                    model.Vender_Index = data.vender_Index;
                    model.Vender_Id = data.vender_Id;
                    model.Vender_Name = data.vender_Name;
                    model.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                    model.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                    model.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                    model.Pallet_license = data.pallet_license;
                    model.Pallet_QtyPlan = data.pallet_QtyPlan;
                    model.Pallet_QtyReceive = data.pallet_QtyReceive;
                    //model.Pallet_QtyReturnGood = data.pallet_QtyReturnGood;
                    model.Pallet_QtyReturnGood = 0;
                    model.Pallet_QtyReturnDmg = data.pallet_QtyReturnDmg;
                    model.Pallet_QtyTotal = data.pallet_QtyTotal;
                    model.Document_status = 2;
                    model.Ref_No1 = data.ref_No1;
                    model.Ref_No2 = data.ref_No2; ///ปีพาเลท
                    model.IsActive = true;
                    model.IsDelete = false;
                    model.IsSystem = false;
                    //model.status_Id = 2;
                    model.Update_By = data.update_By;
                    model.Update_Date = UserDate;

                    dbOb.im_Pallet_loc.Update(model);


                    ////update im_pallet
                    im_Pallet model_ = dbOb.im_Pallet.Find(data.pallet_Index);
                    if (model_ != null)
                    {
                        if (!string.IsNullOrEmpty(data.palletDate))
                        {
                            string[] time = null;
                            time = data.pallettime_text.Split(':');
                            var _time = time[0].PadLeft(2, '0') + ":" + time[1].PadLeft(2, '0');
                            var pallet_date_db = (data.palletDate + _time).toDatetime();
                            model_.Pallet_Date = (DateTime)pallet_date_db;
                        }
                        model_.Vender_Index = data.vender_Index;
                        model_.Vender_Id = data.vender_Id;
                        model_.Vender_Name = data.vender_Name;
                        model_.Vehicle_Index = data.vehicleType_dropdown.vehicleType_Index;
                        model_.VehicleType_Id = data.vehicleType_dropdown.vehicleType_Id;
                        model_.VehicleType_Name = data.vehicleType_dropdown.vehicleType_Name;
                        model_.Pallet_license = data.pallet_license;
                        model_.Pallet_QtyPlan = data.pallet_QtyPlan;
                        model_.Pallet_QtyReceive = data.pallet_QtyReceive;
                        //model_.Pallet_QtyReturnGood = data.pallet_QtyReturnGood;
                        model_.Pallet_QtyReturnGood = data.pallet_QtyReturnGood;
                        model_.Pallet_QtyReturnDmg = data.pallet_QtyReturnDmg;
                        model_.Pallet_QtyTotal = data.pallet_QtyTotal;
                        model_.Document_status = 2;
                        model_.Ref_No1 = data.ref_No1;
                        model_.Ref_No2 = data.ref_No2; ///ปีพาเลท
                        model_.IsActive = true;
                        model_.IsDelete = false;
                        model_.IsSystem = false;
                        //model.status_Id = 2;
                        model_.Update_By = data.update_By;
                        model_.Update_Date = UserDate;

                        dbOb.im_Pallet.Update(model_);
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
                Result.no = data.pallet_Id;
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Deletepallets
        public Result DeletePallet(PalletViewModel data)
        {
            var Result = new Result();
            try
            {
                im_Pallet_loc model = dbOb.im_Pallet_loc.Find(data.pallet_Index);
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

                    dbOb.im_Pallet_loc.Update(model);

                    ////update im_pallet
                    im_Pallet model_ = dbOb.im_Pallet.Find(data.pallet_Index);
                    if (model_ != null)
                    {
                        UserBy = data.update_By;
                        model_.Document_status = 0;
                        model_.IsDelete = true;
                        //model.status_Id = 0;
                       // model_.Update_By = data.cancel_By;
                        //model_.Update_Date = UserDate;
                        model_.Cancel_By = data.cancel_By;
                        model_.Cancel_Date = UserDate;

                        dbOb.im_Pallet.Update(model_);
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

        public PalletSumResultViewModel FilterSum()
        {
            var result = new PalletSumResultViewModel();

            try
            {
                var model_Pallet_year = dbOb.im_Pallet_loc.Where(c => c.Document_status != 0 && !c.IsDelete)
                                               .GroupBy(c => new { c.Ref_No2 })
                                             .Select(x => new
                                             {
                                                 Year = x.Key.Ref_No2,
                                                 Sum_Pallet_Plan = x.Sum(c => c.Pallet_QtyPlan),
                                                 Sum_Pallet_QtyReturnGood = x.Sum(c => c.Pallet_QtyReturnGood),
                                                 Sum_Pallet_QtyReceive = x.Sum(c => c.Pallet_QtyReceive),
                                                 Sum_Pallet_QtyReturnDmg = x.Sum(c => c.Pallet_QtyReturnDmg),
                                                 Sum_Pallet_Outstanding = x.Sum(c => c.Pallet_QtyReturnGood) - x.Sum(c => c.Pallet_QtyReceive),
                                                 Sum_Pallet_Qtybalance_plan = x.Where(c => c.DocumentType_Id == "P001").Sum(c => c.Pallet_QtyPlan),
                                                 Sum_Pallet_Qtybalance_receive = x.Where(c => c.DocumentType_Id == "P001").Sum(c => c.Pallet_QtyReceive),
                                                 Sum_Pallet_Qtybalance_receive_claim = x.Where(c => c.DocumentType_Id == "P002").Sum(c => c.Pallet_QtyReceive)
                                             }).ToList();

                var model_Pallet = dbOb.im_Pallet.Where(c => c.Document_status != 0 && !c.IsDelete)
                                                .GroupBy(c => new { c.Pallet_Date.Year })
                                              .Select(x => new
                                              {
                                                  Year = x.Key.Year,
                                                  Sum_Pallet_Plan = x.Sum(c => c.Pallet_QtyPlan),
                                                  Sum_Pallet_QtyReturnGood = x.Sum(c => c.Pallet_QtyReturnGood),
                                                  Sum_Pallet_QtyReceive = x.Sum(c => c.Pallet_QtyReceive),
                                                  Sum_Pallet_QtyReturnDmg = x.Sum(c => c.Pallet_QtyReturnDmg),
                                                  Sum_Pallet_Outstanding = x.Sum(c => c.Pallet_QtyReturnGood) - x.Sum(c => c.Pallet_QtyReceive),
                                                  Sum_Pallet_Repair = x.Where( c => c.Pallet_QtyPlan == 0).Sum(c => c.Pallet_QtyReceive),
                                                  Sum_pallet_QtyReturnSup = x.Where( c => c.DocumentType_Id == "P005").Sum(c => c.Pallet_QtyReceive),
                                                  Sum_pallet_QtyReturnOR = x.Where(c => c.DocumentType_Id == "P004").Sum(c => c.Pallet_QtyReceive),
                                                  Sum_Pallet_Qtybalance_plan = x.Where(c => c.DocumentType_Id == "P001").Sum(c => c.Pallet_QtyPlan),
                                                  Sum_Pallet_Qtybalance_receive = x.Where(c => c.DocumentType_Id == "P001").Sum(c => c.Pallet_QtyReceive),
                                                  Sum_Pallet_Qtybalance_receive_claim = x.Where(c => c.DocumentType_Id == "P002").Sum(c => c.Pallet_QtyReceive),
                                              }).ToList();

                var model_LentPallet = dbOb.im_LentPallet.Where(c => c.Document_status != 0 && !c.IsDelete)
                                                .GroupBy(c => new { c.LentPallet_Date.Year })
                                              .Select(x => new
                                              {
                                                  Year = x.Key.Year,
                                                  Sum_LentPallet_Lent_OR = x.Where(c => c.Vender_Id == "OR0001").Sum(c => c.LentPallet_QtyLent),
                                                  Sum_LentPallet_Lent_ = x.Sum(c => c.LentPallet_QtyLent),
                                                  Sum_LentPallet_ReturnDmg_OR = x.Where(c => c.Vender_Id == "OR0001").Sum(c => c.LentPallet_QtyReturnDmg),
                                                  Sum_LentPallet_ReturnGood_OR = x.Where(c => c.Vender_Id == "OR0001").Sum(c => c.LentPallet_QtyReturnGood),
                                                  Sum_LentPallet_Lent_Supplier = x.Where(c => c.Vender_Id != "OR0001").Sum(c => c.LentPallet_QtyLent),
                                                  Sum_LentPallet_ReturnDmg_Supplier = x.Where(c => c.Vender_Id != "OR0001").Sum(c => c.LentPallet_QtyReturnDmg),
                                                  Sum_LentPallet_ReturnGood_Supplier = x.Where(c => c.Vender_Id != "OR0001").Sum(c => c.LentPallet_QtyReturnGood),
                                                  Sum_Pallet_ReturnGood = x.Sum(c => c.LentPallet_QtyReturnGood),
                                                  Sum_Pallet_ReturnDmg = x.Sum(c => c.LentPallet_QtyReturnDmg) - x.Sum(c => c.LentPallet_QtyReturnGood),
                                                  Sum_Pallet_ReturnDmg_Repair = x.Sum(c => c.LentPallet_QtyReturnDmg)
                                              }).ToList();

                result.sum_Pallet_Plan = model_Pallet.Sum(c => c.Sum_Pallet_Plan);

                //if ( model_Pallet.Sum(c => c.Sum_Pallet_Plan) >= model_Pallet.Sum(c => c.Sum_Pallet_QtyReceive))
                //{
                //    result.sum_Pallet_Outstanding = model_Pallet.Sum(c => c.Sum_Pallet_Plan) - model_Pallet.Sum(c => c.Sum_Pallet_QtyReceive) ;
                //}
                result.sum_Pallet_Outstanding = (model_Pallet.Sum(c => c.Sum_Pallet_Qtybalance_plan) - model_Pallet.Sum(c => c.Sum_Pallet_Qtybalance_receive)) - model_Pallet.Sum(c => c.Sum_Pallet_Qtybalance_receive_claim);
                result.sum_LentPallet_Lent_OR = model_LentPallet.Sum(c => c.Sum_LentPallet_Lent_OR) - (model_LentPallet.Sum(c => c.Sum_LentPallet_ReturnGood_OR) + model_LentPallet.Sum(c => c.Sum_LentPallet_ReturnDmg_OR));
                result.sum_LentPallet_ReturnDmg_OR = (model_LentPallet.Sum(c => c.Sum_LentPallet_ReturnDmg_OR)) - (model_Pallet.Sum(c => c.Sum_pallet_QtyReturnOR));
                
                result.sum_LentPallet_Lent_Supplier = model_LentPallet.Sum(c => c.Sum_LentPallet_Lent_Supplier) - (model_LentPallet.Sum(c => c.Sum_LentPallet_ReturnGood_Supplier) + model_LentPallet.Sum(c => c.Sum_LentPallet_ReturnDmg_Supplier));

                result.sum_LentPallet_ReturnDmg_Supplier = (model_LentPallet.Sum(c => c.Sum_LentPallet_ReturnDmg_Supplier)) - (model_Pallet.Sum(c => c.Sum_pallet_QtyReturnSup));
                //result.sum_Pallet_ReturnGood = model_LentPallet.Sum(c => c.Sum_Pallet_ReturnGood);
                result.sum_Pallet_ReturnOR = model_Pallet.Sum(c => c.Sum_pallet_QtyReturnOR);
                result.sum_Pallet_ReturnSupplier = model_Pallet.Sum(c => c.Sum_pallet_QtyReturnSup);
                //result.sum_Pallet_ReturnDmg = model_LentPallet.Sum(c => c.Sum_Pallet_ReturnDmg);
                //result.sum_Pallet_ReturnDmg = model_LentPallet.Sum(c => c.Sum_Pallet_ReturnDmg_Repair) - result.sum_Pallet_ReturnGood;
                //result.sum_Pallet_ReturnDmg = (result.sum_LentPallet_ReturnDmg_OR - result.sum_Pallet_ReturnOR) + (result.sum_LentPallet_ReturnDmg_Supplier - result.sum_Pallet_ReturnSupplier);
                result.sum_Pallet_ReturnDmg = (result.sum_LentPallet_ReturnDmg_Supplier) + (result.sum_LentPallet_ReturnDmg_OR);


                //result.sum_Pallet_Available = result.sum_Pallet_Plan - result.sum_Pallet_Outstanding - 
                //                                result.sum_LentPallet_Lent_OR - result.sum_LentPallet_ReturnDmg_OR - 
                //                                result.sum_LentPallet_Lent_Supplier - result.sum_LentPallet_ReturnDmg_Supplier +
                //                                result.sum_Pallet_ReturnOR + result.sum_Pallet_ReturnSupplier;

                result.sum_Pallet_Available = result.sum_Pallet_Plan - result.sum_Pallet_Outstanding - result.sum_LentPallet_Lent_Supplier - result.sum_LentPallet_Lent_OR - result.sum_Pallet_ReturnDmg;
              
                var palletSumYearList = new List<PalletSumYearResultViewModel>();
                foreach (var item in model_Pallet_year)
                {
                    var palletSumYear = new PalletSumYearResultViewModel();
                    palletSumYear.year =  !string.IsNullOrEmpty(item.Year)?Convert.ToInt32(item.Year):0;
                    palletSumYear.sum_Pallet_Receive = item.Sum_Pallet_Plan;
                    palletSumYear.sum_Pallet_Outstanding = (item.Sum_Pallet_Qtybalance_plan - item.Sum_Pallet_Qtybalance_receive) - item.Sum_Pallet_Qtybalance_receive_claim;

                    //if (item.Sum_Pallet_QtyReturnDmg >= item.Sum_Pallet_QtyReturnGood)
                    //{
                    //palletSumYear.sum_Pallet_Outstanding = item.Sum_Pallet_QtyReturnDmg - item.Sum_Pallet_QtyReturnGood ;
                    //}
                    //else
                    //{
                    //    palletSumYear.sum_Pallet_Outstanding = item.Sum_Pallet_QtyReturnDmg;
                    //}
                   

                    palletSumYearList.Add(palletSumYear);
                }
                result.palletSumYear = palletSumYearList;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region printPallet
        public dynamic printPallet(PalletResultViewModel data, string rootPath = "")
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
                item.docket_no = data.pallet_Id;
                item.company_name = data.vender_Name;
                item.truck_registration = data.pallet_license;
                item.palletDate_text = !string.IsNullOrEmpty(data.palletDate_text) ? data.palletDate_text : "";
                item.pallet_year = data.ref_No2;
                item.return_qtygood = data.pallet_QtyReceive;
                item.return_qtydmg = data.pallet_QtyReturnDmg;
                item.return_qtytotal = data.pallet_QtyTotal;
                item.return_qty = data.pallet_QtyPlan;
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
                var reportPath = rootPath + new AppSettingConfig().GetUrl("ReportPrintPalletNew");
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
            var result = new List<ReportExportPalletViewModel>();
            try
            {
                DateTime dateStart = DateTime.Now.toString().toBetweenDate().start;
                DateTime dateEnd = DateTime.Now.toString().toBetweenDate().end;

                var model = dbOb.im_Pallet_loc.AsQueryable();

                if (!string.IsNullOrEmpty(data.vender_Index.ToString()) && data.vender_Index.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    model = model.Where(c => c.Vender_Index == data.vender_Index);
                }
                if (!string.IsNullOrEmpty(data.start_Date.ToString()) && !string.IsNullOrEmpty(data.end_Date.ToString()))
                {
                    dateStart = data.start_Date.toBetweenDate().start;
                    dateEnd = data.end_Date.toBetweenDate().end;
                    model = model.Where(c => c.Pallet_Date >= dateStart && c.Pallet_Date <= dateEnd);
                }
                if (!string.IsNullOrEmpty(data.pallet_Id))
                {
                    model = model.Where(c => data.pallet_Id.Contains(c.Pallet_Id.Replace("\r\n", "")));
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
                    var resultItem = new ReportExportPalletViewModel();
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
                        var impallet = new ReportExportPalletViewModel();
                        impallet.pallet_Id = item.Pallet_Id;
                        impallet.documentType_Name = item.DocumentType_Name;
                        impallet.vender_Name = item.Vender_Name;
                        impallet.vender_Id = item.Vender_Id;
                        //impallet.createDate_text = Convert.ToDateTime(item.Create_Date).ToString("dd/MM/yyyy HH:mm");
                        //impallet.palletDate_text = Convert.ToDateTime(item.Pallet_Date).ToString("dd/MM/yyyy HH:mm");
                        impallet.pallet_QtyPlan = item.Pallet_QtyPlan;
                        impallet.pallet_QtyReceive = item.Pallet_QtyReceive;
                        impallet.pallet_QtyReturnGood = item.Pallet_QtyReturnGood;
                        impallet.pallet_QtyReturnDmg = item.Pallet_QtyReturnDmg;
                        impallet.ref_No1 = !string.IsNullOrEmpty(item.Ref_No1) ? item.Ref_No1 : "NULL" ;
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
                            impallet.updateDate_text = "NULL";
                        }

                        if (item.Create_Date != null)
                        {
                            impallet.createDate = Convert.ToDateTime(item.Create_Date);
                            impallet.create_By = item.Create_By;
                        }
                        else
                        {
                            impallet.create_By = "NULL";
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

                        if (item.Pallet_Date != null)
                        {
                            impallet.palletDate = Convert.ToDateTime(item.Pallet_Date);
                        }
                        impallet.pallet_license = item.Pallet_license;
                        impallet.vehicleType_Name = item.VehicleType_Name;

                        if(item.Document_status == 0)
                            impallet.status_Name = "ยกเลิก";
                        else if (item.Document_status == 1)
                            impallet.status_Name = "รอยืนยัน";
                        else if (item.Document_status == 2)
                            impallet.status_Name = "แก้ไข";

                        result.Add(impallet);
                    }
                }

                rootPath = rootPath.Replace("\\PalletmanageAPI", "");
                var reportPath = rootPath + new AppSettingConfig().GetUrl("ReportExportPallet");

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
