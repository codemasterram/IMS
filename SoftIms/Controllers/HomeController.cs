using SoftIms.Data;
using SoftIms.Data.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SoftIms.Controllers
{

    public class HomeController : BaseController
    {
        protected NotificationService NotificationService;
        public HomeController()
        {
            NotificationService = new NotificationService();
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        #region front end mail service
        [HttpGet]
        public PartialViewResult Send()
        {

            return PartialView();
        }


        [HttpPost]
        public JsonResult Send(string Detail, HttpPostedFileBase[] files, string ScreenshotData)
        {
            var attachments = new List<Attachment>();
            if (!string.IsNullOrEmpty(ScreenshotData))
            {
                string[] words = ScreenshotData.Split(',');
                byte[] bytes = Convert.FromBase64String(words[1]);
                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);

                }
                string name = helper.GenerateString(new Random(), 7);
                string imagePath = Server.MapPath($"~/uploads/Feedback/screenshot/{name}.jpeg");
                image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                attachments.Add(new Attachment(imagePath));
            }
            if (files != null)
            {
                foreach (var file in files)
                {
                    string physicalPath = Server.MapPath($"~/uploads/Feedback/upload/{file.FileName}");
                    
                    file.SaveAs(physicalPath);
                    //image.Save(imagePath);
                    attachments.Add(new Attachment(physicalPath));
                }
            }
            var body = Detail;

            NotificationService.SendEmail(new Smtp(), "iamramsapkota@gmail.com",
                subject: $"Feedback from  ABC COmpany",
                body: body,
                attachetments: attachments);
            return Json(new { success = true });

        }
        #endregion




    }
}