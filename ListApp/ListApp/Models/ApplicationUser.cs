﻿using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ListApp.Models
{
    public class ApplicationUser : INotifyPropertyChanged
    {
        public static ApplicationUser Current { get; private set; } = new ApplicationUser();
        private string fullName;
        private string email;
        private string acessToken;
        private string refreshToken;
        private bool isLoggedIn;

        public ApplicationUser()
        {
            
        }

        public ApplicationUser(string fullName, string email, string acessToken, string refreshToken, bool isLoggedIn)
        {
            FullName = fullName;
            Email = email;
            AcessToken = acessToken;
            RefreshToken = refreshToken;
            IsLoggedIn = isLoggedIn;
        }

        public string FirstName { get => FullName?.Split(' ').FirstOrDefault() ?? string.Empty; }
        public string FullName { get => fullName;  set { fullName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FirstName)); } }
        public string Email { get => email;  set { email = value; OnPropertyChanged(); } }
        public string AcessToken { get => acessToken;  set { acessToken = value; OnPropertyChanged(); } }
        public string RefreshToken { get => refreshToken;  set { refreshToken = value; OnPropertyChanged(); } }
        public bool IsLoggedIn { get => isLoggedIn;  set { isLoggedIn = value; OnPropertyChanged(); } }

        public void Set(string fullName, string email, string accessToken, string refreshToken, bool isLoggedIn = true)
        {
            FullName = fullName;
            Email = email;
            AcessToken = accessToken;
            RefreshToken = refreshToken;
            IsLoggedIn = isLoggedIn;
        }

        public void Set(ApplicationUser applicationUser)
        {
            Current = applicationUser ?? new ApplicationUser();
        }

        public void Unset()
        {
            FullName = string.Empty;
            Email = string.Empty;
            AcessToken = string.Empty;
            RefreshToken = string.Empty;
            IsLoggedIn = false;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
