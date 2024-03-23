using DataAccessLayer.Concrete;
using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ContactBL : ManagerRepository<TableContact, ContactDAL>
    {

        ContactDAL cd = new ContactDAL();
        public TableContact getContactUserByIDBL(int user_id)
        {
           return cd.getContactByUserID(user_id);
        }

        public string CheckContactFirmOrIndividualBL(int user_id)
        {
            return cd.CheckContactFirmOrIndividual(user_id);
        }
    }
}
