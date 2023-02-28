namespace StaffDaemon.Services.GoogleSheets
{
    public interface IGoogleSheetsService
    {
        /// <summary>
        /// Читает данные с листа
        /// <para>Вначале необходимо вызвать метод Initialize</para>
        /// </summary>
        /// <param name="spreadsheetId">Идентификатор таблицы</param>
        /// <param name="rangeName">Диапазон для считывания (формат: Лист!A:Z)</param>
        /// <returns>Прочитанные данные</returns>
        Task<RangeData> ReadDataAsync(string spreadsheetId, string rangeName);

        /// <summary>
        /// Добавляет данные на лист
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="spreadsheetId">Идентификатор таблицы</param>
        /// <param name="rangeName">Диапазон для обновления(формат: Лист!A:Z)</param>
        /// <returns>Обновленный диапазон данных</returns>
        Task<string> AppendDataAsync(IList<IList<object>> data, string spreadsheetId, string rangeName);
    }
}
