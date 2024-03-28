using Jarvis_Windows.Sources.DataAccess.Local;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jarvis_Windows.Sources.DataAccess.Network
{
    public interface IAuthenticationService
    {
        public Task<string?> SignIn(string email, string password);
        public Task<string?> SignUp(string email, string password, string username);
        public Task<string?> SignOut();
        public Task<Account?> GetMe();
        public Task<string?> Refresh();
    }

    public enum AUTHEN_STATE
    {
        AUTHENTICATED = 1,
        EXPIRED,
        NOT_AUTHENTICATED
    }

    public class AuthenticationService : ObserveralObject, IAuthenticationService
    {
        #region Constants
        private const string API_HEADER = "x-jarvis-guid";
        private const string REFRESH_QUERRY = "refreshToken";
        private const string GET_MẸ_ENDPOINT = "/api/v1/auth/me";
        private const string SIGN_IN_ENDPOINT = "/api/v1/auth/sign-in";
        private const string SIGN_UP_ENDPOINT = "/api/v1/auth/sign-up";
        private const string SIGN_OUT_ENDPOINT = "/api/v1/auth/sign-out";
        private const string REFRESH_ENDPOINT = "/api/v1/auth/refresh";
        private const int MAX_REFRESH_COUNT = 3;
        #endregion

        #region Fields
        private string _apiUr;
        private HttpClient _httpClient;
        private ITokenLocalService? _tokenLocalService;
        private static AUTHEN_STATE _authenState;
        private static int _doRefreshCount = 0;
        #endregion

        #region Properties
        public string ApiUr 
        { 
            get => _apiUr; 
            private set => _apiUr = value; 
        }

        public ITokenLocalService? TokenLocalService 
        { 
            get => _tokenLocalService; 
            set => _tokenLocalService = value; 
        }
        public HttpClient HttpClient { 
            get => _httpClient; 
            set => _httpClient = value; 
        }
        public static AUTHEN_STATE AuthenState 
        { 
            get => _authenState; 
            private set => _authenState = value;
        }
        #endregion

        #region Cache
        private string? AccessToken;
        private string? RefreshToken;
        private string? UUID;
        #endregion

        public AuthenticationService(ITokenLocalService tokenLocalService)
        {
            
            _apiUr = DataConfiguration.ApiUrl;
            _httpClient = new HttpClient();
            _tokenLocalService = tokenLocalService;

            if(_tokenLocalService != null)
            {
                AccessToken = _tokenLocalService.GetAccessToken();
                RefreshToken = _tokenLocalService.GetRefreshToken();
                UUID = _tokenLocalService.GetUUID();
                if (!string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken))
                {
                    AuthenState = AUTHEN_STATE.AUTHENTICATED;
                    HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }
                else
                {
                    AuthenState = AUTHEN_STATE.NOT_AUTHENTICATED;
                }
            }
        }

        public async Task<Account?> GetMe()
        {
            //HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add(API_HEADER, UUID);
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = HttpClient.GetAsync(ApiUr + GET_MẸ_ENDPOINT).Result;
            Account account = new Account();
            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic? responseObject = JsonConvert.DeserializeObject(responseContent);
                if (responseObject != null)
                {
                    string id = responseObject.id;
                    account.Username = responseObject.username;
                    account.Email = responseObject.email;
                    account.Role = responseObject.roles[0];
                    WindowLocalStorage.WriteLocalStorage("Username", account.Username);
                    WindowLocalStorage.WriteLocalStorage("Role", account.Role);
                    WindowLocalStorage.WriteLocalStorage("Email", account.Email);
                    if (account.Role.Equals("anonymous"))
                    {
                        AuthenState = AUTHEN_STATE.NOT_AUTHENTICATED;
                    }
                    else if(account.Role.Equals("user"))
                    {
                        AuthenState = AUTHEN_STATE.AUTHENTICATED;
                    }
                }
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Refresh();
            }
            
            return account;
        }

        public async Task<string?> SignIn(string email, string password)
        {
            string requestBody = JsonConvert.SerializeObject(new { email, password });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, ApiUr + SIGN_IN_ENDPOINT);
            request.Content = content;
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic? responseObject = JsonConvert.DeserializeObject(responseContent);

                if(responseObject != null)
                {
                    AccessToken = responseObject.token.accessToken;
                    RefreshToken = responseObject.token.refreshToken;
                    if(TokenLocalService != null)
                    {
                        TokenLocalService.SetAccessToken(AccessToken);
                        TokenLocalService.SetRefreshToken(RefreshToken);
                        AuthenState = AUTHEN_STATE.AUTHENTICATED;
                    }
                }
                return responseContent;
            }
            return null;
        }

        public async Task<string?> SignOut()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ApiUr + SIGN_OUT_ENDPOINT);
            request.Headers.Add(API_HEADER, AccessToken);
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic? responseObject = JsonConvert.DeserializeObject(responseContent);
                if (responseObject != null)
                {
                    AccessToken = null;
                    RefreshToken = null;
                    if(TokenLocalService != null)
                    {
                        TokenLocalService.ClearAccessToken();
                        TokenLocalService.ClearRefreshToken();
                        WindowLocalStorage.WriteLocalStorage("IsAuthenticated", "false");
                        WindowLocalStorage.WriteLocalStorage("Username", "");
                        WindowLocalStorage.WriteLocalStorage("Email", "");
                        WindowLocalStorage.WriteLocalStorage("Role", "");
                    }
                }
                return responseContent;
            }
            return null;
        }

        public async Task<string?> SignUp(string email, string password, string username)
        {
            string requestBody = JsonConvert.SerializeObject(new { email, password, username });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, ApiUr + SIGN_UP_ENDPOINT);
            request.Content = content;
            HttpResponseMessage response = await HttpClient.SendAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic? responseObject = JsonConvert.DeserializeObject(responseContent);

                if(responseObject != null)
                {
                    AccessToken = responseObject.accessToken;
                    RefreshToken = responseObject.refreshToken;
                    if(TokenLocalService != null)
                    {
                        TokenLocalService.SetAccessToken(AccessToken);
                        TokenLocalService.SetRefreshToken(RefreshToken);
                    }
                }
                return responseContent;
            }
            return null;
        }

        public async Task<string?> Refresh()
        {
            HttpClient.DefaultRequestHeaders.Add(API_HEADER, UUID);
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", RefreshToken);
            var response = HttpClient.GetAsync(ApiUr + REFRESH_ENDPOINT).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic? responseObject = JsonConvert.DeserializeObject(responseContent);

                if(responseObject != null)
                {
                    RefreshToken = responseObject.accessToken;
                    if(TokenLocalService != null)
                    {
                        TokenLocalService.SetRefreshToken(RefreshToken);
                    }
                }
                return "refreshed_success";
            }
            else
            {
                await SignOut();                
                return "signed_out";
            }
        }

        public void RefreshTokens()
        {
            AccessToken = TokenLocalService?.GetAccessToken();
            RefreshToken = TokenLocalService?.GetRefreshToken();
        }
    }
}
