using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Services;

namespace Xy.PW.ServicesProviders
{
    public class DialogServiceProvider : IDialogService
    {
        public Task DisplayExceptionAsyncOn<TViewModel>(TViewModel typeProvider, Exception e) where TViewModel : class
        {
            return DisplayExceptionAsyncOn<TViewModel>(e);
        }
        public Task DisplayExceptionAsyncOn<TViewModel>(Exception e) where TViewModel : class
        {
            var window = Application.Current.Windows.OfType<IViewFor<TViewModel>>().SingleOrDefault() as MetroWindow;
            return window.ShowMessageAsync(
                e.GetType().Name,
                e.Message + "\n" + e.StackTrace,
                MessageDialogStyle.Affirmative,
                new MetroDialogSettings()
                {
                    AffirmativeButtonText = "...",
                    ColorScheme = MetroDialogColorScheme.Inverted
                })
                .ContinueWith(task => { });
        }

        public Task<DialogResult> ShowMessageDialogAsyncOn<TViewModel>(string title, string message, 
            string affirmativeButtonText = "OK", string negativeButtonText = null, 
            string firstAuxiliaryButtonText = null, string secondAuxiliaryButtonText = null) where TViewModel : class
        {
            var window = Application.Current.Windows.OfType<IViewFor<TViewModel>>().SingleOrDefault() as MetroWindow;
            var style =
                secondAuxiliaryButtonText != null ? MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary :
                firstAuxiliaryButtonText != null ? MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary :
                negativeButtonText != null ? MessageDialogStyle.AffirmativeAndNegative :
                MessageDialogStyle.Affirmative;

            return window.ShowMessageAsync(title, message, style, new MetroDialogSettings()
                {
                    AffirmativeButtonText = affirmativeButtonText,
                    NegativeButtonText = negativeButtonText,
                    FirstAuxiliaryButtonText = firstAuxiliaryButtonText,
                    SecondAuxiliaryButtonText = secondAuxiliaryButtonText,
                })
                .ContinueWith(task => (DialogResult)task.Result);
        }
    }
}
