using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MoreLinq;
using ReactiveUI;
using Xy.PerfectWorld.Models;

namespace Xy.PerfectWorld.ViewModels
{
    public partial class SettingViewModel : ReactiveObject
    {
        public SettingViewModel()
        {
            InitializeClient();
            InitializeAutoCombat();
        }

        partial void InitializeClient();
        partial void InitializeAutoCombat();

        public async void DisplayException(Exception e)
        {
            var window = Application.Current.MainWindow as MetroWindow;
            await window.ShowMessageAsync(
                e.GetType().Name,
                e.Message + "\n" + e.StackTrace,
                MessageDialogStyle.Affirmative,
                new MetroDialogSettings()
                {
                    AffirmativeButtonText = "...",
                    ColorScheme = MetroDialogColorScheme.Inverted,
                });
        }
    }


}
