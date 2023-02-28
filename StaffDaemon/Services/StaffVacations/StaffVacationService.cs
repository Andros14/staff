using System.Text;
using StaffDaemon.Services.Common;
using StaffDaemon.Services.GoogleSheets;

namespace StaffDaemon.Services.StaffVacations
{
    public class StaffVacationService : BaseService<StaffVacationServiceInitialParams>, IStaffVacationService
    {
        public StaffVacationService(StaffVacationServiceInitialParams initialParams)
            : base(initialParams)
        {
        }

        public async Task NotifyStaffAsync()
        {
            var SpreadsheetId = initialParams.Options.SpreadsheetId;
            var ranges = initialParams.Options.SpreadsheetRanges;
            foreach (var range in ranges)
            {
                await ExecuteSafeAsync(async () =>
                {
                    var rows = await initialParams.GoogleSheetsService.ReadDataAsync(SpreadsheetId, range);
                    await HandleVacationScheduleAsync(rows);
                });
            }
        }

        private async Task HandleVacationScheduleAsync(RangeData rows)
        {
            var isRowStart = false;
            foreach (var row in rows)
            {
                if (row.Count < 1)
                    continue;

                if (row[0].Equals("1"))
                    isRowStart = true;

                if (!isRowStart)
                    continue;

                await HandleStaffMemberVacationAsync(row);
            }
        }

        private async Task HandleStaffMemberVacationAsync(RowData row)
        {
            var index = 0;
            foreach (var cell in row)
            {
                if (cell is not null && DateTime.TryParse(cell, out var vacationDate))
                {
                    if (CheckNeedSendingNotify(vacationDate, initialParams.Options.StaffNotifyDays))
                    {
                        await SendStaffNotifyAsync(row[2], cell, row[index + 1]);
                    }

                    if (CheckNeedSendingNotify(vacationDate, initialParams.Options.LeaderNotifyDays))
                    {
                        await SendStaffNotifyAsync(row[2], cell, row[index + 1]);
                        await SendLeaderNotifyAsync(row[2], row[3], cell, row[index + 1]);
                    }

                    if (CheckNeedSendingNotify(vacationDate, initialParams.Options.HrDepartmentNotifyDays))
                    {
                        await SendHrNotifyAsync(row[2], initialParams.Options.HrDepartmentSlackId, cell, row[index + 1]);
                    }
                }

                index++;
            }
        }

        private bool CheckNeedSendingNotify(DateTime vacationDate, int notifyDays)
        {
            var notifYDate = vacationDate.AddDays(-1 * notifyDays).Date;

            return notifYDate == DateTime.Now.Date;
        }

        private async Task SendStaffNotifyAsync(string address, string vacationDate, string vacationDasCount)
        {
            var subject = "Скоро отпуск!";
            var body = new StringBuilder($"Привет, <{address}>!")
                .AppendLine()
                .AppendLine($"Напоминаю, что в графике отпусков у тебя запланирован отпуск на {vacationDasCount} дней с {vacationDate}.")
                .AppendLine($"Тебе необходимо в течение недели написать заявление на отпуск. Бланки заявлений тут: {initialParams.Options.WikiUrl}")
                .AppendLine($"Если ты хочешь перенести отпуск, пожалуйста, согласуй это с руководителем и сообщи новые даты HR-специалисту <{initialParams.Options.HrDepartmentSlackId}>");

            await initialParams.SlackService.SendMessageAsync(subject, body.ToString(), address);
        }

        private async Task SendLeaderNotifyAsync(string staffAddress, string leaderAddress, string vacationDate, string vacationDasCount)
        {
            var subject = $"Отпуск для <{staffAddress}>";
            var body = new StringBuilder($"Привет, <{leaderAddress}>!")
                .AppendLine()
                .AppendLine($"Напоминаю, что в графике отпусков у твоего сотрудника <{staffAddress}> запланирован отпуск на {vacationDasCount} дней с {vacationDate}.")
                .AppendLine("К сожалению, мы до сих пор не получили ответа о том, пойдет он/она в отпуск или нет. Пожалуйста, поторопи его/ее с принятием решения, и сообщите о датах отпуска в HR отдел в течение 2х дней.")
                .AppendLine("Будем ждать!")
                .AppendLine($"p.s. График отпусков тут: {initialParams.Options.VacationScheduleUrl}");

            await initialParams.SlackService.SendMessageAsync(subject, body.ToString(), leaderAddress);
        }

        private async Task SendHrNotifyAsync(string staffAddress, string hrAddress, string vacationDate, string vacationDasCount)
        {
            var subject = $"Отпуск для <{staffAddress}>";
            var body = new StringBuilder($"Привет, <{hrAddress}>!")
                .AppendLine()
                .AppendLine($"Напоминаю, что в графике отпусков у сотрудника <{staffAddress}> запланирован отпуск на {vacationDasCount} дней с {vacationDate}.")
                .AppendLine($"К сожалению, информация о том, пойдет он/она в отпуск или нет до сих пор не обновлена в графике отпусков: {initialParams.Options.VacationScheduleUrl}")
                .AppendLine($"Пожалуйста, поторопи его/ее с принятием решения, и обнови данные в графике.")
                .AppendLine("Спасибо!");

            await initialParams.SlackService.SendMessageAsync(subject, body.ToString(), hrAddress);
        }
    }
}
