﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace MapRoutingLogic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IO_Operations iO_Operations = new IO_Operations();
            iO_Operations.RunTestCases();
        }
    }
}


