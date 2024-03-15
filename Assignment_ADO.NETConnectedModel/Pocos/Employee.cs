using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocos
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Department? Dept { get; set; }
        public override string ToString()
        {
            return $"Employee: [Id={Id}, Name={FirstName} {LastName}, DeptName={(Dept != null ? Dept.DeptName : "NULL")}]";
        }
    }
}
