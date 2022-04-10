using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerOrder2.Controllers
{
    [Route("{controller}/{action}")]
    public class AjaxController : Controller
    {
        // Ajax 一定要以 JSON 或 XML
        // 以 Ajax 方法呼叫回傳不能使用 return view(物件); 需要直接 return 物件;
        // GET: AjaxController
        public JsonResult GetJson()
        {
            var result = new
            {
                username = "name",
                age = "20"               
            };
            return Json(result);
        }
     
    }
}
