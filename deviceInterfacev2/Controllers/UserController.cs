using BusinessLayer.Concrete;
using deviceInterface.ExtraFiles;
using deviceInterfacev2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using deviceInterfacev2.ExtraFiles;

namespace deviceInterfacev2.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        UserBL user_manager = new UserBL();
        CityBL city_manager = new CityBL();
        CountyBL county_manager = new CountyBL();
        ContactBL contact_manager = new ContactBL();
        DeviceBL device_manager = new DeviceBL();
        PersonBL person_manager = new PersonBL();
        TMBL tm_manager = new TMBL();
        MqttConnector mq = new MqttConnector();

        [HttpGet]
        public JsonResult PersonCountMethod()
        {
            return Json(true);
        }

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

        [HttpGet]
        public IActionResult Register()
        {
            SessionOperations.AbandonSession(this);
            ViewBag.City = returnCities();
            return View();
        }


        [HttpPost]
        public IActionResult Register(TableUser my_user)
        {
            if (ModelState.IsValid)
            {
                var tbox1 = Request.Form["UserPassword"];
                var tbox2 = Request.Form["tboxControl"];
                var existing_user = user_manager.UserRegisterExistingCheckBL(my_user);
                if (existing_user != "")
                {
                    ViewBag.validateError = existing_user;

                    return View();
                }
                else if (tbox1[0] != tbox2)
                {

                    ViewBag.validateError = "Şifreleriniz eşleşmiyor";
                    return View();
                }
                else
                {
                    var cbox1 = Request.Form["link1"];
                    var cbox2 = Request.Form["link2"];
                    var cbox3 = Request.Form["link3"];

                    if (cbox1.Count() == 0 || cbox2.Count() == 0 || cbox3.Count() == 0)
                    {

                        ViewBag.validateError = "Kayıt olmak için sözleşmeleri kabul etmelisiniz.";
                        return View();
                    }
                    else
                    {

                        my_user.UserPassword = Sha256Hash.ComputeSha256Hash(my_user.UserPassword);

                        HttpContext.Session.SetString("IsUserGoingValidation", "yes");
                        HttpContext.Session.SetString("RegisteringMail", my_user.UserMail);

                        return RedirectToAction("EmailValidation", my_user);

                    }


                }
            }
            else
            {

                return View();
            }
        }


        [HttpGet]
        public IActionResult SecurityPolicy()
        {

            return View();
        }


        [HttpGet]
        public IActionResult EmailValidation(TableUser new_user)
        {
            if (HttpContext.Session.GetString("IsUserGoingValidation") == "yes")
            {
                try
                {

                    Random random = new Random();
                    var random_code = random.Next(1000, 9999);
                    HttpContext.Session.SetString("MailRandomCode", random_code.ToString());
                    if (ModelState.IsValid)
                    {
                        var senderEmail = new MailAddress("emirozdemirdeneme@gmail.com", "biocoder");
                        var receiverEmail = new MailAddress(new_user.UserMail, "Receiver");
                        var password = "uqirysrqzrbxjeii";
                        var sub = "onay kodu";
                        var body = random_code;
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = sub,
                            Body = body.ToString()
                        })
                        {
                            smtp.Send(mess);
                        }
                        return View();
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = "Some Error";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Register");
            }

        }



        [HttpPost]
        public IActionResult EmailValidation(string enteredcode, TableUser sended_user)
        {
            var old_code = HttpContext.Session.GetString("MailRandomCode");

            if (enteredcode == null)
            {
                ViewBag.mailValidationText = "Kod girilecek kısmı boş girdiniz.";
                return View();
            }
            else
            {
                if (enteredcode == old_code)
                {
                    user_manager.Add(sended_user);

                    HttpContext.Session.SetInt32("RegisteringUserID", sended_user.UserId);
                    HttpContext.Session.SetString("IsUserGoingContact", "yes");
                    return RedirectToAction("AccountType", "User");
                }
                else
                {
                    ViewBag.mailValidationText = "Yanlış kod girdiniz.";
                    return View();
                }
            }


        }


        [HttpGet]
        public IActionResult AccountType()
        {
            if (HttpContext.Session.GetString("IsUserGoingContact") == "yes")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Register", "User");
            }

        }


        [HttpPost]
        public IActionResult AccountType(string btnInd, string btnFirm)
        {
            if (HttpContext.Session.GetString("IsUserGoingContact") == "yes")
            {
                if (btnInd == "Bireysel Hesap Oluştur")
                {
                    return RedirectToAction("IndividualContact");
                }
                else if (btnFirm == "Kurumsal Hesap Oluştur")
                {
                    return RedirectToAction("FirmContact");
                }
                else
                {
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Register", "User");
            }

        }

        [HttpGet]
        public IActionResult IndividualContact()
        {
            if (HttpContext.Session.GetString("IsUserGoingContact") == "yes")
            {
                ViewBag.City = returnCities();
                return View();
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }


        [HttpPost]
        public IActionResult IndividualContact(TableContact my_contact)
        {

            var tboxTC = Request.Form["ContactTc"];
            var tboxTaxLoc = Request.Form["ContactTaxLoc"];
            var device_unique_key = Request.Form["DeviceUniqueKey"];

            var name = Request.Form["tboxName"];
            var surname = Request.Form["tboxSurname"];

            if (device_unique_key[0] == "")
            {
                ViewBag.validateErrorDeviceKey = "Cihaz numarası boş bırakılamaz";
                ViewBag.City = returnCities();
                return View();
            }

            var device = device_manager.IsDeviceExistingBL(device_unique_key[0].ToString());





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
            else if (device == null)
            {
                ViewBag.validateErrorDeviceKey = "Girdiğiniz cihaz numarası bulunamadı";
                ViewBag.City = returnCities();
                return View();
            }
            else if (name[0] == "")
            {
                ViewBag.validateErrorName = "İsim boş bırakılamaz";
                ViewBag.City = returnCities();
                return View();
            }
            else if (surname[0] == "")
            {
                ViewBag.validateErrorSurname = "Soy isim boş bırakılamaz";
                ViewBag.City = returnCities();
                return View();
            }
            else
            {

                if (user_manager.IsIndividualContactExistingBL(my_contact.ContactTc) != null)
                {
                    ViewBag.validateError = "Bu TC kimlik numarası sisteme kayıtlıdır. Lütfen hesaba giriş yapınız.";
                    ViewBag.City = returnCities();
                    return View();
                }
                else
                {
                    my_contact.ContactUserId = Convert.ToInt32(HttpContext.Session.GetInt32("RegisteringUserID"));
                    int selectedCountyID = Convert.ToInt32(Request.Form["countyDrop"]);
                    my_contact.ContactCounty = selectedCountyID;
                    HttpContext.Session.SetString("RegisteringName", name);
                    HttpContext.Session.SetString("RegisteringSurname", surname);
                    HttpContext.Session.SetString("RegisteringDeviceKey", device.DeviceUniqeKey);
                    HttpContext.Session.SetString("IsUserGoingPersonInfos", "yes");

                    contact_manager.Add(my_contact);
                    HttpContext.Session.SetInt32("ContactID", my_contact.ContactId);
                    return RedirectToAction("LinkDeviceIndividual", "User");
                }

            }


        }

        [HttpGet]
        public IActionResult LinkDeviceIndividual()
        {
            Random random = new Random();
            var random_code = random.Next(1000, 9999);
            HttpContext.Session.SetString("LinkDeviceCode", random_code.ToString());
            mq.sendRandomCodeToDevice(HttpContext.Session.GetString("RegisteringDeviceKey"), random_code.ToString());
            return View();
        }

        [HttpPost]
        public IActionResult LinkDeviceIndividual(string enteredcode)
        {
            if (enteredcode== HttpContext.Session.GetString("LinkDeviceCode"))
            {
                var selected_device = device_manager.findDevicebyUniqeKey(HttpContext.Session.GetString("RegisteringDeviceKey"));
                MqttConnector mq = new MqttConnector();
                mq.sendRandomCodeToDevice(HttpContext.Session.GetString("RegisteringDeviceKey"), 0.ToString());
                TableTm new_tm = new TableTm();
                new_tm.TmdeviceId = selected_device.DeviceId;
                new_tm.TMDate = DateTime.Now;
                tm_manager.Add(new_tm);
                return RedirectToAction("IndividualPersonInfos", "User");
            }
            else
            {
                ViewBag.mailValidationText = "Yanlış kod girdiniz";
                return View();
            }
          
        }

        [HttpGet]
        public IActionResult LinkDeviceFirm()
        {
            Random random = new Random();
            var random_code = random.Next(1000, 9999);
            HttpContext.Session.SetString("LinkDeviceCode", random_code.ToString());
            mq.sendRandomCodeToDevice(HttpContext.Session.GetString("RegisteringDeviceKey"), random_code.ToString());
            return View();
        }

        [HttpPost]
        public IActionResult LinkDeviceFirm(string enteredcode)
        {
            if (enteredcode == HttpContext.Session.GetString("LinkDeviceCode"))
            {
                var selected_device = device_manager.findDevicebyUniqeKey(HttpContext.Session.GetString("RegisteringDeviceKey"));
                mq.MqttConnectionLoad();
                mq.sendRandomCodeToDevice(HttpContext.Session.GetString("RegisteringDeviceKey"), 0.ToString());
                TableTm new_tm = new TableTm();
                new_tm.TmdeviceId = selected_device.DeviceId;
                new_tm.TMDate = DateTime.Now;
                tm_manager.Add(new_tm);
                return RedirectToAction("FirmPersonInfos", "User");
            }
            else
            {
                ViewBag.mailValidationText = "Yanlış kod girdiniz";
                return View();
            }

        }


        [HttpGet]
        public IActionResult FirmContact()
        {

            if (HttpContext.Session.GetString("IsUserGoingContact") == "yes")
            {
                ViewBag.City = returnCities();
                return View();
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }


        [HttpPost]
        public IActionResult FirmContact(TableContact my_contact)
        {

            var tboxfirmName = Request.Form["ContactFirmName"];
            var tboxfirmAdress = Request.Form["ContactFirmAdress"];
            var tboxTaxLoc = Request.Form["ContactTaxLoc"];
            var tboxTaxNo = Request.Form["ContactTaxNo"];

            var device_unique_key = Request.Form["DeviceUniqueKey"];

            if (device_unique_key[0] == "")
            {
                ViewBag.validateErrorDeviceKey = "Cihaz numarası boş bırakılamaz";
                ViewBag.City = returnCities();
                return View();
            }

            var device = device_manager.IsDeviceExistingBL(device_unique_key[0].ToString());

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
            else if (device == null)
            {
                ViewBag.validateErrorDeviceKey = "Girdiğiniz cihaz numarası bulunamadı";
                ViewBag.City = returnCities();
                return View();
            }
            else
            {
                if (user_manager.IsFirmContactExistingBL(my_contact.ContactTaxNo) != null)
                {
                    ViewBag.validateError = "Bu vergi numarası sisteme kayıtlıdır. Lütfen hesaba giriş yapınız.";
                    ViewBag.City = returnCities();
                    return View();
                }
                else
                {
                    my_contact.ContactUserId = Convert.ToInt32(HttpContext.Session.GetInt32("RegisteringUserID"));
                    int selectedCountyID = Convert.ToInt32(Request.Form["countyDrop"]);
                    my_contact.ContactCounty = selectedCountyID;
                    contact_manager.Add(my_contact);
                    HttpContext.Session.SetInt32("ContactID", my_contact.ContactId);
                    HttpContext.Session.SetString("IsUserGoingPersonInfos", "yes");
                    HttpContext.Session.SetString("RegisteringDeviceKey", device.DeviceUniqeKey);
                    return RedirectToAction("LinkDeviceFirm", "User");
                }

            }


        }

        [HttpGet]
        public IActionResult IndividualPersonInfos()
        {

            if (HttpContext.Session.GetString("IsUserGoingPersonInfos") == "yes")
            {

                return View();
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }

        [HttpPost]
        public IActionResult IndividualPersonInfos(TablePerson my_person)
        {
            var tboxPhone = Request.Form["PersonPhone"];
            if (tboxPhone[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "Telefon numarası boş bırakılamaz.";

                return View();
            }
            else
            {
                my_person.PersonName = HttpContext.Session.GetString("RegisteringName");
                my_person.PersonSurname = HttpContext.Session.GetString("RegisteringSurname");
                my_person.PersonPhone = tboxPhone[0];
                my_person.PersonContactId = Convert.ToInt32(HttpContext.Session.GetInt32("ContactID"));
                my_person.PersonMail = (HttpContext.Session.GetString("RegisteringMail"));
                my_person.PersonUserId = Convert.ToInt32(HttpContext.Session.GetInt32("RegisteringUserID"));

                var person_count = user_manager.IsInfosExistingBL(my_person.PersonUserId);
                if (person_count >= 3)
                {

                    HttpContext.Session.SetString("IsUserLogged", "yes");
                    HttpContext.Session.SetString("IsThereThreePersons", "yes");
                    HttpContext.Session.SetInt32("LoggedID", my_person.PersonUserId);
                    return RedirectToAction("MainPage", "Main");
                }
                else
                {
                    person_manager.Add(my_person);
                    HttpContext.Session.SetString("IsUserLogged", "yes");
                    HttpContext.Session.SetInt32("LoggedID", my_person.PersonUserId);
                    return RedirectToAction("MainPage", "Main");
                }




            }

        }


        [HttpGet]
        public IActionResult FirmPersonInfos()
        {

            if (HttpContext.Session.GetString("IsUserGoingPersonInfos") == "yes")
            {

                return View();
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }

        [HttpPost]
        public IActionResult FirmPersonInfos(TablePerson my_person)
        {
            var tboxPhone = Request.Form["PersonPhone"];
            var tboxName = Request.Form["PersonName"];
            var tboxSurname = Request.Form["PersonSurname"];
            if (tboxPhone[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "Telefon numarası boş bırakılamaz.";

                return View();
            }
            else if (tboxName[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "İsim boş bırakılamaz.";

                return View();
            }
            else if (tboxSurname[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "Soy isim boş bırakılamaz.";

                return View();
            }
            else
            {
                my_person.PersonName = tboxName[0];
                my_person.PersonSurname = tboxSurname[0];
                my_person.PersonPhone = tboxPhone[0];
                my_person.PersonContactId = Convert.ToInt32(HttpContext.Session.GetInt32("ContactID"));
                my_person.PersonMail = (HttpContext.Session.GetString("RegisteringMail"));
                my_person.PersonUserId = Convert.ToInt32(HttpContext.Session.GetInt32("RegisteringUserID"));

                var person_count = user_manager.IsInfosExistingBL(my_person.PersonUserId);
                if (person_count >= 3)
                {

                    HttpContext.Session.SetString("IsUserLogged", "yes");
                    HttpContext.Session.SetString("IsThereThreePersons", "yes");
                    HttpContext.Session.SetInt32("LoggedID", my_person.PersonUserId);
                    return RedirectToAction("MainPage", "Main");
                }
                else
                {
                    person_manager.Add(my_person);
                    HttpContext.Session.SetString("IsUserLogged", "yes");
                    HttpContext.Session.SetInt32("LoggedID", my_person.PersonUserId);
                    return RedirectToAction("MainPage", "Main");
                }

            }

        }


        [HttpGet]
        public IActionResult NotCompletedIndividualPersonInfos()
        {

            if (HttpContext.Session.GetString("IsUserGoingPersonInfos") == "yes")
            {

                return View();
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }

        [HttpPost]
        public IActionResult NotCompletedIndividualPersonInfos(TablePerson my_person)
        {
            var tboxPhone = Request.Form["PersonPhone"];
            var tboxName = Request.Form["PersonName"];
            var tboxSurname = Request.Form["PersonSurname"];
            if (tboxPhone[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "Telefon numarası boş bırakılamaz.";

                return View();
            }
            else if (tboxName[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "İsim boş bırakılamaz.";

                return View();
            }
            else if (tboxSurname[0] == "")
            {
                ViewBag.validateErrorFirmAdress = "Soy isim boş bırakılamaz.";

                return View();
            }
            else
            {
                my_person.PersonName = tboxName[0];
                my_person.PersonSurname = tboxSurname[0];
                my_person.PersonPhone = tboxPhone[0];
                my_person.PersonContactId = Convert.ToInt32(HttpContext.Session.GetInt32("ContactID"));
                my_person.PersonMail = (HttpContext.Session.GetString("RegisteringMail"));
                my_person.PersonUserId = Convert.ToInt32(HttpContext.Session.GetInt32("RegisteringUserID"));
                var person_count = user_manager.IsInfosExistingBL(my_person.PersonUserId);
                if (person_count >= 3)
                {

                    HttpContext.Session.SetString("IsUserLogged", "yes");
                    HttpContext.Session.SetString("IsThereThreePersons", "yes");
                    HttpContext.Session.SetInt32("LoggedID", my_person.PersonUserId);
                    return RedirectToAction("MainPage", "Main");
                }
                else
                {
                    person_manager.Add(my_person);
                    HttpContext.Session.SetString("IsUserLogged", "yes");
                    HttpContext.Session.SetInt32("LoggedID", my_person.PersonUserId);
                    return RedirectToAction("MainPage", "Main");
                }


            }

        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult Login()
        {
            SessionOperations.AbandonSession(this);
            return View();
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult Login(TableUser my_user)
        {
            if (my_user.UserMail != null && my_user.UserPassword != null)
            {
                my_user.UserPassword = Sha256Hash.ComputeSha256Hash(my_user.UserPassword);
                var existing_user = user_manager.UserLoginBL(my_user);

                var is_user_admin = user_manager.IsUserAdmin(my_user);

                if (is_user_admin!=null)
                {
                    HttpContext.Session.SetString("IsAdminLogged", "yes");
                    HttpContext.Session.SetInt32("LoggedID", is_user_admin.UserId);

                    return RedirectToAction("AdminMainPage", "Admin");
                }
                else
                {
                    if (existing_user != null)
                    {
                        var isUserCompletedContact = user_manager.isUserCompletedContactBL(my_user.UserMail);
                        var isUserCompletedPersonInfos = user_manager.isUserCompletedPersonInfos(my_user.UserMail);
                        if (isUserCompletedContact.ElementAt(0).Key == "AccountType")
                        {
                            HttpContext.Session.SetString("RegisteringMail", my_user.UserMail);
                            HttpContext.Session.SetString("IsUserGoingContact", "yes");
                            HttpContext.Session.SetInt32("RegisteringUserID", isUserCompletedContact.ElementAt(0).Value);

                            return RedirectToAction("AccountType", "User");
                        }
                        else if (isUserCompletedPersonInfos.ElementAt(0).Key[0] == "GoToContact")
                        {
                            HttpContext.Session.SetString("RegisteringMail", my_user.UserMail);
                            HttpContext.Session.SetString("IsUserGoingContact", "yes");
                            HttpContext.Session.SetInt32("RegisteringUserID", isUserCompletedPersonInfos.ElementAt(0).Value[0]);

                            return RedirectToAction("AccountType", "User");
                        }
                        else if (isUserCompletedPersonInfos.ElementAt(0).Key[0] == "FirmPersonInfos")
                        {
                            HttpContext.Session.SetString("RegisteringMail", my_user.UserMail);
                            HttpContext.Session.SetString("IsUserGoingPersonInfos", "yes");
                            HttpContext.Session.SetInt32("RegisteringUserID", isUserCompletedPersonInfos.ElementAt(0).Value[0]);
                            HttpContext.Session.SetInt32("ContactID", isUserCompletedPersonInfos.ElementAt(0).Value[1]);

                            return RedirectToAction("FirmPersonInfos", "User");
                        }
                        else if (isUserCompletedPersonInfos.ElementAt(0).Key[0] == "NotCompletedIndividualPersonInfos")
                        {
                            HttpContext.Session.SetString("RegisteringMail", my_user.UserMail);
                            HttpContext.Session.SetString("IsUserGoingPersonInfos", "yes");
                            HttpContext.Session.SetInt32("RegisteringUserID", isUserCompletedPersonInfos.ElementAt(0).Value[0]);
                            HttpContext.Session.SetInt32("ContactID", isUserCompletedPersonInfos.ElementAt(0).Value[1]);

                            return RedirectToAction("NotCompletedIndividualPersonInfos", "User");
                        }
                        else
                        {
                            HttpContext.Session.SetString("IsUserLogged", "yes");
                            HttpContext.Session.SetInt32("LoggedID", existing_user.UserId);

                            return RedirectToAction("MainPage", "Main");


                        }



                    }
                    else
                    {
                        @ViewBag.validateError = "Kullanıcı adı veya şifre yanlış.";
                        return View();
                    }
                }

         
            }
            else
            {
                @ViewBag.validateError = "Kullanıcı adı veya şifre boş bırakılamaz.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordSendMailMiddle()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ForgotPasswordSendMailMiddle(string myMail)
        {
            
            HttpContext.Session.SetString("IsUserGoingForgotPassword", "yes");
            HttpContext.Session.SetString("ForgotMail", myMail);
            return RedirectToAction("ForgotPasswordSendMail","User");
        }

        [HttpGet]
        public IActionResult ForgotPasswordSendMail()
        {
            if (HttpContext.Session.GetString("IsUserGoingForgotPassword") == "yes")
            {
                try
                {
                    var sended_mail = HttpContext.Session.GetString("ForgotMail");
                    Random random = new Random();
                    var random_code = random.Next(1000, 9999);
                    HttpContext.Session.SetString("ForgotMailRandomCode", random_code.ToString());
                    ViewBag.sendedMail = sended_mail;
                    var senderEmail = new MailAddress("emirozdemirdeneme@gmail.com", "biocoder");
                    var receiverEmail = new MailAddress(sended_mail, "Receiver");
                    var password = "uqirysrqzrbxjeii";
                    var sub = "şifre değiştirme kodu";
                    var body = random_code;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = sub,
                        Body = body.ToString()
                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();

                }
                catch (Exception)
                {
                    ViewBag.Error = "Some Error";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult ForgotPasswordSendMail(string enteredcode, string enteredmail)
        {
            if (HttpContext.Session.GetString("IsUserGoingForgotPassword") == "yes")
            {
                var old_code = HttpContext.Session.GetString("ForgotMailRandomCode");

                if (enteredcode == null)
                {
                    ViewBag.mailValidationText = "Kod girilecek kısmı boş girdiniz.";
                    return View();
                }
                else
                {
                    if (enteredcode == old_code)
                    {
                        HttpContext.Session.SetString("IsUserGoingChangePassword", "yes");
                        return RedirectToAction("ChangePassword", "User");
                    }
                    else
                    {
                        ViewBag.mailValidationText = "Yanlış kod girdiniz.";
                        return View();
                    }
                    
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("IsUserGoingChangePassword") == "yes")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
           
        }

        [HttpPost]
        public IActionResult ChangePassword(string enteredpass1, string enteredpass2)
        {
            if (HttpContext.Session.GetString("IsUserGoingChangePassword") == "yes")
            {
                if (enteredpass1 == enteredpass2)
                {
                    var sended_mail = HttpContext.Session.GetString("ForgotMail");
                    var hashed_pass = Sha256Hash.ComputeSha256Hash(enteredpass1);
                    user_manager.ChangePasswordBL(sended_mail, hashed_pass);
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    ViewBag.passValidationText = "Şifreler eşleşmiyor.";
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

    }
}
