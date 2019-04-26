using System.IO;

namespace HGE.IO.AudioEngine.sidPlayLib.common.events
{
    internal class EventMixer : Event
    {
        private sidPlayer m_player;

        public override void _event()
        {
            m_player.mixer();
        }

        public EventMixer(sidPlayer player)
            : base("Mixer")
        {
            m_player = player;
        }
        // only used for deserializing
        public EventMixer(sidPlayer player, EventScheduler context, BinaryReader reader, int newId)
            : base(context, reader, newId)
        {
            m_player = player;
        }

        internal override EventType GetEventType()
        {
            return EventType.mixerEvt;
        }
    }
}