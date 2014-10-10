using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShowManagement.Client.WPF.Infrastructure
{
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Finds the first ancestor of the specified type
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            return FindAncestor<T>(current, string.Empty);
        }


        /// <summary>
        /// Finds the first ancestor by name and type
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current, string parentName)
            where T : DependencyObject
        {
            T ancestor = null;

            while (current != null && ancestor == null)
            {
                if (current is T)
                {
                    if (!string.IsNullOrWhiteSpace(parentName))
                    {
                        var frameworkElement = current as FrameworkElement;
                        if (frameworkElement != null && frameworkElement.Name == parentName)
                        {
                            ancestor = (T)current;
                        }
                    }
                    else
                    {
                        ancestor = (T)current;
                    }
                }

                if (ancestor == null)
                {
                    current = VisualTreeHelper.GetParent(current);
                }
            }

            return ancestor;
        }


        /// <summary>
        /// Finds the specific ancestor of an object
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current, T lookupItem)
            where T : DependencyObject
        {
            T ancestor = null;

            while (current != null)
            {
                if (current is T && current == lookupItem)
                {
                    ancestor = (T)current;
                }
                else
                {
                    current = VisualTreeHelper.GetParent(current);
                }
            }

            return ancestor;
        }


        /// <summary>
        /// Finds the first child by type
        /// </summary>
        public static T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            return FindChild<T>(parent, string.Empty);
        }


        /// <summary>
        /// Finds the first child by name and type
        /// </summary>
        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            T foundChild = null;

            if (parent != null)
            {
                int childCount = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < childCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    var childByType = child as T;

                    // Child is of the correct type
                    if (childByType != null)
                    {
                        if (!string.IsNullOrWhiteSpace(childName))
                        {
                            var frameworkElement = child as FrameworkElement;
                            if (frameworkElement != null && frameworkElement.Name == childName)
                            {
                                foundChild = childByType;
                            }
                            else
                            {
                                foundChild = FindChild<T>(child, childName);
                            }
                        }
                        else
                        {
                            foundChild = childByType;
                        }
                    }
                    else
                    {
                        // Recursive call to drill into the child control
                        foundChild = FindChild<T>(child, childName);
                    }

                    // If the recursive calls find the child, stop searching
                    if (foundChild != null)
                    {
                        break;
                    }
                }
            }

            return foundChild;
        }
    }
}
