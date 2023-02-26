using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MaaSArcGISIOS.Common;
using static MaaSArcGISIOS.Model.VworldPlacePoi;
using WebKit;
using System.Drawing;
using SafariServices;
using UIKit;
using Xamarin.Auth;
using Accounts;
using Esri.ArcGISRuntime.Mapping;
using System.Net;
using System.Diagnostics;
using Plugin.HybridWebView.Shared;

namespace MaaSArcGISIOS.LoginView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginWebPage : ContentPage
    {
        private Model.IOAuth2Provider mOauthProvider;
        private Data.SingletonData mSharedData;

        private const string mClientID = "204175858581-0dsn1sv4khjkfjn0r75t3qh1tlq2a2fb.apps.googleusercontent.com";
        private const string mClientSecret = "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";
        private const string mScope = "openid profile email";
        private const string mFlowName = "GeneralOAuthFlow";
        private int count = 0;

        public LoginWebPage(Model.IOAuth2Provider pOauthProvider)
        {
            InitializeComponent();
            count = 0;
            mOauthProvider = pOauthProvider;
            mSharedData = Data.SingletonData.mInstance;

            Console.WriteLine("URL : {0}", mOauthProvider.GetDefaultURL());

            try
            {
                xWebView.Source = mOauthProvider.GetDefaultURL();
                xWebView.OnNavigationStarted += XWebView_OnNavigationStarted;
            }
            catch(Exception ets)
            {

            }
        }

        private void XWebView_OnNavigationStarted(object sender, Plugin.HybridWebView.Shared.Delegates.DecisionHandlerDelegate e)
        {
            if (e.Uri != null)
            {
                if (e.Uri == mOauthProvider.GetDefaultURL())
                {
                    if (count > 0)
                    {
                        this.Dispatcher.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage = new LoginSplashPage();
                        });
                    }
                    count++;
                }
                else
                {
                    if (e.Uri.Contains("signin-google?code") || e.Uri.Contains("signin-kakao?code") || e.Uri.Contains("signin-naver?code"))
                    {
                        xWebView.IsVisible = false;
                        mOauthProvider.GetProviderKey(e.Uri, OauthResultCallBack);
                        xWaitLabel.IsVisible = true;
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                this.Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage = new LoginFailPage("");
                });
            }
        }

        public LoginWebPage()
        {
            InitializeComponent();

            mSharedData = Data.SingletonData.mInstance;
        }


        private void OauthResultCallBack(Constant.OauthProvdierEnum pType, string pProviderKey)
        {
            if (pProviderKey == null)
            {
                App.Current.MainPage = new LoginFailPage("로그인 권한 인증 실패!");
            }
            else
            {
                switch (pType)
                {
                    case Constant.OauthProvdierEnum.Google:
                        {
                            var result = Common.MSSQLProvider.LoginResult("Google", pProviderKey);
                            if (result.Item1 == Constant.LoginReulstStatus.Success)
                            {
                                mSharedData.mOauthUserInfo = new Model.LoginUserOauth
                                {
                                    Providerkey = pProviderKey,
                                    LoginProvider = "Google",
                                };
                                this.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    App.Current.MainPage = new MainPage();
                                });
                            }
                            else
                            {
                                this.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    App.Current.MainPage = new LoginFailPage(result.Item2);
                                });
                            }
                        }
                        break;
                    case Constant.OauthProvdierEnum.Kakao:
                        {
                            var result = Common.MSSQLProvider.LoginResult("KakaoTalk", pProviderKey);
                            if (result.Item1 == Constant.LoginReulstStatus.Success)
                            {
                                mSharedData.mOauthUserInfo = new Model.LoginUserOauth
                                {
                                    Providerkey = pProviderKey,
                                    LoginProvider = "KakaoTalk",
                                };
                                this.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    App.Current.MainPage = new MainPage();
                                });
                            }
                            else
                            {
                                this.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    App.Current.MainPage = new LoginFailPage(result.Item2);
                                });
                            }
                        }
                        break;
                    case Constant.OauthProvdierEnum.Naver:
                        {
                            var result = Common.MSSQLProvider.LoginResult("Naver", pProviderKey);
                            if (result.Item1 == Constant.LoginReulstStatus.Success)
                            {
                                mSharedData.mOauthUserInfo = new Model.LoginUserOauth
                                {
                                    Providerkey = pProviderKey,
                                    LoginProvider = "Naver",
                                };

                                this.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    App.Current.MainPage = new MainPage();
                                });
                            }
                            else
                            {
                                this.Dispatcher.BeginInvokeOnMainThread(() =>
                                {
                                    App.Current.MainPage = new LoginFailPage(result.Item2);
                                });
                            }
                        }

                        break;
                }
            }
        }
    }
}