using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.DataAnalysis;

namespace Xy.PerfectWorld.Models
{
    public class GameModel : ReactiveObject
    {
        private Process process;
        private IntPtr handle;

        public int PID { get { return process.Id; } }

        public GameModel(Process process)
        {
            this.process = process;
            this.handle = Core.OpenProcess(process.Id);

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(x => ).ToProperty()
        }
    }

    public enum GameStatus : int
    {
        Offline = 1, LoggedIn = 2
    }
}
