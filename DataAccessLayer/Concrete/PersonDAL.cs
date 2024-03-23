using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class PersonDAL : EfRepositoryBase<TablePerson, TempDatabaseContext>
    {
        public List<TablePerson> listPersonByUserID(int user_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var list_person = my_context.TablePersons.Where(a=>a.PersonUserId== user_id).ToList();
                return list_person;
            }
        }

        public TablePerson getPersonByPersonID(int person_id)
        {
            using (TempDatabaseContext my_context = new TempDatabaseContext())
            {
                var my_person = my_context.TablePersons.Find(person_id);
                return my_person;
            }
        }
    }
}
