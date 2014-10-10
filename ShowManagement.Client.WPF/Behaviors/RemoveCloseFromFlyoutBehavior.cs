using MahApps.Metro.Controls;
using ShowManagement.Client.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ShowManagement.Client.WPF.Behaviors
{
    public class RemoveCloseFromFlyoutBehavior : Behavior<Flyout>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += this.FlyoutLoaded;
        }
        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= this.FlyoutLoaded;
            
            base.OnDetaching();
        }

        private void FlyoutLoaded(object sender, RoutedEventArgs e)
        {
            var closeButton = VisualTreeHelpers.FindChild<Button>(this.AssociatedObject, "nav");

            if (closeButton != null)
            {
                closeButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
