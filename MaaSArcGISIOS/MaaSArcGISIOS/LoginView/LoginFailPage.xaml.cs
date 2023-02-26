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
    public partial class LoginFailPage : ContentPage
    {
        public LoginFailPage(string pErrorMsg)
        {
            InitializeComponent();

            xErrorMessageLabel.Text = pErrorMsg;
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new LoginView.LoginSplashPage();
        }
    }
}