using MXApi.Models;
using MXApp.MobileService.Helpers;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MXApp.MobileService.Controllers
{
    [Serializable]
    public class ProductionController : ApiController
    {
        SkyNetEntities db = new SkyNetEntities();
        // GET api/values
        [HttpGet]
        [Route("api/Production/GetOpenOrders/")]
        public IHttpActionResult GetOpenOrders()
        {
            var res = db.sp_mob_OpenProdWO().ToList();
            return Json(res);
        }
        [HttpGet]
        [Route("api/Production/UpdateWO/{id}")]
        public IHttpActionResult UpdateWO(int id)
        {
            var res = db.sp_mob_UpdateWOStart(id);
            return Json(res);
        }

        [HttpGet]
        [Route("api/Production/ListWOProds/{id}")]
        public IHttpActionResult ListWOProds(int id)
        {
            var res = db.sp_mob_ListProdWODetails(id).ToList();
            return Json(res);
        }

        [HttpGet]
        [Route("api/Production/GetBillOrdersList/{account}")]
        public IHttpActionResult GetBillOrdersList(string account)
        {
            var res = db.sp_mob_AddProdWODetails(account).ToList();
            return Json(res);
        }

        [HttpPost]
        [Route("api/Production/UpdateProdWOItem")]
        public IHttpActionResult UpdateProdWOItem([FromBody]ProdWODetail model)
        {
            var res = db.sp_mob_UpdateProdWODetails(model.CodeID, model.QTY.Value, model.BillComments);
            return Json(res);
        }
        [HttpPost]
        [Route("api/Production/DeleteProdItem")]
        public IHttpActionResult DeleteProdItem([FromBody]DeleteBillingCodeModel model)
        {
            //before deleting get a copy of it
            RefCode rc = db.RefCodes.Where(x => x.CodeID == model.CodeID).FirstOrDefault();
            var res = db.sp_mob_DeleteProdWODetails(model.CodeID);
            string filenm = "Work Order Delete Confirmation -" + model.CodeID.ToString() + ".xlsx".RemoveUnnecessary();
            string ReportPath = HttpContext.Current.Server.MapPath("~/MobileFile/") + filenm;
            SendMailAndArchiveDelete(model.Account, model.RefNum, model.UserName, model.Vendor, rc, ReportPath);
            return Json(res);
        }

        [HttpPost]
        [Route("api/Production/ConfirmWO")]
        public IHttpActionResult ConfirmWO([FromBody]ConfirmWOModel model)
        {
            var res = db.sp_mob_ConfirmProdWODetails(model.RefNum);
            Random rnd = new Random();
            string filenm = "Work Order Confirmation -" + model.RefNum.ToString() + rnd.Next(10000, 999999) + ".xlsx".RemoveUnnecessary();
            string ReportPath = HttpContext.Current.Server.MapPath("~/MobileFile/") + filenm;
            SendMailAndArchive(model.RefNum, model.UserName, model.Vendor, ReportPath, true);
            return Json(res);
        }

        [HttpPost]
        [Route("api/Production/InsertBillingCode")]
        public IHttpActionResult InsertBillingCode([FromBody]AddBillingCodeModel model)
        {
            var res = db.sp_mob_AppendProdWODetails(model.Account, model.RefNum.ToString(), model.BillCode);
            Random rnd = new Random();
            string filenm = "Work Order Item Added -" + model.RefNum.ToString() + rnd.Next(10000, 999999) + ".xlsx".RemoveUnnecessary();
            string ReportPath = HttpContext.Current.Server.MapPath("~/MobileFile/") + filenm;
            SendMailAndArchive(model.RefNum, model.UserName, model.Vendor, ReportPath, false);
            return Json(res);
        }

        #region Create Report, Archive and Mail
        #region Insert/ Complete
        private void SendMailAndArchive(int refNum, string username, string Vendor, string path, bool isConfirm)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                WMS_Explorer explorer = db.WMS_Explorer.FirstOrDefault();
                if (explorer != null)
                {
                    if (!string.IsNullOrEmpty(explorer.WMS_PODShareDrivePath))
                    {
                        var data1 = db.sp_mob_ListProdWODetails(refNum).ToList();
                        var data2 = db.sp_mob_GetWorkOrderReportHeader(refNum).FirstOrDefault();
                        string ReportFile = "";
                        if (isConfirm == true)
                        {
                            var timeLog = db.tf_mob_prod_CalcWOHours(refNum).ToList();
                            var data3 = db.sp_mob_prod_WOHourDetails(refNum).ToList();
                            ReportFile = CreateExcelReportConfirm(refNum, data1, data2, path, timeLog, data3);
                        }
                        else
                            ReportFile = CreateExcelReport(refNum, data1, data2, path);
                        if (isConfirm)
                        {
                            CopyPODToDrive(ReportFile, explorer.WMS_PODShareDrivePath);
                            FileInfo info = new FileInfo(ReportFile);
                            POD_Repository repository = new POD_Repository();
                            repository.PODKey = refNum.ToString();
                            repository.PODBorn = DateTime.Now;
                            repository.PODCreatedBy = username;
                            repository.PODDescription = "WO Bill Code Added";
                            repository.PODLink = explorer.WMS_PODShareDrivePath + "\\" + info.Name;
                            db.POD_Repository.Add(repository);
                            db.SaveChanges();
                        }
                        string loginEmail = db.Accounts.Where(x => x.Vendor__ == Vendor).FirstOrDefault().acc_billing_email;
                        if (!string.IsNullOrEmpty(loginEmail))
                        {
                            List<string> tom = new List<string>(); ;
                            tom.Add(loginEmail);
                            string subj = "";
                            if (isConfirm)
                                subj = "Work Order Confirmation -";
                            else
                                subj = "Work Order Item Added -";
                            Helpers.EmailSender.SendWorkOrderReportEmail(explorer, tom, ReportFile, subj + refNum.ToString());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RefNum"></param>
        /// <param name="type">0 for add, 1 for delete</param>
        private string CreateExcelReport(int RefNum, List<sp_mob_ListProdWODetails_Result> data, sp_mob_GetWorkOrderReportHeader_Result headerData, string path)
        {
            //string filenm = "Work Order Add Confirmation -" + RefNum.ToString() + DateTime.Now.ToString() + ".xlsx".RemoveUnnecessary();
            string ReportPath = path;
            FileInfo fileInfo = new FileInfo(ReportPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                var workSheet = package.Workbook.Worksheets.Add("REF_" + RefNum);
                workSheet.View.ShowGridLines = true;
                workSheet.DefaultColWidth = 18;

                ExcelRange Rng = workSheet.Cells[1, 1, 3, 8];
                GetHeader(Rng, RefNum);
                GetInfoCell(workSheet, headerData);
                GetWorkOrderSummaryHeader(workSheet);
                GetWorkOrderRowsHeader(workSheet);
                InsertDataForWorkOrder(workSheet, data);

                package.Save();
            }
            return ReportPath;

        }
        private void GetHeader(ExcelRange Rng, int? refNum)
        {
            Rng.Merge = true;
            Rng.Value = "Work Order Details for Ref " + refNum.ToString() + "";
            Rng.Style.Font.Size = 20;
            Rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            Rng.Style.Font.Bold = true;
            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(32, 55, 100));
        }
        private void GetInfoCell(ExcelWorksheet worksheet, sp_mob_GetWorkOrderReportHeader_Result headerRow)
        {
            worksheet.Cells["A4"].Value = "AccountName";
            worksheet.Cells["A5"].Value = "RefNumShipTo";
            worksheet.Cells["A6"].Value = "RefNumRef1";
            worksheet.Cells["A7"].Value = "RefNumRef2";
            worksheet.Cells["A8"].Value = "RefNumComments";
            worksheet.Cells["A9"].Value = "ESD_ShipDate";
            worksheet.Cells["A10"].Value = "ESD_CancelDate";
            worksheet.Cells["A11"].Value = "ProductionCode";
            worksheet.Cells["A12"].Value = "ReferencePullID";
            worksheet.Cells["A4:A12"].AutoFitColumns();
            //merge cells

            worksheet.Cells["B4:C4"].Merge = true;
            worksheet.Cells["B5:C5"].Merge = true;
            worksheet.Cells["B6:C6"].Merge = true;
            worksheet.Cells["B7:C7"].Merge = true;
            worksheet.Cells["B8:C8"].Merge = true;
            worksheet.Cells["B9:C9"].Merge = true;
            worksheet.Cells["B10:C10"].Merge = true;
            worksheet.Cells["B11:C11"].Merge = true;
            worksheet.Cells["B12:C12"].Merge = true;

            worksheet.Cells["B4"].Value = "     " + headerRow.AccountName + "     ";
            worksheet.Cells["B5"].Value = "     " + headerRow.RefNumShipTo + "     ";
            worksheet.Cells["B6"].Value = "     " + headerRow.RefNumRef1 + "     ";
            worksheet.Cells["B7"].Value = "     " + headerRow.RefNumRef2 + "     ";
            worksheet.Cells["B8"].Value = "  " + headerRow.RefNumComments + " ";
            worksheet.Cells["B9"].Value = "     " + headerRow.ESD_ShipDate + "     ";
            worksheet.Cells["B9"].Style.Numberformat.Format = "m/d/yy";
            worksheet.Cells["B10"].Value = "     " + headerRow.ESD_CancelDate + "     ";
            worksheet.Cells["B10"].Style.Numberformat.Format = "m/d/yy";
            worksheet.Cells["B11"].Value = "     " + headerRow.ProductionCode + "     ";
            worksheet.Cells["B12"].Value = "     " + headerRow.ReferencePullID + "     ";
            worksheet.Cells["B4:C12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["B4:C12"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(155, 194, 230));
            worksheet.Cells["B4:C12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B4:C12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["A4:C12"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C12"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["B4:C12"].AutoFitColumns(20);
        }

        private void GetWorkOrderSummaryHeader(ExcelWorksheet worksheet, bool isManual = false)
        {
            worksheet.Cells["A16:F16"].Merge = true;
            worksheet.Cells["A16"].Value = "Work Order Summary";
            worksheet.Cells["A16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A16"].Style.Font.Bold = true;
            worksheet.Cells["A16"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            worksheet.Cells["A16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A16"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(32, 55, 100));
            worksheet.Cells["A16:F16"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A16:F16"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A16:F16"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A16:F16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        private void GetWorkOrderRowsHeader(ExcelWorksheet worksheet, bool isManual = false)
        {
            worksheet.Cells["A17"].Value = "BillCodeDescription";
            worksheet.Cells["B17"].Value = "QTY";
            worksheet.Cells["C17"].Value = "BillComments";
            worksheet.Cells["D17"].Value = "BillCode";
            worksheet.Cells["E17"].Value = "Account";
            worksheet.Cells["F17"].Value = "StaticOutboundRate";
            worksheet.Cells["A17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["D17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["B17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["C17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["D17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["E17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A17:F17"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].AutoFitColumns(20);
        }

        private void InsertDataForWorkOrder(ExcelWorksheet worksheet, List<sp_mob_ListProdWODetails_Result> data)
        {
            int startCellNo = 18;
            string LastStyle = "";
            foreach (var item in data)
            {
                worksheet.Cells["A" + startCellNo].Value = item.BillCodeDescription;
                worksheet.Cells["B" + startCellNo].Value = item.QTY;
                worksheet.Cells["C" + startCellNo].Value = item.BillComments;
                worksheet.Cells["D" + startCellNo].Value = item.BillCode;
                worksheet.Cells["E" + startCellNo].Value = item.Account;
                worksheet.Cells["F" + startCellNo].Value = item.StaticOutboundRate;
                startCellNo = startCellNo + 1;
            }
            startCellNo = startCellNo - 1;
            worksheet.Cells["A18:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F" + startCellNo].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F" + startCellNo].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F" + startCellNo].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F" + startCellNo].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["A18:F" + startCellNo].AutoFitColumns();
        }


        #endregion

        #region ConfirmMail
        private string CreateExcelReportConfirm(int RefNum, List<sp_mob_ListProdWODetails_Result> data, sp_mob_GetWorkOrderReportHeader_Result headerData, string path, List<tf_mob_prod_CalcWOHours_Result> time, List<sp_mob_prod_WOHourDetails_Result> data3)
        {
            //string filenm = "Work Order Add Confirmation -" + RefNum.ToString() + DateTime.Now.ToString() + ".xlsx".RemoveUnnecessary();
            string ReportPath = path;
            FileInfo fileInfo = new FileInfo(ReportPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                var workSheet = package.Workbook.Worksheets.Add("REF_" + RefNum);
                workSheet.View.ShowGridLines = true;
                workSheet.DefaultColWidth = 18;

                ExcelRange Rng = workSheet.Cells[1, 1, 3, 8];
                GetHeaderConfirm(Rng, RefNum);
                GetInfoCellConfirm(workSheet, headerData, time.FirstOrDefault());
                GetWorkOrderSummaryHeaderConfirm(workSheet);
                GetWorkOrderRowsHeaderConfirm(workSheet);
                InsertDataForWorkOrderConfirm(workSheet, data);

                var TrackWorkSheet = package.Workbook.Worksheets.Add("WO TIME LOG Details");
                TrackWorkSheet.View.ShowGridLines = true;
                TrackWorkSheet.DefaultColWidth = 18;
                GetHeadersForTimeLog(TrackWorkSheet);
                InsertDataForTimeLog(TrackWorkSheet, data3);

                package.Save();
            }
            return ReportPath;

        }
        private void InsertDataForTimeLog(ExcelWorksheet worksheet, List<sp_mob_prod_WOHourDetails_Result> data)
        {
            if (data != null && data.Count > 0)
            {
                int startCellNo = 2;
                foreach (var item in data)
                {
                    worksheet.Cells["A" + startCellNo].Value = item.ReferencePullID;
                    worksheet.Cells["B" + startCellNo].Value = item.EmployeeID;
                    worksheet.Cells["C" + startCellNo].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                    worksheet.Cells["C" + startCellNo].Value = item.InTimeStamp;
                    worksheet.Cells["D" + startCellNo].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss";
                    worksheet.Cells["D" + startCellNo].Value = item.OutTimeStamp;
                    worksheet.Cells["E" + startCellNo].Value = item.WOHours;
                    worksheet.Cells["F" + startCellNo].Value = item.WOMinutes;
                    worksheet.Cells["G" + startCellNo].Value = item.FirstName;
                    worksheet.Cells["H" + startCellNo].Value = item.Lastname;
                    if (item.OutTimeStamp == null)
                    {
                        worksheet.Cells["A" + startCellNo + ":H" + startCellNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["A" + startCellNo + ":H" + startCellNo].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 0, 0));
                    }
                    startCellNo = startCellNo + 1;
                }
                startCellNo = startCellNo - 1;
                worksheet.Cells["A2:H" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:H" + startCellNo].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:H" + startCellNo].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:H" + startCellNo].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A2:H" + startCellNo].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A2:H" + startCellNo].AutoFitColumns();
            }
        }
        private void GetHeadersForTimeLog(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].Value = "ReferencePullID";
            worksheet.Cells["B1"].Value = "EmployeeID";
            worksheet.Cells["C1"].Value = "InTimeStamp";
            worksheet.Cells["D1"].Value = "OutTimeStamp";
            worksheet.Cells["E1"].Value = "WOHours";
            worksheet.Cells["F1"].Value = "WOMinutes";
            worksheet.Cells["G1"].Value = "FirstName";
            worksheet.Cells["H1"].Value = "LastName";
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["B1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["F1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["G1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["B1"].Style.Font.Bold = true;
            worksheet.Cells["C1"].Style.Font.Bold = true;
            worksheet.Cells["D1"].Style.Font.Bold = true;
            worksheet.Cells["E1"].Style.Font.Bold = true;
            worksheet.Cells["F1"].Style.Font.Bold = true;
            worksheet.Cells["G1"].Style.Font.Bold = true;
            worksheet.Cells["H1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["B1"].Style.Font.Bold = true;
            worksheet.Cells["C1"].Style.Font.Bold = true;
            worksheet.Cells["D1"].Style.Font.Bold = true;
            worksheet.Cells["E1"].Style.Font.Bold = true;
            worksheet.Cells["F1"].Style.Font.Bold = true;
            worksheet.Cells["G1"].Style.Font.Bold = true;
            worksheet.Cells["H1"].Style.Font.Bold = true;


            worksheet.Cells["A1:H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:H1"].AutoFitColumns(30);
        }
        private void GetHeaderConfirm(ExcelRange Rng, int? refNum)
        {
            Rng.Merge = true;
            Rng.Value = "Work Order Details for Ref " + refNum.ToString() + "";
            Rng.Style.Font.Size = 20;
            Rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            Rng.Style.Font.Bold = true;
            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(32, 55, 100));
        }
        private void GetInfoCellConfirm(ExcelWorksheet worksheet, sp_mob_GetWorkOrderReportHeader_Result headerRow, tf_mob_prod_CalcWOHours_Result time)
        {
            if (time != null)
            {
                if (time.WOHours == null)
                    time.WOHours = 0;
                if (time.WOMinutes == null)
                    time.WOMinutes = 0;
            }
            worksheet.Cells["A4"].Value = "AccountName";
            worksheet.Cells["A5"].Value = "RefNumShipTo";
            worksheet.Cells["A6"].Value = "RefNumRef1";
            worksheet.Cells["A7"].Value = "RefNumRef2";
            worksheet.Cells["A8"].Value = "RefNumComments";
            worksheet.Cells["A9"].Value = "ESD_ShipDate";
            worksheet.Cells["A10"].Value = "ESD_CancelDate";
            worksheet.Cells["A11"].Value = "ProductionCode";
            worksheet.Cells["A12"].Value = "ReferencePullID";
            worksheet.Cells["A13"].Value = "WO TIME LOG";
            worksheet.Cells["A14"].Value = "WO HEAD COUNT";
            worksheet.Cells["A4:A14"].AutoFitColumns();
            //merge cells

            worksheet.Cells["B4:C4"].Merge = true;
            worksheet.Cells["B5:C5"].Merge = true;
            worksheet.Cells["B6:C6"].Merge = true;
            worksheet.Cells["B7:C7"].Merge = true;
            worksheet.Cells["B8:C8"].Merge = true;
            worksheet.Cells["B9:C9"].Merge = true;
            worksheet.Cells["B10:C10"].Merge = true;
            worksheet.Cells["B11:C11"].Merge = true;
            worksheet.Cells["B12:C12"].Merge = true;
            worksheet.Cells["B13:C13"].Merge = true;
            worksheet.Cells["B14:C14"].Merge = true;

            worksheet.Cells["B4"].Value = "     " + headerRow.AccountName + "     ";
            worksheet.Cells["B5"].Value = "     " + headerRow.RefNumShipTo + "     ";
            worksheet.Cells["B6"].Value = "     " + headerRow.RefNumRef1 + "     ";
            worksheet.Cells["B7"].Value = "     " + headerRow.RefNumRef2 + "     ";
            worksheet.Cells["B8"].Value = "  " + headerRow.RefNumComments + " ";
            worksheet.Cells["B9"].Value = "     " + headerRow.ESD_ShipDate + "     ";
            worksheet.Cells["B9"].Style.Numberformat.Format = "m/d/yy";
            worksheet.Cells["B10"].Value = "     " + headerRow.ESD_CancelDate + "     ";
            worksheet.Cells["B10"].Style.Numberformat.Format = "m/d/yy";
            worksheet.Cells["B11"].Value = "     " + headerRow.ProductionCode + "     ";
            worksheet.Cells["B12"].Value = "     " + headerRow.ReferencePullID + "     ";
            if (time != null)
                worksheet.Cells["B13"].Value = "     " + time.WOHours + ":" + time.WOMinutes + "     ";
            else
                worksheet.Cells["B13"].Value = "00:00      ";
            if (time != null)
                worksheet.Cells["B14"].Value = "     " + time.EmployeeCount + "     ";
            worksheet.Cells["B4:C14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["B4:C14"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(155, 194, 230));
            worksheet.Cells["B13:C14"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0));
            worksheet.Cells["B4:C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B4:C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["A4:C14"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C14"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C14"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["B4:C14"].AutoFitColumns(20);
        }

        private void GetWorkOrderSummaryHeaderConfirm(ExcelWorksheet worksheet, bool isManual = false)
        {
            worksheet.Cells["A18:F18"].Merge = true;
            worksheet.Cells["A18"].Value = "Work Order Summary";
            worksheet.Cells["A18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18"].Style.Font.Bold = true;
            worksheet.Cells["A18"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            worksheet.Cells["A18"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A18"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(32, 55, 100));
            worksheet.Cells["A18:F18"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F18"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F18"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F18"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        private void GetWorkOrderRowsHeaderConfirm(ExcelWorksheet worksheet, bool isManual = false)
        {
            worksheet.Cells["A19"].Value = "BillCodeDescription";
            worksheet.Cells["B19"].Value = "QTY";
            worksheet.Cells["C19"].Value = "BillComments";
            worksheet.Cells["D19"].Value = "BillCode";
            worksheet.Cells["E19"].Value = "Account";
            worksheet.Cells["F19"].Value = "StaticOutboundRate";
            worksheet.Cells["A19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["D19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["B19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["C19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["D19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["E19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["F19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A19:F19"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A19:F19"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A19:F19"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A19:F19"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A19:F19"].AutoFitColumns(20);
        }

        private void InsertDataForWorkOrderConfirm(ExcelWorksheet worksheet, List<sp_mob_ListProdWODetails_Result> data)
        {
            if (data != null && data.Count > 0)
            {
                int startCellNo = 20;
                string LastStyle = "";
                foreach (var item in data)
                {
                    worksheet.Cells["A" + startCellNo].Value = item.BillCodeDescription;
                    worksheet.Cells["B" + startCellNo].Value = item.QTY;
                    worksheet.Cells["C" + startCellNo].Value = item.BillComments;
                    worksheet.Cells["D" + startCellNo].Value = item.BillCode;
                    worksheet.Cells["E" + startCellNo].Value = item.Account;
                    worksheet.Cells["F" + startCellNo].Value = item.StaticOutboundRate;
                    startCellNo = startCellNo + 1;
                }
                startCellNo = startCellNo - 1;
                worksheet.Cells["A20:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A20:F" + startCellNo].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A20:F" + startCellNo].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A20:F" + startCellNo].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A20:F" + startCellNo].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A20:F" + startCellNo].AutoFitColumns();
            }
        }
        #endregion

        #region Delete Mail
        private void SendMailAndArchiveDelete(string account, int refNum, string username, string Vendor, RefCode data1, string path)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                WMS_Explorer explorer = db.WMS_Explorer.FirstOrDefault();
                if (explorer != null)
                {
                    if (!string.IsNullOrEmpty(explorer.WMS_PODShareDrivePath))
                    {
                        var data2 = db.sp_mob_GetWorkOrderReportHeader(refNum).FirstOrDefault();
                        string ReportFile = CreateExcelReportDeleted(refNum, data1, data2, path);

                        string loginEmail = db.Accounts.Where(x => x.Vendor__ == Vendor).FirstOrDefault().acc_billing_email;
                        if (!string.IsNullOrEmpty(loginEmail))
                        {
                            List<string> tom = new List<string>(); ;
                            tom.Add(loginEmail);
                            Helpers.EmailSender.SendWorkOrderReportEmail(explorer, tom, ReportFile, "Work Order Item Deleted -" + refNum.ToString());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RefNum"></param>
        /// <param name="type">0 for add, 1 for delete</param>
        private string CreateExcelReportDeleted(int RefNum, RefCode data, sp_mob_GetWorkOrderReportHeader_Result headerData, string path)
        {
            //string filenm = "Work Order Delete Confirmation -" + RefNum.ToString() + DateTime.Now.ToString() + ".xlsx".RemoveUnnecessary();
            string ReportPath = path;
            FileInfo fileInfo = new FileInfo(ReportPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                var workSheet = package.Workbook.Worksheets.Add("REF_" + RefNum);
                workSheet.View.ShowGridLines = true;
                workSheet.DefaultColWidth = 18;

                ExcelRange Rng = workSheet.Cells[1, 1, 3, 8];
                GetHeaderDel(Rng, RefNum);
                GetInfoCellDel(workSheet, headerData);
                GetWorkOrderSummaryHeaderDel(workSheet);
                GetWorkOrderRowsHeaderDel(workSheet);
                InsertDataForWorkOrderDel(workSheet, data);
                package.Save();
            }
            return ReportPath;

        }
        private void GetHeaderDel(ExcelRange Rng, int? refNum)
        {
            Rng.Merge = true;
            Rng.Value = "Work Order Details for Ref " + refNum.ToString() + "";
            Rng.Style.Font.Size = 20;
            Rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            Rng.Style.Font.Bold = true;
            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
            Rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(32, 55, 100));
        }
        private void GetInfoCellDel(ExcelWorksheet worksheet, sp_mob_GetWorkOrderReportHeader_Result headerRow)
        {
            worksheet.Cells["A4"].Value = "AccountName";
            worksheet.Cells["A5"].Value = "RefNumShipTo";
            worksheet.Cells["A6"].Value = "RefNumRef1";
            worksheet.Cells["A7"].Value = "RefNumRef2";
            worksheet.Cells["A8"].Value = "RefNumComments";
            worksheet.Cells["A9"].Value = "ESD_ShipDate";
            worksheet.Cells["A10"].Value = "ESD_CancelDate";
            worksheet.Cells["A11"].Value = "ProductionCode";
            worksheet.Cells["A12"].Value = "ReferencePullID";
            worksheet.Cells["A4:A12"].AutoFitColumns();
            //merge cells

            worksheet.Cells["B4:C4"].Merge = true;
            worksheet.Cells["B5:C5"].Merge = true;
            worksheet.Cells["B6:C6"].Merge = true;
            worksheet.Cells["B7:C7"].Merge = true;
            worksheet.Cells["B8:C8"].Merge = true;
            worksheet.Cells["B9:C9"].Merge = true;
            worksheet.Cells["B10:C10"].Merge = true;
            worksheet.Cells["B11:C11"].Merge = true;
            worksheet.Cells["B12:C12"].Merge = true;

            worksheet.Cells["B4"].Value = "     " + headerRow.AccountName + "     ";
            worksheet.Cells["B5"].Value = "     " + headerRow.RefNumShipTo + "     ";
            worksheet.Cells["B6"].Value = "     " + headerRow.RefNumRef1 + "     ";
            worksheet.Cells["B7"].Value = "     " + headerRow.RefNumRef2 + "     ";
            worksheet.Cells["B8"].Value = "  " + headerRow.RefNumComments + " ";
            worksheet.Cells["B9"].Value = "     " + headerRow.ESD_ShipDate + "     ";
            worksheet.Cells["B9"].Style.Numberformat.Format = "m/d/yy";
            worksheet.Cells["B10"].Value = "     " + headerRow.ESD_CancelDate + "     ";
            worksheet.Cells["B10"].Style.Numberformat.Format = "m/d/yy";
            worksheet.Cells["B11"].Value = "     " + headerRow.ProductionCode + "     ";
            worksheet.Cells["B12"].Value = "     " + headerRow.ReferencePullID + "     ";
            worksheet.Cells["B4:C12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["B4:C12"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(155, 194, 230));
            worksheet.Cells["B4:C12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B4:C12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            worksheet.Cells["A4:C12"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C12"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A4:C12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["B4:C12"].AutoFitColumns();
        }

        private void GetWorkOrderSummaryHeaderDel(ExcelWorksheet worksheet, bool isManual = false)
        {
            worksheet.Cells["A16:F16"].Merge = true;
            worksheet.Cells["A16"].Value = "Work Order Summary";
            worksheet.Cells["A16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A16"].Style.Font.Bold = true;
            worksheet.Cells["A16"].Style.Font.Color.SetColor(System.Drawing.Color.White);
            worksheet.Cells["A16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A16"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(32, 55, 100));
            worksheet.Cells["A16:F16"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A16:F16"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A16:F16"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A16:F16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        private void GetWorkOrderRowsHeaderDel(ExcelWorksheet worksheet, bool isManual = false)
        {
            worksheet.Cells["A17"].Value = "BillCodeDescription";
            worksheet.Cells["B17"].Value = "QTY";
            worksheet.Cells["C17"].Value = "BillComments";
            worksheet.Cells["D17"].Value = "BillCode";
            worksheet.Cells["E17"].Value = "Account";
            worksheet.Cells["F17"].Value = "StaticOutboundRate";
            worksheet.Cells["A17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["D17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["B17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["C17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["D17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["E17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["F17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A17:F17"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A17:F17"].AutoFitColumns();
        }

        private void InsertDataForWorkOrderDel(ExcelWorksheet worksheet, RefCode data)
        {

            worksheet.Cells["A18"].Value = data.BillCodeDescription;
            worksheet.Cells["B18"].Value = data.QTY;
            worksheet.Cells["C18"].Value = data.BillComments;
            worksheet.Cells["D18"].Value = data.BillCode;
            worksheet.Cells["E18"].Value = data.Account;
            worksheet.Cells["F18"].Value = data.StaticOutboundRate;

            worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A18:F18"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F18"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F18"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A18:F18"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["A18:F18"].AutoFitColumns();
        }
        #endregion

        #region Copy
        protected void CopyPODToDrive(string filePath, string PODDrivePath)
        {
            try
            {
                FileInfo info = new FileInfo(filePath);
                File.Copy(filePath, PODDrivePath + "\\" + info.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion

        [HttpGet]
        [Route("api/Production/GetFilesList/{id}")]
        public IHttpActionResult GetFilesList(int id)
        {
            var res = db.POD_Repository.Where(x => x.PODKey.Trim() == id.ToString()).ToList();
            return Json(res);
        }
        [HttpGet]
        [Route("api/Production/GetClockedEmployeesList/{id}")]
        public IHttpActionResult GetClockedEmployeesList(int id)
        {
            var res = db.sp_mob_TimeTrack_GetClockedInEmployees(id).ToList();
            return Json(res);
        }

        [HttpGet]
        [Route("api/Production/ClockOutAll/{id}")]
        public IHttpActionResult ClockOutAll(int id)
        {
            var res = db.sp_mob_TimeTrack_ClockoutAll(id);
            return Json(res);
        }

        [HttpGet]
        [Route("api/Production/GetEmployeesList")]
        public IHttpActionResult GetEmployeesList()
        {
            var res = db.sp_mob_TimeTrack_ListEmployee().ToList();
            return Json(res);
        }

        [HttpPost]
        [Route("api/Production/ClockEmployee")]
        public IHttpActionResult ClockEmployee([FromBody]ClockEmployeeModel model)
        {
            var res = db.sp_mob_TimeTrack_ClockIn_Out_Employee(model.RefNum, model.BadgeID, model.CheckinType, model.Temp,model.WoNumID);
            return Json(res);
        }

        [HttpGet]
        [Route("api/Production/GetPDFFile/{id}")]
        public IHttpActionResult GetPDFFile(int id)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                string filePath = db.POD_Repository.Where(x => x.PODID == id).FirstOrDefault()?.PODLink;
                if (!string.IsNullOrEmpty(filePath))
                {
                    var path = filePath.Replace("\\\\", "");
                    path = path.Replace(@"\", @"/");
                    var filename = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        Random rnd = new Random();
                        string randomVal = rnd.Next(10000, 999999).ToString();
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + randomVal + ".pdf"), true);
                        return Json(filename + randomVal + ".pdf");
                    }
                    catch (Exception ex)
                    {
                        return Json("Exception" + ex.Message);
                    }
                }
                return Json("");
            }

        }


        [HttpGet]
        [Route("api/Production/GetExcelFile/{id}")]
        public IHttpActionResult GetExcelFile(int id)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                string filePath = db.POD_Repository.Where(x => x.PODID == id).FirstOrDefault()?.PODLink;
                if (!string.IsNullOrEmpty(filePath))
                {
                    var path = filePath.Replace("\\\\", "");
                    path = path.Replace(@"\", @"/");
                    var filename = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        Random rnd = new Random();
                        string randomVal = rnd.Next(10000, 999999).ToString();
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + randomVal + ".xlsx"), true);
                        return Json(filename + randomVal + ".xlsx");
                    }
                    catch (Exception ex)
                    {
                        return Json("Exception" + ex.Message);
                    }
                }
                return Json("");
            }
        }
        [HttpGet]
        [Route("api/Production/GetWordFile/{id}")]
        public IHttpActionResult GetWordFile(int id)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                string filePath = db.POD_Repository.Where(x => x.PODID == id).FirstOrDefault()?.PODLink;
                if (!string.IsNullOrEmpty(filePath))
                {
                    var path = filePath.Replace("\\\\", "");
                    path = path.Replace(@"\", @"/");
                    var filename = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        Random rnd = new Random();
                        string randomVal = rnd.Next(10000, 999999).ToString();
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + randomVal + ".docx"), true);
                        return Json(filename + randomVal + ".docx");
                    }
                    catch (Exception ex)
                    {
                        return Json("Exception" + ex.Message);
                    }
                }
                return Json("");
            }
        }

        [HttpGet]
        [Route("api/Production/GetImageFile/{id}")]
        public IHttpActionResult GetImageFile(int id)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                string filePath = db.POD_Repository.Where(x => x.PODID == id).FirstOrDefault()?.PODLink;
                if (!string.IsNullOrEmpty(filePath))
                {
                    var path = filePath.Replace("\\\\", "");
                    path = path.Replace(@"\", @"/");
                    var filename = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        Random rnd = new Random();
                        string randomVal = rnd.Next(10000, 999999).ToString();
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + randomVal + ".jpeg"), true);
                        return Json(filename + randomVal + ".jpeg");
                    }
                    catch (Exception ex)
                    {
                        return Json("Exception" + ex.Message);
                    }
                }
                return Json("");
            }
        }
        [Route("api/Production/UploadImage")]
        public async Task<string> UploadImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        var fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();
                        string RefId = fileName.Split('^')[0];
                        string UserName = fileName.Split('^')[1];
                        string filenm = fileName.Split('^')[2].RemoveUnnecessary();
                        var filePath = "~/MobileFile/" + filenm;

                        postedFile.SaveAs(HttpContext.Current.Server.MapPath(filePath));

                        //Copy files to POD drive and table
                        using (SkyNetEntities db = new SkyNetEntities())
                        {
                            WMS_Explorer wms = db.WMS_Explorer.FirstOrDefault();
                            if (wms != null && !string.IsNullOrEmpty(wms.WMS_PODShareDrivePath))
                            {
                                CopyPODToDrive(HttpContext.Current.Server.MapPath(filePath), wms.WMS_PODShareDrivePath);
                                POD_Repository repository = new POD_Repository();
                                repository.PODBorn = DateTime.Now;
                                repository.PODCreatedBy = UserName != null ? UserName : "";
                                repository.PODKey = RefId != null ? RefId : "0";
                                repository.PODLink = wms.WMS_PODShareDrivePath + @"\" + filenm;
                                repository.PODDescription = "WO Product View";
                                db.POD_Repository.Add(repository);
                                db.SaveChanges();
                            }
                        }
                        return "/Uploads/" + fileName;
                    }
                }

            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return "no files";
        }

        [HttpGet]
        [Route("api/Production/GetAccountEmail/{id}")]
        public IHttpActionResult GetAccountEmail(string id)
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                var res = db.Accounts.Where(x => x.Vendor__ == id).FirstOrDefault();
                if (res != null)
                {
                    return Json(res.acc_billing_email);
                }
                else
                {
                    return Json("");
                }
            }
        }

        [HttpGet]
        [Route("api/Production/GetMailSettings")]
        public IHttpActionResult GetMailSettings()
        {
            using (SkyNetEntities db = new SkyNetEntities())
            {
                var res = db.WMS_Explorer.FirstOrDefault();
                res.WMS_Application_Backround = null;
                res.WMS_Application_Icon = null;
                if (res != null)
                {
                    return Json(res);
                }
                else
                {
                    return Json("");
                }
            }
        }

        [HttpPost]
        [Route("api/Production/SendDocumentMail")]
        public IHttpActionResult SendDocumentMail([FromBody]SendDocumentMailModel model)
        {
            try
            {
                using (SkyNetEntities db = new SkyNetEntities())
                {
                    string filePath = db.POD_Repository.Where(x => x.PODID == model.PODID).FirstOrDefault()?.PODLink;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        var path = filePath;
                        WMS_Explorer wMS = db.WMS_Explorer.FirstOrDefault();
                        if (wMS != null)
                        {
                            Random rnd = new Random();
                            string randomVal = rnd.Next(10000, 999999).ToString();
                            FileInfo info = new FileInfo(path);
                            string nameWithoutExt = Path.GetFileNameWithoutExtension(path);
                            string ext = Path.GetExtension(path);
                            string NewFileName = nameWithoutExt + randomVal + ext;

                            File.Copy(path, HttpContext.Current.Server.MapPath("~/MobileFile/" + NewFileName), true);
                            var res = db.Accounts.Where(x => x.Vendor__ == model.VendorID).FirstOrDefault();
                            List<string> to = new List<string>();
                            to.Add(res.acc_billing_email);
                            Helpers.EmailSender.SendReportEmail(wMS, to, HttpContext.Current.Server.MapPath("~/MobileFile/" + NewFileName), "Attached File from MOXIE");
                            return Json("The document has been sent on mail");
                        }
                    }
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                return Json("Exception:" + ex.Message);
            }
        }
    }
}
