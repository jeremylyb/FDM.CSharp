using Pocos;
using Repository;
using System;

namespace Assignment_ADO.NETConnectedModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Data Source=DESKTOP-DQUSH87; Initial Catalog=CompanyFdm; Integrated Security=sspi;";

            var empRepo = new EmployeeRepository(connectionString);

            var depRepo = new DepartmentRepository(connectionString);
            var empList = empRepo.ReadAllRows();
            foreach (var emp in empList)
            {
                Console.WriteLine(emp);
            }

            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            Console.WriteLine("**************************** Persist new emp ***************************");
            var financeDept = depRepo.ReadRowByName("Finance Department");
            var emp1 = new Employee() { FirstName = "Mark", LastName = "Brown", Dept = financeDept };
            empRepo.CreateNewRow(emp1);
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }

            Console.WriteLine("**************************** Persist new Dept ***************************");

            var techDept = new Department() { DeptName = "Technology Department" };
            depRepo.CreateNewRow(techDept);
            Console.WriteLine("-----------Read all rows from Dept Repo");
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            Console.WriteLine("**************************** Delete Dept ID 2 ***************************");

            depRepo.DeleteRow(2);
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine("**************************** Delete Dept ID 1 ***************************");
            depRepo.DeleteRow(1);
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }

            Console.WriteLine("**************************** Update Emp ID 7 mark brown ***************************");
            emp1 = empRepo.ReadRowById(7);
            techDept = depRepo.ReadRowByName("Technology Department");
            emp1.Dept = techDept;
            Console.WriteLine(emp1);
            empRepo.UpdateRow(emp1);
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine("**************************** Update Emp ID 2 Jeremy Lam ***************************");
            var emp2 = empRepo.ReadRowById(2);
            emp2.Dept = techDept;
            Console.WriteLine(emp2);
            empRepo.UpdateRow(emp2);
            Console.WriteLine("ALL EMPLOYEES");
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine("ALL DEPARTMENTS");
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }

            Console.WriteLine("**************************** Create new sales dept and update emp id 1 and 3 ***************************");
            var salesDept = new Department() { DeptName = "Sales Department" };
            depRepo.CreateNewRow(salesDept);
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            Console.WriteLine($"Before adding to DB {salesDept}");
            salesDept = depRepo.ReadRowByName("Sales Department");
            Console.WriteLine($"After adding to DB {salesDept}");
            Console.WriteLine();
            Console.WriteLine("Updating employees");
            var johnDoe = empRepo.ReadRowById(1);
            var jasonChan = empRepo.ReadRowByName("Jason", "Chan");
            johnDoe.Dept = salesDept;
            jasonChan.Dept = salesDept;
            empRepo.UpdateRow(johnDoe);
            empRepo.UpdateRow(jasonChan);
            Console.WriteLine("ALL EMPLOYEES");
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine("ALL DEPARTMENTS");
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
            Console.WriteLine("**************************** delete empid 7 and 6 ***************************");
            empRepo.DeleteRow(johnDoe);
            var zzz999 = empRepo.ReadRowByName("ZZZ", "999");
            var abc123 = empRepo.ReadRowById(5);
            empRepo.DeleteRow(zzz999);
            empRepo.DeleteRow(abc123);
            empRepo.DeleteRow(7);
            Console.WriteLine("ALL EMPLOYEES");
            foreach (var emp in empRepo.ReadAllRows())
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine("ALL DEPARTMENTS");
            foreach (var dep in depRepo.ReadAllRows())
            {
                Console.WriteLine(dep);
            }
        }
    }
}
