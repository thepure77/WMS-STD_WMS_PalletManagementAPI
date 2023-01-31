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
using ClosedXML.Excel;

namespace OnhandSummaryBusiness
{
    public class OnhandSummaryService
    {
        private MasterDbContext db;
        private OutboundDbContext dbOb;

        public OnhandSummaryService()
        {
            db = new MasterDbContext();
            dbOb = new OutboundDbContext();
        }

        public OnhandSummaryService(MasterDbContext db,OutboundDbContext dbOb)
        {
            this.db = db;
            this.dbOb = dbOb;
        }


        public ResultOnhandSummaryViewModel Filter(OnhandSummaryViewModel model)
        {
            //GET CONFIG CONNECTIONSTRING
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false);
            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("Outbound_ConnectionString").ToString();
            SqlConnection conn = new SqlConnection(connectionString);
            // END

            var result_ = new ResultOnhandSummaryViewModel();
            DataTable dt = new DataTable();           
            var dateStart = DateTime.Now.toString().toCVDateString();
            var dateEnd = DateTime.Now.toString().toCVDateString();            
            try
            {
                if (!string.IsNullOrEmpty(model.start_Date.ToString()) && !string.IsNullOrEmpty(model.end_Date.ToString()))
                {
                    dateStart =    model.start_Date.toCVDateString();
                    dateEnd = model.end_Date.toCVDateString();
                }

                var vendor = "";
                if (model.listVendor != null && model.listVendor.Count() > 0)
                {
                    foreach (var i in model.listVendor)
                    {
                        if (vendor == "")
                        {
                            vendor = i.id;
                        }
                        else
                        {
                            vendor = vendor + "," + i.id;
                        }

                    }

                    vendor = "'" + vendor + "'";
                }
                else
                {
                    vendor = "null";
                }
                
                if (model.types == "01")
                {
                    var data = new ResultDataViewModel();

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',null,'01',1"; 


                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = new SqlCommand(strsql, conn);
                    da.Fill(ds, "result");
                    dt = ds.Tables["result"];

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (dt.Rows.Count > 0)
                    {                      

                        if (dt.Columns.Count > 0)
                        {
                            
                            List<string> str_list = new List<string>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "flex")
                                {
                                    string str = col.ColumnName;
                                    str_list.Add(str);
                                }
                            }
                            data.header = str_list;
                        }
                        dt.Columns.Remove("flex");
                        data.detail = dt;
                    }

                    result_.pttor_data = data;

                    //// SUM LentPallet_QtyReturnDmg

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql_sumqtyDmg = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',null,'01',2";

                    SqlDataAdapter da2 = new SqlDataAdapter();
                    DataSet ds2 = new DataSet();
                    da2.SelectCommand = new SqlCommand(strsql_sumqtyDmg, conn);
                    da2.Fill(ds2, "result");
                    

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (ds2.Tables["result"].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds2.Tables["result"].Rows)
                        {
                            if (!string.IsNullOrEmpty(item["sum_qtyDmg"].ToString()))
                                result_.sumdmg_or = Convert.ToInt32(item["sum_qtyDmg"]);
                        }
                    }
                }
                else if (model.types == "02")
                {
                    var data = new ResultDataViewModel();

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',{vendor},'02',1";


                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = new SqlCommand(strsql, conn);
                    da.Fill(ds, "result");
                    dt = ds.Tables["result"];

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (dt.Rows.Count > 0)
                    {

                        if (dt.Columns.Count > 0)
                        {

                            List<string> str_list = new List<string>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "flex")
                                {
                                    string str = col.ColumnName;
                                    str_list.Add(str);
                                }
                            }
                            data.header = str_list;
                        }
                        dt.Columns.Remove("flex");
                        data.detail = dt;
                    }
                    result_.supplier_data = data;

                    //// SUM LentPallet_QtyReturnDmg

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql_sumqtyDmg = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',{vendor},'02',2";

                    SqlDataAdapter da2 = new SqlDataAdapter();
                    DataSet ds2 = new DataSet();
                    da2.SelectCommand = new SqlCommand(strsql_sumqtyDmg, conn);
                    da2.Fill(ds2, "result");


                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (ds2.Tables["result"].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds2.Tables["result"].Rows)
                        {
                            if (!string.IsNullOrEmpty(item["sum_qtyDmg"].ToString()))
                                result_.sumdmg_supplier = Convert.ToInt32(item["sum_qtyDmg"]);
                        }
                    }
                }
                else
                {
                    /// data_pttor ///
                    var pttor_data = new ResultDataViewModel();

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',null,'01',1";


                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    da.SelectCommand = new SqlCommand(strsql, conn);
                    da.Fill(ds, "result");
                    dt = ds.Tables["result"];

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (dt.Rows.Count > 0)
                    {

                        if (dt.Columns.Count > 0)
                        {

                            List<string> str_list = new List<string>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "flex")
                                {
                                    string str = col.ColumnName;
                                    str_list.Add(str);
                                }
                            }
                            pttor_data.header = str_list;
                        }
                        dt.Columns.Remove("flex");
                        pttor_data.detail = dt;
                    }

                    result_.pttor_data = pttor_data;

                    //// SUM LentPallet_QtyReturnDmg

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql_sumqtyDmg = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',null,'01',2";

                    SqlDataAdapter da2 = new SqlDataAdapter();
                    DataSet ds2 = new DataSet();
                    da2.SelectCommand = new SqlCommand(strsql_sumqtyDmg, conn);
                    da2.Fill(ds2, "result");


                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (ds2.Tables["result"].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds2.Tables["result"].Rows)
                        {
                            if (!string.IsNullOrEmpty(item["sum_qtyDmg"].ToString()))
                                result_.sumdmg_or = Convert.ToInt32(item["sum_qtyDmg"]);
                        }                           
                    }

                    /// end data_pttor ///



                    /// data_subplier ///
                    var supplier_data = new ResultDataViewModel();

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    
                    var strsql_sub = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',{vendor},'02',1";


                    SqlDataAdapter da_2 = new SqlDataAdapter();
                    DataSet ds_2 = new DataSet();
                    da_2.SelectCommand = new SqlCommand(strsql_sub, conn);
                    da_2.Fill(ds_2, "result");
                    dt = ds_2.Tables["result"];

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (dt.Rows.Count > 0)
                    {

                        if (dt.Columns.Count > 0)
                        {

                            List<string> str_list = new List<string>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName != "flex")
                                {
                                    string str = col.ColumnName;
                                    str_list.Add(str);
                                }
                            }
                            supplier_data.header = str_list;
                        }

                        dt.Columns.Remove("flex");
                        supplier_data.detail = dt;
                    }

                    result_.supplier_data = supplier_data;

                    //// SUM LentPallet_QtyReturnDmg

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    var strsql_sumqtyDmg2 = $@"EXEC sp_GetData_OnhandSummary_Report '{dateStart}','{dateEnd}',{vendor},'02',2";

                    SqlDataAdapter da_sum = new SqlDataAdapter();
                    DataSet ds_sum = new DataSet();
                    da_sum.SelectCommand = new SqlCommand(strsql_sumqtyDmg2, conn);
                    da_sum.Fill(ds_sum, "result");


                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (ds_sum.Tables["result"].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds_sum.Tables["result"].Rows)
                        {
                            if (!string.IsNullOrEmpty(item["sum_qtyDmg"].ToString()))
                                result_.sumdmg_supplier = Convert.ToInt32(item["sum_qtyDmg"]);
                        }
                    }
                    /// end data_subplier ///
                }
                return result_;
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                throw ex;
            }
        }

        #region exportExcel        
        public dynamic generate_excel(OnhandSummaryViewModel Models, string rootPath = "")
        {          
            try
            {
                var report_data = Filter(Models);

                var wb = new XLWorkbook();
                var ws2 = wb.Worksheets.Add("On Hand Summary", 0);
                ws2.PageSetup.PagesWide = 1;
                ws2.PageSetup.Margins.Top = 0.5;
                ws2.PageSetup.Margins.Bottom = 0.76;
                ws2.PageSetup.Margins.Left = 0.70;
                ws2.PageSetup.Margins.Right = 0.10;
                ws2.PageSetup.Margins.Footer = 0.26;
                ws2.PageSetup.Margins.Header = 0.26;
                ws2.PageSetup.PaperSize = XLPaperSize.A4Paper;
                get_detail(ws2, report_data, Models);

                //rootPath = "\\ReportAPI";
                rootPath = rootPath.Replace("\\PalletmanageAPI", "");
                string fileName = "On_Hand_Summary_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Utils objReport = new Utils();
                var saveLocation = objReport.PhysicalPath(fileName, rootPath);
                wb.SaveAs(saveLocation);
                if (File.Exists(saveLocation))
                    return saveLocation;
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception("Create Fail => " + ex);
            }
        }
        public void get_detail(IXLWorksheet xSheet, ResultOnhandSummaryViewModel data, OnhandSummaryViewModel filter)
        {
            xSheet.Columns().Style.Font.FontSize = 14;
            xSheet.Columns().Style.Font.FontName = "TH Sarabun New";

            xSheet.Columns().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            xSheet.Columns("B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            xSheet.Cell("A1").Value = "On Hand Summary";
            xSheet.Cell("A1").Style.Font.FontSize = 20;
            xSheet.Cell("A1").Style.Font.Bold = true;
            xSheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            xSheet.Range("A1:D1").Merge();

            if (filter.types == "01")
            {
                xSheet.Cell("A3").Value = "รวมพาเลทที่ใช้งานไม่ได้ :  " + data.sumdmg_or;
                xSheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                xSheet.Range("A3:C3").Merge();


                if (data.pttor_data.detail != null)
                {

                    ///Gen Header
                    var row = 5;
                    var colunm = 1;
                    foreach (var header in data.pttor_data.header)
                    {
                        xSheet.Cell(row, colunm).Value = header;
                        xSheet.Column(colunm).Width = 12;
                        xSheet.Cell(row, colunm).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        colunm++;
                    }
                    xSheet.Range(xSheet.Cell(row,1), xSheet.Cell(row,(colunm - 1))).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    ///Gen Detail
                    row = row + 1;
                    var lastrow = row + data.pttor_data.detail.Rows.Count;
                    xSheet.Cell(row, 1).InsertData(data.pttor_data.detail.Rows);

                    /// วาดเส้นตาราง
                    var range = xSheet.Range(xSheet.Cell(5, 1), xSheet.Cell((lastrow - 1), (colunm - 1)));
                    range.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                }
                else
                {
                    ///Gen Header
                    var row = 5;
                    var colunm = 1;
                    xSheet.Cell(row, colunm).Value = "NO DATA";
                    xSheet.Range("A" + row + ":P" + row).Merge();
                    xSheet.Range(xSheet.Cell("A" + row), xSheet.Cell("P" + row)).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    /// วาดเส้นตาราง
                    var range = xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, 16));
                    range.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;

                }
            }
            else if (filter.types == "02")
            {
                xSheet.Cell("A3").Value = "รวมพาเลทที่ใช้งานไม่ได้ :  " + data.sumdmg_supplier;
                xSheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                xSheet.Range("A3:C3").Merge();

                //xSheet.Cell("D3").Value = data.sumdmg_supplier;

                if (data.supplier_data.detail != null)
                {
                    ///Gen Header
                    var row = 5;
                    var colunm = 1;
                    foreach (var header in data.supplier_data.header)
                    {
                        xSheet.Cell(row, colunm).Value = header;
                        xSheet.Column(colunm).Width = 12;
                        xSheet.Cell(row, colunm).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        colunm++;
                    }
                    xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, (colunm - 1))).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    ///Gen Detail
                    row = row + 1;
                    var lastrow = row + data.supplier_data.detail.Rows.Count;
                    xSheet.Cell(row, 1).InsertData(data.supplier_data.detail.Rows);

                    /// วาดเส้นตาราง
                    var range = xSheet.Range(xSheet.Cell(5, 1), xSheet.Cell((lastrow - 1), (colunm - 1)));
                    range.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                }
                else
                {
                    ///Gen Header
                    var row = 5;
                    var colunm = 1;
                    xSheet.Cell(row, colunm).Value = "NO DATA";
                    xSheet.Range("A" + row + ":P" + row).Merge();
                    xSheet.Range(xSheet.Cell("A" + row), xSheet.Cell("P" + row)).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    /// วาดเส้นตาราง
                    var range = xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, 16));
                    range.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                }
            }
            else
            {
                xSheet.Cell("A3").Value = "รวมพาเลทที่ใช้งานไม่ได้ :  " + data.sumdmg_or;
                xSheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                xSheet.Range("A3:C3").Merge();

               // xSheet.Cell("D3").Value = data.sumdmg_or;

                var row = 1;
                var colunm = 1;
                var lastrow = row;

                if (data.pttor_data.detail != null)
                {
                    ///Gen Header
                    row = 5;
                    colunm = 1;
                    foreach (var header in data.pttor_data.header)
                    {
                        xSheet.Cell(row, colunm).Value = header;
                        xSheet.Column(colunm).Width = 12;
                        xSheet.Cell(row, colunm).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        colunm++;
                    }
                    xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, (colunm - 1))).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    ///Gen Detail
                    row = row + 1;
                    lastrow = row + data.pttor_data.detail.Rows.Count;
                    xSheet.Cell(row, 1).InsertData(data.pttor_data.detail.Rows);

                    /// วาดเส้นตาราง
                    var range = xSheet.Range(xSheet.Cell(5, 1), xSheet.Cell((lastrow - 1), (colunm - 1)));
                    range.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                }
                else
                {
                    ///Gen Header
                    row = 5;
                    colunm = 1;
                    lastrow = row;
                    xSheet.Cell(row, colunm).Value = "NO DATA";
                    xSheet.Range("A" + row + ":P" + row).Merge();
                    xSheet.Range(xSheet.Cell("A" + row), xSheet.Cell("P" + row)).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    /// วาดเส้นตาราง
                    var range = xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, 16));
                    range.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                }

               
                //// Gen table supplier
                row = lastrow + 5;
                colunm = 1;
                var start_table2 = row;

                xSheet.Cell("A" + row).Value = "รวมพาเลทที่ใช้งานไม่ได้ :  " + data.sumdmg_supplier;
                xSheet.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                xSheet.Range("A" + row + ":C" + row).Merge();

                //xSheet.Cell("D" + row).Value = data.sumdmg_supplier;

                if (data.supplier_data.detail != null)
                {
                    ///Gen Header
                    row = row + 2;
                    start_table2 = row;
                    foreach (var header in data.supplier_data.header)
                    {
                        xSheet.Cell(row, colunm).Value = header;
                        xSheet.Column(colunm).Width = 12;
                        xSheet.Cell(row, colunm).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        colunm++;
                    }
                    xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, (colunm - 1))).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    ///Gen Detail
                    row = row + 1;
                    var lastrow2 = row + data.supplier_data.detail.Rows.Count;
                    xSheet.Cell(row, 1).InsertData(data.supplier_data.detail.Rows);

                    /// วาดเส้นตาราง
                    var range2 = xSheet.Range(xSheet.Cell(start_table2, 1), xSheet.Cell((lastrow2 - 1), (colunm - 1)));
                    range2.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range2.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range2.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range2.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                   
                }
                else
                {
                    ///Gen Header
                    row = row + 2;
                    colunm = 1;
                    xSheet.Cell(row, colunm).Value = "NO DATA";
                    xSheet.Range("A" + row + ":P" + row).Merge();
                    xSheet.Range(xSheet.Cell("A" + row), xSheet.Cell("P" + row)).Style.Fill.SetBackgroundColor(XLColor.LightGray);

                    /// วาดเส้นตาราง
                    var range2 = xSheet.Range(xSheet.Cell(row, 1), xSheet.Cell(row, 16));
                    range2.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.Black;
                    range2.Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.TopBorderColor = ClosedXML.Excel.XLColor.Black;
                    range2.Style.Border.LeftBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.LeftBorderColor = ClosedXML.Excel.XLColor.Black;
                    range2.Style.Border.RightBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    range2.Style.Border.RightBorderColor = ClosedXML.Excel.XLColor.Black;
                }
            }
            xSheet.Column("B").Width = 36.60;            
        }
        #endregion
    }
}
