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

namespace GuitarTab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string FILE_LOC = "../../Resources/ResourceDocument.txt";

        public MainWindow()
        {
            InitializeComponent();
            createViewModel();
        }

        public void createViewModel()
        {
            var factory = new ViewModelFactory();
            factory.runFactory(FILE_LOC);
            factory.linkToView(main);
            factory.initPart();
        }
    }
}
