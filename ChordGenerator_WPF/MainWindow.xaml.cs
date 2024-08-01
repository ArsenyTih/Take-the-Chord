using System.Windows;
using System.Windows.Input;

namespace ChordGenerator_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static Connect connect;

        public MainWindow()
        {
            InitializeComponent();
            connect = new(this);
            try
            {
                connect.history = connect.ReadJSON("History");
                connect.AddButtonsToHistory();
                connect.favourites = connect.ReadJSON("Favourites");
                connect.AddButtonsToFavourites();
            }
            catch { }
            this.KeyDown += mainWindow_KeyDown;
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (SettingsFrame.Visibility == Visibility.Visible)
                    connect.OpenSettingsHelp();
                else if (HistoryFrame.Visibility == Visibility.Visible && FavouritesFrame.Visibility == Visibility.Visible)
                {
                    if (connect.lastOpenedFrame is History)
                        connect.OpenHistoryHelp();
                    else if (connect.lastOpenedFrame is Favourites)
                        connect.OpenFavouritesHelp();
                }
                else if (HistoryFrame.Visibility == Visibility.Visible)
                    connect.OpenHistoryHelp();
                else if (FavouritesFrame.Visibility == Visibility.Visible)
                    connect.OpenFavouritesHelp();
                else
                    connect.OpenStartMenuHelp();
            }
        }

        private void btn_History_Click(object sender, RoutedEventArgs e)
        {
            connect.lastOpenedFrame = HistoryFrame;
            HistoryFrame.Visibility = Visibility.Visible;
        }

        private void btn_Favourite_Click(object sender, RoutedEventArgs e)
        {
            connect.lastOpenedFrame = FavouritesFrame;
            FavouritesFrame.Visibility = Visibility.Visible;
        }

        private void btn_NextFingering_Click(object sender, RoutedEventArgs e)
        {
            if (connect.GuitarFingerings.Length != 0)
            {
                connect.ShowOrHideFingering(Visibility.Hidden);
                connect.FingeringIndex++;
                connect.ShowOrHideFingering(Visibility.Visible);
                ChooseFingering.Text = connect.FingeringNumber;
            }
        }

        private void btn_PrevFingering_Click(object sender, RoutedEventArgs e)
        {
            if (connect.GuitarFingerings.Length != 0)
            {
                connect.ShowOrHideFingering(Visibility.Hidden);
                connect.FingeringIndex--;
                connect.ShowOrHideFingering(Visibility.Visible);
                ChooseFingering.Text = connect.FingeringNumber;
            }
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Visibility = Visibility.Visible;
        }

        private void btn_AddToFavourites_Click(object sender, RoutedEventArgs e)
        {
            if (connect.Chord != null)
                connect.AddToFavourites();
        }

        private void btn_CreateChord_Click(object sender, RoutedEventArgs e)
        {
            connect.ShowOrHideFingering(Visibility.Hidden);
            connect.Chord = new(); // создается аккорд
            connect.AddToHisory(); // добавление элемента в историю
            connect.UpdateInfoOnScreen(); // обновление информации об аккорде на экране
            ChooseFingering.IsReadOnly = false;
            connect.ShowOrHideFingering(Visibility.Visible); // показывается 1-ая аппликатура
        }

        private void btn_Help_Click(object sender, RoutedEventArgs e)
        {
            connect.OpenMainHelp();
        }

        private void PlayCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            connect.PlayFingering();
        }

        private void ChooseFingering_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                connect.ShowOrHideFingering(Visibility.Hidden);
                connect.FingeringNumber = ChooseFingering.Text;
                ChooseFingering.Text = connect.FingeringNumber;
                connect.ShowOrHideFingering(Visibility.Visible);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            connect.WriteJSON("History", connect.history);
            connect.WriteJSON("Favourites", connect.favourites);
        }
    }
}
