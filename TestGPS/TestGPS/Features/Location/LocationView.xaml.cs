namespace TestGPS.Features.Location
{
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationView
    {
        public LocationView()
        {
            ViewModel = new LocationViewModel();
            InitializeComponent();
        }


        public LocationViewModel ViewModel
        {
            get => (LocationViewModel)this.BindingContext;
            set => BindingContext = value;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel?.OnDisappearing();
        }
    }
}
