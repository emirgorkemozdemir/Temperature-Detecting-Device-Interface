using DataAccessLayer.Concrete;
using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class UserBL : ManagerRepository<TableUser, UserDAL>
    {
        UserDAL user_dal = new UserDAL();

        public string UserRegisterExistingCheckBL(TableUser my_user)
        {
            return user_dal.UserRegisterExistingCheckDAL(my_user);
        }

        public TableUser UserLoginBL(TableUser my_user)
        {
            return user_dal.UserLoginDAL(my_user);
        }

        public Dictionary<string, int> isUserCompletedContactBL(string mail_adress)
        {
            return user_dal.isUserCompletedContact(mail_adress);
        }

        public Dictionary<List<string>, List<int>> isUserCompletedPersonInfos(string mail_adress)
        {
            return user_dal.isUserCompletedPersonInfos(mail_adress);
        }

        public TableContact IsIndividualContactExistingBL(string contact_tc)
        {
            return user_dal.IsIndividualContactExisting(contact_tc);
        }

        public TableContact IsFirmContactExistingBL(string contact_tc)
        {
            return user_dal.IsFirmContactExisting(contact_tc);
        }

        public int IsInfosExistingBL(int user_id)
        {
           return user_dal.IsInfosExisting(user_id);
        }

        public void ChangePasswordBL(string mail_adress, string new_password)
        {
           user_dal.ChangePassword(mail_adress, new_password);
        }

        public TableUser getUserByID(int id)
        {
            return user_dal.getUserByID(id);
        }
        public TableUser IsUserAdmin(TableUser my_user)
        {
            return user_dal.IsUserAdmin(my_user);
        }



            //public List<string> MergedInfos(int user_id)
            //{
            //    return user_dal.MergedInfos(user_id); 
            //}
        }
}
