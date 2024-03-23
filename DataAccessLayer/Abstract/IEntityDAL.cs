using EntityLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IEntityDAL<T> where T : class, IDatabaseEntity, new()
    {
        void Add(T temp_entity);
        void Delete(T temp_entity);
        void Edit(T temp_entity);
        List<T> List();
    
    }
}
