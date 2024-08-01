using ChordRandomizer;
using System.Windows;
using NAudio.Midi;
using System.Diagnostics;
using System.Windows.Controls;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace ChordGenerator_WPF
{
    internal class Connect
    {
        public MainWindow mainWindow; // Главный (начальный) экран

        private Chord chord; // Аккорд
        private GuitarFingering[] guitarFingerings = []; // Массив аппликатур
        int fingeringIndex = 0; // Индекс в массиве аппликатур
        public int[] frets; // Массив из номеров ладов отдельной аппликатуры
        public Dictionary<string, Chord> history = new(maxChordsInHistory + 1); // Словарь сохранённых в истории аккордов
        public Dictionary<string, Chord> favourites = new(maxChordsInFavourites); // Словарь сохранённых в избранном аккордов
        public object lastOpenedFrame; // Последний открытый фрейм (для контекстной справки)

        // Статические поля класса MusicalNotes
        public List<string> MN_AllowedRootNotes = MusicalNotes.AllowedRootNotes;
        public string[] MN_Notes = MusicalNotes.Notes;
        public Dictionary<string, int> MN_AllowedAddedNotes = MusicalNotes.AllowedAddedNotes;
        public string[] MN_AddedNotes = MusicalNotes.AddedNotes;
        public int MN_MaxDistanceBetweenExtremeFrets = GuitarFingering.MaxDistanceBetweenExtremeFrets;

        // Максимальное количество аккордов, хранимое в памяти
        const int maxChordsInHistory = 30;
        const int maxChordsInFavourites = 30;

        // Звук
        public MidiOut midiOut = new(0);
        public CancellationTokenSource cts;

        #region Свойства
        public GuitarFingering[] GuitarFingerings { get => guitarFingerings; }

        public Chord Chord
        {
            get
            {
                return chord;
            }
            set
            {
                chord = value;
                guitarFingerings = chord.ChooseFingeringsByMaxDistanceBetweenExtremeNotes().ToArray();
                FingeringIndex = 0;
            }
        }

        public int FingeringIndex
        {
            get
            {
                return fingeringIndex;
            }
            set
            {
                if (guitarFingerings.Length != 0)
                {
                    if (0 <= value && value < guitarFingerings.Length)
                        fingeringIndex = value;
                    else if (value < 0)
                        fingeringIndex = guitarFingerings.Length - 1;
                    else if (value >= guitarFingerings.Length)
                        fingeringIndex = 0;
                }
            }
        }

        public string FingeringNumber
        {
            get
            {
                if (GuitarFingerings.Length == 0)
                    return FingeringIndex.ToString();
                return (FingeringIndex + 1).ToString();
            }
            set
            {
                if (int.TryParse(value, out int num))
                {
                    if (1 <= num && num <= guitarFingerings.Length)
                        FingeringIndex = num - 1;
                    else if (num < 1)
                        FingeringIndex = 0;
                    else if (num > guitarFingerings.Length)
                        FingeringIndex = guitarFingerings.Length - 1;
                }
            }
        }
        #endregion

        #region Конструктор
        /// <summary>
        /// Конструктор класса Connect
        /// </summary>
        /// <param name="window"> Ссылка на главное окно приложения </param>
        public Connect(MainWindow window)
        {
            mainWindow = window;
            midiOut.Send(MidiMessage.ChangePatch(25, 1).RawData);
            lastOpenedFrame = window;
        }
        #endregion

        #region Функции
        /// <summary>
        /// Обновляет массив с аппликатурами.
        /// </summary>
        public void UpdateFingerings()
        {
            FingeringIndex = 0;
            guitarFingerings = Chord.ChooseFingeringsByMaxDistanceBetweenExtremeNotes().ToArray();
        }

        /// <summary>
        /// Обновляет счётчик и кол-во аппликатур.
        /// </summary>
        public void UpdateInfoOnScreen()
        {
            mainWindow.ChordName.Text = Chord.ChordName; // Вывод названия
            mainWindow.ChooseFingering.MaxLength = GuitarFingerings.Length.ToString().Length; // Максимальное кол-во вводимых символов в поле ChooseFIngering
            mainWindow.FingeringCounter.Text = GuitarFingerings.Length.ToString(); // Вывод количества аппликатур у аккорда
            mainWindow.ChooseFingering.Text = FingeringNumber; // Вывод текущей аппликатуры
        }

        /// <summary>
        /// Обновляет массива с аппликатурами и счётчик (для случаев, когда поменялось масимальное расстояние между ладами).
        /// </summary>
        public void UpdateFingeringsAndCounterValue()
        {
            if (chord != null)
            {
                ShowOrHideFingering(Visibility.Hidden);
                UpdateFingerings();
                mainWindow.FingeringCounter.Text = GuitarFingerings.Length.ToString();
                mainWindow.ChooseFingering.Text = FingeringNumber.ToString();
                mainWindow.ChooseFingering.MaxLength = GuitarFingerings.Length.ToString().Length;
                ShowOrHideFingering(Visibility.Visible);
            }
        }

        /// <summary>
        /// Добавляет аккорд в "Избранное".
        /// </summary>
        public void AddToHisory()
        {
            if (history.Count == 0)
            {
                history.TryAdd(chord.ChordName, chord);
                mainWindow.HistoryFrame.WrapPanelHistory.Children.Insert(0, new Button() { Content = Chord.ChordName });
            }
            else
            {
                history.TryAdd(chord.ChordName, chord);
                if (((Button)mainWindow.HistoryFrame.WrapPanelHistory.Children[0]).Content.ToString() != chord.ChordName)
                    mainWindow.HistoryFrame.WrapPanelHistory.Children.Insert(0, new Button() { Content = Chord.ChordName });
                if (history.Count > maxChordsInHistory)
                {
                    history.Remove(history.First().Key);
                    history = history.ToArray().ToDictionary();
                }
                while (mainWindow.HistoryFrame.WrapPanelHistory.Children.Count > maxChordsInHistory)
                {
                    UIElement btn = mainWindow.HistoryFrame.WrapPanelHistory.Children[^1];
                    mainWindow.HistoryFrame.WrapPanelHistory.Children.Remove(btn);
                    btn = null;
                }
            }
        }

        /// <summary>
        /// Показывает выбранный в "Истории" аккорд.
        /// </summary>
        /// <param name="chordName"> Имя аккорда </param>
        public void ShowChordFromHystory(string chordName)
        {
            ShowOrHideFingering(Visibility.Hidden);
            chord = history[chordName];
            UpdateFingerings();
            UpdateInfoOnScreen();
            ShowOrHideFingering(Visibility.Visible);
        }

        /// <summary>
        /// Добавляет аккорд в "Избранное".
        /// </summary>
        public void AddToFavourites()
        {
            if (favourites.Count < maxChordsInFavourites)
            {
                if (favourites.TryAdd(chord.ChordName, chord))
                {
                    ChordFromFavourites chordFromFavourites = new();
                    chordFromFavourites.btnNameChord.Content = chord.ChordName;
                    mainWindow.FavouritesFrame.WrapPanelFavourites.Children.Insert(0, chordFromFavourites);
                }
                else
                    MessageBox.Show("В \"избранном\" уже есть этот аккорд.");
            }
            else
                MessageBox.Show("\"Избранное заполнено\". Невозможно добавить аккорд.");
        }

        /// <summary>
        /// Удаляет аккорд из "Избранного".
        /// </summary>
        /// <param name="chordFromFavourites"> Аккорд из "Избранного" </param>
        public void DeleteFromFavourites(ChordFromFavourites chordFromFavourites)
        {
            MainWindow.connect.favourites.Remove(chordFromFavourites.btnNameChord.Content.ToString());
            mainWindow.FavouritesFrame.WrapPanelFavourites.Children.Remove(chordFromFavourites);
            chordFromFavourites = null;
        }

        /// <summary>
        /// Показывает выбранный в "Избранном" аккорд.
        /// </summary>
        /// <param name="chordName"> Имя аккорда </param>
        public void ShowChordFromFavourites(string chordName)
        {
            ShowOrHideFingering(Visibility.Hidden);
            chord = favourites[chordName];
            UpdateFingerings();
            UpdateInfoOnScreen();
            ShowOrHideFingering(Visibility.Visible);
        }

        /// <summary>
        /// Записывает словарь в файл.
        /// </summary>
        /// <param name="filename"> Название файла </param>
        /// <param name="collection"> Словарь, в который будет записываться информация </param>
        public void WriteJSON(string filename, Dictionary<string, Chord> collection)
        {
            FileStream f = new FileStream(filename, FileMode.Truncate);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,                                   // Устанавливаем дополнительные пробелы (элементы расположены просто в строчку друг за другом)
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,  // Кодируем русские символы
                PropertyNameCaseInsensitive = true,                     // Игнорируем регистр имен
            };
            JsonSerializer.Serialize(f, collection, options);
            // Сохраняем заданный объект в заданном потоке с определенными настройками
            f.Close();
        }

        /// <summary>
        /// Читает информацию из файла.
        /// </summary>
        /// <param name="filename"> Название файла </param>
        /// <returns> Возращаемый словарь </returns>
        public Dictionary<string, Chord> ReadJSON(string filename)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,               // Устанавливаем дополнительные пробелы
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true, // Игнорируем регистр имен
                IncludeFields = true                // Включаем поля в сериализацию/десериализацию
            };
            string json = File.ReadAllText(filename);
            Dictionary<string, Chord>? collection = JsonSerializer.Deserialize<Dictionary<string, Chord>>(json, options);
            return collection;
        }

        /// <summary>
        /// Добавляет кнопки в панель "История".
        /// </summary>
        public void AddButtonsToHistory()
        {
            foreach (Chord chord in history.Values)
            {
                mainWindow.HistoryFrame.WrapPanelHistory.Children.Insert(0, new Button() { Content = chord.ChordName });
            }
        }

        /// <summary>
        /// Добавляет кнопки в панель "Избранное".
        /// </summary>
        public void AddButtonsToFavourites()
        {
            foreach (Chord chord in favourites.Values)
            {
                ChordFromFavourites chordFromFavourites = new();
                chordFromFavourites.btnNameChord.Content = chord.ChordName;
                mainWindow.FavouritesFrame.WrapPanelFavourites.Children.Insert(0, chordFromFavourites);
            }
        }

        /// <summary>
        /// Показывет или скрывает аппликатуру по индексу в массиве аппликатур
        /// </summary>
        /// <param name="visibility"> Параметр видимости объекта </param>
        public void ShowOrHideFingering(Visibility visibility)
        {
            if (guitarFingerings.Length != 0)
            {
                int index = 0;
                frets = guitarFingerings[FingeringIndex].Frets;

                // 1 струна
                if (frets[index] == -1) mainWindow.Line1_01.Visibility = visibility;
                else if (frets[index] == 0) mainWindow.Line1_0.Visibility = visibility;
                else mainWindow.Line1.Children[frets[index] - 1].Visibility = visibility;
                index++;

                // 2 струна
                if (frets[index] == -1) mainWindow.Line2_01.Visibility = visibility;
                else if (frets[index] == 0) mainWindow.Line2_0.Visibility = visibility;
                else mainWindow.Line2.Children[frets[index] - 1].Visibility = visibility;
                index++;

                // 3 струна
                if (frets[index] == -1) mainWindow.Line3_01.Visibility = visibility;
                else if (frets[index] == 0) mainWindow.Line3_0.Visibility = visibility;
                else mainWindow.Line3.Children[frets[index] - 1].Visibility = visibility;
                index++;

                // 4 струна
                if (frets[index] == -1) mainWindow.Line4_01.Visibility = visibility;
                else if (frets[index] == 0) mainWindow.Line4_0.Visibility = visibility;
                else mainWindow.Line4.Children[frets[index] - 1].Visibility = visibility;
                index++;

                // 5 струна
                if (frets[index] == -1) mainWindow.Line5_01.Visibility = visibility;
                else if (frets[index] == 0) mainWindow.Line5_0.Visibility = visibility;
                else mainWindow.Line5.Children[frets[index] - 1].Visibility = visibility;
                index++;

                // 6 струна
                if (frets[index] == -1) mainWindow.Line6_01.Visibility = visibility;
                else if (frets[index] == 0) mainWindow.Line6_0.Visibility = visibility;
                else mainWindow.Line6.Children[frets[index] - 1].Visibility = visibility;
            }
        }

        /// <summary>
        /// Открывает справку.
        /// </summary>
        /// <param name="relativePath"> Относительный путь к справке </param>
        private void OpenHelp(string relativePath)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory; // Получаем путь к директории приложения
            string beginLink = "file:///";
            string absolutePath = string.Concat(beginLink, appPath, relativePath); // Формируем абсолютный путь к файлу

            try
            {
                Process p = new();
                p.StartInfo = new ProcessStartInfo(absolutePath) { UseShellExecute = true };
                p.Start();
            }
            catch
            {
                MessageBox.Show("Справка не найдена.");
            }
        }

        /// <summary>
        /// Открывает контекстную справку.
        /// </summary>
        public void OpenMainHelp()
        {
            OpenHelp("Help\\index.htm");
        }

        /// <summary>
        /// Открывает контекстную справку в разделе "Начальное меню".
        /// </summary>
        public void OpenStartMenuHelp()
        {
            OpenHelp("Help\\nachalnoe_menyu.htm");
        }

        /// <summary>
        /// Открывает контекстную справку в разделе "История".
        /// </summary>
        public void OpenHistoryHelp()
        {
            OpenHelp("Help\\istoriya.htm");
        }

        /// <summary>
        /// Открывает контекстную справку в разделе "Избранное".
        /// </summary>
        public void OpenFavouritesHelp()
        {
            OpenHelp("Help\\izbrannoe.htm");
        }

        /// <summary>
        /// Открывает контекстную справку в разделе "Настройки".
        /// </summary>
        public void OpenSettingsHelp()
        {
            OpenHelp("Help\\nastrojki.htm");
        }

        /// <summary>
        /// Воспроизводит ноты аккорда в данной аппликатуре.
        /// </summary>
        /// <returns></returns>
        public async Task PlayFingering()
        {
            if (cts != null) // Если снова воспроизводится аккорд
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }

            if (chord != null && guitarFingerings.Length != 0)
            {
                cts = new CancellationTokenSource();
                List<int> midiIndexes = guitarFingerings[FingeringIndex].NotesMidiIndexes; // Сохранение индексов нот
                await Task.Delay(150);
                foreach (var midiIndex in midiIndexes) // Воспроизведение нот
                {
                    if (cts.IsCancellationRequested) break;
                    await Task.Run(() => PlayNote(midiIndex, cts.Token));
                }
            }
        }

        /// <summary>
        /// Воспроизводит ноту с заданным индексом в MIDI.
        /// </summary>
        /// <param name="midiIndex"> Индекс ноты в MIDI </param>
        /// <param name="token"> Токен для прекращения воспроизведения </param>
        /// <returns></returns>
        private async Task PlayNote(int midiIndex, CancellationToken token)
        {
            if (token.IsCancellationRequested) return; // Если начал звучать новый аккорд
            await Task.Run(() => midiOut.Send(MidiMessage.StartNote(midiIndex, 127, 1).RawData), token);
            await Task.Delay(200, token);
        }
        #endregion
    }
}
