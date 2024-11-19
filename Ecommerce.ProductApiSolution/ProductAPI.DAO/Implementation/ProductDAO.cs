using ProductApi.Domain.Entities;
using ProductAPI.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.DAO.Implementation
{
    public class ProductDAO : BaseDAO<Product>, IProductDAO
    {
        private static ProductDAO? _instance;
        public static ProductDAO Instance => _instance ??= new ();

        public ProductDAO() : base()
        {
            
        }
    }
}
