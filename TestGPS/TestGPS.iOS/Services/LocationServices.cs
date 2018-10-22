[assembly: Xamarin.Forms.Dependency(typeof(TestGPS.iOS.Services.LocationServices))]
namespace TestGPS.iOS.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using CoreLocation;
    using Foundation;
    using Plugin.Permissions;
    using Plugin.Permissions.Abstractions;
    using TestGPS.EventsArgsImplements;
    using TestGPS.Features.Location;
    using UIKit;
    using Xamarin.Forms.Maps;

    public class LocationServices : ILocationService
    {
        private readonly IPermissions permissionsService;
        private CLLocationManager locationManager;
        private Position lastPosition;

        public LocationServices()
        {
            this.permissionsService = CrossPermissions.Current;
            RequestPermission().ConfigureAwait(true);
        }


        public Position LastPosition
        { 
            get => this.lastPosition;
            set
            {
                this.lastPosition = value;
                PositionChanged?.Invoke(this, new LocationEventArgs(this.lastPosition));
            }
        }

        public event EventHandler<LocationEventArgs> PositionChanged;

        public async Task StartLocationAsync()
        {
            await DemandPermissions();
            this.locationManager = new CLLocationManager
            {
                PausesLocationUpdatesAutomatically = false
            };

            this.locationManager.LocationsUpdated += LocationManager_LocationsUpdated;
            this.locationManager.AuthorizationChanged += LocationManager_AuthorizationChanged;
            this.locationManager.LocationUpdatesPaused += LocationManager_LocationUpdatesPaused;
            this.locationManager.LocationUpdatesResumed += LocationManager_LocationUpdatesResumed;
            this.locationManager.Failed += LocationManager_Failed;
            this.locationManager.DeferredUpdatesFinished += LocationManager_DeferredUpdatesFinished;
            this.locationManager.MonitoringFailed += LocationManager_MonitoringFailed;

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                this.locationManager.RequestAlwaysAuthorization(); // works in background
                this.locationManager.RequestWhenInUseAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
                this.locationManager.AllowsBackgroundLocationUpdates = true;
#pragma warning restore XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version
            }

            if (CLLocationManager.LocationServicesEnabled)
            {
                this.locationManager.DistanceFilter = 5;
                this.locationManager.DesiredAccuracy = CLLocation.AccuracyBest;

                this.locationManager.StartUpdatingLocation();
                this.locationManager.StartMonitoringSignificantLocationChanges();

                if (this.locationManager.Location?.Coordinate != null)
                    LastPosition = new Position(this.locationManager.Location.Coordinate.Latitude, this.locationManager.Location.Coordinate.Longitude);
            }
            else
            {
                throw new Exception("Error initializing LocationService.");
            }
        }


        public Task StopLocationAsync()
        {
            this.locationManager?.StopUpdatingLocation();
            this.locationManager?.StopMonitoringSignificantLocationChanges();
            this.locationManager?.Dispose();
            this.locationManager = null;
            return Task.CompletedTask;
        }

        private void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            Debug.WriteLine($"Locations updated.");
            LastPosition = new Position(e.Locations[0].Coordinate.Latitude, e.Locations[0].Coordinate.Longitude);
        }

        private void LocationManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
            Debug.WriteLine($"Authorization changed: {e.Status.ToString()}");
        }

        private void LocationManager_LocationUpdatesPaused(object sender, EventArgs e)
        {
            Debug.WriteLine($"Location updates paused.");
        }

        private void LocationManager_LocationUpdatesResumed(object sender, EventArgs e)
        {
            Debug.WriteLine($"Location updates resumed.");
        }

        private void LocationManager_Failed(object sender, NSErrorEventArgs e)
        {
            Debug.WriteLine($"Failed: {e.Error}");
        }

        private void LocationManager_DeferredUpdatesFinished(object sender, NSErrorEventArgs e)
        {
            Debug.WriteLine($"Deferred updates finished: {e.Error}");
        }

        private void LocationManager_MonitoringFailed(object sender, CLRegionErrorEventArgs e)
        {
            Debug.WriteLine($"Monitoring failed: {e.Error}");
        }

        private async Task DemandPermissions()
        {
            bool r = await CheckPermissions();

            Dictionary<Permission, PermissionStatus> results = await RequestPermission();

            foreach (KeyValuePair<Permission, PermissionStatus> result in results)
            {
                if (result.Value != PermissionStatus.Granted)
                {
                    if (permissionsService.OpenAppSettings())
                        break;
                }
            }
            if (!await CheckPermissions())
                throw new Exception("Location permissions was missings");
        }

        private async Task<Dictionary<Permission, PermissionStatus>> RequestPermission()
        {
            return await this.permissionsService.RequestPermissionsAsync(new Permission[] { Permission.Location, Permission.LocationAlways, Permission.LocationWhenInUse });
        }

        private async Task<bool> CheckPermissions()
        {
            bool havePermissions = await this.permissionsService.CheckPermissionStatusAsync(Permission.Location) == PermissionStatus.Granted
                                 & await this.permissionsService.CheckPermissionStatusAsync(Permission.LocationAlways) == PermissionStatus.Granted
                                 & await this.permissionsService.CheckPermissionStatusAsync(Permission.LocationWhenInUse) == PermissionStatus.Granted;
            return havePermissions;
        }
    }
}
