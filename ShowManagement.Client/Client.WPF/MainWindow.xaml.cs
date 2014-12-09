using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
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

namespace ShowManagement.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            string versionNumber = "DEV";

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                versionNumber = string.Format("{0}.{1}.{2}.{3}",
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.Major,
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor,
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.Build,
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision);

            }

            this.version.Content = string.Format("v{0}", versionNumber);
        }
    }
}
