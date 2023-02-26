using System;
using System.Collections.Generic;

namespace MaaSArcGISIOS.Common
{
    public class Constant
    {
        public const string kApiHost = "http://59.86.222.60:8080";
        // public const string kApiHost = "http://maas.saeaict.com";
        //public const string kApiHost = "http://home.showmethecode.info:5000";    
        //public const string kApiHost = "https://home.showmethecode.info:5001";
        //public const string kApiHost = "http://115.85.183.12";
        public const string kMapPath = "/map";
        public const string kApiKakao = "/api/oauth/kakao";
        public const string kApiKakaoCode = "api/oauth/kakao/code";
        public const string kApiKakaoLogin = "/api/oauth/kakao/login";
        public const string kApiKakaoauthorize = "/oauth/authorize";
        public const string kApiNaver = "/api/oauth/naver";
        public const string kApiNaverCode = "/api/oauth/naver/code";
        public const string kApiNaverLogin = "/api/oauth/naver/login";
        public const string kApiEnvironPressure = "/api/rawdata/environ/pressure";
        public const string kApiEnvironTemperature = "/api/rawdata/environ/temperature";
        public const string kApiMPURaw = "/api/rawdata/mpu/raw";
        public const string kApiRadarPDAT = "/api/rawdata/radar/pdat";
        public const string kApiRadarTDAT = "/api/rawdata/radar/tdat";

        public static readonly int mDefualtPageTick = 25;

        public static readonly int mMPUDataCollectSize = 30;
        public static readonly float mMPUGravityValue = 9.80665f;

        public static readonly double mPDatDistanceValue = 1.0;
        public static readonly double mPDatAngleValue = 1.0;

        public const string mBluetoothDeviceName = "SM nRF52832";
        public const string mHttpRestApiHost = "https://59.86.222.61:8087/";
        public const string mSignUpApiWebPage = "https://maas.saeaict.com:8087";
        public const string mGoogleMapAltitudeAPIKey = "AIzaSyAw0ef_rfUZ2nxhILs4HjvtzuTvZ_qt5T4";
        public const string mVworldAPIKey = "A1803118-5D91-3A27-B8BF-4A204225FEE3";

        public static Dictionary<string, Tuple<double, double>> mWeatherSiteDict;

        public static Dictionary<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType, string> mDirectionManeuverDict;

        public static ShellCurrentStatus mDefaultPageStatus = ShellCurrentStatus.Defualt;
        public static ShellCurrentStatus mNavigatePageStatus = ShellCurrentStatus.Defualt;

        public static int[] mPdatRandomArray =
        {
            0,0,0,0,0,0,0,0,0,0,
            1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,
            2,2,2,2,2,2,2,2,2,2,
            0,0,0,0,0,0,0,0,0,0,
            1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,
            2,2,2,2,2,2,2,2,2,2,
            1,1,1,1,1,3,4,3,5,3,
            0,0,0,0,0,0,0,0,0,0,
        };

        public static int[] mMPURandomArray =
{
            0,0,0,0,0,0,0,0,0,0,
            1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,
            2,2,2,2,2,3,3,3,3,3,
            0,0,0,0,0,0,0,0,0,0,
            1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,
            2,2,2,2,2,4,4,4,7,10,
            1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,
        };


        // Environment Service UUID
        public static Guid EnvServiceId = Guid.Parse("c0680200-7626-447d-8a60-be2588632de5");
        public static Guid EnvPressCharId = Guid.Parse("c0680201-7626-447d-8a60-be2588632de5"); // Environment Pressure characteristic
        public static Guid EnvTempCharId = Guid.Parse("c0680202-7626-447d-8a60-be2588632de5"); // Environment Temperature characteristic
        public static Guid EnvContCharId = Guid.Parse("c068020F-7626-447d-8a60-be2588632de5"); // Environment controller characteristic

        // Environment Service UUID
        public static Guid MPUServiceId = Guid.Parse("c0680300-7626-447d-8a60-be2588632de5");
        public static Guid MPURawCharId = Guid.Parse("c0680301-7626-447d-8a60-be2588632de5"); // MPU9250 Raw characteristic
        public static Guid MPUContCharId = Guid.Parse("c068030f-7626-447d-8a60-be2588632de5"); // MPU9250 controller characteristic

        // K-LD7 Radar Service UUID
        public static Guid RadarServiceId = Guid.Parse("c0680400-7626-447d-8a60-be2588632de5");
        public static Guid RdrPdatCharId = Guid.Parse("c0680401-7626-447d-8a60-be2588632de5"); // Radar PDAT characteristic
        public static Guid RdrTdatCharId = Guid.Parse("c0680402-7626-447d-8a60-be2588632de5"); // Radar TDAT characteristic
        public static Guid RdrContCharId = Guid.Parse("c068040f-7626-447d-8a60-be2588632de5");

        //Battery
        public static Guid UserServiceId = Guid.Parse("c0680500-7626-447d-8a60-be2588632de5");
        public static Guid BatteryServiceId = Guid.Parse("c0680501-7626-447d-8a60-be2588632de5");

        // GPS Service UUID
        //public static Guid GPSServiceId = Guid.Parse("C0680500-7626-447D-8A60-BE2588632DE5");
        //public static Guid GPSRMCCharId = Guid.Parse("C0680501-7626-447D-8A60-BE2588632DE5"); // GPS RMC characteristic
        //public static Guid GPSContCharId = Guid.Parse("C068050F-7626-447D-8A60-BE2588632DE5"); // GPS RMC characteristic

        // User Service UUID
        //public static Guid UserServiceId = Guid.Parse("C0680600-7626-447D-8A60-BE2588632DE5");
        //public static Guid UserButtonCharId = Guid.Parse("C0680601-7626-447D-8A60-BE2588632DE5"); // User Button characteristic
        //public static Guid UserLEDCharId = Guid.Parse("C0680602-7626-447D-8A60-BE2588632DE5"); // User LED characteristic

        public enum ShellCurrentStatus
        {
            Defualt = 0,
            StartPath = 1,
            EndPath = 2,
            Navi = 3,
        }

        public enum LoginReulstStatus
        {
            None = 0,
            Error = 1,
            Fail = 2,
            Success = 3,
        }

        public enum OauthProvdierEnum
        {
            None = -1,
            Google = 0,
            Kakao,
            Naver,
        }

        public enum SettingFileEnum
        {
            None = -1,
            DefaultView = 0,
            NaviationView,
            WarningSetting,
        }

        public enum BluetoothStatus
        {
            None = -1,
            Disconnect,
            Disconnecting,
            Connecting,
            Connection,
        }

        public static void InitDirectionManeuver()
        {
            mDirectionManeuverDict = new Dictionary<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType, string>();

            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Unknown, "");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Stop, "도착");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Straight, "직진");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.BearLeft, "왼쪽길");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.BearRight, "오른쪽길");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TurnLeft, "좌회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TurnRight, "우회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.SharpLeft, "깊게 좌회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.SharpRight, "깊게 우회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.UTurn, "유턴");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Ferry, "건넘");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Roundabout, "원형 교차로");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.HighwayMerge, "고속도로 병합");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.HighwayExit, "고속도로 퇴장");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.HighwayChange, "고속도로 변경");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.ForkCenter, "중앙 유지");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.ForkLeft, "좌회전 유지");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.ForkRight, "우회전 유지");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Depart, "출발");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TripItem, "TripItem");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.EndOfFerry, "끝에서건넘");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.RampRight, "우회전 경사");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.RampLeft, "좌회전 경사");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TurnLeftRight, "좌회전 후 우회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TurnRightLeft, "우회전 후 좌회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TurnRightRight, "우회전 후 우회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.TurnLeftLeft, "좌회전 후 좌회전");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.PedestrianRamp, "보행자 주의");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Elevator, "Elevator");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Escalator, "Escalator");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Stairs, "Stairs");
            mDirectionManeuverDict.Add(Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.DoorPassage, "DoorPassage");
        }

        public static void InitWeatherSite()
        {
            mWeatherSiteDict = new Dictionary<string, Tuple<double, double>>();

            //제주도
            mWeatherSiteDict.Add("고산", new Tuple<double, double>(33.3027435, 126.181314));
            mWeatherSiteDict.Add("서귀포", new Tuple<double, double>(33.253925, 126.5597875));
            mWeatherSiteDict.Add("성산", new Tuple<double, double>(33.4420963, 126.9109642));
            mWeatherSiteDict.Add("제주", new Tuple<double, double>(33.4273366, 126.5758344));

            //강원도
            mWeatherSiteDict.Add("강릉", new Tuple<double, double>(37.74913611, 128.8784972));
            mWeatherSiteDict.Add("대관령", new Tuple<double, double>(37.673628, 128.7060956));
            mWeatherSiteDict.Add("동해", new Tuple<double, double>(37.5247583, 129.1142625));
            mWeatherSiteDict.Add("북강릉", new Tuple<double, double>(37.835074, 128.814373));
            mWeatherSiteDict.Add("북춘천", new Tuple<double, double>(37.906620, 127.739345));
            mWeatherSiteDict.Add("속초", new Tuple<double, double>(38.207169, 128.59184));
            mWeatherSiteDict.Add("영월", new Tuple<double, double>(37.183774, 128.46185));
            mWeatherSiteDict.Add("원주", new Tuple<double, double>(37.3423179, 127.9199688));
            mWeatherSiteDict.Add("인제", new Tuple<double, double>(38.069732, 128.170352));
            mWeatherSiteDict.Add("정선군", new Tuple<double, double>(37.380609, 128.660871));
            mWeatherSiteDict.Add("철원", new Tuple<double, double>(38.146861, 127.313472));
            mWeatherSiteDict.Add("춘천", new Tuple<double, double>(37.8813739, 127.7300034));
            mWeatherSiteDict.Add("태백", new Tuple<double, double>(37.164132, 128.985735));
            mWeatherSiteDict.Add("홍천", new Tuple<double, double>(37.697207, 127.888518));

            //충정도
            mWeatherSiteDict.Add("금산", new Tuple<double, double>(36.108857, 127.488213));
            mWeatherSiteDict.Add("대전", new Tuple<double, double>(36.3504396, 127.3849508));
            mWeatherSiteDict.Add("보령", new Tuple<double, double>(36.333452, 126.612803));
            mWeatherSiteDict.Add("보은", new Tuple<double, double>(36.489455, 127.729485));
            mWeatherSiteDict.Add("부여", new Tuple<double, double>(36.275658, 126.909775));
            mWeatherSiteDict.Add("서산", new Tuple<double, double>(36.7849216, 126.4502766));
            mWeatherSiteDict.Add("세종", new Tuple<double, double>(36.4803512, 127.2894325));
            mWeatherSiteDict.Add("제천", new Tuple<double, double>(37.132646, 128.191037));
            mWeatherSiteDict.Add("천안", new Tuple<double, double>(36.815147, 127.113892));
            mWeatherSiteDict.Add("청주", new Tuple<double, double>(36.6424987, 127.488975));
            mWeatherSiteDict.Add("추풍령", new Tuple<double, double>(36.2145727, 127.9986835));
            mWeatherSiteDict.Add("충주", new Tuple<double, double>(36.991105, 127.926012));
            mWeatherSiteDict.Add("홍성", new Tuple<double, double>(36.601324, 126.660775));

            //전라도
            mWeatherSiteDict.Add("강진군", new Tuple<double, double>(34.64209, 126.7672));
            mWeatherSiteDict.Add("고창", new Tuple<double, double>(35.435836, 126.701973)); //고창군
            mWeatherSiteDict.Add("고창군", new Tuple<double, double>(35.514183, 126.674088)); //고창군 부안면
            mWeatherSiteDict.Add("고흥", new Tuple<double, double>(34.6047049, 127.275507));
            mWeatherSiteDict.Add("광양시", new Tuple<double, double>(34.9406575, 127.6958987));
            mWeatherSiteDict.Add("광주", new Tuple<double, double>(35.160032, 126.851338));
            mWeatherSiteDict.Add("군산", new Tuple<double, double>(35.9676263, 126.736875));
            mWeatherSiteDict.Add("남원", new Tuple<double, double>(35.416432, 127.390438));
            mWeatherSiteDict.Add("목포", new Tuple<double, double>(34.811875, 126.3923375));
            mWeatherSiteDict.Add("보성군", new Tuple<double, double>(34.771458, 127.080088));
            mWeatherSiteDict.Add("순창군", new Tuple<double, double>(35.374476, 127.137489));
            mWeatherSiteDict.Add("순천", new Tuple<double, double>(34.9506984, 127.487243));
            mWeatherSiteDict.Add("여수", new Tuple<double, double>(34.760425, 127.662313));
            mWeatherSiteDict.Add("영광군", new Tuple<double, double>(35.2772127, 126.5120143));
            mWeatherSiteDict.Add("완도", new Tuple<double, double>(34.3110391, 126.7548524));
            mWeatherSiteDict.Add("임실", new Tuple<double, double>(35.6178286, 127.2890774));
            mWeatherSiteDict.Add("장수", new Tuple<double, double>(34.5903, 127.3182));
            mWeatherSiteDict.Add("장흥", new Tuple<double, double>(34.681622, 126.9070507));
            mWeatherSiteDict.Add("전주", new Tuple<double, double>(35.824171, 127.14805));
            mWeatherSiteDict.Add("정읍", new Tuple<double, double>(35.569867, 126.856038));
            mWeatherSiteDict.Add("해남", new Tuple<double, double>(34.573558, 126.599225));
            mWeatherSiteDict.Add("흑산도", new Tuple<double, double>(34.6829792, 125.4273709));

            //경상도
            mWeatherSiteDict.Add("거제", new Tuple<double, double>(34.8804572, 128.6211703));
            mWeatherSiteDict.Add("거창", new Tuple<double, double>(35.686698, 127.909538));
            mWeatherSiteDict.Add("경주시", new Tuple<double, double>(35.856242, 129.224784));
            mWeatherSiteDict.Add("구미", new Tuple<double, double>(36.1195987, 128.3443));
            mWeatherSiteDict.Add("김해시", new Tuple<double, double>(35.228574, 128.889322));
            mWeatherSiteDict.Add("남해", new Tuple<double, double>(34.837707, 127.892475));
            mWeatherSiteDict.Add("대구", new Tuple<double, double>(35.87139, 128.601763));
            mWeatherSiteDict.Add("문경", new Tuple<double, double>(36.586522, 128.186787));
            mWeatherSiteDict.Add("밀양", new Tuple<double, double>(35.503856, 128.746712));
            mWeatherSiteDict.Add("봉화", new Tuple<double, double>(36.893114, 128.732503));
            mWeatherSiteDict.Add("부산", new Tuple<double, double>(35.179816, 129.0750223));
            mWeatherSiteDict.Add("북창원", new Tuple<double, double>(35.2278771, 128.6818746));
            mWeatherSiteDict.Add("산청", new Tuple<double, double>(35.415557, 127.873458));
            mWeatherSiteDict.Add("상주", new Tuple<double, double>(36.411002, 128.159229));
            mWeatherSiteDict.Add("안동", new Tuple<double, double>(36.568425, 128.7295375));
            mWeatherSiteDict.Add("양산시", new Tuple<double, double>(35.335049, 129.037339));
            mWeatherSiteDict.Add("영덕", new Tuple<double, double>(36.415034, 129.365267));
            mWeatherSiteDict.Add("영주", new Tuple<double, double>(36.805667, 128.624063));
            mWeatherSiteDict.Add("영천", new Tuple<double, double>(35.97326, 128.938613));
            mWeatherSiteDict.Add("울릉도", new Tuple<double, double>(37.484455, 130.905697));
            mWeatherSiteDict.Add("울산", new Tuple<double, double>(35.5394773, 129.3112994));
            mWeatherSiteDict.Add("울진", new Tuple<double, double>(36.993087, 129.400394));
            mWeatherSiteDict.Add("의령군", new Tuple<double, double>(35.3222239, 128.261676));
            mWeatherSiteDict.Add("의성", new Tuple<double, double>(36.3527158, 128.6971711));
            mWeatherSiteDict.Add("진주", new Tuple<double, double>(35.180325, 128.107646));
            mWeatherSiteDict.Add("창원", new Tuple<double, double>(35.2278771, 128.6818746));
            mWeatherSiteDict.Add("청송군", new Tuple<double, double>(36.4362793, 129.0571263));
            mWeatherSiteDict.Add("통영", new Tuple<double, double>(34.85439, 128.433112));
            mWeatherSiteDict.Add("포항", new Tuple<double, double>(36.0190333, 129.3433898));
            mWeatherSiteDict.Add("함양군", new Tuple<double, double>(35.520536, 127.725245));
            mWeatherSiteDict.Add("합천", new Tuple<double, double>(35.56666, 128.165799));

            //경기,서울,인천
            mWeatherSiteDict.Add("강화", new Tuple<double, double>(37.746498, 126.488052));
            mWeatherSiteDict.Add("동두천", new Tuple<double, double>(37.903662, 127.060671));
            mWeatherSiteDict.Add("백령도", new Tuple<double, double>(37.959042, 124.665370));
            mWeatherSiteDict.Add("서울", new Tuple<double, double>(37.566531, 126.977967));
            mWeatherSiteDict.Add("수원", new Tuple<double, double>(37.263476, 127.028646));
            mWeatherSiteDict.Add("양평", new Tuple<double, double>(37.491791, 127.487597));
            mWeatherSiteDict.Add("이천", new Tuple<double, double>(37.272342, 127.435034));
            mWeatherSiteDict.Add("인천", new Tuple<double, double>(37.4559418, 126.7051505));
            mWeatherSiteDict.Add("파주", new Tuple<double, double>(37.760186, 126.779883));
        }
    }
}
