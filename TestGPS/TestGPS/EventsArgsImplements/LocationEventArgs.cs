namespace TestGPS.EventsArgsImplements
{
    using System;
    using Xamarin.Forms.Maps;

    public class LocationEventArgs : EventArgs
    {
        public LocationEventArgs(Position position)
        {
            Position = position;
        }

        public Position Position { get; set; }
    }
}
