using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Xy.PerfectWorld.Services;
using ReactiveUI;
using MahApps.Metro.Controls;
using System.Windows;

namespace Xy.PW.ServicesProviders
{
    public class ViewServiceProvider : IViewService
    {
        public void ShowViewFor<T>() where T : class
        {
            var view = Locator.Current.GetService<IViewFor<T>>();
            var window = view as MetroWindow;

            if (!window.IsVisible)
            {
                window.Show();
            }

            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }

            if (!window.Topmost)
            {
                // bring window to front
                window.Topmost = true;
                window.Topmost = false;
            }
        }
    }
}
