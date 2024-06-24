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

namespace ChordGenerator_WPF
{
    /// <summary>
    /// Логика взаимодействия для Favourites.xaml
    /// </summary>
    public partial class Favourites : UserControl
    {
        public Favourites()
        {
            InitializeComponent();
        }

        private void btn_CloseFavourites_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
