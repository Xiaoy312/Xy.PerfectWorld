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
        public Game Game { get; }
        public Process Process { get; }

        GameStatus status;
        public GameStatus Status
        {
            get { return status; }
            set { this.RaiseAndSetIfChanged(ref status, value); }
        }
        string player;
        public string Player
        {
            get { return player; }
            set { this.RaiseAndSetIfChanged(ref player, value); }
        }

        public GameModel(Process process)
        {
            Process = process;
            Game = new Game(Core.Attach(process));

            Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    this.Status = Game.GameStatus;
                    this.Player = new Character(Game).Name.Value;
                });
        }
    }
}
