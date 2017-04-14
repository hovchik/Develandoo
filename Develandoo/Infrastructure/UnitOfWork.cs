using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Develandoo.Infrastructure
{
    /// <summary>
/// I used pattern UnitOfWork for make more easyly working with data, save/get/update
/// </summary>
    public class UnitOfWork : IDisposable
    {
        DevelandooBaseEntities context = new DevelandooBaseEntities();
        private UserRepository repos;
        private bool disposed = false;
        public UserRepository Users => repos ?? (repos = new UserRepository(context));

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// Dispose pattern, in fact its generated from Visual studio Resharper, it make sense for lot of code ))
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}