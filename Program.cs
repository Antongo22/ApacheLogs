﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacheLogs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Apache.Parse("config.txt");
        }
    }
}
