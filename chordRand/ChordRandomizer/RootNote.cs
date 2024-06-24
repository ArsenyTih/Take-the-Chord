namespace ChordRandomizer
{
    public class RootNote : Note
    {
        #region Поля: названия нот октавы в порядке от главной ноты
        private string[] octaveFromRoot = new string[12];
        #endregion


        #region Свойства для приватных полей
        /// <summary>
        /// Массив названий нот октавы в порядке от главной ноты.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если <paramref name="value.Length"/> не равна 12
        /// </exception>
        public string[] OctaveFromRoot
        {
            get => octaveFromRoot;
            set
            {
                if (value.Length != 12) throw new ArgumentException(nameof(value.Length));
                octaveFromRoot = value;
            }
        }
        #endregion


        #region Конструкторы: с параметром имени и индекса
        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        public RootNote() : base() { }

        /// <summary>
        /// Конструктор с параметром имени ноты.
        /// </summary>
        /// <param name="name">Название ноты</param>
        public RootNote(string name) : base(name)
        {
            OctaveFromRoot = ArrangeOctaveFromRoot();
        }

        /// <summary>
        /// Конструктор с параметром индекса в октаве от главной ноты.
        /// </summary>
        /// <param name="index">Индекс в октаве от главной ноты</param>
        public RootNote(int index) : base(index)
        {
            OctaveFromRoot = ArrangeOctaveFromRoot();
        }
        #endregion


        #region Методы: сортировка нот от главной
        /// <summary>
        /// Сортирует ноты октавы относительно главной.
        /// </summary>
        /// <returns>Массив названий нот октавы относительно главной ноты.</returns>
        string[] ArrangeOctaveFromRoot()
        {
            string[] newOctave = new string[12];
            newOctave[0] = NoteName; // Добавление тоники

            int current = 1;
            for (int i = Index + 1; i < 12; i++) // Добавление нот после тоники
            {
                newOctave[current] = Notes[i];
                current++;
            }

            for (int i = 0; i < Index; i++) // Добавление нот до тоники
            {
                newOctave[current] = Notes[i];
                current++;
            }

            return newOctave;
        }
        #endregion
    }
}
