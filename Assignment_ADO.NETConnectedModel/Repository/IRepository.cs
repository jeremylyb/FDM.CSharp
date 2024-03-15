using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal interface IRepository<T>
    {
        public bool CreateNewRow(T entity);
        public IEnumerable<T> ReadAllRows();
        public T ReadRowById(int id);
        public bool UpdateRow(T entity);
        public bool DeleteRow(int id);
        public bool DeleteRow(T entity);


    }
}
