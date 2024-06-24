using System.Windows;
using System.Windows.Controls;

namespace ChordGenerator_WPF
{
    /// <summary>
    /// Логика взаимодействия для History.xaml
    /// </summary>
    public partial class History : UserControl
    {
        public History()
        {
            InitializeComponent();
        }

        private void btn_CloseHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void History_Elem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.connect.ShowChordFromHystory(((Button)sender).Content.ToString());
        }
    }
}
