using Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal interface IDepartmentRepository : IRepository<Department>

    {
        public Department ReadRowByName(string name);    
    }
}
