using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public static class Game
    {
        public static int BaseAddress { get; private set; }

        public static void Initialize()
        {
            const string ProcessName = "elementclient";
            var client = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            if (client == null)
            {
                throw new Exception("Client not found : " + ProcessName);
            }

            Core.Attach(client);
            BaseAddress = client.MainModule.BaseAddress.ToInt32();
        }
    }
}
