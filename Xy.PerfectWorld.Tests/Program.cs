using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Initialize();

            Character.Instance.DumpProperties();
            NpcContainer.Instance.DumpProperties();
            GroundContainer.Instance.DumpProperties();

            foreach (var item in GroundContainer.Instance.GetItems())
            {
                Debug.WriteLine(item);
            }
        }
    }
}