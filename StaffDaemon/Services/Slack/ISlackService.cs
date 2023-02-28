namespace StaffDaemon.Services.Slack
{
    public interface ISlackService
    {
        /// <summary>
        /// Отправляет сообщение в Slack
        /// </summary>
        /// <param name="subject">Тема сообщения</param>
        /// <param name="body">Текст сообщения</param>
        /// <param name="resrecipient">Получатель</param>
        /// <param name="attachFilePath">Путь до прикрепленного файла</param>
        /// <returns>true, если сообщение отправлено успешно, иначе - false</returns>
        Task<bool> SendMessageAsync(string subject, string body, string resrecipient, string attachFilePath = default);
    }
}
