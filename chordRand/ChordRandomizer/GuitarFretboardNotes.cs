namespace ChordRandomizer
{
    public class GuitarFretboardNotes
    {
        #region Поля: двумерные массивы названий нот и их частот, массив индексов нот открытых струн в MIDI
        /// <summary>
        /// Двумерный массив названий нот на грифе гитары. 
        /// <list type="bullet"><item>notesOnGuitarFretboard[0] - это ноты 1-й струны.</item>
        /// <item>notesOnGuitarFretboard[1] - это ноты 2-й струны.</item>
        /// <item>...</item>
        /// <item>notesOnGuitarFretboard[5] - это ноты 6-й струны.</item></list>
        /// (Струны нумеруются от самой <strong>тонкой</strong> до самой <strong>толстой</strong>)
        /// </summary>
        public string[,] notesOnGuitarFretboard = new string[6, 16]
        {
            { "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G" },
            { "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D" },
            { "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#" },
            { "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F" },
            { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C" },
            { "E", "F", "F#", "G", "G#", "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G" }
        };

        /// <summary>
        /// Двумерный массив частот нот на грифе гитары. 
        /// <list type="bullet"><item>notesOnGuitarFretboard[0] - это частоты нот 1-й струны.</item>
        /// <item>notesOnGuitarFretboard[1] - это частоты нот 2-й струны.</item>
        /// <item>...</item>
        /// <item>notesOnGuitarFretboard[5] - это частоты нот 6-й струны.</item></list>
        /// (Струны нумеруются от самой <strong>тонкой</strong> до самой <strong>толстой</strong>)
        /// </summary>
        public int[,] notesFrequencies = new int[6, 16]
        {
            { 330, 349, 370, 392, 415, 440, 466, 494, 523, 554, 587, 622, 659, 698, 740, 784 },
            { 247, 262, 277, 294, 311, 330, 349, 370, 392, 415, 440, 466, 494, 523, 554, 587 },
            { 196, 207, 220, 233, 247, 262, 277, 294, 311, 330, 349, 370, 392, 415, 440, 466 },
            { 148, 156, 165, 175, 185, 196, 207, 220, 233, 247, 262, 277, 294, 311, 330, 349 },
            { 110, 117, 123, 131, 139, 148, 156, 165, 175, 185, 196, 207, 220, 233, 247, 262 },
            { 82, 87, 93, 98, 103, 110, 117, 123, 131, 139, 148, 156, 165, 175, 185, 196 }
        };

        /// <summary>
        /// Массив индексов нот открытых струн гитары в соответствии с нумерацией в MIDI.
        /// </summary>
        public int[] openStringsNotesMidiIndexes = [64, 59, 55, 50, 45, 40];
        #endregion
    }
}
