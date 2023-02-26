using Esri.ArcGISRuntime.Ogc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace MaaSArcGISIOS.Common
{
    #region Default
    public partial class EsriMapComponent
    {
        private readonly int mReRoutedTimeLimit = 15;
        private readonly int mGPSEPSG = 4326;
        private readonly string mLocateURI = "https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
        private readonly string mVWorldURI = "http://api.vworld.kr/req/wmts/1.0.0/" + Constant.mVworldAPIKey +  "/WMTSCapabilities.xml";
        private readonly string mBaroEMapURI = "http://210.117.198.32:6080/arcgis/rest/services/NGII_BaseMAP/MapServer/WMTS/1.0.0/WMTSCapabilities.xml";
        private Esri.ArcGISRuntime.Xamarin.Forms.MapView mEsriMapview;
        private Esri.ArcGISRuntime.UI.GraphicsOverlayCollection mGraphicsOverlayCollection;
        private bool mRoutedFlag = false;
        private bool mRoutedOn = false;

        private Action<string> mCatchCallBack;
        private Action<string> mRouteCurrentSiteInfoCallBack;
        private Action<string> mRouteCurrentDirectionManeuverCallBack;
        private Action<string> mRouteFutureDirectionManeuverCallBack;
        private Action<long, int> mRouteSearchTimeCallBack;
        private Action<Xamarin.Essentials.Location> mRoutedDisplayLocationCallBack;
        private Action<Xamarin.Essentials.Location, Xamarin.Essentials.Location> mRoutedFutureLocationCallBack;

        public EsriMapComponent(Action<string> pCatchCallBack,
            Action<string> pRouteCurrentSiteInfoCallBack,
            Action<string> pRouteCurrentDirectionManeuverCallBack,
            Action<string> pRouteFutureDirectionManeuverCallBack,
            Action<long, int> pRouteSearchTimeCallBack,
            Action<Xamarin.Essentials.Location> pRoutedDisplayLocationCallBack,
            Action<Xamarin.Essentials.Location, Xamarin.Essentials.Location> pRoutedFutureLocationCallBack,
            bool pRouted = false)
        {
            mEsriMapview = new Esri.ArcGISRuntime.Xamarin.Forms.MapView();
            mGraphicsOverlayCollection = new Esri.ArcGISRuntime.UI.GraphicsOverlayCollection
            {
                new Esri.ArcGISRuntime.UI.GraphicsOverlay(),
            };
            mEsriMapview.GraphicsOverlays = mGraphicsOverlayCollection;

            mRoutedFlag = pRouted;
            mCatchCallBack = pCatchCallBack;
            mRouteCurrentSiteInfoCallBack = pRouteCurrentSiteInfoCallBack;
            mRouteCurrentDirectionManeuverCallBack = pRouteCurrentDirectionManeuverCallBack;
            mRouteFutureDirectionManeuverCallBack = pRouteFutureDirectionManeuverCallBack;
            mRouteSearchTimeCallBack = pRouteSearchTimeCallBack;
            mRoutedDisplayLocationCallBack = pRoutedDisplayLocationCallBack;
            mRoutedFutureLocationCallBack = pRoutedFutureLocationCallBack;
        }

        public async void CreateMap(string pIdentifier = "Base")
        {
            try
            {
                var _Map = new Esri.ArcGISRuntime.Mapping.Map(Esri.ArcGISRuntime.Mapping.BasemapStyle.ArcGISNavigation);
                Esri.ArcGISRuntime.Mapping.WmtsLayer _WmtsLayer = new Esri.ArcGISRuntime.Mapping.WmtsLayer(new Uri(mVWorldURI), pIdentifier);
                await _WmtsLayer.LoadAsync();
                _Map.OperationalLayers.Add(_WmtsLayer);
                mEsriMapview.Map = _Map;
            }
            catch (Exception ets)
            {
                mCatchCallBack(ets.Message);
            }
        }

        public Esri.ArcGISRuntime.Xamarin.Forms.MapView GetMapView()
        {
            return mEsriMapview;
        }

        public async void SetMapViewCenterAsync()
        {
            if (mLocationChangeLastPosition != null)
            {
                await mEsriMapview.SetViewpointCenterAsync(CreateMapPoint(mLocationChangeLastPosition));
            }
        }

        public async void SetMapViewCenterAsync(Xamarin.Essentials.Location pLocation)
        {
            await mEsriMapview.SetViewpointCenterAsync(CreateMapPoint(pLocation));
        }

        public async void SetMapViewPointAsync(Esri.ArcGISRuntime.Geometry.MapPoint pMapPoint, double pScale = 4000)
        {
            if (await mEsriMapview.SetViewpointAsync(new Esri.ArcGISRuntime.Mapping.Viewpoint(pMapPoint, pScale)))
            {
                mMapInitScale = true;
            }
            else
            {
                mMapInitScale = false;
            }
        }

        public async void SetMapViewPointAsync(Xamarin.Essentials.Location pLocation, double pScale = 4000)
        {
            if(await mEsriMapview.SetViewpointAsync(new Esri.ArcGISRuntime.Mapping.Viewpoint(CreateMapPoint(pLocation), pScale)))
            {
                mMapInitScale = true;
            }
            else
            {
                mMapInitScale = false;
            }
        }

        public Esri.ArcGISRuntime.Geometry.MapPoint ConvertoWGS84(double pYvalue, double pXvalue, int pSourceEPSG)
        {
            Esri.ArcGISRuntime.Geometry.MapPoint _Source = new Esri.ArcGISRuntime.Geometry.MapPoint(pXvalue, pYvalue, new Esri.ArcGISRuntime.Geometry.SpatialReference(pSourceEPSG));
            Esri.ArcGISRuntime.Geometry.MapPoint _Destination = (Esri.ArcGISRuntime.Geometry.MapPoint)Esri.ArcGISRuntime.Geometry.GeometryEngine.Project(_Source, new Esri.ArcGISRuntime.Geometry.SpatialReference(4326));

            return _Destination;
        }

        public Esri.ArcGISRuntime.Geometry.MapPoint ConvertoWGS84(double pLatitude, double pLogitude)
        {
            return new Esri.ArcGISRuntime.Geometry.MapPoint(pLogitude, pLatitude, Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
        }

        public Esri.ArcGISRuntime.Geometry.MapPoint CreateMapPoint(Xamarin.Essentials.Location pLocation)
        {
            return new Esri.ArcGISRuntime.Geometry.MapPoint(pLocation.Longitude, pLocation.Latitude, Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
        }
    }
    #endregion

    #region Window On/Off
    public partial class EsriMapComponent
    {
        bool InitFlag = false;
        public void InitAppearing()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                if (InitFlag == false)
                {
                    InitFlag = true;
                    InitNavigate();
                }
                AppearingNavigateEvnet();
            });
        }

        public void InitDisAppearing()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                mMapInitScale = false;
                DisAppearingNavigateEvent();
            });
        }

        private async void InitNavigate()
        {
            try
            {
                var pLocation = await Xamarin.Essentials.Geolocation.GetLocationAsync(new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5)));
                if(pLocation != null)
                {
                    SetMapViewPointAsync(pLocation);
                }
                InitRoutedNavigate();
            }
            catch (Exception ets)
            {

            }
        }
    }
    #endregion

    #region Routed / Navigate
    public partial class EsriMapComponent
    {
        //private readonly Uri mRoutingURI = new Uri("https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World");
        private readonly Uri mRoutingURI = new Uri("https://route-api.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World");

        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteTask mRouteTask;
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteParameters mRouteParams;

        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteResult mRouteResult = null;
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Route mRouted = null;
        private Esri.ArcGISRuntime.Navigation.RouteTracker mRouteTracker = null;
        private IReadOnlyList<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuver> mDirectionsList;

        private Esri.ArcGISRuntime.Geometry.MapPoint mRoutedDestinationMapPosition = null;
        private Xamarin.Essentials.Location mLocationChangeLastPosition = null;
        private Xamarin.Essentials.Location mRoutedCurrentLocation = null;

        private Esri.ArcGISRuntime.UI.Graphic mRouteAheadGraphic;
        private Esri.ArcGISRuntime.UI.Graphic mRouteTraveledGraphic;
        private Esri.ArcGISRuntime.UI.Graphic mRoutedEndGraphic;
        private Esri.ArcGISRuntime.UI.Graphic mCurrnetMarkerGraphic;

        private Esri.ArcGISRuntime.UI.Graphic mTestMarkerGraphic;

        private List<Esri.ArcGISRuntime.UI.Graphic> mAvoidPolygonGrapic;
        private bool mRoutedProgress = false;
        private System.Diagnostics.Stopwatch mSimulateStopWatch;
        private System.Diagnostics.Stopwatch mReRoutedStopWatch;

        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop[] EmptyStoplist;
        private Esri.ArcGISRuntime.Tasks.NetworkAnalysis.PolygonBarrier[] EmptyPolygonBarrierlist;

        private double mLastRequestSpeed = 20.0;
        private Xamarin.Essentials.Location[] mLastRequestLocations = null;
        private int mLastRequestRoutedType = -1;
        private bool mLastRequestAutoFlag = false;
        private bool mMapInitScale = false;
        public async void InitRoutedNavigate()
        {
            try
            {
                EmptyStoplist = new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop[0];
                EmptyPolygonBarrierlist = new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.PolygonBarrier[0];
                if (mRoutedFlag)
                {
                    mSimulateStopWatch = new System.Diagnostics.Stopwatch();
                    mReRoutedStopWatch = new System.Diagnostics.Stopwatch();
                    mRouteTask = await Esri.ArcGISRuntime.Tasks.NetworkAnalysis.RouteTask.CreateAsync(mRoutingURI);
                    mRouteParams = await mRouteTask.CreateDefaultParametersAsync();
                    mRouteParams.ReturnDirections = true;
                    mRouteParams.ReturnStops = true;
                    mRouteParams.ReturnRoutes = true;
                    mRouteParams.DirectionsLanguage = "ko";
                    mRouteParams.ReturnPolygonBarriers = true;
                    mRouteParams.OutputSpatialReference = Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84;
                }

                Esri.ArcGISRuntime.UI.GraphicsOverlay _GrapicOverlay = this.mGraphicsOverlayCollection.FirstOrDefault();

                //mNMEALocationDataSource = new Esri.ArcGISRuntime.Location.NmeaLocationDataSource(Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);

                if (mEsriMapview.LocationDisplay != null)
                {
                    mEsriMapview.LocationDisplay.DataSource = Esri.ArcGISRuntime.Location.LocationDataSource.CreateDefault();
                }

                mRouteAheadGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(
                    Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Dash,
                    System.Drawing.Color.BlueViolet, 5)
                };

                mRouteTraveledGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(
                        Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Solid,
                        System.Drawing.Color.LightBlue, 3)
                };

                mRoutedEndGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol(Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbolStyle.Square, System.Drawing.Color.Red, 15),
                };

                mCurrnetMarkerGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol(Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.FromArgb(0, 122, 194), 20),
                };

                mTestMarkerGraphic = new Esri.ArcGISRuntime.UI.Graphic
                {
                    Symbol = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol(Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.OrangeRed, 20),
                };

                mAvoidPolygonGrapic = new List<Esri.ArcGISRuntime.UI.Graphic>();
                _GrapicOverlay.Graphics.Add(mCurrnetMarkerGraphic);
            }
            catch (Exception ets)
            {

            }
        }

        public bool IsInitMapScale()
        {
            return mMapInitScale;
        }

        public void SetCurrentMapPoint(Xamarin.Essentials.Location pLocation, bool pVisible)
        {
            if (pVisible)
            {
                if (mCurrnetMarkerGraphic.IsVisible == false)
                {
                    mCurrnetMarkerGraphic.IsVisible = true;
                }
                mCurrnetMarkerGraphic.Geometry = CreateMapPoint(pLocation);

                mLocationChangeLastPosition = pLocation;
            }
            else
            {
                mCurrnetMarkerGraphic.IsVisible = false;
            }
        }

        public void SetCurrentMapPoint(Esri.ArcGISRuntime.Geometry.MapPoint pMapPoint, bool pVisible)
        {
            if (pVisible)
            {
                if (mCurrnetMarkerGraphic.IsVisible == false)
                {
                    mCurrnetMarkerGraphic.IsVisible = true;
                }
                mCurrnetMarkerGraphic.Geometry = pMapPoint;
            }
            else
            {
                mCurrnetMarkerGraphic.IsVisible = false;
            }
        }

        public bool GetRoutedProgress()
        {
            return mRoutedProgress;
        }

        private void AppearingNavigateEvnet()
        {
            try
            {
            }
            catch
            {

            }
        }

        private void DisAppearingNavigateEvent()
        {
            try
            {
                mRoutedProgress = false;
                DisppearingRouted();
            }
            catch
            {

            }
        }


        private void DefaultLocationDataSource_LocationChanged(object sender, Esri.ArcGISRuntime.Location.Location e)
        {
        }

        public void RoutedAddress(bool pSimulation, int pRequestType, Esri.ArcGISRuntime.Geometry.MapPoint pDestination, Esri.ArcGISRuntime.Geometry.MapPoint pSource, double pSpeed, Xamarin.Essentials.Location[] pLocations = null)
        {
            if (pDestination == null || pSource == null)
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", "RoutedAddress Set Fail", "OK");
                });
                return;
            }
            mLastRequestAutoFlag = pSimulation;
            mLastRequestRoutedType = pRequestType;
            mRoutedDestinationMapPosition = pDestination;
            mLastRequestSpeed = pSpeed;
            mLastRequestLocations = pLocations;

            List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop> stopPoints = new List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop>();
            stopPoints.Add(new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop(pSource));
            stopPoints.Add(new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop(pDestination));
            List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.PolygonBarrier> avoidPoints = new List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.PolygonBarrier>();

            DisppearingRouted();
            if (pLocations != null)
            {
                foreach (var item in pLocations)
                {
                    List<Esri.ArcGISRuntime.Geometry.MapPoint> polygonPoints = new List<Esri.ArcGISRuntime.Geometry.MapPoint>
                    {
                        CreateMapPoint(new Xamarin.Essentials.Location(item.Latitude - 0.0008,item.Longitude - 0.0008)),
                        CreateMapPoint(new Xamarin.Essentials.Location(item.Latitude + 0.0008,item.Longitude - 0.0008)),
                        CreateMapPoint(new Xamarin.Essentials.Location(item.Latitude + 0.0008,item.Longitude + 0.0008)),
                        CreateMapPoint(new Xamarin.Essentials.Location(item.Latitude - 0.0008,item.Longitude + 0.0008)),
                    };

                    var mahouRivieraPolygon = new Esri.ArcGISRuntime.Geometry.Polygon(polygonPoints);
                    var polygonSymbolOutline = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
                    var polygonFillSymbol = new Esri.ArcGISRuntime.Symbology.SimpleFillSymbol(Esri.ArcGISRuntime.Symbology.SimpleFillSymbolStyle.Solid, System.Drawing.Color.Orange, polygonSymbolOutline);

                    // Create a polygon graphic with the geometry and fill symbol.
                    var polygonGraphic = new Esri.ArcGISRuntime.UI.Graphic(mahouRivieraPolygon, polygonFillSymbol);
                    avoidPoints.Add(new Esri.ArcGISRuntime.Tasks.NetworkAnalysis.PolygonBarrier(mahouRivieraPolygon));
                    mAvoidPolygonGrapic.Add(polygonGraphic);
                }
            }

            Task.Run(() =>
            {
                System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

                if (pSimulation == true)
                {
                    _stopwatch.Start();
                    AppearingRouted(stopPoints, pRequestType, pDestination, pSimulation, pSpeed, avoidPoints.ToArray(), _stopwatch);
                }
                else
                {
                    _stopwatch.Start();
                    AppearingRouted(stopPoints, pRequestType, pDestination, pSimulation, pSpeed, avoidPoints.ToArray(), _stopwatch);
                    while (_stopwatch.IsRunning)
                    {
                        if (_stopwatch.ElapsedMilliseconds > 6000)
                        {
                            _stopwatch.Stop();
                            mRouteSearchTimeCallBack(-1, pRequestType);
                            break;
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }
            });
        }
        private async void DisppearingRouted()
        {
            try
            {
                mRoutedOn = false;
                mRoutedProgress = false;
                if (mRoutedFlag)
                {
                    if (mReRoutedStopWatch != null)
                    {
                        mReRoutedStopWatch.Stop();
                        mReRoutedStopWatch.Reset();
                    }

                    if (mSimulateStopWatch != null)
                    {
                        mSimulateStopWatch.Stop();
                        mSimulateStopWatch.Reset();
                    }


                    Esri.ArcGISRuntime.UI.GraphicsOverlay _GrapicOverlay = this.mGraphicsOverlayCollection.FirstOrDefault();
                    if (mRouteAheadGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mRouteAheadGraphic);
                        mRoutedEndGraphic.Geometry = null;
                    }
                    if (mRouteTraveledGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mRouteTraveledGraphic);
                        mRouteTraveledGraphic.Geometry = null;
                    }
                    if (mRoutedEndGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mRoutedEndGraphic);
                        mRoutedEndGraphic.Geometry = null;
                    }

                    if (mTestMarkerGraphic != null)
                    {
                        _GrapicOverlay.Graphics.Remove(mTestMarkerGraphic);
                        mTestMarkerGraphic.Geometry = null;
                    }

                    if (mAvoidPolygonGrapic != null)
                    {
                        foreach (var item in mAvoidPolygonGrapic)
                        {
                            _GrapicOverlay.Graphics.Remove(item);
                        }
                        mAvoidPolygonGrapic.Clear();
                    }

                    if (mRouteParams != null)
                    {
                        mRouteParams.ClearStops();
                        mRouteParams.ClearPolylineBarriers();
                        mRouteParams.ClearPointBarriers();
                        mRouteParams.ClearPolylineBarriers();

                        mRouteParams.SetStops(EmptyStoplist);
                        mRouteParams.SetPolygonBarriers(EmptyPolygonBarrierlist);
                    }

                    if (mRouteTracker != null)
                    {
                        mRouteTracker.TrackingStatusChanged -= RouteTracker_TrackingStatusChanged;
                    }

                    if (mEsriMapview.LocationDisplay != null)
                    {
                        if (mEsriMapview.LocationDisplay.DataSource != null)
                        {
                            await mEsriMapview.LocationDisplay.DataSource.StopAsync();
                        }
                    }
                }
            }
            catch (Exception ets)
            {
            }
        }

        private async void AppearingRouted(List<Esri.ArcGISRuntime.Tasks.NetworkAnalysis.Stop> pStopList, int pRequestType, Esri.ArcGISRuntime.Geometry.MapPoint pDestination, bool pSimulation, double pSpeed, Esri.ArcGISRuntime.Tasks.NetworkAnalysis.PolygonBarrier[] pAvoidPoints, System.Diagnostics.Stopwatch pStopwatch)
        {
            mRoutedOn = true;
            Esri.ArcGISRuntime.UI.GraphicsOverlay _GrapicOverlay = this.mGraphicsOverlayCollection.FirstOrDefault();
            if (pStopList != null)
            {
                mRouteParams.SetStops(pStopList);
            }
            if (pAvoidPoints != null)
            {
                mRouteParams.SetPolygonBarriers(pAvoidPoints);
            }
            try
            {
                mRouteResult = await mRouteTask.SolveRouteAsync(mRouteParams);

                if (pSimulation == false)
                {
                    if (pStopwatch.ElapsedMilliseconds > 5000)
                    {
                        return;
                    }
                }

                pStopwatch.Stop();
                mRouteSearchTimeCallBack((long)pStopwatch.ElapsedMilliseconds, pRequestType);

                mRouted = mRouteResult.Routes[0];
                await mEsriMapview.SetViewpointGeometryAsync(mRouted.RouteGeometry, 100);
                mDirectionsList = mRouted.DirectionManeuvers;

                if (mRouteTracker != null)
                {
                    mRouteTracker.TrackingStatusChanged -= RouteTracker_TrackingStatusChanged;
                }

                mRouteTracker = new Esri.ArcGISRuntime.Navigation.RouteTracker(mRouteResult, 0, true);
                mRouteTracker.TrackingStatusChanged += RouteTracker_TrackingStatusChanged;
                if (pSimulation)
                {
                    var simulationParameters = new Esri.ArcGISRuntime.Location.SimulationParameters(DateTimeOffset.Now, pSpeed * 1000.0 / 3600.0);
                    var simulatedDataSource = new Esri.ArcGISRuntime.Location.SimulatedLocationDataSource();

                    simulatedDataSource.SetLocationsWithPolyline(mRouted.RouteGeometry, simulationParameters);
                    mEsriMapview.LocationDisplay.DataSource = new RouteTrackerDisplayLocationDataSource(simulatedDataSource, mRouteTracker);
                }
                else
                {
                    mEsriMapview.LocationDisplay.DataSource = new RouteTrackerDisplayLocationDataSource(Esri.ArcGISRuntime.Location.LocationDataSource.CreateDefault(), mRouteTracker);
                }
                mRoutedEndGraphic.Geometry = pDestination;
                mRouteAheadGraphic.Geometry = mRouted.RouteGeometry;
                _GrapicOverlay.Graphics.Add(mRouteAheadGraphic);
                _GrapicOverlay.Graphics.Add(mRouteTraveledGraphic);
                _GrapicOverlay.Graphics.Add(mRoutedEndGraphic);
                _GrapicOverlay.Graphics.Add(mTestMarkerGraphic);

                foreach (var item in mAvoidPolygonGrapic)
                {
                    _GrapicOverlay.Graphics.Add(item);
                }

                mSimulateStopWatch.Start();
                mReRoutedStopWatch.Start();
                await mEsriMapview.LocationDisplay.DataSource.StartAsync();
            }
            catch (Esri.ArcGISRuntime.Http.ArcGISWebException ets)
            {
                if (pStopwatch.ElapsedMilliseconds > 5000)
                {

                }
                else
                {
                    mCatchCallBack("경로를 완성할 수 없습니다.");
                }
            }
            catch (Exception ets)
            {
            }
        }

        private void RouteTracker_TrackingStatusChanged(object sender, Esri.ArcGISRuntime.Navigation.RouteTrackerTrackingStatusChangedEventArgs e)
        {
            try
            {
                var _RouteLocation = new Xamarin.Essentials.Location(e.TrackingStatus.DisplayLocation.Position.Y, e.TrackingStatus.DisplayLocation.Position.X);

                mRoutedDisplayLocationCallBack(_RouteLocation);
                mRoutedCurrentLocation = _RouteLocation;

                if (e.TrackingStatus.ManeuverProgress.RemainingGeometry.Parts.Count > 0)
                {
                    if (e.TrackingStatus.ManeuverProgress.RemainingGeometry.Parts.First().PointCount > 1)
                    {
                        var _Destination = e.TrackingStatus.ManeuverProgress.RemainingGeometry.Parts.First().Points[1];
                        var _Source = e.TrackingStatus.DisplayLocation.Position;
                        var _totaldistance = Math.Round(Xamarin.Essentials.Location.CalculateDistance(_Source.Y, _Source.X, _Destination.Y, _Destination.X, Xamarin.Essentials.DistanceUnits.Kilometers) * 1000.0, 3);

                        if (_totaldistance > 300.0)
                        {
                            var _disntaceX = (_Destination.X - _Source.X);
                            var _disntaceY = (_Destination.Y - _Source.Y);
                            var _ratio = 300.0 / _totaldistance;
                            var ttss = new Xamarin.Essentials.Location(_Source.Y + (_disntaceY * _ratio), _Source.X + (_disntaceX * _ratio));
                            mTestMarkerGraphic.Geometry = CreateMapPoint(ttss);
                            mRoutedFutureLocationCallBack(ttss, _RouteLocation);
                        }
                        else
                        {
                            mTestMarkerGraphic.Geometry = _Destination;
                            mRoutedFutureLocationCallBack(new Xamarin.Essentials.Location(_Destination.Y, _Destination.X), _RouteLocation);
                        }
                    }
                    else
                    {
                        var _Destination = e.TrackingStatus.ManeuverProgress.RemainingGeometry.Parts.First().EndPoint;
                        var _Source = e.TrackingStatus.DisplayLocation.Position;
                        var _totaldistance = Math.Round(Xamarin.Essentials.Location.CalculateDistance(_Source.Y, _Source.X, _Destination.Y, _Destination.X, Xamarin.Essentials.DistanceUnits.Kilometers) * 1000.0, 3);

                        if (_totaldistance > 300.0)
                        {
                            var _disntaceX = (_Destination.X - _Source.X);
                            var _disntaceY = (_Destination.Y - _Source.Y);
                            var _ratio = 300.0 / _totaldistance;
                            var ttss = new Xamarin.Essentials.Location(_Source.Y + (_disntaceY * _ratio), _Source.X + (_disntaceX * _ratio));
                            mTestMarkerGraphic.Geometry = CreateMapPoint(ttss);
                            mRoutedFutureLocationCallBack(ttss, _RouteLocation);
                        }
                        else
                        {
                            mTestMarkerGraphic.Geometry = _Destination;
                            mRoutedFutureLocationCallBack(new Xamarin.Essentials.Location(_Destination.Y, _Destination.X), _RouteLocation);
                        }
                    }
                }

                mEsriMapview.SetViewpointCenterAsync(e.TrackingStatus.DisplayLocation.Position);
                Esri.ArcGISRuntime.Navigation.TrackingStatus status = e.TrackingStatus;
                if (status.IsOnRoute)
                {
                    mReRoutedStopWatch.Restart();
                    if (status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.NotReached || status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.Approaching)
                    {
                        mRoutedProgress = true;
                        if (mDirectionsList[status.CurrentManeuverIndex].ManeuverMessages.Count > 0)
                        {
                            if (mDirectionsList[status.CurrentManeuverIndex].ManeuverMessages.First().Text.Contains("/"))
                            {
                                mRouteCurrentSiteInfoCallBack(String.Format("현재 정보 : {0}", mDirectionsList[status.CurrentManeuverIndex].ManeuverMessages.First().Text.Split('/').First()));
                            }
                            else
                            {
                                mRouteCurrentSiteInfoCallBack(String.Format("현재 정보 : {0}", mDirectionsList[status.CurrentManeuverIndex].ManeuverMessages.First().Text));
                            }
                        }
                        else
                        {
                            mRouteCurrentSiteInfoCallBack(String.Format("현재 정보 : "));
                        }

                        if (status.CurrentManeuverIndex + 1 < mDirectionsList.Count)
                        {
                            if (mDirectionsList[status.CurrentManeuverIndex + 1].ManeuverType == Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Stop)
                            {
                                mRouteCurrentDirectionManeuverCallBack("현재 안내사항 : 도착까지 " + status.ManeuverProgress.RemainingDistance.Distance.ToString("0.0m"));
                            }
                            else
                            {
                                mRouteCurrentDirectionManeuverCallBack("현재 안내사항 : " + status.ManeuverProgress.RemainingDistance.Distance.ToString("0.0m") + " 앞에서 " + Constant.mDirectionManeuverDict[mDirectionsList[status.CurrentManeuverIndex + 1].ManeuverType]);
                            }
                            if (status.CurrentManeuverIndex + 2 < mDirectionsList.Count)
                            {
                                mRouteFutureDirectionManeuverCallBack("다음 안내사항 : " + mDirectionsList[status.CurrentManeuverIndex + 1].Length.ToString("0.0m") + " 앞에서 " + Constant.mDirectionManeuverDict[mDirectionsList[status.CurrentManeuverIndex + 2].ManeuverType]);
                            }
                            else
                            {
                                mRouteFutureDirectionManeuverCallBack(String.Empty);
                            }
                        }
                        else
                        {
                            mRouteCurrentDirectionManeuverCallBack("현재 안내사항 : " + Constant.mDirectionManeuverDict[Esri.ArcGISRuntime.Tasks.NetworkAnalysis.DirectionManeuverType.Stop] + "까지 " + status.ManeuverProgress.RemainingDistance.Distance.ToString("0.0m"));
                            mRouteFutureDirectionManeuverCallBack(String.Empty);
                        }
                        mRouteAheadGraphic.Geometry = status.RouteProgress.RemainingGeometry;
                        mRouteTraveledGraphic.Geometry = status.RouteProgress.TraversedGeometry;
                    }
                    else if (status.DestinationStatus == Esri.ArcGISRuntime.Navigation.DestinationStatus.Reached)
                    {
                        mRoutedProgress = false;
                        mRouteCurrentSiteInfoCallBack(String.Format("도착! 걸린 시간 : {0}초", mSimulateStopWatch.ElapsedMilliseconds / 1000));
                        mRouteCurrentDirectionManeuverCallBack(String.Empty);
                        mRouteFutureDirectionManeuverCallBack(String.Empty);

                        mRouteAheadGraphic.Geometry = null;
                        mRoutedEndGraphic.Geometry = null;
                        mRouteTraveledGraphic.Geometry = status.RouteResult.Routes[0].RouteGeometry;

                        if (status.RemainingDestinationCount > 1)
                        {
                            mRouteTracker.SwitchToNextDestinationAsync();
                        }
                        else
                        {
                            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                            {
                                DisppearingRouted();
                            });
                        }
                    }
                }
                else
                {
                    if (status.CurrentManeuverIndex + 1 < mDirectionsList.Count)
                    {
                        mRouteCurrentSiteInfoCallBack(String.Format("현재 정보 : Off Routed"));
                        if (mReRoutedStopWatch.ElapsedMilliseconds > 15000)
                        {
                            mReRoutedStopWatch.Stop();
                            mReRoutedStopWatch.Reset();
                            mRoutedProgress = false;
                            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                            {
                                if (mRoutedCurrentLocation != null)
                                {
                                    RoutedAddress(mLastRequestAutoFlag, mLastRequestRoutedType, mRoutedDestinationMapPosition, CreateMapPoint(mRoutedCurrentLocation), mLastRequestSpeed, mLastRequestLocations);
                                }
                            });
                        }
                    }
                    else
                    {
                        mRouteCurrentSiteInfoCallBack(String.Format("현재 정보 : Off Routed 도착"));

                        mRouteCurrentDirectionManeuverCallBack(String.Empty);
                        mRouteFutureDirectionManeuverCallBack(String.Empty);

                        mRouteAheadGraphic.Geometry = null;
                        mRoutedEndGraphic.Geometry = null;
                        mRouteTraveledGraphic.Geometry = status.RouteResult.Routes[0].RouteGeometry;

                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                        {
                            DisppearingRouted();
                        });
                    }
                }
            }
            catch
            {

            }
        }
    }
    #endregion

    public class RouteTrackerDisplayLocationDataSource : Esri.ArcGISRuntime.Location.LocationDataSource
    {
        private Esri.ArcGISRuntime.Location.LocationDataSource _inputDataSource;
        private Esri.ArcGISRuntime.Navigation.RouteTracker _routeTracker;
        public RouteTrackerDisplayLocationDataSource(Esri.ArcGISRuntime.Location.LocationDataSource dataSource, Esri.ArcGISRuntime.Navigation.RouteTracker routeTracker)
        {
            // Set the data source
            _inputDataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            // Set the route tracker.
            _routeTracker = routeTracker ?? throw new ArgumentNullException(nameof(routeTracker));

            // Change the tracker location when the source location changes.
            _inputDataSource.LocationChanged += InputLocationChanged;

            // Update the location output when the tracker location updates.
            _routeTracker.TrackingStatusChanged += TrackingStatusChanged;
        }

        private void InputLocationChanged(object sender, Esri.ArcGISRuntime.Location.Location e)
        {
            // Update the tracker location with the new location from the source (simulation or GPS).
            _routeTracker.TrackLocationAsync(e);
        }

        private void TrackingStatusChanged(object sender, Esri.ArcGISRuntime.Navigation.RouteTrackerTrackingStatusChangedEventArgs e)
        {
            // Check if the tracking status has a location.
            if (e.TrackingStatus.DisplayLocation != null)
            {
                // Call the base method for LocationDataSource to update the location with the tracked (snapped to route) location.
                UpdateLocation(e.TrackingStatus.DisplayLocation);
            }
        }

        protected override System.Threading.Tasks.Task OnStartAsync()
        {
            return _inputDataSource.StartAsync();
        }

        protected override System.Threading.Tasks.Task OnStopAsync()
        {
            try
            {
                _inputDataSource.LocationChanged -= InputLocationChanged;
                _routeTracker.TrackingStatusChanged -= TrackingStatusChanged;
            }
            catch
            {

            }
            return _inputDataSource.StopAsync();
        }
    }
}
