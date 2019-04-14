using System;

namespace HGE.Events
{
    public class HydraTimerEvent : EventArgs
    {
        private readonly long doubleSeconds;

        public HydraTimerEvent(long duration)
        {
            doubleSeconds = duration;
        }

        public int Milliseconds => TimeSpan.FromSeconds(doubleSeconds).Milliseconds;
        public int Seconds => TimeSpan.FromSeconds(doubleSeconds).Seconds;
        public int Minutes => TimeSpan.FromSeconds(doubleSeconds).Minutes;
        public int Hours => TimeSpan.FromSeconds(doubleSeconds).Hours;
    }
}