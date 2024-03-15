namespace Pocos
{
    public class Department
    {
        public int Id { get; set; }
        public string? DeptName { get; set; }
        public ISet<Employee>? Employees { get; set; }

        public override string ToString()
        {
            string employeesString = Employees == null ? "null" :
                string.Join(", ", Employees.Select(e => $"[Id={e.Id}, FirstName={e.FirstName}, LastName={e.LastName}]"));
            return $"Department : [Id={Id}, DeptName={DeptName}, Employees={employeesString}]";
        }
    }
}