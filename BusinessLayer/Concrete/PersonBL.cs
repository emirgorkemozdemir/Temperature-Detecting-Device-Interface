using DataAccessLayer.Concrete;
using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class PersonBL : ManagerRepository<TablePerson, PersonDAL>
    {
        PersonDAL pd = new PersonDAL();
        public List<TablePerson> listPersonByUserIDBL(int user_id)
        {
          return  pd.listPersonByUserID(user_id);
        }

        public TablePerson getPersonByPersonIDBL(int person_id)
        {
            return pd.getPersonByPersonID(person_id);   
        }
    }
}
