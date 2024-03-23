using Microsoft.AspNetCore.Mvc;

namespace deviceInterface.ExtraFiles
{
    public class SessionOperations
    {
        public static void AbandonSession(Controller myController)
        {
            myController.HttpContext.Session.Remove("IsUserGoingValidation");
            myController.HttpContext.Session.Remove("IsUserGoingContact");
            myController.HttpContext.Session.Remove("IsUserGoingPersonInfos");
            myController.HttpContext.Session.Remove("IsAdminLogged");
            myController.HttpContext.Session.Remove("IsUserGoingForgotPassword");
            myController.HttpContext.Session.Remove("RegisteringMail");
            myController.HttpContext.Session.Remove("RegisteringDeviceKey");
            myController.HttpContext.Session.Remove("LinkDeviceUniqueKey");
            myController.HttpContext.Session.Remove("LinkDeviceName");
            myController.HttpContext.Session.Remove("LinkDeviceCode");
            myController.HttpContext.Session.Remove("MailRandomCode");
            myController.HttpContext.Session.Remove("ForgotMailRandomCode");
            myController.HttpContext.Session.Remove("ForgotMail");
            myController.HttpContext.Session.Remove("IsUserGoingChangePassword");
            myController.HttpContext.Session.Remove("IsUserLogged");
            myController.HttpContext.Session.Remove("LoggedID");
            myController.HttpContext.Session.Remove("RegisteringUserID");
            myController.HttpContext.Session.Remove("ContactID");
        }
    }
}
