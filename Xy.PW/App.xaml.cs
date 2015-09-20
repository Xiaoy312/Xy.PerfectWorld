using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Reactive.Disposables;
using System.Xml.Linq;
using Xy.PerfectWorld.ViewModels;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Services;
using System.Diagnostics;
using Xy.PerfectWorld.Models;

namespace Xy.PW
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if !DEBUG
            // log unhandled exception
            DispatcherUnhandledException += (s, args) => ReportUnhandledException(args.Exception);
#endif

            BuildVersionCheck();
            LicenseValidation();
            AdministrativeRightsCheck();

            if (!AttachToClient())
                Shutdown();

            AppBootstrapper.Initialize();
            AppBootstrapper.Run();
        }

        /// <summary>
        /// Show an error report for release version preventing the wrong build being shipped
        /// </summary>
        private void BuildVersionCheck()
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                MessageBox.Show("Please contact your administrator.", "Incorrect Software Version", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }
#endif
        }
        private void LicenseValidation()
        {
            if (!Debugger.IsAttached && !License.CheckLicense())
            {
                var hwid = License.GetHardwareID();
                Clipboard.SetText(hwid);
                MessageBox.Show($"Please contact your administrator with your Hardware ID. It has been copied to your clipbaord.\n HardwareID : {hwid}", "Unlicensed Software", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown();
            }
        }
        private void AdministrativeRightsCheck()
        {
            var isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                MessageBox.Show("XyPW requires administrative rights to work. Please restart as admin.", this.GetType().Namespace, MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }
        }
        /// <summary>
        /// Attach to a single instance of client
        /// </summary>
        /// <returns>If a client is chosen and attached</returns>
        private bool AttachToClient()
        {
            var clients = Process.GetProcessesByName("elementclient");
            if (clients.Count() == 1)
            {
                var client = clients.Single();
                var game = new GameModel(client);

                Locator.CurrentMutable.RegisterConstant(game, typeof(GameModel));
            }
            else
            {
                // temporarily preventing the app from shutting down when last window closes
                var mode = ShutdownMode;
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                using (Disposable.Create(() => ShutdownMode = mode))
                {
                    var viewModel = new ClientSelectorViewModel();
                    viewModel.Attach.Subscribe(game => Locator.CurrentMutable.RegisterConstant(game, typeof(GameModel)));

                    var view = new ClientSelectorView() { ViewModel = viewModel };
                    view.ShowDialog();
                }
            }

            return Locator.Current.GetService<GameModel>() != null;
        }

        /// <summary>
        /// Show an error message for the exception and log in a file
        /// </summary>
        private void ReportUnhandledException(Exception exception)
        {
            Func<Exception, XElement> convertToXml = null;
            convertToXml = e => e == null ? null :
                new XElement(e.GetType().Name,
                    new XElement("FullName", e.GetType().FullName),
                    new XElement("Message", e.Message),
                    new XElement("Source", e.Source),
                    new XElement("TargetSite", e.TargetSite),
                    new XElement("StackTrace", e.StackTrace),
                    new XElement("InnerException", convertToXml(e.InnerException))
                    );

            var path = Path.GetFullPath($"error log/{DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}.xml");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            convertToXml(exception).Save(path);

            var message = "An error log has been produced for this error, which can be found at \n" + path + "\n" +
                "Here is an summary of the error : \n\n" + exception;
            MessageBox.Show(message, "Something went wrong...", MessageBoxButton.OK, MessageBoxImage.Error);

            Shutdown();
        }
    }
}
