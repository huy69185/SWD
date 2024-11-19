﻿using ProductApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.DAO.Interface
{
    public interface IProductDAO : IBaseDAO<Product>
    {
    }
}