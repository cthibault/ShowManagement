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

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            var carDictionary = new Dictionary<string, List<CarModel>>
            {
                { "Ford", new List<CarModel>
                    {
                        new CarModel("One", "Car"),
                        new CarModel("Two", "Car"),
                        new CarModel("Three", "Car"),
                        new CarModel("Four", "Car"),
                    }
                },
                { "Toyota", new List<CarModel>
                    {
                        new CarModel("Five", "Car"),
                        new CarModel("Six", "Truck"),
                    }
                },
            };

            var model = new Model { Vehicles = carDictionary };

            this.DataContext = model;
        }
    }

    public class Model
    {
        public Dictionary<string, List<CarModel>> Vehicles { get; set; }
    }

    public class CarModel
    {
        public CarModel(string name, string modelType)
        {
            this.Name = name;
            this.ModelType = modelType;
        }

        public string Name { get; set; }
        public string ModelType { get; set; }
    }
}
