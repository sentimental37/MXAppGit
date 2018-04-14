using MXApi.Models;
using MXApp.MobileService.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MXApp.MobileService.Controllers
{
    public class ShippingController : ApiController
    {
        SkyNetEntities db = new SkyNetEntities();
        // GET api/values
        [HttpPost]
        [Route("api/Shipping/GetShippingViewLoads")]
        public IHttpActionResult GetShippingViewLoads([FromBody]GetShippingViewLoadsModel model)
        {
            var res = db.sp_mob_ship_viewloads(model.WHS, model.Date).ToList();
            return Json(res);
        }

        // GET api/values
        [HttpGet]
        [Route("api/Shipping/GetShippingViewLoadDetails/{id}")]
        public IHttpActionResult GetShippingViewLoadDetails(int id)
        {
            var res = db.sp_mob_ship_viewloaddetails(id).ToList();
            return Json(res);
        }

        [HttpPost]
        [Route("api/Shipping/ConfirmDetail")]
        public IHttpActionResult ConfirmDetail([FromBody]ConfirmDetailsModel model)
        {
            var res = db.sp_mob_ship_loadshow_yesno_confirmdetail(model.mBOLVICS, model.type);
            return Json(res);
        }

        [HttpPost]
        [Route("api/Shipping/ConfirmMaster")]
        public IHttpActionResult ConfirmMaster([FromBody]ConfirmMasterModel model)
        {
            var res = db.sp_mob_ship_loadshow_yesno_confirmmaster(model.type, model.eDIAPPID);
            return Json(res);
        }

        [Route("api/Shipping/UploadImage")]
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
                        string PODKey = fileName.Split('^')[0];
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
                                repository.PODKey = PODKey != null ? PODKey : "0";
                                repository.PODLink = wms.WMS_PODShareDrivePath + @"\" + filenm;
                                repository.PODDescription = "Shipping Product View";
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

        #region FileTab
        [HttpGet]
        [Route("api/Shipping/GetFilesList/{id}")]
        public IHttpActionResult GetFilesList(string id)
        {
            var res = db.POD_Repository.Where(x => x.PODKey.Trim() == id).ToList();
            return Json(res);
        }

        [HttpGet]
        [Route("api/Shipping/GetPDFFile/{id}")]
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
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + ".pdf"), true);
                        return Json(filename + ".pdf");
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
        [Route("api/Shipping/GetExcelFile/{id}")]
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
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + ".xlsx"), true);
                        return Json(filename + ".xlsx");
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
        [Route("api/Shipping/GetWordFile/{id}")]
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
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + ".docx"), true);
                        return Json(filename + ".docx");
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
        [Route("api/Shipping/GetImageFile/{id}")]
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
                        File.Copy(filePath, HttpContext.Current.Server.MapPath("~/MobileFile/" + filename + ".jpeg"), true);
                        return Json(filename + ".jpeg");
                    }
                    catch (Exception ex)
                    {
                        return Json("Exception" + ex.Message);
                    }
                }
                return Json("");
            }
        }
        #endregion

        [HttpPost]
        [Route("api/Shipping/UpdateViewLoadItem")]
        public IHttpActionResult UpdateViewLoadItem([FromBody]ViewLoadModel model)
        {
            EDIAppTable table = db.EDIAppTables.Where(x => x.EDIAPPID == model.EDIAPPID).FirstOrDefault();
            if (table != null)
            {
                table.DepartingDoor = model.DepartingDoor;
                table.AppIDComments = model.AppIDComments;
                int res = db.SaveChanges();
                return Json(res);
            }
            return Json(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("api/Shipping/UpdateViewLoadDetailsItem")]
        public IHttpActionResult UpdateViewLoadDetailsItem([FromBody]ViewLoadDetailsModel model)
        {
            EDIAppTable_Child table = db.EDIAppTable_Child.Where(x => x.ChildAPPID == model.ChildAPPID).FirstOrDefault();
            if (table != null)
            {
                table.MBOLPalletCount = model.MBOLPalletCount;
                table.MBOLShipComments = model.MBOLShipComments;
                int res = db.SaveChanges();
                return Json(res);
            }
            return Json(HttpStatusCode.NotFound);
        }

        // GET api/values
        [HttpGet]
        [Route("api/Shipping/LoadWHSList")]
        public IHttpActionResult LoadWHSList()
        {
            var res = db.View_WHS.ToList();
            return Json(res);
        }
        [HttpGet]
        [Route("api/Shipping/FullPickUpStatus/{id}")]
        public IHttpActionResult FullPickUpStatus(int id)
        {
            var res = db.EDIAppTable_Child.Where(x => x.EDIAPPID == id).All(x => x.FullPickUp == true);
            return Json(res);
        }
        [HttpGet]
        [Route("api/Shipping/GetLocationsList/{id}")]
        public IHttpActionResult GetLocationsList(string id)
        {
            var res = db.sp_mob_ship_viewloads_locations(id).ToList();
            return Json(res);
        }
        [HttpPost]
        [Route("api/Shipping/SendDocumentMail")]
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
                            FileInfo info = new FileInfo(path);
                            File.Copy(path, HttpContext.Current.Server.MapPath("~/MobileFile/" + info.Name), true);
                            var res = db.logins.Where(x => x.Name == model.VendorID).FirstOrDefault();
                            if (!string.IsNullOrEmpty(res.login_email))
                            {
                                List<string> to = new List<string>();
                                to.Add(res.login_email);
                                Helpers.EmailSender.SendReportEmail(wMS, to, HttpContext.Current.Server.MapPath("~/MobileFile/" + info.Name), "Attached File from MOXIE");
                            }
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
