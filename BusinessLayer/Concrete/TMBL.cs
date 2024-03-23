using DataAccessLayer.Concrete;
using deviceInterfacev2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class TMBL : ManagerRepository<TableTm, TMDAL>
    {
        TMDAL td = new TMDAL();
        public List<ReportModel> getLastMonthValues(string unique_key)
        {
            return td.getLastMonthValues(unique_key);
        }

    }
}
