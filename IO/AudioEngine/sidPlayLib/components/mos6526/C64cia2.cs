using System.IO;
using HGE.IO.AudioEngine.sidPlayLib.common.events;

namespace HGE.IO.AudioEngine.sidPlayLib.components.mos6526
{
    /// <summary>
    /// CIA 2 specifics: Generates NMIs
    /// </summary>
    internal class C64cia2 : MOS6526
    {
        private sidPlayer m_player;

        public override void portA()
        {
        }

        public override void portB()
        {
        }

        public override void interrupt(bool state)
        {
            if (state)
            {
                m_player.interruptNMI();
            }
        }

        public C64cia2(sidPlayer player)
            : base(player.m_scheduler)
        {
            m_player = player;
        }
        // only used for deserializing
        public C64cia2(sidPlayer player, BinaryReader reader, EventList events)
            : base(player.m_scheduler, reader, events)
        {
            m_player = player;
        }
    }
}