namespace ChordRandomizer
{
    public class MusicalNotes
    {
        public enum NotesNames { A, Ais, B, C, Cis, D, Dis, E, F, Fis, G, Gis }

        /// <summary>
        /// Массив 12 нот октавы от ноты "до" до ноты "си".
        /// </summary>
        public static readonly string[] Notes = 
            ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];

        /// <summary>
        /// Массив 11 возможных добавочных ступеней.
        /// </summary>
        public static readonly string[] AddedNotes =
            ["2-", "2", "m3", "M3", "4", "4+/5-", "5", "5+/6-", "6", "7", "maj7"];


        /// <summary>
        /// Список главных нот, выбранных пользователем.
        /// </summary>
        public static List<string> AllowedRootNotes = 
            ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];

        /// <summary>
        /// Список добавочных нот, выбранных пользователем, 
        /// с соответствующими индексами в октаве от главной ноты.
        /// </summary>
        public static Dictionary<string, int> AllowedAddedNotes = new(11)
        {
            ["2-"] = 1,
            ["2"] = 2,
            ["m3"] = 3,
            ["M3"] = 4,
            ["4"] = 5,
            ["4+/5-"] = 6,
            ["5"] = 7,
            ["5+/6-"] = 8,
            ["6"] = 9,
            ["7"] = 10,
            ["maj7"] = 11
        };
    }
}