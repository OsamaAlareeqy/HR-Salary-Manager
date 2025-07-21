using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salary_Cal
{
    public class AttendanceRecord
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool InTime { get; set; }
        public bool OutTime { get; set; }
        public double RegularTime { get; set; }
        public double OverTime { get; set; }
    }

}
