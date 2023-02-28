using RestSharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StaffDaemon.PlatformUtils.Extensions;
using StaffDaemon.PlatformUtils.Helpers;
using StaffDaemon.Services.Common;

namespace StaffDaemon.Services.Slack
{
    public class SlackService : BaseService<SlackServiceInitialParams>, ISlackService
    {
        public SlackService(SlackServiceInitialParams initialParams)
            : base(initialParams)
        {
        }

        public async Task<bool> SendMessageAsync(string subject, string body, string resrecipient, string attachFilePath = default)
        {
            if (resrecipient.IsEmpty())
            {
                initialParams.Logger.LogError($"Can't send message: '{GetMessage(subject, body)}'. Resrecipient is empty");
                return false;
            }

            return attachFilePath.IsEmpty()
                ? await SendMessageAsync(subject, body, resrecipient)
                : await SendFileAsync(subject, body, resrecipient, attachFilePath);
        }

        private async Task<bool> SendMessageAsync(string subject, string body, string resrecipient)
        {
            var request = GetAuthorizationRequest("chat.postMessage", Method.POST);

            request.AddJsonBody(new SlackMessage()
            {
                Channel = resrecipient,
                UserName = initialParams.Options.UserName,
                Text = GetMessage(subject, body)
            });

            return await SendRequestAsync(request);
        }

        private async Task<bool> SendFileAsync(string subject, string body, string resrecipient, string attachFilePath)
        {
            var request = GetAuthorizationRequest("files.upload", Method.POST);

            request.AddParameter("channels", resrecipient);
            request.AddParameter("initial_comment", GetMessage(subject, body));
            request.AddFile("file", attachFilePath, "multipart/form-data");

            return await SendRequestAsync(request);
        }

        private RestRequest GetAuthorizationRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method)
            {
                JsonSerializer = RestSharpHelper.GetJsonSerializer()
            };
            request.AddHeader("Authorization", $"Bearer {initialParams.Options.ApiToken}");

            return request;
        }

        private string GetMessage(string subject, string body)
        {
            return body.IsEmpty() ? subject : $"{subject} ```{body}```";
        }

        private async Task<bool> SendRequestAsync(RestRequest request)
        {
            var requestBody = request.Parameters.FirstOrDefault(x => x.Type == ParameterType.RequestBody);
            var result = await ExecuteSafeAsync(async () =>
            {
                var response = await initialParams.RestClient.ExecutePostAsync(request);
                if (!response.IsSuccessful)
                {
                    initialParams.Logger.LogError($"Can't send message '{requestBody?.Value.ToString()}. Slack return: {response.Content}");
                }

                return response.IsSuccessful;
            });

            initialParams.Logger.LogDebug($"Sent message: {requestBody?.Value.ToString()} with result: {(result ? "success" : "fail")}");

            return result;
        }
    }
}
