[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace TestGPS
{
    using TestGPS.Features.Location;
    using Xamarin.Forms;


    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LocationView();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
