using Plugin.HybridWebView.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaaSArcGISIOS.LoginView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginSplashPage : ContentPage
    {
        public LoginSplashPage()
        {
            InitializeComponent();
            string _destpath = System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "AccelCal.csv");
            try
            {
                string _sourcepath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AccelCal.csv");
                if (System.IO.File.Exists(_sourcepath))
                {
                    System.IO.File.Copy(_sourcepath, _destpath, true);
                }
            }
            catch
            {

            }
            finally
            {
                System.IO.File.Copy("AccelCal.csv", _destpath, true);
            }
        }

        void SignUpButton_Clicked(System.Object sender, System.EventArgs e)
        {
            AsyncOpenBroswer(Common.Constant.mSignUpApiWebPage);
        }

        private async void AsyncOpenBroswer(string pBroswerAddress)
        {
            await Xamarin.Essentials.Browser.OpenAsync(pBroswerAddress);
        }

        void GoogleLoginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new LoginWebPage();
        }

        void KakaoLoginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new LoginWebPage(new Common.OAuth2KakaoProvider());
        }

        void NaverLoginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new LoginWebPage(new Common.OAuth2NaverProvider());
        }

        private void Webviews_OnNavigationError(object sender, int e)
        {
        }

        private void Webviews_OnNavigationCompleted(object sender, string e)
        {
        }

        private void Webviews_OnNavigationStarted(object sender, Plugin.HybridWebView.Shared.Delegates.DecisionHandlerDelegate e)
        {
        }

        private void Webviews_OnContentLoaded(object sender, EventArgs e)
        {
        }
    }
}