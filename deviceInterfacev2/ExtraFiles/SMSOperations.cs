
using netgsmWebService;

namespace deviceInterfacev2.ExtraFiles
{
    public class SMSOperations
    {
        public string sendSMS1N(string[] nums, string message)
        {
            smsnnClient smsgonder1n = new smsnnClient();
            string username = "3246060917";
            string password = "V9.RkK1W";
            string header = "Biocoder";
            String gelen_cevap = smsgonder1n.smsGonder1NV3Async(username,password,header,message,nums,"TR","","","",0,"3246060917").ToString();
            return gelen_cevap;
        }
    }
}
