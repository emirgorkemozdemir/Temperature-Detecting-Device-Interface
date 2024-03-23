using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ManagerRepository<TEntity, DAL> : IEntityManager<TEntity> where TEntity : class, IDatabaseEntity, new()
         where DAL : IEntityDAL<TEntity>, new()
    {
        public void Add(TEntity temp_entity)
        {
            DAL dal = new DAL();
            dal.Add(temp_entity);
        }

        public void Delete(TEntity temp_entity)
        {
            DAL dal = new DAL();
            dal.Delete(temp_entity);
        }

        public void Edit(TEntity temp_entity)
        {
            DAL dal = new DAL();
            dal.Edit(temp_entity);
        }

        public List<TEntity> List()
        {
            DAL dal = new DAL();
            return dal.List();
        }
    }
    
    
}
