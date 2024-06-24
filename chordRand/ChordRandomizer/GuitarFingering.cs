using System.Text.Json.Serialization;

namespace ChordRandomizer
{
    public class GuitarFingering : GuitarFretboardNotes
    {
        #region Поля: названия нот аппликатуры; номера ладов, на к-х зажимаются струны; индексы нот в MIDI; расстояние между крайними нотами
        private List<string> fingeringNotesNames = new(6);
        private int[] frets = new int[6];
        private List<int> frequences = new(6);
        private List<int> notesMidiIndexes = new(6);
        private int distanceBetweenExtremeFrets = 0;
        private static int maxDistanceBetweenExtremeFrets;
        #endregion


        #region Свойства для приватных полей
        public List<string> FingeringNotesNames {  get => fingeringNotesNames; set => fingeringNotesNames = value; }
        public int[] Frets { get => frets; set => frets = value; }
        public List<int> Frequences { get => frequences; set => frequences = value; }
        public List<int> NotesMidiIndexes { get => notesMidiIndexes; set => notesMidiIndexes = value; }

        /// <summary>
        /// Расстояние в ладах между крайними нотами в аппликатуре.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, когда <paramref name="value"/> меньше 0 или больше 15.
        /// </exception>
        public int DistanceBetweenExtremeFrets
        {
            get => distanceBetweenExtremeFrets;
            set
            {
                if (value < 0 || value > 15) throw new ArgumentOutOfRangeException(nameof(value));
                distanceBetweenExtremeFrets = value;
            }
        }

        /// <summary>
        /// Максимальное допустимое расстояние в ладах между крайними нотами в аппликатуре.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, когда <paramref name="value"/> меньше 0 или больше 15.
        /// </exception>
        public static int MaxDistanceBetweenExtremeFrets
        {
            get => maxDistanceBetweenExtremeFrets;
            set
            {
                if (value < 0 || value > 15) throw new ArgumentOutOfRangeException(nameof(value));
                maxDistanceBetweenExtremeFrets = value;
            }
        }
        #endregion


        #region Конструкторы
        public GuitarFingering(List<string> fingeringNotesNames)
        {
            this.fingeringNotesNames = new(fingeringNotesNames);
        }

        public GuitarFingering(GuitarFingering fingering)
        {
            fingeringNotesNames = new(fingering.fingeringNotesNames);
            fingering.frets.CopyTo(frets, 0);
            DistanceBetweenExtremeFrets = CalculateDistanceBetweenExtremeFrets();
            for (int i = 5; i > -1; i--)
            {
                if (frets[i] != -1)
                {
                    frequences.Add(notesFrequencies[i, frets[i]]);
                    notesMidiIndexes.Add(openStringsNotesMidiIndexes[i] + frets[i]);
                }
            }
        }

        [JsonConstructor]
        public GuitarFingering(List<string> fingeringNotesNames, int[] frets, List<int> frequences, List<int> notesMidiIndexes, int distanceBetweenExtremeFrets)
        {
            this.fingeringNotesNames = fingeringNotesNames;
            this.frets = frets;
            this.frequences = frequences;
            this.notesMidiIndexes = notesMidiIndexes;
            this.distanceBetweenExtremeFrets = distanceBetweenExtremeFrets;
        }
        #endregion


        #region Методы: Нахождение расстояния между крайними нотами, Equals и GetHashCode
        public int CalculateDistanceBetweenExtremeFrets()
        {
            var nonNegativeFretsIndexes = frets.Select(x => x)
                .Where(x => x > 0);
            if (nonNegativeFretsIndexes.Any())
            {
                int maxFret = nonNegativeFretsIndexes.Max();
                int minFret = nonNegativeFretsIndexes.Min();
                return maxFret - minFret;
            }
            else return 0;
        }


        public override bool Equals(object? other)
        {
            if (other != null && other is GuitarFingering fingering)
            {
                if (fingeringNotesNames.SequenceEqual(fingering.fingeringNotesNames) 
                    && frets.SequenceEqual(fingering.frets)) return true;
                else return false;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var noteName in fingeringNotesNames)
                {
                    hash = hash * 23 + noteName.GetHashCode();
                }

                foreach (int fret in frets)
                {
                    if (fret != 0) hash = hash * 23 + fret;
                }

                foreach (int index in notesMidiIndexes)
                {
                    if (index != 0) hash = hash * 23 + index;
                }

                hash = hash * 23 + distanceBetweenExtremeFrets;

                return hash;
            }
        }
        #endregion
    }
}
