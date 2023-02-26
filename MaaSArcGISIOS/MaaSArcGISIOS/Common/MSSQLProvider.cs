using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaaSArcGISIOS.Common
{
    public class MSSQLProvider
    {
        System.Data.SqlClient.SqlConnection mConnection;
        public MSSQLProvider()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder _Builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            _Builder.DataSource = "59.86.222.61";
            _Builder.InitialCatalog = "MaaS";
            _Builder.UserID = "SaeICTTest";
            _Builder.Password = "1q2w3e4r!@";
            _Builder.IntegratedSecurity = false;

            mConnection = new System.Data.SqlClient.SqlConnection(_Builder.ConnectionString);
        }

        public void CloseMSSQL()
        {
            if (mConnection != null)
            {
                mConnection.Close();
            }
        }

        public void InsertMSSQLData(string pQuery, Action<string> pCallBack)
        {
            if (mConnection == null)
                return;

            try
            {
                mConnection.Open();
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(pQuery, mConnection);
                sqlCommand.ExecuteNonQuery();
                mConnection.Close();
            }
            catch (Exception ets)
            {
                mConnection.Close();
                pCallBack(string.Format("{0},{1}", pQuery, ets.Message));
            }
        }
        public System.Collections.Generic.List<Xamarin.Essentials.Location> SelectMPU(bool pSimulation, Model.LoginUserOauth pUserOauth, Xamarin.Essentials.Location pStartLocation, Xamarin.Essentials.Location pEndLocation, double pThreshold)
        {
            if (mConnection == null)
                return null;
            try
            {
                mConnection.Open();

                System.Collections.Generic.List<Xamarin.Essentials.Location> _Result = new System.Collections.Generic.List<Xamarin.Essentials.Location>();

                double _LatitudeStart = 0.0;
                double _LatitudeEnd = 0.0;
                double _LongitudeStart = 0.0;
                double _LongitudeEnd = 0.0;
                if (Math.Round(pStartLocation.Latitude, 4) < Math.Round(pEndLocation.Latitude, 4))
                {
                    _LatitudeStart = Math.Round(pStartLocation.Latitude, 4) - 0.00005;
                    _LatitudeEnd = Math.Round(pEndLocation.Latitude, 4) + 0.00005;
                }
                else
                {
                    _LatitudeStart = Math.Round(pEndLocation.Latitude, 4) - 0.00005;
                    _LatitudeEnd = Math.Round(pStartLocation.Latitude, 4) + 0.00005;
                }
                if (Math.Round(pStartLocation.Longitude, 4) < Math.Round(pEndLocation.Longitude, 4))
                {
                    _LongitudeStart = Math.Round(pStartLocation.Longitude, 4) - 0.00005;
                    _LongitudeEnd = Math.Round(pEndLocation.Longitude, 4) + 0.00005;
                }
                else
                {
                    _LongitudeStart = Math.Round(pEndLocation.Longitude, 4) - 0.00005;
                    _LongitudeEnd = Math.Round(pStartLocation.Longitude, 4) + 0.00005;
                }


                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT TOP (50) ROUND(AVG([latitude]),4) as [latitude] , ROUND(AVG([longitude]),4) as [longitude] , COUNT([id]) as [total_count] FROM [MaaS].[dbo].[{0}] WHERE ([acc_x] >= {1} OR [acc_y] >= {1} or [acc_z] >= {1}) AND ([latitude] >= {2} AND [latitude] <= {3}) AND ([longitude] >= {4} AND [longitude] <= {5}) GROUP BY ROUND([latitude],4), ROUND([longitude],4) ORDER BY total_count DESC"
                    , pSimulation == true ? "SMpuRaw" : "MpuRaw"
                    , pThreshold
                    , _LatitudeStart
                    , _LatitudeEnd
                    , _LongitudeStart
                    , _LongitudeEnd);
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), mConnection);
                var dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    _Result.Add(new Xamarin.Essentials.Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"])));
                }
                mConnection.Close();
                return _Result;
            }
            catch
            {
                mConnection.Close();
                return null;
            }
        }

        public System.Collections.Generic.List<Xamarin.Essentials.Location> SelectAngle(bool pSimulation, Model.LoginUserOauth pUserOauth, Xamarin.Essentials.Location pStartLocation, Xamarin.Essentials.Location pEndLocation, double pThreshold)
        {
            if (mConnection == null)
                return null;
            try
            {
                mConnection.Open();
                System.Collections.Generic.List<Xamarin.Essentials.Location> _Result = new System.Collections.Generic.List<Xamarin.Essentials.Location>();

                double _LatitudeStart = 0.0;
                double _LatitudeEnd = 0.0;
                double _LongitudeStart = 0.0;
                double _LongitudeEnd = 0.0;
                if (Math.Round(pStartLocation.Latitude, 4) < Math.Round(pEndLocation.Latitude, 4))
                {
                    _LatitudeStart = Math.Round(pStartLocation.Latitude, 4) - 0.00005;
                    _LatitudeEnd = Math.Round(pEndLocation.Latitude, 4) + 0.00005;
                }
                else
                {
                    _LatitudeStart = Math.Round(pEndLocation.Latitude, 4) - 0.00005;
                    _LatitudeEnd = Math.Round(pStartLocation.Latitude, 4) + 0.00005;
                }
                if (Math.Round(pStartLocation.Longitude, 4) < Math.Round(pEndLocation.Longitude, 4))
                {
                    _LongitudeStart = Math.Round(pStartLocation.Longitude, 4) - 0.00005;
                    _LongitudeEnd = Math.Round(pEndLocation.Longitude, 4) + 0.00005;
                }
                else
                {
                    _LongitudeStart = Math.Round(pEndLocation.Longitude, 4) - 0.00005;
                    _LongitudeEnd = Math.Round(pStartLocation.Longitude, 4) + 0.00005;
                }

                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT TOP (50) ROUND(AVG([latitude]),4) as [latitude] , ROUND(AVG([longitude]),4) as [longitude] , COUNT([id]) as [total_count] FROM [MaaS].[dbo].[{0}] WHERE (ABS([Inclination]) >= {1}) AND ([latitude] >= {2} AND [latitude] <= {3}) AND ([longitude] >= {4} AND [longitude] <= {5}) GROUP BY ROUND([latitude],4), ROUND([longitude],4) ORDER BY total_count DESC"
                    , pSimulation == true ? "SEnvironPressure" : "EnvironPressure"
                    , pThreshold
                    , _LatitudeStart
                    , _LatitudeEnd
                    , _LongitudeStart
                    , _LongitudeEnd);

                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), mConnection);
                var dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    _Result.Add(new Xamarin.Essentials.Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"])));
                }
                mConnection.Close();
                return _Result;
            }
            catch
            {
                mConnection.Close();
                return null;
            }
        }

        public System.Collections.Generic.List<Xamarin.Essentials.Location> SelectRadar(bool pSimulation, Model.LoginUserOauth pUserOauth, Xamarin.Essentials.Location pStartLocation, Xamarin.Essentials.Location pEndLocation, double pThreshold)
        {
            if (mConnection == null)
                return null;
            try
            {
                mConnection.Open();

                System.Collections.Generic.List<Xamarin.Essentials.Location> _Result = new System.Collections.Generic.List<Xamarin.Essentials.Location>();

                double _LatitudeStart = 0.0;
                double _LatitudeEnd = 0.0;
                double _LongitudeStart = 0.0;
                double _LongitudeEnd = 0.0;
                if (Math.Round(pStartLocation.Latitude, 4) < Math.Round(pEndLocation.Latitude, 4))
                {
                    _LatitudeStart = Math.Round(pStartLocation.Latitude, 4) - 0.00005;
                    _LatitudeEnd = Math.Round(pEndLocation.Latitude, 4) + 0.00005;
                }
                else
                {
                    _LatitudeStart = Math.Round(pEndLocation.Latitude, 4) - 0.00005;
                    _LatitudeEnd = Math.Round(pStartLocation.Latitude, 4) + 0.00005;
                }
                if (Math.Round(pStartLocation.Longitude, 4) < Math.Round(pEndLocation.Longitude, 4))
                {
                    _LongitudeStart = Math.Round(pStartLocation.Longitude, 4) - 0.00005;
                    _LongitudeEnd = Math.Round(pEndLocation.Longitude, 4) + 0.00005;
                }
                else
                {
                    _LongitudeStart = Math.Round(pEndLocation.Longitude, 4) - 0.00005;
                    _LongitudeEnd = Math.Round(pStartLocation.Longitude, 4) + 0.00005;
                }


                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT TOP (50) ROUND(AVG([latitude]),4) as [latitude] , ROUND(AVG([longitude]),4) as [longitude] , COUNT([id]) as [total_count] FROM [MaaS].[dbo].[{0}] WHERE ([ObjectsCount] >= {1}) AND ([latitude] >= {2} AND [latitude] <= {3}) AND ([longitude] >= {4} AND [longitude] <= {5}) GROUP BY ROUND([latitude],4), ROUND([longitude],4) ORDER BY total_count DESC"
                    , pSimulation == true ? "SRadarPdat" : "RadarPdat"
                    , pThreshold
                    , _LatitudeStart
                    , _LatitudeEnd
                    , _LongitudeStart
                    , _LongitudeEnd);
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), mConnection);
                var dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    _Result.Add(new Xamarin.Essentials.Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"])));
                }
                mConnection.Close();
                return _Result;
            }
            catch
            {
                mConnection.Close();
                return null;
            }
        }

        public System.Collections.Generic.List<Xamarin.Essentials.Location> SelectMPUFrontWarning(bool pSimulation, Model.LoginUserOauth pUserOauth, Xamarin.Essentials.Location pStartLocation, Xamarin.Essentials.Location pEndLocation, double pThreshold)
        {
            if (mConnection == null)
                return null;
            try
            {
                mConnection.Open();

                System.Collections.Generic.List<Xamarin.Essentials.Location> _Result = new System.Collections.Generic.List<Xamarin.Essentials.Location>();

                double _LatitudeStart = 0.0;
                double _LatitudeEnd = 0.0;
                double _LongitudeStart = 0.0;
                double _LongitudeEnd = 0.0;
                if (Math.Round(pStartLocation.Latitude, 5) < Math.Round(pEndLocation.Latitude, 5))
                {
                    _LatitudeStart = Math.Round(pStartLocation.Latitude, 5) - 0.05;
                    _LatitudeEnd = Math.Round(pEndLocation.Latitude, 5) + 0.05;
                }
                else
                {
                    _LatitudeStart = Math.Round(pEndLocation.Latitude, 5) - 0.05;
                    _LatitudeEnd = Math.Round(pStartLocation.Latitude, 5) + 0.05;
                }
                if (Math.Round(pStartLocation.Longitude, 5) < Math.Round(pEndLocation.Longitude, 5))
                {
                    _LongitudeStart = Math.Round(pStartLocation.Longitude, 5) - 0.05;
                    _LongitudeEnd = Math.Round(pEndLocation.Longitude, 5) + 0.05;
                }
                else
                {
                    _LongitudeStart = Math.Round(pEndLocation.Longitude, 5) - 0.05;
                    _LongitudeEnd = Math.Round(pStartLocation.Longitude, 5) + 0.05;
                }


                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT ROUND(AVG([latitude]),5) as [latitude] , ROUND(AVG([longitude]),5) as [longitude] , COUNT([id]) as [total_count] FROM [MaaS].[dbo].[{0}] WHERE ([acc_x] >= {1} OR [acc_y] >= {1} or [acc_z] >= {1}) AND ([latitude] >= {2} AND [latitude] <= {3}) AND ([longitude] >= {4} AND [longitude] <= {5}) GROUP BY ROUND([latitude],5), ROUND([longitude],5)"
                    , pSimulation == true ? "SMpuRaw" : "MpuRaw"
                    , pThreshold
                    , _LatitudeStart
                    , _LatitudeEnd
                    , _LongitudeStart
                    , _LongitudeEnd);
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), mConnection);
                var dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    _Result.Add(new Xamarin.Essentials.Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"])));
                }
                mConnection.Close();
                return _Result;
            }
            catch
            {
                mConnection.Close();
                return null;
            }
        }

        public System.Collections.Generic.List<Xamarin.Essentials.Location> SelectAngleFrontWarning(bool pSimulation, Model.LoginUserOauth pUserOauth, Xamarin.Essentials.Location pStartLocation, Xamarin.Essentials.Location pEndLocation, double pThreshold)
        {
            if (mConnection == null)
                return null;
            try
            {
                mConnection.Open();
                System.Collections.Generic.List<Xamarin.Essentials.Location> _Result = new System.Collections.Generic.List<Xamarin.Essentials.Location>();

                double _LatitudeStart = 0.0;
                double _LatitudeEnd = 0.0;
                double _LongitudeStart = 0.0;
                double _LongitudeEnd = 0.0;
                if (Math.Round(pStartLocation.Latitude, 5) < Math.Round(pEndLocation.Latitude, 5))
                {
                    _LatitudeStart = Math.Round(pStartLocation.Latitude, 5) - 0.05;
                    _LatitudeEnd = Math.Round(pEndLocation.Latitude, 5) + 0.05;
                }
                else
                {
                    _LatitudeStart = Math.Round(pEndLocation.Latitude, 5) - 0.05;
                    _LatitudeEnd = Math.Round(pStartLocation.Latitude, 5) + 0.05;
                }
                if (Math.Round(pStartLocation.Longitude, 5) < Math.Round(pEndLocation.Longitude, 5))
                {
                    _LongitudeStart = Math.Round(pStartLocation.Longitude, 5) - 0.05;
                    _LongitudeEnd = Math.Round(pEndLocation.Longitude, 5) + 0.05;
                }
                else
                {
                    _LongitudeStart = Math.Round(pEndLocation.Longitude, 5) - 0.05;
                    _LongitudeEnd = Math.Round(pStartLocation.Longitude, 5) + 0.05;
                }

                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT ROUND(AVG([latitude]),5) as [latitude] , ROUND(AVG([longitude]),5) as [longitude] , COUNT([id]) as [total_count] FROM [MaaS].[dbo].[{0}] WHERE (ABS([Inclination]) >= {1}) AND ([latitude] >= {2} AND [latitude] <= {3}) AND ([longitude] >= {4} AND [longitude] <= {5}) GROUP BY ROUND([latitude],5), ROUND([longitude],5)"
                    , pSimulation == true ? "SEnvironPressure" : "EnvironPressure"
                    , pThreshold
                    , _LatitudeStart
                    , _LatitudeEnd
                    , _LongitudeStart
                    , _LongitudeEnd);

                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), mConnection);
                var dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    _Result.Add(new Xamarin.Essentials.Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"])));
                }
                mConnection.Close();
                return _Result;
            }
            catch
            {
                mConnection.Close();
                return null;
            }
        }

        public System.Collections.Generic.List<Xamarin.Essentials.Location> SelectRadarFrontWarning(bool pSimulation, Model.LoginUserOauth pUserOauth, Xamarin.Essentials.Location pStartLocation, Xamarin.Essentials.Location pEndLocation, double pThreshold)
        {
            if (mConnection == null)
                return null;
            try
            {
                mConnection.Open();

                System.Collections.Generic.List<Xamarin.Essentials.Location> _Result = new System.Collections.Generic.List<Xamarin.Essentials.Location>();

                double _LatitudeStart = 0.0;
                double _LatitudeEnd = 0.0;
                double _LongitudeStart = 0.0;
                double _LongitudeEnd = 0.0;
                if (Math.Round(pStartLocation.Latitude, 5) < Math.Round(pEndLocation.Latitude, 5))
                {
                    _LatitudeStart = Math.Round(pStartLocation.Latitude, 5) - 0.05;
                    _LatitudeEnd = Math.Round(pEndLocation.Latitude, 5) + 0.05;
                }
                else
                {
                    _LatitudeStart = Math.Round(pEndLocation.Latitude, 5) - 0.05;
                    _LatitudeEnd = Math.Round(pStartLocation.Latitude, 5) + 0.05;
                }
                if (Math.Round(pStartLocation.Longitude, 5) < Math.Round(pEndLocation.Longitude, 5))
                {
                    _LongitudeStart = Math.Round(pStartLocation.Longitude, 5) - 0.05;
                    _LongitudeEnd = Math.Round(pEndLocation.Longitude, 5) + 0.05;
                }
                else
                {
                    _LongitudeStart = Math.Round(pEndLocation.Longitude, 5) - 0.05;
                    _LongitudeEnd = Math.Round(pStartLocation.Longitude, 5) + 0.05;
                }

                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT ROUND(AVG([latitude]),5) as [latitude] , ROUND(AVG([longitude]),5) as [longitude] , COUNT([id]) as [total_count] FROM [MaaS].[dbo].[{0}] WHERE ([ObjectsCount] >= {1}) AND ([latitude] >= {2} AND [latitude] <= {3}) AND ([longitude] >= {4} AND [longitude] <= {5}) GROUP BY ROUND([latitude],5), ROUND([longitude],5) "
                    , pSimulation == true ? "SRadarPdat" : "RadarPdat"
                    , pThreshold
                    , _LatitudeStart
                    , _LatitudeEnd
                    , _LongitudeStart
                    , _LongitudeEnd);
                System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), mConnection);
                var dr = sqlCommand.ExecuteReader();
                while (dr.Read())
                {
                    _Result.Add(new Xamarin.Essentials.Location(Convert.ToDouble(dr["latitude"]), Convert.ToDouble(dr["longitude"])));
                }
                mConnection.Close();
                return _Result;
            }
            catch
            {
                mConnection.Close();
                return null;
            }
        }


        private static System.Data.SqlClient.SqlConnection ConnectInfo()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder _Builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            _Builder.DataSource = "59.86.222.61";
            _Builder.InitialCatalog = "MaaS";
            _Builder.UserID = "SaeICTTest";
            _Builder.Password = "1q2w3e4r!@";
            _Builder.IntegratedSecurity = false;
            _Builder.MultipleActiveResultSets = true;
            return new System.Data.SqlClient.SqlConnection(_Builder.ConnectionString);
        }

        public static Tuple<Common.Constant.LoginReulstStatus, string> LoginResult(string pLoginProvider, string pProviderKey)
        {
            System.Data.SqlClient.SqlConnection _Conn = ConnectInfo();
            try
            {
                System.Data.SqlClient.SqlDataAdapter _Adapter = new System.Data.SqlClient.SqlDataAdapter();

                System.Text.StringBuilder _QueryBuilder = new System.Text.StringBuilder();
                _QueryBuilder.AppendFormat("SELECT LoginProvider,ProviderKey FROM dbo.AspNetUserLogins");
                _Adapter.SelectCommand = new System.Data.SqlClient.SqlCommand(_QueryBuilder.ToString(), _Conn);

                System.Data.DataSet ds = new System.Data.DataSet();
                _Adapter.Fill(ds);
                System.Data.DataTable dt = ds.Tables[0];

                if (dt.Rows == null)
                {
                    _Conn.Close();
                    return new Tuple<Common.Constant.LoginReulstStatus, string>(Common.Constant.LoginReulstStatus.None, "입력자 정보 리스트가 없습니다.");
                }

                if (dt.Rows.Count < 1)
                {
                    _Conn.Close();
                    return new Tuple<Common.Constant.LoginReulstStatus, string>(Common.Constant.LoginReulstStatus.None, "입력자 정보 리스트가 없습니다.");
                }
                else
                {
                    bool check = false;

                    foreach (System.Data.DataRow item in dt.Rows)
                    {
                        Console.WriteLine("LoginProvider : {0} , ProviderKey : {1}", item["LoginProvider"], item["ProviderKey"]);
                        if ((item["LoginProvider"].ToString() == pLoginProvider) && (item["ProviderKey"].ToString() == pProviderKey))
                        {
                            check = true;
                            break;
                        }
                    }

                    _Conn.Close();
                    if (check)
                    {
                        return new Tuple<Common.Constant.LoginReulstStatus, string>(Common.Constant.LoginReulstStatus.Success, "");
                    }
                    else
                    {
                        return new Tuple<Common.Constant.LoginReulstStatus, string>(Common.Constant.LoginReulstStatus.Fail, "SNS 로그인은 성공하였으나 사용자 정보가 없습니다. 회원가입을 하지 않았다면 회원가입을 진행해 주십시오.");
                    }
                }
            }
            catch (Exception ets)
            {
                _Conn.Close();
                return new Tuple<Common.Constant.LoginReulstStatus, string>(Common.Constant.LoginReulstStatus.Error, ets.Message);
            }
        }
    }
}
