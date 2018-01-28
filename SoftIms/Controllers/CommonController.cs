using SoftIms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftIms.Controllers
{
    public class CommonController : BaseController
    {
       public JsonResult GetEmployee(int? departmentId, Object selectedValue = null)
        {
            var data = ViewHelper.GetEmployeeList(departmentId, selectedValue);
            return Json(data, JsonRequestBehavior.AllowGet);

        }

    }
}