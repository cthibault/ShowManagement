using Microsoft.Practices.Unity;
using ShowManagement.Client.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
    /// Interaction logic for ServicesView.xaml
    /// </summary>
    public partial class ServicesView : UserControl
    {
        public ServicesView()
        {
            InitializeComponent();

            this.ViewModel = App.UnityContainer.Resolve<ViewModels.IServicesViewModel>();

            this.DataContext = this.ViewModel;

            this.InvokeRefreshShows();
        }
        private void InvokeRefreshShows()
        {
            var peer = UIElementAutomationPeer.CreatePeerForElement(this.refreshShowsBtn);
            var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private IServicesViewModel ViewModel { get; set; }
    }
}
