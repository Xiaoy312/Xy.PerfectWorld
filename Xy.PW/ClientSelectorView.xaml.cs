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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using ReactiveUI;
using Xy.PerfectWorld.ViewModels;

namespace Xy.PW
{
    /// <summary>
    /// Interaction logic for ClientSelectorView.xaml
    /// </summary>
    public partial class ClientSelectorView : MetroWindow, IViewFor<ClientSelectorViewModel>
    {
        public ClientSelectorView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
            this.WhenAnyObservable(x => x.ViewModel.Attach).Subscribe(_ => this.Close());
        }

        #region IViewFor<ClientSelectorViewModel> Members
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ClientSelectorViewModel), typeof(ClientSelectorView), new PropertyMetadata(null));

        public ClientSelectorViewModel ViewModel
        {
            get { return (ClientSelectorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ClientSelectorViewModel)value; }
        }
        #endregion
    }
}
