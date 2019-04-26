using System.IO;

namespace HGE.IO.AudioEngine.sidPlayLib.components.mos656x
{
    /// <summary>
    /// The VIC emulation is very generic and here we need to effectively wire it
    /// into the computer (like adding a chip to a PCB).
    /// 
    /// @author Ken Händel
    /// </summary>
    internal class C64VIC : MOS656X
    {
        private sidPlayer m_player;

        protected override void interrupt(bool state)
        {
            m_player.interruptIRQ(state);
        }

        protected override void addrctrl(bool state)
        {
            m_player.signalAEC(state);
        }

        public C64VIC(sidPlayer player)
            : base(player.m_scheduler)
        {
            m_player = player;
        }
        // only used for deserializing
        public C64VIC(sidPlayer player, BinaryReader reader, int newid)
            : base(player.m_scheduler, reader, newid)
        {
            m_player = player;
        }

        internal override EventType GetEventType()
        {
            return EventType.vicEvt;
        }
    }
}