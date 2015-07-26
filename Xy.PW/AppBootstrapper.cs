using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Services;
using Xy.PerfectWorld.ViewModels;
using Xy.PW.ServicesProviders;

namespace Xy.PW
{
    public static class AppBootstrapper
    {
        public static void Initialize()
        {
            var resolver = Locator.CurrentMutable;

            RegisterServices(resolver);
            RegisterViews(resolver);
        }

        public static void Run()
        {
            var view = Locator.Current.GetService<IViewFor<MainViewModel>>() as MainView;
            view.ShowDialog();

            Environment.Exit(0);
        }

        public static void RegisterServices(IMutableDependencyResolver resolver)
        {
            resolver.RegisterConstant(new ViewServiceProvider(), typeof(IViewService));
            resolver.RegisterConstant(new DialogServiceProvider(), typeof(IDialogService));
        }

        public static void RegisterViews(IMutableDependencyResolver resolver)
        {
            resolver.RegisterViewAndViewModel<SettingView, SettingViewModel>();
            resolver.RegisterViewAndViewModel<MainView, MainViewModel>();
        }

        private static void RegisterViewAndViewModel<TView, TViewModel>(this IMutableDependencyResolver resolver)
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : ReactiveObject, new()
        {
            var viewModel = new TViewModel();
            Func<TView> viewFactory = () =>
                Application.Current.Windows.OfType<TView>().FirstOrDefault() ?? 
                new TView() { ViewModel = viewModel };

            resolver.RegisterConstant(viewModel, viewModel.GetType());
            resolver.Register(viewFactory, typeof(IViewFor<TViewModel>));
        }
    }
}
