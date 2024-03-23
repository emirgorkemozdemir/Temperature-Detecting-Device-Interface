using BusinessLayer.Concrete;
using deviceInterfacev2.Models;
using Microsoft.AspNetCore.Mvc;

namespace deviceInterfacev2.Controllers
{
    public class AdminController : Controller
    {

        DeviceBL device_manager = new DeviceBL();

        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult AdminMainPage()
        {
            if (HttpContext.Session.GetString("IsAdminLogged") == "yes")
            {
                var devices_list = device_manager.List();
                return View(devices_list);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
              
        }

        [HttpGet]
        public IActionResult AdminAddDevice()
        {
            if (HttpContext.Session.GetString("IsAdminLogged") == "yes")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
          
        }

        [HttpPost]
        public IActionResult AdminAddDevice(TableDevice device)
        {
            if (HttpContext.Session.GetString("IsAdminLogged") == "yes")
            {
                device_manager.Add(device);
                return RedirectToAction("AdminMainPage", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
           
        }

        public IActionResult AdminDeleteDevice(int id)
        {

            if (HttpContext.Session.GetString("IsAdminLogged") == "yes")
            {
                device_manager.deleteDeviceByUniqueKey(id.ToString());
                return RedirectToAction("AdminMainPage", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
    }
}
