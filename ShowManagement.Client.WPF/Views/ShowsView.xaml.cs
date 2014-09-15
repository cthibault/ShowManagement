using Microsoft.Practices.Unity;
using ShowManagement.Client.WPF.ViewModels;
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

namespace ShowManagement.Client.WPF.Views
{
    /// <summary>
    /// Interaction logic for ShowsView.xaml
    /// </summary>
    public partial class ShowsView : UserControl
    {
        public ShowsView()
        {
            this.ViewModel = App.UnityContainer.Resolve<ShowsViewModel>();
            this.DataContext = this.ViewModel;

            InitializeComponent();
        }

        private ShowsViewModel ViewModel { get; set; }
    }
}
