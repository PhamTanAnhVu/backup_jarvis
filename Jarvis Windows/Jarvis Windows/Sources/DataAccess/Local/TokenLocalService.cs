using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;

namespace Jarvis_Windows.Sources.DataAccess.Local
{
    public interface ITokenLocalService
    {
        #region New Handlers
        public Token GetTokens();
        public void SetTokens(Token? tokens);
        public void ClearTokens();
        #endregion
        public string? GetAccessToken();
        public void SetAccessToken(string accessToken);
        public void ClearAccessToken();
        public string? GetRefreshToken();
        public void SetRefreshToken(string refreshToken);
        public void ClearRefreshToken();
        public string? GetUUID();
        public void SetUUID(string uuid);
        public void ClearUUID();
        public bool HasToken();
    }

    public class TokenLocalService : ObserveralObject, ITokenLocalService
    {
        private readonly string ACCESS_TOKEN_KEY = "access_token";
        private readonly string REFRESH_TOKEN_KEY = "refresh_token";
        private readonly string UUID_KEY = "ClientID";

        public string? GetAccessToken()
        {
            return WindowLocalStorage.ReadLocalStorage(ACCESS_TOKEN_KEY);
        }

        public void SetAccessToken(string accessToken)
        {
            WindowLocalStorage.WriteLocalStorage(ACCESS_TOKEN_KEY, accessToken);
        }

        public string? GetRefreshToken()
        {
            return WindowLocalStorage.ReadLocalStorage(REFRESH_TOKEN_KEY);
        }

        public void SetRefreshToken(string refreshToken)
        {
            WindowLocalStorage.WriteLocalStorage(REFRESH_TOKEN_KEY, refreshToken);
        }

        public string? GetUUID()
        {
            string? uuid = WindowLocalStorage.ReadLocalStorage(UUID_KEY);
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = Guid.NewGuid().ToString();
                SetUUID(uuid);
            }
            return uuid;
        }

        public void SetUUID(string uuid)
        {
            WindowLocalStorage.WriteLocalStorage(UUID_KEY, uuid);
        }

        public bool HasToken()
        {
            return !string.IsNullOrEmpty(GetAccessToken()) 
                && !string.IsNullOrEmpty(GetRefreshToken());
        }

        public void ClearAccessToken()
        {
            WindowLocalStorage.RemoveLocalStorage(ACCESS_TOKEN_KEY);
        }

        public void ClearRefreshToken()
        {
            WindowLocalStorage.RemoveLocalStorage(REFRESH_TOKEN_KEY);
        }

        public void ClearUUID()
        {
            WindowLocalStorage.RemoveLocalStorage(UUID_KEY);
        }

        public Token GetTokens()
        {
            string accessToken = WindowLocalStorage.ReadLocalStorage(ACCESS_TOKEN_KEY);
            string refreshToken = WindowLocalStorage.ReadLocalStorage(REFRESH_TOKEN_KEY);
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                return new Token();

            return new Token(accessToken, refreshToken);
        }

        public void SetTokens(Token? tokens)
        {
            if(tokens != null && !string.IsNullOrEmpty(tokens.AccessToken) && !string.IsNullOrEmpty(tokens.RefreshToken))
            {
                WindowLocalStorage.WriteLocalStorage(ACCESS_TOKEN_KEY, tokens.AccessToken);
                WindowLocalStorage.WriteLocalStorage(REFRESH_TOKEN_KEY, tokens.RefreshToken);
            }
        }

        public void ClearTokens()
        {
            ClearAccessToken();
            ClearRefreshToken();
        }
    }
}
