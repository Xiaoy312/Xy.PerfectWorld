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
        private Core core;
        private Game game;

        public int ProcessID { get; }

        public GameModel(Process process)
        {
            ProcessID = process.Id;
            core = Core.Attach(process);
            game = new Game(core);

            //Observable.Interval(TimeSpan.FromSeconds(1))
            //    .Select(_ => game.)
        }
    }
}
