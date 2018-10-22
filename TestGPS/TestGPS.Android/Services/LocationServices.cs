[assembly:Xamarin.Forms.Dependency(typeof(TestGPS.Droid.Services.LocationServices))]
namespace TestGPS.Droid.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.Locations;
    using Android.Runtime;
    using Plugin.Permissions;
    using Plugin.Permissions.Abstractions;
    using TestGPS.EventsArgsImplements;
    using TestGPS.Features.Location;
    using Xamarin.Forms.Maps;

    public class LocationServices : Java.Lang.Object, ILocationService, ILocationListener
    {
        private readonly IPermissions permissionsService;
        private LocationManager locationManager;
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

        public void OnLocationChanged(Location location)
        {
            Debug.WriteLine($"Location changed: {location.Latitude}, {location.Longitude}.");
            LastPosition = new Position(location.Latitude, location.Longitude);
        }

        public void OnProviderDisabled(string provider)
        {
            Debug.WriteLine($"Provider disabled: {provider}.");
        }

        public void OnProviderEnabled(string provider)
        {
            Debug.WriteLine($"Provider enabled: {provider}.");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Android.OS.Bundle extras)
        {
            Debug.WriteLine($"Status changed. Provider: {provider}. Status: {status.ToString()}.");
        }

        public async Task StartLocationAsync()
        {
            await DemandPermissions();
            this.locationManager = Application.Context.GetSystemService(Context.LocationService) as LocationManager;

            this.locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 5000, 3, this);
            this.locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 5000, 3, this);
            this.locationManager.RequestLocationUpdates(LocationManager.PassiveProvider, 5000, 3, this);
        }

        public async Task StopLocationAsync()
        {
            await Task.Run(() =>
            {
                this.locationManager?.RemoveUpdates(this);
                this.locationManager?.Dispose();
                this.locationManager = null;
            });
        }




        private async Task DemandPermissions()
        {
            bool r = await CheckPermissions();

            Dictionary<Permission, PermissionStatus> results = await RequestPermission();

            foreach (KeyValuePair<Permission, PermissionStatus> result in results)
            {
                if (result.Value != PermissionStatus.Granted)
                {
                    if (this.permissionsService.OpenAppSettings())
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
