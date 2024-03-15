using Pocos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private string _connectionString;

        public DepartmentRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool CreateNewRow(Department entity)
        {
            bool entityIsCreated = false;

            try
            {
                /*      Step 1: Establish SqlConnection and open the connection in try
                */
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // step 2: Create command instance turn on identity insert so that SQL can be responsbile
                    // for ID creation
                    using (IDbCommand identityCmd = conn.CreateCommand())
                    {
                        identityCmd.CommandText = "SET IDENTITY_INSERT Employee ON";
                        identityCmd.ExecuteNonQuery();
                    }
                    // Step 3: Create command instance for SQL insert command

                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        var ifDeptExist = ReadRowByName(entity.DeptName);
                        if (object.ReferenceEquals(ifDeptExist, null))
                        {
                            cmd.CommandText = "INSERT INTO Departments (DEPT_NAME) VALUES (@DeptName)";

                            // Step 4: Map the parameters to the SQL query using SqlParameter class
                            IDbDataParameter param1 = new SqlParameter();
                            param1.ParameterName = "@DeptName";
                            param1.Value = entity.DeptName;
                            cmd.Parameters.Add(param1);

                            // Step 5: Execute the SqlQuery
                            var numberAffectedRows = cmd.ExecuteNonQuery();
                            Console.WriteLine($"Number of affected rows = {numberAffectedRows}");
                            entityIsCreated = true;
                        }
                        else
                        {
                            Console.WriteLine("Department exist in database");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
                
            return entityIsCreated;
        }
        

        public bool DeleteRow(int id)
        {
            bool isRowDeleted = false;
            // step1 establish connection and open
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Step2: create the Delete SQL command
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Departments WHERE DEPT_NO=@DEPT_ID";

                        // step3 : define the parameters for the SQL delete command
                        IDataParameter param1 = new SqlParameter();
                        param1.ParameterName = "@DEPT_ID";
                        param1.Value = id;
                        cmd.Parameters.Add(param1);

                        // Step4 : Execute the SQL query

                        var numberAffectedRows = cmd.ExecuteNonQuery();
                        Console.WriteLine($"numAffectedRows = {numberAffectedRows}");
                        isRowDeleted = true;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            return isRowDeleted;
        }

        public bool DeleteRow(Department entity)
        {
            return DeleteRow(entity.Id);
        }

        public IEnumerable<Department> ReadAllRows()
        {
            var departments = new Dictionary<int, Department>();
            /* Step 1: Establishing connection using SqlConnection class - Creating a SqlConnection Instance
                    
                    - using keyword
                    When you use the using statement with an object that implements the 
                    IDisposable interface, like IDbConnection, it ensures that the Dispose() 
                    method of that object is called automatically when the code exits the using 
                    block. This ensures the closing of the connection to the database.
                    - SqlConnection class
                    SqlConnection is used to establish a connection to a SQL Server database. 
                    Once established, you can use this connection to execute SQL commands (queries, 
                    updates, etc.) against the database.
                    SqlConnection implements IDbConnection Interface
                    - IDbConnection interface type
                    IDbConnection is an interface provided by ADO.NET that represents a connection 
                    to a data source. It defines a contract for database connections, including 
                    methods like Open(), Close(), and properties like ConnectionString.
                    - IDbConnection type vs SqlConnection type
                    By using IDbConnection, you can write more generic code that can work with any 
                    database provider, not just SQL Server. This is useful for scenarios where you 
                    might want your code to be able to switch between different database providers 
                    without needing significant changes.
             */
            using (IDbConnection conn = new SqlConnection(_connectionString))       // I can pass _connectionString through constructor or assignment
            {
                /*
                 *  Open() method establishes the connection officially
                 */

                try
                {
                    conn.Open(); ;

                    /*  Step 2: Create a command instance from the SqlConnection instance and establish
                                the sql command
                                Must create a CreateCommand instance rather than passing in directly to 

                                SqlConnection constructor
                                - IDbCommand (IDb allow more generic code to work with other db provider)
                                IDbCommand is an interface provided by ADO.NET that represents a command 
                                to be executed against a data source. It defines a set of properties and methods for 
                                executing SQL commands, such as queries, updates, and stored procedures. Here's an 
                                overview of the IDbCommand interface:  
                                - .CommandText
                                Define the Sql command
                     */
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        var SQLQuery = "SELECT d.DEPT_NO, d.DEPT_NAME, e.EMP_ID, e.EMP_FST, e.EMP_LST " +
                                        "FROM Departments d LEFT JOIN Employee e on d.DEPT_NO = e.FK_DEPT_NO";


                        cmd.CommandText = SQLQuery;


                        /*  Step 3: Utilize ExecuteReader() method on the command instance to read the database
                         *          and pass it into a IDataReader object         
                                    - ExecuteReader() 
                                    The ExecuteReader() method in ADO.NET is used to execute a SQL command 
                                    that typically returns rows of data, such as a SELECT query, and retrieves 
                                    the results as a data reader. 
                                    ExecuteReader() returns a DataReader object - IDataReader (agnostic across DB 
                                    or SqlDataReader object)
                                    - IDataReader 
                                    An interface provided by ADO.NET that represents a forward-only, read-only stream 
                                    of rows from a data source. It defines methods for reading data from the result 
                                    set, such as Read(), GetBoolean(), GetString(), etc. This interface is database 
                                    agnostic, meaning it can be used with different database providers.
                         */
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            /*  Step 4: Map out the data object from the Reader object
                             *      - Read()
                             *      The Read() method is a method provided by the IDataReader interface in ADO.NET. It's 
                             *      used to advance the data reader to the next record in the result set, allowing you to 
                             *      sequentially read each row of data returned by a SQL query.
                             *      - GetInt, GetString 
                             *      Getter methods to parse the data into specifc types
                             */
                            while (reader.Read())
                            {
                                int deptID = reader.GetInt32(0);
                                if (!departments.ContainsKey(deptID))
                                {
                                    //Console.WriteLine("department database DOES NOT contain deptID");
                                    var dept = new Department();
                                    dept.Id = deptID;
                                    dept.DeptName = reader.GetString(1);
                                    var emp = new Employee();
                                    ISet<Employee> employeeList = new HashSet<Employee>();
                                    if (!reader.IsDBNull(2))
                                    {
                                        //Console.WriteLine("emp list is not null");
                                        emp.Id = reader.GetInt32(2);
                                        emp.FirstName = reader.GetString(3);
                                        emp.LastName = reader.GetString(4);
                                        emp.Dept = dept;
                                        employeeList.Add(emp);
                                        dept.Employees = employeeList;
                                        departments.Add(deptID, dept);
                                    }
                                    else
                                    {
                                        //Console.WriteLine("emp list is null");
                                        dept.Employees = employeeList;
                                        departments.Add(deptID, dept);
                                    }
                                }
                                else
                                {
                                    //Console.WriteLine("department database contain deptID");
                                    var dept = new Department();
                                    dept.Id = deptID;
                                    dept.DeptName = reader.GetString(1);
                                    var emp = new Employee();
                                    if (!object.ReferenceEquals(reader.GetInt32(2), null))
                                    {
                                        emp.Id = reader.GetInt32(2);
                                        emp.FirstName = reader.GetString(3);
                                        emp.LastName = reader.GetString(4);
                                        emp.Dept = dept;
                                        departments[deptID].Employees.Add(emp);
                                    }

                                }
                                

                            }
       

                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
            return departments.Values;
        }

        public Department ReadRowById(int id)
        {
            var dept = new Department();
            // Step1 establish the connection and open
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Step2 : create the search by ID SQL command
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT d.DEPT_NO, d.DEPT_NAME, e.EMP_ID, e.EMP_FST, e.EMP_LST " +
                                        "FROM Departments d LEFT JOIN Employee e on d.DEPT_NO = e.FK_DEPT_NO" +
                                        "WHERE DEPT_NO=@DEPT_ID";

                        // Step3 : in the SQL there is parameters define.
                        IDbDataParameter param1 = new SqlParameter();
                        param1.ParameterName = "@DEPT_ID";
                        param1.Value = id;
                        cmd.Parameters.Add(param1);

                        // step 4: Execute the SQL using ExecuteReader
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int deptID = reader.GetInt32(0);
                                // Check if dept.ID == zero because if it is the first row of the return list of dept from sql command
                                // We need to check this because the SQL command return multiple dept.ID because of joined with emp table
                                if (dept.Id == 0)
                                //if (Object.ReferenceEquals(dept.Id, null))
                                {
                                    dept.Id = id;
                                    dept.DeptName = reader.GetString(1);
                                    var emp = new Employee();
                                    if (!reader.IsDBNull(2))
                                    {
                                        emp.Id = reader.GetInt32(2);
                                        emp.FirstName = reader.GetString(3);
                                        emp.LastName = reader.GetString(4);
                                        emp.Dept = dept;
                                        ISet<Employee> employeeList = new HashSet<Employee>();
                                        employeeList.Add(emp);
                                        dept.Employees = employeeList;
                                    }
                                    else
                                    {
                                        ISet<Employee> employeeList = new HashSet<Employee>();
                                        dept.Employees = employeeList;
                                    }

                                }
                                // If dept.ID != 0, then the dept has already been assigned with dept attributes, just need to
                                // Update its employee list.
                                else
                                {
                                    var emp = new Employee();
                                    emp.Id = reader.GetInt32(2);
                                    emp.FirstName = reader.GetString(3);
                                    emp.LastName = reader.GetString(4);
                                    emp.Dept = dept;
                                    dept.Employees.Add(emp);
                                    
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            return dept;
        }

        public Department ReadRowByName(string name)
        {
            var dept = new Department();
            // Step1 establish the connection and open
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();



                    // Step2 : create the search by ID SQL command
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT d.DEPT_NO, d.DEPT_NAME, e.EMP_ID, e.EMP_FST, e.EMP_LST " +
                                        "FROM Departments d LEFT JOIN Employee e on d.DEPT_NO = e.FK_DEPT_NO " +
                                        "WHERE DEPT_NAME=@DEPT_NAME";

                        // Step3 : in the SQL there is parameters define.
                        IDbDataParameter param1 = new SqlParameter();
                        param1.ParameterName = "@DEPT_NAME";
                        param1.Value = name;
                        cmd.Parameters.Add(param1);

                        // step 4: Execute the SQL using ExecuteReader
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ISet<Employee> employeeList = new HashSet<Employee>();
                                //Console.WriteLine($"{reader.GetInt32(0)}, {reader.GetString(1)}");

                                // Check if dept.ID == zero because if it is the first row of the return list of dept from sql command
                                if (dept.Id == 0)           
                                {
                                    dept.Id = reader.GetInt32(0);
                                    dept.DeptName = reader.GetString(1);
                                    var emp = new Employee();
                                    if (!reader.IsDBNull(2))
                                    {
                                        emp.Id = reader.GetInt32(2);
                                        emp.FirstName = reader.GetString(3);
                                        emp.LastName = reader.GetString(4);
                                        emp.Dept = dept;
                                        employeeList.Add(emp);
                                        dept.Employees = employeeList;
                                    }
                                    else
                                    {
                                        dept.Employees = employeeList;
                                    }

                                }
                                else
                                {
                                    /* If dept.ID is not equal to zero, means we already assign dept to its dept attributes
                                     * already and we are updating the employee list. In this route, there is no need to check if 
                                     * emp list is empty or not since it is confirm not empty
                                     */
                                    var emp = new Employee();
                                    emp.Id = reader.GetInt32(2);
                                    emp.FirstName = reader.GetString(3);
                                    emp.LastName = reader.GetString(4);
                                    emp.Dept = dept;
                                    employeeList.Add(emp);
                                    dept.Employees = employeeList;

                                }
                            }
                            else
                            {
                                return null;
                            }
                        }


                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            return dept;
        }

        public bool UpdateRow(Department entity)
        {
            {
                bool entityIsUpdated = false;

                try
                {
                    // Step 1: Set up connection and open
                    using (IDbConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();

                        //Step 2: Create the update SQL command
                        using (IDbCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "UPDATE Department set DEPT_NAME=@DEPT_NAME " +
                                             "WHERE DEPT_NO=@DEPT_ID";

                            // Step 3 : define the parameter for the SQL commnad
                            IDbDataParameter param1 = new SqlParameter("DEPT_ID", SqlDbType.Int);
                            param1.Value = entity.Id;
                            cmd.Parameters.Add(param1);

                            IDbDataParameter param2 = new SqlParameter("DEPT_NAME", SqlDbType.Int);
                            param2.Value = entity.DeptName;
                            cmd.Parameters.Add(param2);

                            // step 4: execute the query
                            var numberAffectedRows = cmd.ExecuteNonQuery();
                            Console.WriteLine($"numAffectedRows = {numberAffectedRows}");
                            entityIsUpdated = true;


                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                return entityIsUpdated;
            }
        }
    }
}
