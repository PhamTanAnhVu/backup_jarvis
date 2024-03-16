using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Windows.ApplicationModel.Chat;
using System.Collections.ObjectModel;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using System.Collections.Generic;

namespace Jarvis_Windows.Sources.DataAccess.Network;

public sealed class JarvisApi
{
    private static JarvisApi? _instance;
    const string _endpoint = "/api/v1";
    const string _actionEndpoint = $"{_endpoint}/ai-action/";
    const string _chatEndpoint = $"{_endpoint}/ai-chat/";

    private static HttpClient? _client;
    private static string? _apiUrl;
    private static string? _apiHeaderID;

    private JarvisApi()
    {
        _client = new HttpClient();
        _apiUrl = DataConfiguration.ApiUrl;
    }

    public static JarvisApi Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JarvisApi();
            }
            return _instance;
        }
    }

    public async Task<string?> ApiHandler(string requestBody, string endPoint)
    {
        if (WindowLocalStorage.ReadLocalStorage("ApiHeaderID") == "")
        {
            WindowLocalStorage.WriteLocalStorage("ApiHeaderID", Guid.NewGuid().ToString());
            WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", "10");
        }

        _apiHeaderID = WindowLocalStorage.ReadLocalStorage("ApiHeaderID");

        var contentData = new StringContent(requestBody, Encoding.UTF8, "application/json");
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl + endPoint);
            request.Content = contentData;
            request.Headers.Add("x-jarvis-guid", _apiHeaderID);
            HttpResponseMessage response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                int remainingUsage = responseObject.remainingUsage;
                //try
                //{
                //    remainingUsage = responseObject.remainingUsage;
                //}
                //catch
                //{
                //    remainingUsage = int.Parse(WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")) - 1;
                //}

                WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", remainingUsage.ToString());

                string finalMessage = responseObject.message;
                return finalMessage;
            }

            return null;
        }
        catch (Exception ex)
        {
            throw ex.GetBaseException();
        }
    }

    // ---------------------------------- Non custom AI Actions ---------------------------------- //
    public async Task<string?> TranslateHandler(String content, String lang)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "translate",
            content = content,
            metadata = new
            {
                translateTo = lang
            }
        });

        return await ApiHandler(requestBody, _actionEndpoint);
    }

    public async Task<string?> ReviseHandler(String content)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "revise",
            content = content
        });

        return await ApiHandler(requestBody, _actionEndpoint);
    }

    public async Task<string?> AskHandler(String content, string action)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "ask",
            content = content,
            action = action
        });

        return await ApiHandler(requestBody, _actionEndpoint);
    }

    public async Task<string?> ChatHandler(string content, ObservableCollection<AIChatMessage> ChatHistory)
    {
        List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

        for (int i = 1; i < ChatHistory.Count - 2; i++)
        {
            string chatMessage = ChatHistory[i].Message;
            string role = (i % 2 == 0) ? "user" : "assistant";
            var messageDict = new Dictionary<string, string>
            {
                { "role", role },
                { "content", chatMessage }
            };

            messages.Add(messageDict);
        }

        var requestBody = JsonConvert.SerializeObject(new
        {
            content = content,
            metadata = new
            {
                conversation = new
                {
                    messages = messages
                }
            }
        });

        return await ApiHandler(requestBody, _chatEndpoint);
    }


    public async Task<string?> AIHandler(string content, string action)
    {
        return await CustomAiActionHandler(content, action);
    }

    private async Task<string?> CustomAiActionHandler(string content, string action)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "custom",
            content = content,
            action = action
        });

        if (content == "")
        {
            requestBody = JsonConvert.SerializeObject(new
            {
                type = "custom",
                action = action
            });
        }


        return await ApiHandler(requestBody, _actionEndpoint);
    }

    // ---------------------------------- Custom AI Actions ---------------------------------- //
}