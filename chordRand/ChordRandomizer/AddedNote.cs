namespace ChordRandomizer
{
    public class AddedNote : Note
    {
        #region Поля: главная нота, для к-й данная нота является добавочной, и индекс в октаве от главной ноты
        private RootNote? relatedRootNote;
        private int octaveIndex = 1;
        #endregion


        #region Свойства для приватных полей
        /// <summary>
        /// Главная нота, для к-й данная нота является добавочной.
        /// </summary>
        /// <exception cref="ArgumentNullException">Если <paramref name="value"/> = null.</exception>
        /// <exception cref="ArgumentException">Если названия ноты нет в списке всех нот.</exception>
        public RootNote? RelatedRootNote
        {
            get => relatedRootNote;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (!AllowedRootNotes.Contains(value.NoteName)) throw new ArgumentException(nameof(value.NoteName));
                relatedRootNote = value;
            }
        }

        /// <summary>
        /// Индекс в октаве от главной ноты.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Если <paramref name="value"/> меньше 1 или больше 11.
        /// </exception>
        public int OctaveIndex
        {
            get => octaveIndex;
            set
            {
                if (value < 1 || value > 11)
                    throw new ArgumentOutOfRangeException(nameof(value));
                octaveIndex = value;
            }
        }
        #endregion


        #region Конструкторы: с параметрами имени ноты / индекса и связанной главной ноты
        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        public AddedNote() : base() { }

        /// <summary>
        /// Конструктор с параметрами имени ноты и связанной главной ноты.
        /// </summary>
        /// <param name="noteName">Название ноты</param>
        /// <param name="rootNote">Связанная с ней главная нота</param>
        public AddedNote(string noteName, RootNote rootNote) : base(noteName)
        {
            RelatedRootNote = rootNote;
            OctaveIndex = Array.IndexOf(rootNote.OctaveFromRoot, noteName);
        }

        /// <summary>
        /// Конструктор с параметрами индекса и связанной главной ноты.
        /// </summary>
        /// <param name="octaveIndex">Индекс ноты в октаве от главной ноты</param>
        /// <param name="rootNote">Связанная с ней главная нота</param>
        public AddedNote(int octaveIndex, RootNote rootNote)
        {
            NoteName = rootNote.OctaveFromRoot[octaveIndex];
            Index = Array.IndexOf(Notes, noteName);
            RelatedRootNote = rootNote;
            OctaveIndex = octaveIndex;
        }
        #endregion
    }
}
