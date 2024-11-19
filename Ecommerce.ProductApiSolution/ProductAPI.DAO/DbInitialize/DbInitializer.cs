using Microsoft.EntityFrameworkCore;
using ProductAPI.DAO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.DAO.DbInitialize
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ProductDbContext  _dbContext;

        public DbInitializer()
        {
            _dbContext = new ();
        }

        public void Initialize()
        {
            if (_dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                _dbContext.Database.Migrate();
            };
        }
    }
}
