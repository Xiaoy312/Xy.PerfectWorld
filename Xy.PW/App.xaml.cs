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

namespace Xy.PW
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // show an error report for release version, and prevent wrong version being shipped
#if !DEBUG
            DispatcherUnhandledException += (s, args) => ReportUnhandledException(args.Exception);
#else
            if (Environment.UserName != "Xiaoy")
            {
                ReportUnhandledException(new Exception("Tell Xiaoy that he shipped the wrong the version."));
            }
#endif
            // check admin rights
            var isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("XyPW requires administrative rights to work. Please restart as admin.");
            }

            var view = new MainView();
            view.DataContext = null;

            view.Show();
        }

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
                "Here is an summary of the error : \n" + exception;
            MessageBox.Show(message, "Something went wrong...", MessageBoxButton.OK, MessageBoxImage.Error);

            Shutdown();
        }
    }
}
