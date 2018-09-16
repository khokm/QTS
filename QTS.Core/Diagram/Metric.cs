namespace QTS.Core.Diagram
{
    /// <summary>
    /// Представляет выбор типа показателя системы.
    /// </summary>
    enum MetricType
    {
        /// <summary>
        /// Целочисленное значение.
        /// </summary>
        Integer,
        /// <summary>
        /// Дробное значение.
        /// </summary>
        Float,
        /// <summary>
        /// Вероятность.
        /// </summary>
        Probality
    }

    /// <summary>
    /// Инкапсулирует метод, который вычисляет значение показателя системы.
    /// </summary>
    /// <param name="diagram">Анализируемая система.</param>
    /// <returns>Значение показателя системы.</returns>
    delegate float Formula(IAnalyzable diagram);

    /// <summary>
    /// Представляет показатель системы.
    /// </summary>
    struct Metric
    {
        /// <summary>
        /// Формула для расчета значения показателя.
        /// </summary>
        public Formula Formula { get; }

        /// <summary>
        /// Название показателя.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Единицы измерения.
        /// </summary>
        public string Units { get; }

        /// <summary>
        /// Тип показателя.
        /// </summary>
        public MetricType Type { get; }

        /// <summary>
        /// Создает новый показатель системы.
        /// </summary>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="units"><see cref="Units"/></param>
        /// <param name="formula"><see cref="Formula"/></param>
        /// <param name="type"><see cref="Type"/></param>
        public Metric(string name, string units, Formula formula, MetricType type)
        {
            Formula = formula;
            Name = name;
            Units = units;
            Type = type;
        }
    }
}
