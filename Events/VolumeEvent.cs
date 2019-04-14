using System;

namespace HGE.Events
{
    public class VolumeEvent : EventArgs
    {
        public VolumeEvent(int currentVolume)
        {
            Volume = currentVolume;
        }

        public int Volume { get; set; }
    }
}