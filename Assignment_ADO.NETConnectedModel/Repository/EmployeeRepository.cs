using Pocos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool CreateNewRow(Employee entity)
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
                    // Step 3: Create command instance for SQL insert command
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        var doesEmpExistInDB = ReadRowByName(entity.FirstName, entity.LastName);
                        if (object.ReferenceEquals(doesEmpExistInDB, null))
                        {
                            cmd.CommandText = "SET IDENTITY_INSERT Employee ON";
                            var numberAffectedRows = cmd.ExecuteNonQuery();
                            Console.WriteLine($"Identity command executed, {numberAffectedRows}");

                            cmd.CommandText = "INSERT INTO Employee (EMP_ID, EMP_FST, EMP_LST, FK_DEPT_NO) " +
                                                "VALUES (NEXT VALUE FOR [dbo].Employee_Id_Seq, @EMP_FST, @EMP_LST, @FK_DEPT_NO)";

                            // Step 4: Mape the employee object into the query
                            IDbDataParameter param1 = new SqlParameter();
                            param1.ParameterName = "@EMP_FST";
                            param1.Value = entity.FirstName;
                            cmd.Parameters.Add(param1);
                            IDbDataParameter param2 = new SqlParameter();
                            param2.ParameterName = "@EMP_LST";
                            param2.Value = entity.LastName;
                            cmd.Parameters.Add(param2);
                            IDbDataParameter param3 = new SqlParameter();
                            param3.ParameterName = "@FK_DEPT_NO";
                            param3.Value = entity.Dept.Id;
                            cmd.Parameters.Add(param3);

                            // step 5: execute the SqlQuery
                            numberAffectedRows = cmd.ExecuteNonQuery();
                            Console.WriteLine($"Number of affected rows = {numberAffectedRows}");
                            entityIsCreated = true;
                            // NO need to add this employee to Department's employeeList cause in DB, it doesn't maintain an employee list
                        }
                        else
                        {
                            Console.WriteLine($"Employee with name {entity.FirstName}, {entity.LastName} exist in database");
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
            // Step 1 : establish the connection and open
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    //Step 2 : Create the Delete command:
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Employee WHERE EMP_ID=@EMP_ID";

                        // step 3 : add the parameters to the SQL Command
                        IDbDataParameter param1 = new SqlParameter();
                        param1.ParameterName = "@EMP_ID";
                        param1.Value = id;
                        cmd.Parameters.Add(param1);

                        // step 4: execute the SQL
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

        public bool DeleteRow(Employee entity)
        {
            return DeleteRow(entity.Id);
        }

        public IEnumerable<Employee> ReadAllRows()
        {
            List<Employee> empList = new List<Employee>();
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.Connection = conn;
                        var SQLQuery = "Select e.EMP_ID, e.EMP_FST, e.EMP_LST, e.FK_DEPT_NO, D.DEPT_NAME" +
                                        " from Employee e LEFT JOIN Departments d on e.FK_DEPT_NO=d.DEPT_NO";
                        cmd.CommandText = SQLQuery;

                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dept = new Department();
                                var emp = new Employee();

                                // Cause Id, FirstName, LastName are structured in DB as Not null. Can go straight to colum
                                emp.Id = reader.GetInt32(0);
                                emp.FirstName = reader.GetString(1);
                                emp.LastName = reader.GetString(2);
                                if (!reader.IsDBNull(3))
                                {
                                    dept.Id = reader.GetInt32(3);
                                    dept.DeptName = reader.GetString(4);
                                    emp.Dept = dept;
                                    empList.Add(emp);
                                }
                                else
                                {
                                    emp.Dept = null;
                                    empList.Add(emp);
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
             return empList;
        }


        

        public Employee ReadRowById(int id)
        {
            Employee emp = new Employee();
            // sTep 1 : establishing the connection and open
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Step 2 : create the Read by row Id sql command

                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        var SQLQuery = "Select e.EMP_ID, e.EMP_FST, e.EMP_LST, e.FK_DEPT_NO, d.DEPT_NAME " +
                                        "FROM Employee e LEFT JOIN Departments d on e.FK_DEPT_NO=d.DEPT_NO " +
                                        "WHERE EMP_ID=@EMP_ID";
                        cmd.CommandText = SQLQuery;

                        // Step 3 : define the parameters for the SQL command before using ExecuteReader
                        IDbDataParameter param1 = new SqlParameter();

                        param1.ParameterName = "@EMP_ID";
                        param1.Value = id;
                        cmd.Parameters.Add(param1);

                        // Step 4: after defining the parameters, execute the SQL using ExecuteReader since
                        // we are reading
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Department dept = new Department();
                                emp.Id = id;
                                emp.FirstName = reader.GetString(1);
                                emp.LastName = reader.GetString(2);
                                if (!reader.IsDBNull(3))
                                {
                                    dept.Id = reader.GetInt32(3);
                                    dept.DeptName = reader.GetString(4);
                                    emp.Dept = dept;
                                }
                                else
                                {
                                    emp.Dept = null;
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
            return emp;
        }

        public Employee ReadRowByName(string firstName, string lastName)
        {
            Employee emp = new Employee();
            // sTep 1 : establishing the connection and open
            try
            {
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Step 2 : create the Read by row Id sql command

                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        var SQLQuery = "Select e.EMP_ID, e.EMP_FST, e.EMP_LST, e.FK_DEPT_NO, d.DEPT_NAME " +
                                        "FROM Employee e LEFT JOIN Departments d on e.FK_DEPT_NO=d.DEPT_NO " +
                                        "WHERE EMP_FST=@EMP_FST AND EMP_LST=@EMP_LST";
                        cmd.CommandText = SQLQuery;

                        // Step 3 : define the parameters for the SQL command before using ExecuteReader
                        IDbDataParameter param1 = new SqlParameter();
                        param1.ParameterName = "@EMP_FST";
                        param1.Value = firstName;
                        cmd.Parameters.Add(param1);

                        IDbDataParameter param2 = new SqlParameter();
                        param2.ParameterName = "@EMP_LST";
                        param2.Value = lastName;
                        cmd.Parameters.Add(param2);

                        // Step 4: after defining the parameters, execute the SQL using ExecuteReader since
                        // we are reading
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Department dept = new Department();
                                emp.Id = reader.GetInt32(0);
                                emp.FirstName = firstName;
                                emp.LastName = lastName;
                                if (!reader.IsDBNull(3))
                                {
                                    dept.Id = reader.GetInt32(3);
                                    dept.DeptName = reader.GetString(4);
                                    emp.Dept = dept;
                                }
                                else
                                {
                                    emp.Dept = null;
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
            return emp;
        }

        public bool UpdateRow(Employee entity)
        {
            bool entityIsUpdated = false;

            try
            {
                // Step 1: Set up connection and open
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    Console.WriteLine(entity.Dept.Id);
                    //Step 2: Create the update SQL command
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE Employee set EMP_FST=@EMP_FST, EMP_LST=@EMP_LST," +
                            "FK_DEPT_NO=@FK_DEPT_NO WHERE EMP_ID=@EMP_ID";
                        
                        // Step 3 : define the parameter for the SQL commnad
                        IDbDataParameter param1 = new SqlParameter("EMP_ID", SqlDbType.Int);
                        param1.Value = entity.Id;
                        cmd.Parameters.Add(param1);

                        IDbDataParameter param2 = new SqlParameter("EMP_FST", SqlDbType.VarChar);
                        param2.Value = entity.FirstName;
                        cmd.Parameters.Add(param2);

                        IDbDataParameter param3 = new SqlParameter("EMP_LST", SqlDbType.VarChar);
                        param3.Value = entity.LastName;
                        cmd.Parameters.Add(param3);

                        IDbDataParameter param4 = new SqlParameter("FK_DEPT_NO", SqlDbType.Int);
                        param4.Value = entity.Dept.Id;
                        cmd.Parameters.Add(param4);

                        Console.WriteLine($"{param1.Value}, {param2.Value}, {param3.Value}, {param4.Value}");
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
