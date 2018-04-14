using MXApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MXApp.MobileService.Controllers
{
    public class AuthController : ApiController
    {
        SkyNetEntities db = new SkyNetEntities();

        [HttpPost]
        public IHttpActionResult Login([FromBody]LoginModel model)
        {
            var login = db.logins.Where(x => x.Name == model.UserName.Trim() && x.Password == model.Password.Trim()).FirstOrDefault();
            return Json(login);
        }
    }
}
