using SQLite;
using Jarvis_Windows.Sources.Utils.Core;
using System.Collections.ObjectModel;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Jarvis_Windows.Sources.DataAccess;
using Stfu.Linq;
using Microsoft.Expression.Interactivity.Media;
using Jarvis_Windows.Sources.DataAccess.Network;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Threading.Tasks;
public class ConversationManager
{
    readonly SQLiteConnection _connection;
    private static ConversationManager? _instance;
    public int _selectedIdx;
    public int ConversationCount
    {
        get
        {
            return _connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ConversationModel;");
        }
    }
    public static ConversationManager Instance()
    {
        if (_instance is null)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Jarvis_Windows_Conversations.db");
            _instance = new ConversationManager(dbPath);
        }

        return _instance;
    }
    public ConversationManager(string dbPath)
    {
        _connection = new SQLiteConnection(dbPath);
        _connection.CreateTable<ConversationModel>();
        _selectedIdx = -1;
    }

    public void UpdateConversation(ConversationModel conversation)
    {
        _connection.Update(conversation);
    }

    public void DeleteConversation(int conversationIdx)
    {
        _connection.Delete<ConversationModel>(conversationIdx);
        DropChatMessageTable(conversationIdx);
        ReArrangeIdx();
    }

    public void AddConversation(ConversationModel conversation)
    {
        _connection.Insert(conversation);
        CreateChatMessageTable(conversation.Idx);
        ReArrangeIdx();
    }

    public ConversationModel GetConversation(int conversationIdx)
    {
        var getConversationQuery = $@"
            SELECT *
            FROM ConversationModel
            WHERE Idx = ?;
        ";

        return _connection.Query<ConversationModel>(getConversationQuery, conversationIdx).FirstOrDefault();
    }

    public ObservableCollection<ConversationModel> GetAllConversations()
    {
        var conversations = _connection.Table<ConversationModel>().OrderBy(c => c.Idx).ToList();
        return new ObservableCollection<ConversationModel>(conversations);
    }

    private void ReArrangeIdx()
    {
        var conversations = _connection.Table<ConversationModel>().OrderBy(c => c.Idx).ToList();
        for (int i = 0; i < conversations.Count; i++)
        {
            if (conversations[i].Idx != i)
            {
                _connection.Execute("UPDATE ConversationModel SET Idx = ? WHERE Idx = ?", i, conversations[i].Idx);
                var oldTableName = $"Messages_{conversations[i].Idx}";
                var newTableName = $"Messages_{i}";
                if (_selectedIdx == conversations[i].Idx) _selectedIdx = i;
                _connection.Execute($"ALTER TABLE {oldTableName} RENAME TO {newTableName}");

            }
        }
    }

    private void CreateChatMessageTable(int conversationIdx)
    {
        var tableName = $"Messages_{conversationIdx}";
        var createTableQuery = $@"
            CREATE TABLE IF NOT EXISTS {tableName} (
                Idx INTEGER PRIMARY KEY AUTOINCREMENT,
                IsServer INTEGER,
                IsUser INTEGER,
                Message TEXT,
                SelectedModelIdx INTEGER
            )";

        _connection.Execute(createTableQuery);
    }

    private void DropChatMessageTable(int conversationIdx)
    {
        var tableName = $"Messages_{conversationIdx}";
        var dropTableQuery = $"DROP TABLE IF EXISTS {tableName}";
        _connection.Execute(dropTableQuery);
    }

    public ObservableCollection<AIChatMessage> LoadChatMessages(int conversationIdx)
    {
        if (conversationIdx == -1)
        {
            return new ObservableCollection<AIChatMessage>();
        }

        var tableName = $"Messages_{conversationIdx}";
        var selectQuery = $@"
            SELECT * FROM {tableName}";

        var messages = _connection.Query<AIChatMessage>(selectQuery, conversationIdx).ToList();

        return new ObservableCollection<AIChatMessage>(messages);
    }


    public async Task UpdateChatMessage(AIChatMessage message, bool isUpdate)
    {
        if (_selectedIdx == -1)
        {
            _selectedIdx = ConversationCount;
            if (message.IsUser)
                AddConversation(new ConversationModel { Idx = _selectedIdx });
        }

        if (isUpdate)
        {
            var tableName = $"Messages_{_selectedIdx}";
            var updateQuery = $@"
                UPDATE {tableName}
                SET IsServer = ?,
                    IsUser = ?,
                    Message = ?,
                    SelectedModelIdx = ?
                WHERE Idx = ?";

            _connection.Execute(updateQuery, message.IsServer, message.IsUser, message.Message, message.SelectedModelIdx, message.Idx);
        }
        else
        {
            AddChatMessage(_selectedIdx, message);
        }

        await UpdateConversationLastMessage(_selectedIdx, message.Message, message.IsServer);
    }

    public void AddChatMessage(int conversationIdx, AIChatMessage message)
    {
        var tableName = $"Messages_{conversationIdx}";
        var insertQuery = $@"
            INSERT INTO {tableName} (IsServer, IsUser, Message, SelectedModelIdx)
            VALUES (?, ?, ?, ?)";

        _connection.Execute(insertQuery, message.IsServer, message.IsUser, message.Message, message.SelectedModelIdx);
        ReArrangeChatMessageIdx(conversationIdx);
    }


    private void ReArrangeChatMessageIdx(int conversationIdx)
    {
        var tableName = $"Messages_{conversationIdx}";
        var messages = _connection.Query<AIChatMessage>($"SELECT * FROM {tableName} ORDER BY Idx").ToList();
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i].Idx != i)
            {
                _connection.Execute($"UPDATE {tableName} SET Idx = ? WHERE Idx = ?", i, messages[i].Idx);
            }
        }
    }

    private async Task UpdateConversationLastMessage(int conversationIdx, string lastMessage, bool isServer)
    {
        ConversationModel conversation = GetConversation(conversationIdx);
        if (string.IsNullOrEmpty(conversation.Title))
        {
            conversation.Title = "`10492365-021812-08374-120384";
            conversation.IsSelected = conversation.IsShowConversation = true;
            conversation.FavoriteColor = DataConfiguration.FilterConversationColor(0);
            conversation.FavoriteData = DataConfiguration.FilterConversationData(0);
        }


        if (conversation.Title == "`10492365-021812-08374-120384" && isServer)
        {
            conversation.Title = await GenerateTitle(conversationIdx, lastMessage);
        }

        conversation.Title = conversation.Title.Replace("\"", "");
        conversation.LastMessage = lastMessage.Length <= 50 ? lastMessage : lastMessage.Substring(0, 50) + "...";
        conversation.LastUpdatedTime = DateTime.Now.ToString();
        conversation.LastUpdatedTimeDT = DateTime.Now;
        _connection.Update(conversation);      
    }

    private async Task<string> GenerateTitle(int conversationIdx, string lastMessage)
    {
        List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();
        var requestBody = JsonConvert.SerializeObject(new
        {
            content = $"Give me a concise title (between 3 to 5 words) for this paragraph \"{lastMessage}\".",
            metadata = new
            {
                conversation = new
                {
                    messages = messages
                }
            }
        });

        var contentData = new StringContent(requestBody, Encoding.UTF8, "application/json");
        
        HttpClient? _client = new HttpClient();

        _client.DefaultRequestHeaders.Clear();
        var request = new HttpRequestMessage(HttpMethod.Post, DataConfiguration.ApiUrl + "/api/v1/ai-chat/");
        request.Content = contentData;
        request.Headers.Add("x-jarvis-guid", Guid.NewGuid().ToString());
        HttpResponseMessage response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = response.Content.ReadAsStringAsync().Result;
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            return responseObject.message;
        }

        return "New Chat";
    }
}
