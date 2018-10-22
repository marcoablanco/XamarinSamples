namespace TestGPS.Features.Location
{
    using System;
    using System.Threading.Tasks;
    using TestGPS.EventsArgsImplements;
    using Xamarin.Forms.Maps;

    public interface ILocationService
    {
        event EventHandler<LocationEventArgs> PositionChanged;
        Position LastPosition { get; }
        Task StartLocationAsync();
        Task StopLocationAsync();
    }
}
