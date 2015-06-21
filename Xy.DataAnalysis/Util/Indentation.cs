using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xy.DataAnalysis.Util
{
    internal class Indentation : IDisposable
    {
        public Indentation()
        {
            Debug.Indent();
        }

        public void Dispose()
        {
            Debug.Unindent();
        }

        public static Indentation Indent()
        {
            return new Indentation();
        }
    }
}
