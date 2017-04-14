using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Develandoo.Infrastructure
{
    public class UserRepository : IRepository<Users>
    {
        private DevelandooBaseEntities dbContext;

        public UserRepository(DevelandooBaseEntities dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Create(Users user)
        {
            dbContext.Users.Add(user);
        }

        public void Update(Users user)
        {
            dbContext.Entry(user).State = EntityState.Modified;

        }

        public Users Get(int userId)
        {
            return dbContext.Users.Find(userId);
        }

        public Users GetWithPredict(Func<Users, bool> predicate)
        {
            return dbContext.Users.FirstOrDefault(predicate);
        }

        public int CountByRule(Func<Users, bool> predicate)
        {
            return dbContext.Users.Count(predicate);
        }
    }
}