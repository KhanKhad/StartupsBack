using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StartupsBack.Controllers
{
    public class StartupsManagementController : Controller
    {
        public StartupsManagementController() 
        {
            HttpContext.Response.Cookies.Append("LastVisit", DateTime.Now.ToString("dd/MM/yyyy hh-mm-ss"));
            //HttpContext.Request.ReadFromJsonAsync
        }
    }
}
