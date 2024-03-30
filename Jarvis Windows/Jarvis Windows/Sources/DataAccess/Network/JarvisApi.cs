﻿using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Windows.ApplicationModel.Chat;
using System.Collections.ObjectModel;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using Jarvis_Windows.Sources.DataAccess.Local;
using Windows.Media.Protection.PlayReady;
using System.IO;

namespace Jarvis_Windows.Sources.DataAccess.Network;

public sealed class JarvisApi
{
    private static JarvisApi? _instance;
    const string _endpoint = "/api/v1";
    const string _actionEndpoint = $"{_endpoint}/ai-action/";
    const string _chatEndpoint = $"{_endpoint}/ai-chat/";
    const string _apiUserUsageEndpoint = $"{_endpoint}/tokens/usage/";

    private static HttpClient? _client;
    private static string? _apiUrl;
    private static string? _apiHeaderID;

    public JarvisApi(IAuthenticationService authenticationService)
    {
        _client = new HttpClient();
        _apiUrl = DataConfiguration.ApiUrl;

        /*if(AuthenticationService.AuthenState == AUTHEN_STATE.AUTHENTICATED)
        {
            _apiHeaderID = WindowLocalStorage.ReadLocalStorage("AccessToken");
        }
        else if(AuthenticationService.AuthenState == AUTHEN_STATE.NOT_AUTHENTICATED)
        {
            var localApiHeaderID = WindowLocalStorage.ReadLocalStorage("ApiHeaderID");
            if (String.IsNullOrEmpty(localApiHeaderID))
            {
                _apiHeaderID = Guid.NewGuid().ToString();
                WindowLocalStorage.WriteLocalStorage("ApiHeaderID", _apiHeaderID);
                WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", "10");
            }
            else
            {
                _apiHeaderID = localApiHeaderID;
            }
        }*/
        //_apiHeaderID = WindowLocalStorage.ReadLocalStorage("ClientID");
        _apiHeaderID = DependencyInjection.GetService<ITokenLocalService>().GetUUID();
        AuthenService = authenticationService;
    }

    public static JarvisApi Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JarvisApi(DependencyInjection.GetService<IAuthenticationService>());
            }
            return _instance;
        }
    }

    public IAuthenticationService AuthenService { get; set; }

    public async Task<string>? APIUsageHandler()
    {
        try
        {
            HttpResponseMessage response;
            if (AuthenticationService.AuthenState == AUTHEN_STATE.AUTHENTICATED)
            {
                var token = DependencyInjection.GetService<ITokenLocalService>().GetAccessToken();
                _client.DefaultRequestHeaders.Add("x-jarvis-guid", _apiHeaderID);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await _client.GetAsync(_apiUrl + _apiUserUsageEndpoint);
            }
            else
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl + _apiUserUsageEndpoint);
                request.Headers.Add("x-jarvis-guid", _apiHeaderID);
                response = await _client.SendAsync(request);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
                int remainingUsage = responseObject.availableTokens;
                string dateString = responseObject.date;
                if (DateTime.TryParseExact(dateString, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                {
                    WindowLocalStorage.WriteLocalStorage("RecentDate", dateTime.Day.ToString());
                }

                WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", remainingUsage.ToString());
                //WindowLocalStorage.WriteLocalStorage("IsAuthenticated", "true");

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

    public async Task<string?> ApiHandler(string requestBody, string endPoint)
    {
        var contentData = new StringContent(requestBody, Encoding.UTF8, "application/json");
        try
        {
            HttpResponseMessage response;
            if (AuthenticationService.AuthenState == AUTHEN_STATE.AUTHENTICATED)
            {
                var token = DependencyInjection.GetService<ITokenLocalService>().GetAccessToken();
                _client.DefaultRequestHeaders.Add("x-jarvis-guid", _apiHeaderID);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await _client.PostAsync(_apiUrl + endPoint, contentData);
            }
            else
            {
                _client.DefaultRequestHeaders.Clear();
                var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl + endPoint);
                request.Content = contentData;
                request.Headers.Add("x-jarvis-guid", _apiHeaderID);
                response = await _client.SendAsync(request);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                int remainingUsage = responseObject.remainingUsage;

                WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", remainingUsage.ToString());
                Logging.Log($"Unauthenticated call: {remainingUsage}");

                string finalMessage = responseObject.message;
                return finalMessage;
            }
            else
            {
                var refreshResult = await AuthenService.Refresh();
                if(!String.IsNullOrEmpty(refreshResult) && refreshResult.Equals("refreshed_success"))
                {
                    return await ApiHandler(requestBody, endPoint);
                }
                else if(!String.IsNullOrEmpty(refreshResult) &&  refreshResult.Equals("signed_out"))
                {
                    Process.GetCurrentProcess().Kill();
                }
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

public static class Logging
{
    private static bool isLogging = true;
    public static void Log(string message)
    {
        if (!isLogging) return;

        string _logFilePath = "C:\\Users\\vupham\\Desktop\\logJarvis.txt";
        try
        {
            using (StreamWriter _streamWriter = new StreamWriter(_logFilePath, true))
            {
                _streamWriter.WriteLine($"{DateTime.Now} - {message}");
            }
        }

        catch { return; }
    }
}
