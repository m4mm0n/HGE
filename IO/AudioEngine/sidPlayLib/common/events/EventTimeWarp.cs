using System.IO;

namespace HGE.IO.AudioEngine.sidPlayLib.common.events
{
    internal class EventTimeWarp : Event
    {
        internal EventScheduler m_scheduler;

        public override void _event()
        {
            m_scheduler._event();
        }

        public EventTimeWarp(EventScheduler context)
            : base("Time Warp")
        {
            m_scheduler = context;
        }
        // only used for deserializing
        public EventTimeWarp(EventScheduler context, BinaryReader reader, int newId)
            : base(context, reader, newId)
        {
            m_scheduler = context;
        }

        internal override EventType GetEventType()
        {
            return EventType.TimeWarpEvt;
        }
    }
}