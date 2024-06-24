namespace ChordRandomizer
{
    public class Note : MusicalNotes
    {
        #region Поля: название ноты в буквенной нотации и индекс в октаве от ноты "до"
        protected string noteName = "";
        protected int index = 0;
        #endregion


        #region Свойства для приватных полей
        /// <summary>
        /// Название ноты в <see href="https://ru.wikipedia.org/wiki/Буквенная_нотация"> буквенной нотации</see>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если ноте даётся название, не присутствующее в доступных названиях.
        /// </exception>
        public string NoteName
        {
            get => noteName;
            set
            {
                if (!Notes.Contains(value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                noteName = value;
            }
        }

        /// <summary>
        /// Индекс ноты в октаве от ноты "до".
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если <paramref name="value"/> меньше 0 или больше 11.
        /// </exception>
        public int Index
        {
            get => index;
            set
            {
                if (value < 0 || value > 11) // Т.к. в массиве нот октавы 12 названий нот
                    throw new ArgumentOutOfRangeException(nameof(value));
                index = value;
            }
        }
        #endregion


        #region Конструкторы: без параметров, с параметром имени и индекса
        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        public Note() { }

        /// <summary>
        /// Конструктор с параметром названия ноты в 
        /// <see href="https://ru.wikipedia.org/wiki/Буквенная_нотация">буквенной нотации</see>.
        /// </summary>
        /// <param name="name">Название ноты в 
        /// <see href="https://ru.wikipedia.org/wiki/Буквенная_нотация">буквенной нотации</see></param>
        public Note(string name)
        { 
            NoteName = name;
            Index = Array.IndexOf(Notes, name);
        }

        /// <summary>
        /// Конструктор с параметром индекса ноты в октаве от ноты "до".
        /// </summary>
        /// <param name="index">Индекс ноты в октаве от ноты "до"</param>
        public Note(int index)
        {
            Index = index;
            NoteName = Notes[index];
        }

        //[JsonConstructor]
        //public Note(string noteName, int index)
        //{
        //    NoteName = noteName;
        //    Index = index;
        //}
        #endregion


        #region Методы: ToString
        /// <summary>
        /// Преобразуеь элемент класса Note в string.
        /// </summary>
        /// <returns>Название ноты в 
        /// <see href="https://ru.wikipedia.org/wiki/Буквенная_нотация">буквенной нотации</see>.</returns>
        public override string ToString() { return NoteName; }
        #endregion
    }
}
