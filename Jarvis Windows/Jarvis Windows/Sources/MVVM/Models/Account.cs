namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class Account
    {
        private string? _username;
        private string? _password;
        private string? _email;
        private string? _role;

        public Account()
        {
            _username = string.Empty;
            _password = string.Empty;
            _email = string.Empty;
            _role = string.Empty;
        }

        public Account(string username, string email, string role)
        {
            _username = username;
            _email = email;
            _role = role;
        }

        public string? Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string? Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string? Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string? Role
        {
            get { return _role; }
            set { _role = value; }
        }
    }
}
