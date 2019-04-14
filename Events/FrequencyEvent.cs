using System;

namespace HGE.Events
{
    public class FrequencyEvent : EventArgs
    {
        public FrequencyEvent(int currentFrequency)
        {
            Frequency = currentFrequency;
        }

        public int Frequency { get; set; }
    }
}