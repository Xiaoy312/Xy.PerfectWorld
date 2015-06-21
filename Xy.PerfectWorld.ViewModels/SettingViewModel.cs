using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.ViewModels
{
    public class SettingViewModel : ReactiveObject
    {
        public ReactiveList<Process> RunningProcesses { get; }
        public IReactiveDerivedList<GameModel> RunningGames { get; }

        public SettingViewModel()
        {
            RunningProcesses = new ReactiveList<Process>();
            RunningGames = RunningProcesses.CreateDerivedCollection(x => new GameModel(x));



            RefreshGameListCommand = ReactiveCommand.Create();
            RefreshGameListCommand.Subscribe(RefreshGameList);
            RefreshGameListCommand.Execute(null);
        }

        public ReactiveCommand<object> RefreshGameListCommand { get; }
        public void RefreshGameList(object unused)
        {
            RunningProcesses.RemoveAll(RunningProcesses.Where(x => x.HasExited));
            RunningProcesses.AddRange(
                Process.GetProcessesByName("elementclient")
                    .Except(RunningProcesses));
        }
    }
}
