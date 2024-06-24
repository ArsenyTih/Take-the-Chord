using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ChordRandomizer
{
    public class Chord
    {
        readonly Random rnd = new();

        #region Поля: имя аккорда, кол-во нот, список нот, список индексов нот, список аппликатур
        private string chordName = "";
        private int numOfNotes = 0;
        private List<Note> chordNotes = new(6);
        private List<int> chordNotesIndexes = new(6);
        public HashSet<int> addedNotesOctaveIndexes = new(12);
        private HashSet<GuitarFingering> guitarFingerings = new(0);
        #endregion


        #region Свойства для приватных полей
        /// <summary>
        /// Имя аккорда в <see href="https://ru.wikipedia.org/wiki/Буквенно-цифровое_обозначение_аккорда">
        /// буквенно-цифровом обозначении</see>.
        /// <para><em>Определяется в отдельной функции при инициализации переменной типа Chord.</em></para>
        /// </summary>
        public string ChordName { get => chordName; set => chordName = value; }

        /// <summary>
        /// Количество нот аккорда.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается если <paramref name="value"/> меньше 2 или больше 6.
        /// </exception>
        public int NumOfNotes
        { 
            get => numOfNotes;
            set
            {
                if (value < 2 || value > 6) 
                    throw new ArgumentOutOfRangeException(nameof(NumOfNotes));
                numOfNotes = value;
            }
        }

        /// <summary>
        /// Список нот аккорда.
        /// <para><em>Выбирается в отдельных функциях при инициализации переменной типа Chord.</em></para>
        /// </summary>
        /// <exception cref = "ArgumentNullException"> 
        /// Выбрасывается если <paramref name="value"/> равен null.</exception>
        /// <exception cref = "ArgumentException">
        /// Выбрасывается если <paramref name="value.Count"/> не равен кол-ву нот аккорда.</exception>
        public List<Note> ChordNotes
        { 
            get => chordNotes;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (value.Count != NumOfNotes) throw new ArgumentException(nameof(value.Count));
                chordNotes = value;
            }
        }

        /// <summary>
        /// Список индексов нот аккорда.
        /// </summary>
        /// <exception cref = "ArgumentNullException"> 
        /// Выбрасывается если <paramref name="value"/> равен null.</exception>
        /// <exception cref = "ArgumentException">
        /// Выбрасывается если <paramref name="value.Count"/> не равен кол-ву нот аккорда.</exception>
        public List<int> ChordNotesIndexes
        {
            get => chordNotesIndexes; 
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (value.Count != NumOfNotes) throw new ArgumentException(nameof(value.Count));
                chordNotesIndexes = value;
            }
        }

        /// <summary>
        /// Список аппликатур аккорда.
        /// <para><em>Создаётся в отдельной функции при инициализации переменной типа Chord.</em></para>
        /// </summary>
        /// <exception cref = "ArgumentNullException">
        /// Выбрасывается если <paramref name="value"/> равен null.</exception>
        public HashSet<GuitarFingering> GuitarFingerings 
        {
            get => guitarFingerings;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                guitarFingerings = value;
            }
        }
        #endregion


        #region Конструкторы: без параметра, с параметром количества нот и списком нот
        /// <summary>
        /// Конструктор без параметра (все параметры аккорда выбираются случайно).
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Chord()
        {
            NumOfNotes = rnd.Next(2, Math.Min(7, MusicalNotes.AllowedAddedNotes.Count + 2)); // В аккорде минимум 2 ноты
            if (ChooseRootNote() && ChooseAddedNotes()) // Если успешно выбраны ноты аккорда
            {
                MakeChordName();
                MakeGuitarFingerings();
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Конструктор с параметром количества нот в аккорде.
        /// </summary>
        /// <param name="numOfNotes">Количество нот аккорда</param>
        public Chord(int numOfNotes)
        {
            NumOfNotes = numOfNotes;
            if (ChooseRootNote() && ChooseAddedNotes()) // Если успешно выбраны ноты аккорда
            {
                MakeChordName();
                MakeGuitarFingerings();
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Конструктор с параметром списка нот аккорда (добавлен для тестирования).
        /// </summary>
        /// <param name="chordNotes">Список нот аккорда</param>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, когда <paramref name="chordNotes"/> равен null 
        /// или содержит 0 элементов</exception>
        public Chord(List<Note> chordNotes)
        {
            if (chordNotes != null && chordNotes.Count != 0 
                && chordNotes[0] is RootNote && chordNotes[1..].All(x => x is AddedNote))
            {
                NumOfNotes = chordNotes.Count;
                ChordNotes = chordNotes;
                foreach (Note note in chordNotes) chordNotesIndexes.Add(note.Index);
                MakeChordName();
                MakeGuitarFingerings();
            }
            else throw new InvalidOperationException();
        }

        [JsonConstructor]
        public Chord(string chordName, int numOfNotes, List<Note> chordNotes, List<int> chordNotesIndexes, HashSet<int> addedNotesOctaveIndexes, HashSet<GuitarFingering> guitarFingerings)
        {
            this.chordName = chordName;
            this.numOfNotes = numOfNotes;
            this.chordNotes = chordNotes;
            this.chordNotesIndexes = chordNotesIndexes;
            this.addedNotesOctaveIndexes = addedNotesOctaveIndexes;
            this.guitarFingerings = guitarFingerings;
        }
        #endregion


        #region Методы создания аккорда: определение главной ноты, доп. ступеней, названия и списка аппликатур
        /// <summary>
        /// Выбирает главную ноту для аккорда из диапазона заданных пользователем нот.
        /// </summary>
        /// <returns><see langword="true"/> если главная нота выбрана</returns>
        public bool ChooseRootNote()
        {
            if (MusicalNotes.AllowedRootNotes.Count != 0)
            {
                int chosenRootNotesIndex = rnd.Next(MusicalNotes.AllowedRootNotes.Count);
                string chosenNote = MusicalNotes.AllowedRootNotes[chosenRootNotesIndex];
                int index = Array.IndexOf(MusicalNotes.Notes, chosenNote);
                chordNotesIndexes.Add(index);
                chordNotes.Add(new RootNote(index));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Выбирает нужное количество добавочных ступеней для аккорда.
        /// </summary>
        /// <returns><see langword="true"/> если добавочные ступени выбраны</returns>
        public bool ChooseAddedNotes()
        {
            if (MusicalNotes.AllowedAddedNotes.Count != 0)
            {
                RootNote rootNote = (RootNote)chordNotes[0];
                HashSet<int> chosenNotesIndexes = new(numOfNotes - 1);
                int[] allowedOctaveIndexes = MusicalNotes.AllowedAddedNotes.Values.ToArray();

                for (int i = 0; i < numOfNotes - 1; i++)
                {
                    int octaveIndex;
                    do
                    {
                        octaveIndex = allowedOctaveIndexes[rnd.Next(allowedOctaveIndexes.Length)];
                    } while (chosenNotesIndexes.Contains(octaveIndex));
                    chosenNotesIndexes.Add(octaveIndex);

                    AddedNote newNote = new(octaveIndex, rootNote);
                    chordNotesIndexes.Add(newNote.Index);
                    chordNotes.Add(newNote);
                }
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Определяет имя аккорда в 
        /// <see href="https://ru.wikipedia.org/wiki/Буквенно-цифровое_обозначение_аккорда">
        /// буквенно-цифровом обозначении</see> на основе выбранных нот.
        /// </summary>
        public void MakeChordName()
        {
            // Подготовка: создание списка индексов добавочных ступеней относительно главной ноты
            for (int i = 1; i < numOfNotes; i++)
            {
                addedNotesOctaveIndexes.Add(((AddedNote)chordNotes[i]).OctaveIndex);
            }

            // Создание названия для аккорда
            #region Этап 1: Определение главной ноты
            chordName += chordNotes[0].NoteName;
            #endregion

            #region Этап 2: Определение вида аккорда
            ChooseChordType();
            #endregion

            #region Этап 3: Вспомогательные ступени
            // Если присутствует 7 ступень (как она влияет на названия)
            if (addedNotesOctaveIndexes.Contains(10))
            {
                SupplementNameAccordingToThePresenceOf7th();
            }
            else // Если нет 7 ступени
            {
                SupplementNameAccordingToTheAbsenceOf7th();
            }

            if (addedNotesOctaveIndexes.Contains(1)) chordName += "add(2-)";

            SupplementNameDependingOnThePresenceOf5th();
            #endregion
        }

        #region Дополнительные функции для создания названия аккорда
        /// <summary>
        /// Определяет вид аккорда и добавляет соответствующее обозначение в название аккорда.
        /// </summary>
        public void ChooseChordType()
        {
            if (addedNotesOctaveIndexes.Contains(4)) // Мажорный, увеличенный или аккорд Хендрикса
            {
                if (addedNotesOctaveIndexes.Contains(8) 
                    && !addedNotesOctaveIndexes.Contains(7)) chordName += "aug";
                if (addedNotesOctaveIndexes.Contains(3)) chordName += "add(#9)";
            }
            else if (addedNotesOctaveIndexes.Contains(3)) // Минорный или уменьшенный
            {
                if (addedNotesOctaveIndexes.Contains(6) 
                    && !addedNotesOctaveIndexes.Contains(7)) chordName += "dim";
                else chordName += "m";
            }
            else if (addedNotesOctaveIndexes.Contains(2) || addedNotesOctaveIndexes.Contains(5)) // Задержанные аккорды
            {
                if (addedNotesOctaveIndexes.Contains(2)) chordName += "sus2";
                if (addedNotesOctaveIndexes.Contains(5)) chordName += "sus4";
            }
            else chordName += "(no3)";
        }

        /// <summary>
        /// Дополняет название аккорда нужными обозначениями с учётом наличия 7 ступени.
        /// </summary>
        public void SupplementNameAccordingToThePresenceOf7th()
        {
            // Если мажорный, минорный или нейтральный аккорд
            if (addedNotesOctaveIndexes.Contains(3) 
                || addedNotesOctaveIndexes.Contains(4) || chordName.Contains("no3")) 
            {
                if (addedNotesOctaveIndexes.Contains(2) 
                    && addedNotesOctaveIndexes.Contains(5)) chordName += "9add4";
                else if (addedNotesOctaveIndexes.Contains(2)) chordName += "9";
                else if (addedNotesOctaveIndexes.Contains(5)) chordName += "11";
                if (chordName.Contains('9') || chordName.Contains("11")) // 6 и 7+ ступени
                {
                    if (addedNotesOctaveIndexes.Contains(9))
                    {
                        if (Char.IsDigit(chordName[^1])) chordName += "/6";
                        else chordName += "6";
                    }
                    if (addedNotesOctaveIndexes.Contains(11)) chordName += "maj7";
                }
                else
                {
                    if (addedNotesOctaveIndexes.Contains(9) || addedNotesOctaveIndexes.Contains(11))
                    {
                        if (addedNotesOctaveIndexes.Contains(9))
                        {
                            if (Char.IsDigit(chordName[^1])) chordName += "/13";
                            else chordName += "13";
                        }
                        if (addedNotesOctaveIndexes.Contains(11))
                        {
                            if (chordName.Contains("13")) chordName += "maj7";
                            else chordName += "maj13";
                        }
                    }
                    else chordName += "7";
                }
            }
            else if (addedNotesOctaveIndexes.Contains(2) 
                || addedNotesOctaveIndexes.Contains(5)) // Если задержанный аккорд
            {
                if (addedNotesOctaveIndexes.Contains(2) && addedNotesOctaveIndexes.Contains(5)) 
                    chordName = chordName.Replace("sus2sus4", "7sus2sus4");
                else if (addedNotesOctaveIndexes.Contains(2)) chordName = chordName.Replace("sus2", "7sus2");
                else chordName = chordName.Replace("sus4", "7sus4");

                if (addedNotesOctaveIndexes.Contains(9) 
                    || addedNotesOctaveIndexes.Contains(11)) // 6 и 7+ ступени
                {
                    if (chordName.Contains('7'))
                    {
                        if (addedNotesOctaveIndexes.Contains(9))
                        {
                            if (Char.IsDigit(chordName[^1])) chordName += "/6";
                            else chordName += "6";
                        }
                        if (addedNotesOctaveIndexes.Contains(11)) chordName += "maj7";
                    }
                    else
                    {
                        if (addedNotesOctaveIndexes.Contains(9) && addedNotesOctaveIndexes.Contains(11)) 
                            chordName += "maj7maj13";
                        else if (addedNotesOctaveIndexes.Contains(9))
                        {
                            if (Char.IsDigit(chordName[^1])) chordName += "/13";
                            else chordName += "13";
                        }
                        else if (addedNotesOctaveIndexes.Contains(11)) chordName += "maj13";
                    }
                }
            }
        }

        /// <summary>
        /// Дополняет название аккорда нужными обозначениями с учётом отсутствия 7 ступени.
        /// </summary>
        public void SupplementNameAccordingToTheAbsenceOf7th()
        {
            if (addedNotesOctaveIndexes.Contains(3) || addedNotesOctaveIndexes.Contains(4))
            {
                if (addedNotesOctaveIndexes.Contains(2) 
                    && addedNotesOctaveIndexes.Contains(5)) chordName += "add2add4";
                else if (addedNotesOctaveIndexes.Contains(2)) chordName += "add9";
                else if (addedNotesOctaveIndexes.Contains(5)) chordName += "add11";
            }

            if (addedNotesOctaveIndexes.Contains(9))
            {
                if (Char.IsDigit(chordName[^1])) chordName += "/6";
                else chordName += "6";
            }
            if (addedNotesOctaveIndexes.Contains(11)) chordName += "maj7";
        }

        /// <summary>
        /// Дополняет название аккорда нужными обозначениями в зависимости от наличия 5 ступени.
        /// </summary>
        public void SupplementNameDependingOnThePresenceOf5th()
        {
            // Если есть квинта (как она влияет на обозначения других ступеней)
            if (addedNotesOctaveIndexes.Contains(7)) 
            {
                if (addedNotesOctaveIndexes.Contains(6) && !chordName.Contains("dim")) chordName += "add(4+)";
                if (addedNotesOctaveIndexes.Contains(8) && !chordName.Contains("aug")) chordName += "add(6-)";
            }
            else // Если нет квинты
            {
                if (addedNotesOctaveIndexes.Contains(6) && !chordName.Contains("dim"))
                {
                    if (Char.IsDigit(chordName[^1])) chordName += "/5-";
                    else chordName += "5-";
                }
                else if (addedNotesOctaveIndexes.Contains(8) && !chordName.Contains("aug"))
                {
                    if (Char.IsDigit(chordName[^1]) || chordName[^1] == Char.Parse("-")) chordName += "/5+";
                    else chordName += "5+";
                }
                else if (!chordName.Contains("dim") && !chordName.Contains("aug")) chordName += "(no5)";
            }
        }
        #endregion

        /// <summary>
        /// Созданаёт список гитарных аппликатур к аккорду.
        /// </summary>
        public void MakeGuitarFingerings()
        {
            string[] possibleNotes = new string[numOfNotes + 1];
            possibleNotes[0] = "";
            for (int i = 0; i < numOfNotes; i++) possibleNotes[i + 1] = chordNotes[i].NoteName;

            string rootNoteName = chordNotes[0].NoteName;
            foreach (string e_Note in possibleNotes)
                foreach (string B_Note in possibleNotes)
                    foreach (string G_Note in possibleNotes)
                        foreach (string D_Note in possibleNotes)
                            foreach (string A_Note in possibleNotes)
                                foreach (string E_Note in possibleNotes)
                                {
                                    List<string> allFingeringNotes = [ e_Note, B_Note, G_Note, 
                                        D_Note, A_Note, E_Note ];
                                    for (int i = 1; i < 6; i++)
                                    {
                                        List<string> fingeringNotes = allFingeringNotes[..(i + 1)];

                                        if (fingeringNotes[^1] == rootNoteName
                                            && chordNotes.All(x => fingeringNotes.Contains(x.NoteName)))
                                        {
                                            for (int j = i + 1; j < 6; j++) fingeringNotes.Add("");
                                            GuitarFingering fingering = new(fingeringNotes);
                                            AddFingeringsWithDifferentFretIndexes(fingering);
                                        }
                                        
                                    }
                                }
        }

        /// <summary>
        /// Добавляет в список аппликатур все варианты аппликатур для заданного списка нот.
        /// </summary>
        /// <param name="fingering">Аппликатура со списком названий нот</param>
        public void AddFingeringsWithDifferentFretIndexes(GuitarFingering fingering)
        {
            int[,] possibleFrets = new int[6, 2];
            for (int i = 0; i < 6; i++)
            {
                string[] guitarString = Enumerable.Range(0, 16)
                    .Select(x => fingering.notesOnGuitarFretboard[i, x]).ToArray();
                string note = fingering.FingeringNotesNames[i];
                possibleFrets[i, 0] = Array.IndexOf(guitarString, note);
                possibleFrets[i, 1] = Array.LastIndexOf(guitarString, note);
            }

            for (int e_fret = 0; e_fret < 2; e_fret++)
                for (int B_fret = 0; B_fret < 2; B_fret++)
                    for (int G_fret = 0; G_fret < 2; G_fret++)
                        for (int D_fret = 0; D_fret < 2; D_fret++)
                            for (int A_fret = 0; A_fret < 2; A_fret++)
                                for (int E_fret = 0; E_fret < 2; E_fret++)
                                {
                                    int[] possibleFretsArrIndexes = 
                                        [e_fret, B_fret, G_fret, D_fret, A_fret, E_fret];
                                    for (int i = 0; i < 6; i++)
                                        fingering.Frets[i] = possibleFrets[i, possibleFretsArrIndexes[i]];

                                    GuitarFingering newFingering = new(fingering);
                                    if (!guitarFingerings.Contains(newFingering))
                                        guitarFingerings.Add(newFingering);
                                }
        }

        /// <summary>
        /// Выбор аппликатур с расстоянием между крайними нотами меньше заданного пользователем.
        /// </summary>
        /// <returns>Hashset аппликатур с расстоянием между крайними нотами меньше заданного пользователем</returns>
        public HashSet<GuitarFingering> ChooseFingeringsByMaxDistanceBetweenExtremeNotes()
        {
            return guitarFingerings.Select(fingering => fingering)
                .Where(fingering => fingering.DistanceBetweenExtremeFrets <= GuitarFingering.MaxDistanceBetweenExtremeFrets)
                .ToHashSet();
        }
        #endregion

        /// <summary>
        /// Преобразует элемент класса Chord в string.
        /// </summary>
        /// <returns>Имя аккорда в 
        /// <see href="https://ru.wikipedia.org/wiki/Буквенно-цифровое_обозначение_аккорда">
        /// буквенно-цифровом обозначении.</see></returns>
        public override string ToString() { return ChordName; }
    }
}
