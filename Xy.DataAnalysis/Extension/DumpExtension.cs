using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xy.DataAnalysis.Extension
{
    public static class DumpExtension
    {
        public static void Dump(this object value)
        {
            Debug.WriteLine(value);
        }
    }
}
