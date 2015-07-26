using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Xy.PerfectWorld.ViewModels;
using ReactiveUI;
using Splat;
using Xy.PerfectWorld.Services;

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

            AppBootstrapper.Initialize();
            AppBootstrapper.Run();
        }

        /// <summary>
        /// Show an error report for release version preventing the wrong build being shipped
        /// </summary>
        private void BuildVersionCheck()
        {
#if DEBUG
            if (Environment.UserName != "Xiaoy")
            {
                MessageBox.Show("Please contact your administrator.", "Incorrect Software Version", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }
#endif
        }
        private void LicenseValidation()
        {
            if (Environment.UserName != "Xiaoy" && !License.CheckLicense())
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
