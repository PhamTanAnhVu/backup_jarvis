using Jarvis_Windows.Sources.Utils.Core;

namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public Token()
        {
            AccessToken = "";
            RefreshToken = "";
        }

        public Token(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
