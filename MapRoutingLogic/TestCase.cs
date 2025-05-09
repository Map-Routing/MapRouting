using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    public class TestCase
    {
        public Map TestMap {  get; set; }
        public List<Query> Queries { get; set; }
        public List<Output> ActualOutputs { get; set; }
        public List<Output> Outputs { get; set; } // Output of the program
        public double TotalExecNoIO { get; set; } //ms (total execution time without IO (without reading from files)
        public double TotalExec { get; set; } // ms (total executin include all program

    }
}
