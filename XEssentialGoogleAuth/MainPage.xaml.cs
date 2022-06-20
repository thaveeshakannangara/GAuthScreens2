using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using XEssentialGoogleAuth.MyConstants;

namespace XEssentialGoogleAuth
{
    public partial class MainPage : ContentPage
    {
        //string authenticationUrl = "http://localhost:38604/mobileauth/";
        string authenticationUrl = "http://gauthtest.azurewebsites.net/mobileauth/";
        private JsonSerializer _serializer = new JsonSerializer();

        private string _AuthToken;
        public string AuthToken
        {
            get => _AuthToken;
            set
            {
                if (value == _AuthToken) return;
                _AuthToken = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }


       async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            string scheme = "Google";

            try
            {
                WebAuthenticatorResult r = null;

                if (scheme.Equals("Apple")
                    && DeviceInfo.Platform == DevicePlatform.iOS
                    && DeviceInfo.Version.Major >= 13)
                {
                    r = await AppleSignInAuthenticator.AuthenticateAsync();
                }
                else if(scheme.Equals("Google"))
                {
                    var authUrl = new Uri(authenticationUrl + scheme);
                    var callbackUrl = new Uri("xamarinessentials://");

                    r = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);

                }

                AuthToken = r?.AccessToken ?? r?.IdToken;
                GetUserInfoUsingToken(AuthToken);
            }
            catch (Exception ex)
            {
                AuthToken = string.Empty;

                await App.Current.MainPage.DisplayAlert("Alert",ex.Message,"Ok");
            }
        }

        private async void GetUserInfoUsingToken(string authToken)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
            var httpResponseMessage = await httpClient.GetAsync("tokeninfo?access_token=" + authToken);
            using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                var jsoncontent = _serializer.Deserialize<GoogleResponseModel>(json);
                Preferences.Set("UserToken", authToken);
                //Not  a best way to save auth token and check if authtoken has expired insted try implementing refresh token
                await Navigation.PushAsync(new MyDashboardPage(jsoncontent, authToken));
            }

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if(!string.IsNullOrEmpty(Preferences.Get("UserToken","")))
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
                var httpResponseMessage = await httpClient.GetAsync("tokeninfo?access_token=" + Preferences.Get("UserToken", ""));
                using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    var jsoncontent = _serializer.Deserialize<GoogleResponseModel>(json);
                    //Not  a best way to save auth token and check if authtoken has expired insted try implementing refresh token

                    if (jsoncontent.access_type == "online")
                    {
                        App.Current.MainPage = new NavigationPage(new MyDashboardPage(jsoncontent, string.Empty));
                    }
                    else
                    {
                        return;
                    }
                  
                }

            }
     
        }

    }
}
