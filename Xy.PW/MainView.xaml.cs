using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xy.PerfectWorld.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace Xy.PW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void ShowSetting(object sender, RoutedEventArgs e)
        {
            var view = Application.Current.Windows.OfType<SettingView>().SingleOrDefault() ??
                new SettingView() { DataContext = AppViewModel.Instance.SettingVM };

            view.Show();
        }
    }
}
