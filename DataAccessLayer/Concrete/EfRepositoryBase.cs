
using DataAccessLayer.Abstract;
using deviceInterfacev2.Models;
using EntityLayer.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class EfRepositoryBase<Tentity, Tcontext> : IEntityDAL<Tentity> where Tentity : class, IDatabaseEntity, new()
         where Tcontext : TempDatabaseContext, new()
    {
        public void Add(Tentity tempEntity)
        {
            using (Tcontext context = new Tcontext())
            {
                var addEntity = context.Entry(tempEntity);

                addEntity.State = EntityState.Added;

                context.SaveChanges();
            }
        }

        public void Delete(Tentity tempEntity)
        {

            using (Tcontext context = new Tcontext())
            {
                var deletingEntity = context.Entry(tempEntity);

                deletingEntity.State = EntityState.Deleted;

                context.SaveChanges();
            }
        }

        public void Edit(Tentity temp_entity)
        {
            using (Tcontext context = new Tcontext())
            {
                var editEntity = context.Entry(temp_entity);

                editEntity.State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        public List<Tentity> List()
        {
            using (Tcontext context = new Tcontext())
            {
                return context.Set<Tentity>().ToList();
            }
        }

        public List<Tentity> ListAll()
        {

            using (Tcontext context = new Tcontext())
            {
                return context.Set<Tentity>().ToList();
            }
        }
    }


}
