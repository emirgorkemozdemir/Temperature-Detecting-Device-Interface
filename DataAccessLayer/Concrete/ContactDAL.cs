using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class ContactDAL : EfRepositoryBase<TableContact, TempDatabaseContext>
    {
        public TableContact getContactByUserID(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_contact = my_context.TableContacts.FirstOrDefault(a => a.ContactUserId == user_id);
                return selected_contact;
            }

        }

        public string CheckContactFirmOrIndividual(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var selected_contact = my_context.TableContacts.FirstOrDefault(a => a.ContactUserId == user_id);

                if (selected_contact.ContactTc==null)
                {
                    return "FirmContactInfos";
                }
                else
                {
                    return "IndividualContactInfos";
                }
            }
        }
    }
}
