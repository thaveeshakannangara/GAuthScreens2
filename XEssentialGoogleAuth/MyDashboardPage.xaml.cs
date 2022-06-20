using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using XEssentialGoogleAuth.MyConstants;

namespace XEssentialGoogleAuth
{
    public partial class MyDashboardPage : ContentPage
    {
        public MyDashboardPage(GoogleResponseModel model, string token)
        {
            InitializeComponent();
            LoggedinUserEmail.Text = model.email;
            LoggedinUserToken.Text = token;
        }


        async void Button_Clicked(object sender, EventArgs args)
        {
            Preferences.Clear();
            
        }
    }
}
