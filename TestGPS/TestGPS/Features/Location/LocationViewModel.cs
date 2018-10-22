namespace TestGPS.Features.Location
{
    using TestGPS.EventsArgsImplements;
    using Xamarin.Forms;

    public class LocationViewModel : BindableObject
    {
        private readonly ILocationService locationService;
        private string latitude;
        private string longitude;

        public LocationViewModel()
        {
            this.locationService = DependencyService.Get<ILocationService>();
        }

        public string Longitude
        {
            get => longitude;
            set
            {
                longitude = value;
                OnPropertyChanged();
            }
        }

        public string Latitude
        {
            get => latitude;
            set
            {
                latitude = value;
                OnPropertyChanged();
            }
        }

        public void OnAppearing()
        {
            this.locationService.StartLocationAsync();
            this.locationService.PositionChanged += LocationService_PositionChanged;
        }

        public void OnDisappearing()
        {
            this.locationService.StopLocationAsync();
            this.locationService.PositionChanged -= LocationService_PositionChanged;
        }


        private void LocationService_PositionChanged(object sender, LocationEventArgs e)
        {
            Latitude = e.Position.Latitude.ToString();
            Longitude = e.Position.Longitude.ToString();
        }
    }
}
