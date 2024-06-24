using System;
using System.Windows;
using System.Windows.Controls;

namespace ChordGenerator_WPF
{
    /// <summary>
    /// Логика взаимодействия для ButtonChordDelete.xaml
    /// </summary>
    public partial class ChordFromFavourites : UserControl
    {
        public ChordFromFavourites()
        {
            InitializeComponent();
        }

        private void btnDeleteChord_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.connect.DeleteFromFavourites(this);
        }

        private void btnNameChord_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.connect.ShowChordFromFavourites(btnNameChord.Content.ToString());
        }
    }
}
