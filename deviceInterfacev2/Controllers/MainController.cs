using deviceInterface.ExtraFiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Concrete;
using deviceInterfacev2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using deviceInterfacev2.ExtraFiles;
using Microsoft.AspNetCore.Http;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using System.Text;
using iText.Html2pdf;
using System.Globalization;

namespace deviceInterfacev2.Controllers
{
    public class MainController : Controller
    {
        UserBL user_manager = new UserBL();
        ContactBL contact_manager = new ContactBL();
        CityBL city_manager = new CityBL();
        CountyBL county_manager = new CountyBL();
        PersonBL person_manager = new PersonBL();
        DeviceBL device_manager = new DeviceBL();
        TMBL tm_manager = new TMBL();
        MqttConnector mq = new MqttConnector();

        public List<SelectListItem> returnCities()
        {
            var city_list = city_manager.List();
            List<SelectListItem> select_list = new List<SelectListItem>();
            foreach (var city in city_list)
            {
                select_list.Add(new SelectListItem(text: city.CityName.ToString(), value: city.CityId.ToString()));
            }

            return select_list;
        }
        public JsonResult GetCounty(int city_id)
        {
            var county_list = county_manager.List();
            var new_county_list = county_list.Where(m => m.CountyCityId == city_id).ToList();
            var jsonlist = Json(new_county_list);
            return jsonlist;
        }

        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult MainPage()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                HttpContext.Session.Remove("MainPageOperationMessage");
                if (HttpContext.Session.GetString("IsThereThreePersons") == "yes")
                {
                    ViewBag.logged_user_id = HttpContext.Session.GetInt32("LoggedID");
                    ViewBag.PersonCountError = "3 adet kayıtlı kullanıcınız bulunmaktadır. Daha fazla kullanıcı ekleyemezsiniz. Mevcut kullanıcı bilglerinizi Kullanıcı Bilgilerim sekmesinden değiştirebilirsiniz.";
                    return View();
                }
                else
                {
                    ViewBag.logged_user_id = HttpContext.Session.GetInt32("LoggedID");
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            SessionOperations.AbandonSession(this);
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public IActionResult UserInfos()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {

                var selected_user = user_manager.getUserByID(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                return View(selected_user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public IActionResult UserInfos(TableUser editing_user, string tboxControl)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                if (ModelState.IsValid)
                {
                    if (editing_user.UserPassword == tboxControl)
                    {
                        ViewBag.SuccessMsg = "Bilgiler guncellendi";
                        editing_user.UserPassword = Sha256Hash.ComputeSha256Hash(editing_user.UserPassword);
                        user_manager.Edit(editing_user);
                        return View();
                    }
                    else
                    {
                        @ViewBag.validateError = "Şifreler eşleşmiyor.";
                        return View();
                    }
                }
                else
                {
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public IActionResult RedirectMeContactInfos()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                HttpContext.Session.Remove("MainPageOperationMessage");
                var selected_contact_type = contact_manager.CheckContactFirmOrIndividualBL(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                if (selected_contact_type == "FirmContactInfos")
                {
                    return RedirectToAction("FirmContactInfos", "Main");
                }
                else
                {
                    return RedirectToAction("IndividualContactInfos", "Main");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [ResponseCache(CacheProfileName = "Never")]
        [HttpGet]
        public IActionResult FirmContactInfos()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {

                var selected_contact = contact_manager.getContactUserByIDBL(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                ViewBag.City = returnCities();
                return View(selected_contact);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        [HttpPost]
        public IActionResult FirmContactInfos(TableContact editing_contact)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var tboxfirmName = Request.Form["ContactFirmName"];
                var tboxfirmAdress = Request.Form["ContactFirmAdress"];
                var tboxTaxLoc = Request.Form["ContactTaxLoc"];
                var tboxTaxNo = Request.Form["ContactTaxNo"];

                if (tboxfirmName[0] == "")
                {
                    ViewBag.validateErrorFirmName = "Firma adı boş bırakılamaz";
                    ViewBag.City = returnCities();
                    return View();
                }
                else if (tboxTaxLoc[0] == "")
                {
                    ViewBag.validateErrortaxloc = "Vergi dairesi boş bırakılamaz";
                    ViewBag.City = returnCities();
                    return View();
                }
                else if (tboxfirmAdress[0] == "")
                {
                    ViewBag.validateErrorFirmAdress = "Firma adresi boş bırakılamaz";
                    ViewBag.City = returnCities();
                    return View();
                }
                else if (tboxTaxNo[0] == "")
                {
                    ViewBag.validateErrortaxNo = "Vergi numarası boş bırakılamaz";
                    ViewBag.City = returnCities();
                    return View();
                }
                else
                {
                    int selectedCountyID = Convert.ToInt32(Request.Form["countyDrop"]);
                    editing_contact.ContactCounty = selectedCountyID;
                    contact_manager.Edit(editing_contact);
                    HttpContext.Session.SetString("MainPageOperationMessage", "Adres bilgileri başarıyla güncellendi.");
                    ViewBag.City = returnCities();
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [ResponseCache(CacheProfileName = "Never")]
        [HttpGet]
        public IActionResult IndividualContactInfos()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {

                var selected_contact = contact_manager.getContactUserByIDBL(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                ViewBag.City = returnCities();
                return View(selected_contact);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public IActionResult IndividualContactInfos(TableContact editing_contact)
        {
            var tboxTC = Request.Form["ContactTc"];
            var tboxTaxLoc = Request.Form["ContactTaxLoc"];

            if (tboxTC[0] == "")
            {
                ViewBag.validateErrortc = "TC Kimlik no boş bırakılamaz";
                ViewBag.City = returnCities();
                return View();
            }
            else if (tboxTaxLoc[0] == "")
            {
                ViewBag.validateErrortaxloc = "Vergi dairesi boş bırakılamaz";
                ViewBag.City = returnCities();
                return View();
            }
            else
            {
                int selectedCountyID = Convert.ToInt32(Request.Form["countyDrop"]);
                editing_contact.ContactCounty = selectedCountyID;
                contact_manager.Edit(editing_contact);
                ViewBag.City = returnCities();
                return View("MainPage");
            }
        }

        public IActionResult PersonsMainPage()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var personList = person_manager.listPersonByUserIDBL(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                return View(personList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public IActionResult DeletePerson(int id)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var my_person = person_manager.getPersonByPersonIDBL(id);
                person_manager.Delete(my_person);
                return RedirectToAction("PersonsMainPage", "Main");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public IActionResult EditPerson(int id)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var my_person = person_manager.getPersonByPersonIDBL(id);
                return View(my_person);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost]
        public IActionResult EditPerson(TablePerson editing_person)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var tboxPhone = Request.Form["PersonPhone"];
                var tboxName = Request.Form["PersonName"];
                var tboxSurname = Request.Form["PersonSurname"];
                var tboxMail = Request.Form["PersonMail"];

                if (tboxPhone[0] == "")
                {
                    @ViewBag.validateError = "Telefon numarası boş girilemez";
                    return View();
                }
                else if (tboxName[0] == "")
                {
                    @ViewBag.validateError = "İsim boş girilemez";
                    return View();
                }
                else if (tboxSurname[0] == "")
                {

                    @ViewBag.validateError = "Soy isim boş girilemez";
                    return View();
                }
                else if (tboxMail[0] == "")
                {
                    @ViewBag.validateError = "Mail adresi boş girilemez";
                    return View();
                }
                else
                {
                    person_manager.Edit(editing_person);
                    return RedirectToAction("PersonsMainPage", "Main");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public IActionResult AddPerson()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }



        [HttpPost]
        public IActionResult AddPerson(TablePerson my_person)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var tboxPhone = Request.Form["PersonPhone"];
                var tboxName = Request.Form["PersonName"];
                var tboxSurname = Request.Form["PersonSurname"];
                var tboxMail = Request.Form["PersonMail"];

                if (tboxPhone[0] == "")
                {
                    @ViewBag.validateError = "Telefon numarası boş girilemez";
                    return View();
                }
                else if (tboxName[0] == "")
                {
                    @ViewBag.validateError = "İsim boş girilemez";
                    return View();
                }
                else if (tboxSurname[0] == "")
                {

                    @ViewBag.validateError = "Soy isim boş girilemez";
                    return View();
                }
                else if (tboxMail[0] == "")
                {
                    @ViewBag.validateError = "Mail adresi boş girilemez";
                    return View();
                }
                else
                {
                    my_person.PersonUserId = Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID"));
                    my_person.PersonContactId = (contact_manager.getContactUserByIDBL(my_person.PersonUserId)).ContactId;
                    person_manager.Add(my_person);
                    return RedirectToAction("PersonsMainPage", "Main");
                }


            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        public IActionResult CheckMQTT()
        {
            MqttConnector mqttc = new MqttConnector();
            mqttc.MqttConnectionLoad();
            return View();
        }

        [HttpGet]
        public IActionResult MyDevices()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var devices_list = device_manager.ListAllDevicesWithClosestValues(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                return View(devices_list);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public IActionResult LinkDeviceForUser()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost]
        public IActionResult LinkDeviceForUser(string enteredkey, string enteredname)
        {

            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                if (device_manager.IsDeviceExistingBL(enteredkey) != null)
                {
                    Random random = new Random();
                    var random_code = random.Next(1000, 9999);
                    HttpContext.Session.SetString("LinkDeviceCode", random_code.ToString());
                    HttpContext.Session.SetString("LinkDeviceUniqueKey", enteredkey);
                    HttpContext.Session.SetString("LinkDeviceName", enteredname);
                    mq.sendRandomCodeToDevice(enteredkey, random_code.ToString());
                    return RedirectToAction("LinkDeviceForUserApproveCode", "Main");
                }
                else
                {
                    ViewBag.mailValidationText = "Böyle bir cihaz id numarası bulunmamaktadır.";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public IActionResult LinkDeviceForUserApproveCode()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public IActionResult LinkDeviceForUserApproveCode(string enteredcode)
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                if (enteredcode == HttpContext.Session.GetString("LinkDeviceCode"))
                {
                    var selected_device = device_manager.findDevicebyUniqeKey(HttpContext.Session.GetString("LinkDeviceUniqueKey"));
                    if (selected_device != null)
                    {
                        selected_device.DeviceConnectedUser = HttpContext.Session.GetInt32("LoggedID");
                        selected_device.DeviceName = HttpContext.Session.GetString("LinkDeviceName");
                        mq.sendRandomCodeToDevice(HttpContext.Session.GetString("LinkDeviceUniqueKey"), 0.ToString());
                        device_manager.Edit(selected_device);
                        TableTm new_tm = new TableTm();
                        new_tm.TmdeviceId = selected_device.DeviceId;
                        new_tm.TMDate = DateTime.Now;
                        tm_manager.Add(new_tm);
                        return RedirectToAction("MainPage", "Main");
                    }
                    else
                    {
                        return RedirectToAction("MyDevices", "Main");
                    }
                }
                else
                {
                    ViewBag.mailValidationText = "Kod yanlış";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        public IActionResult MyReports()
        {
            if (HttpContext.Session.GetString("IsUserLogged") == "yes")
            {
                var devices_list = device_manager.ListAllDevices(Convert.ToInt32(HttpContext.Session.GetInt32("LoggedID")));
                return View(devices_list);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public IActionResult DownloadReport(string id)
        {
            var mylist = tm_manager.getLastMonthValues(id.ToString());
            var device = device_manager.findDevicebyUniqeKey(id.ToString());
            //Building an HTML string.
            StringBuilder sb = new StringBuilder();


            //Table start.
            sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-family: Arial; font-size: 10pt;'>");

            //Building the Header row.
            sb.Append("<tr>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Tarih</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Sıcaklık Saat 9</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Sıcaklık Saat 13</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Sıcaklık Saat 16</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Nem Saat 9</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Nem Saat 13</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Nem Saat 16</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Maximum Sıcaklık</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Minimum Sıcaklık</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Maximum Nem</th>");
            sb.Append("<th style='background-color: #B8DBFD;border: 1px solid #ccc'>Minimum Nem</th>");
            sb.Append("</tr>");

            //Building the Data rows.
            for (int i = 0; i < mylist.Count; i++)
            {

                sb.Append("<tr>");
                //Append data.
                sb.Append("<td style='border: 1px solid #ccc'>");
                sb.Append(mylist[i].date.ToShortDateString());
                sb.Append("</td>");

                if (mylist[i].t9 != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].t9);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }



                if (mylist[i].t13 != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].t13);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }



                if (mylist[i].t16 != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].t16);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }


                if (mylist[i].m9 != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].m9);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }

                if (mylist[i].m13 != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].m13);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }



                if (mylist[i].m16 != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].m16);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }

                if (mylist[i].maxt != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].maxt);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }

                if (mylist[i].mint != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].mint);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }


                if (mylist[i].maxm != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].maxm);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }


                if (mylist[i].minm != null)
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(mylist[i].minm);
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append("<td style='border: 1px solid #ccc'>");
                    sb.Append(" ");
                    sb.Append("</td>");
                }

                sb.Append("</tr>");
            }

            //Table end.
            sb.Append("</table>");

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
            {
                ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
                PdfWriter writer = new PdfWriter(byteArrayOutputStream);
                PdfDocument pdfDocument = new PdfDocument(writer);
                pdfDocument.SetDefaultPageSize(PageSize.A4);
                HtmlConverter.ConvertToPdf(stream, pdfDocument);
                pdfDocument.Close();
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Date.Month);
                return File(byteArrayOutputStream.ToArray(), "application/pdf", $"{device.DeviceName} {monthName} {DateTime.Now.Date.Year}.pdf");
            }

        }
    }


}
