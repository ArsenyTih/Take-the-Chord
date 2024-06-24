using ChordRandomizer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ChordGenerator_WPF
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        int lastDisabledRootNoteIndexToggleButton = 0;
        int lastDisabledAddedNoteIndexToggleButton = 0;
        
        public SettingsControl()
        {
            InitializeComponent();
            SliderOfFrets.Value = 5;
        }

        private void btn_CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void btn_Note_Checked(object sender, RoutedEventArgs e)
        {
            string NoteName = ((ToggleButton)sender).Name.Replace("is", "#");
            MainWindow.connect.MN_AllowedRootNotes.Remove(NoteName);
            //MessageBox.Show(string.Join("\n", MusicalNotes.AllowedRootNotes.Select(x => x)));
            if (MainWindow.connect.MN_AllowedRootNotes.Count == 1)
            {
                lastDisabledRootNoteIndexToggleButton = Array.IndexOf(MainWindow.connect.MN_Notes, MainWindow.connect.MN_AllowedRootNotes[0]);
                RootNotes.Children[lastDisabledRootNoteIndexToggleButton].IsEnabled = false;
            }
        }

        private void btn_Note_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.connect.MN_AllowedRootNotes.Count == 1)
            {
                RootNotes.Children[lastDisabledRootNoteIndexToggleButton].IsEnabled = true;
            }
            string NoteName = ((ToggleButton)sender).Name.Replace("is", "#");
            MainWindow.connect.MN_AllowedRootNotes.Add(NoteName);
            //MessageBox.Show(string.Join("\n", MusicalNotes.AllowedRootNotes.Select(x => x)));
        }

        private void btn_Degree_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string AddedNoteName = ((ToggleButton)sender).Name.Replace("_", "").Replace("plus", "+").Replace("minus", "-").Replace("Slash", "/");
            MainWindow.connect.MN_AllowedAddedNotes.Remove(AddedNoteName);
            //MessageBox.Show(string.Join("\n", MusicalNotes.AllowedAddedNotes.Select(x => x)));
            if (MainWindow.connect.MN_AllowedAddedNotes.Count == 1)
            {
                lastDisabledAddedNoteIndexToggleButton = Array.IndexOf(MainWindow.connect.MN_AddedNotes, MainWindow.connect.MN_AllowedAddedNotes.Last().Key);
                AddedNotes.Children[lastDisabledAddedNoteIndexToggleButton].IsEnabled = false;
            }
        }

        private void btn_Degree_Unchecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (MainWindow.connect.MN_AllowedAddedNotes.Count == 1)
            {
                AddedNotes.Children[lastDisabledAddedNoteIndexToggleButton].IsEnabled = true;
            }
            string AddedNoteName = ((ToggleButton)sender).Name.Replace("_", "").Replace("plus", "+").Replace("minus", "-").Replace("Slash", "/");
            MainWindow.connect.MN_AllowedAddedNotes.Add(AddedNoteName, Array.IndexOf(MainWindow.connect.MN_AddedNotes, AddedNoteName) + 1);
            //MessageBox.Show(string.Join("\n", MusicalNotes.AllowedAddedNotes.Select(x => x)));
        }

        private void SliderOfFrets_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GuitarFingering.MaxDistanceBetweenExtremeFrets = (int)SliderOfFrets.Value;
            //MessageBox.Show(GuitarFingering.MaxDistanceBetweenExtremeFrets.ToString());
            if (MainWindow.connect != null)
                MainWindow.connect.UpdateFingeringsAndCounterValue();
        }
    }
}
