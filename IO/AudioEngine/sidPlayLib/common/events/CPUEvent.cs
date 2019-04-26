using System.IO;
using HGE.IO.AudioEngine.sidPlayLib.components.mos6510;

namespace HGE.IO.AudioEngine.sidPlayLib.common.events
{
    internal class CPUEvent : Event
    {
        internal MOS6510 owner;

        public CPUEvent(MOS6510 owner)
            : base("CPU")
        {
            this.owner = owner;
        }
        // only used for deserializing
        public CPUEvent(EventScheduler context, BinaryReader reader, int newId)
            : base(context, reader, newId)
        {
        }

        public override void _event()
        {
            owner.eventContext.schedule(owner.cpuEvent, 1, owner.m_phase);
            owner.clock();
        }

        internal override EventType GetEventType()
        {
            return EventType.cpuEvt;
        }
    }
}