using System.IO;
using HGE.IO.AudioEngine.sidPlayLib.common.events;

namespace HGE.IO.AudioEngine.sidPlayLib.components.mos6526
{
    /// <summary>
    /// CIA 1 specifics: Generates IRQs
    /// </summary>
    internal class C64cia1 : MOS6526
    {
        private sidPlayer m_player;

        private short lp;

        public override void interrupt(bool state)
        {
            m_player.interruptIRQ(state);
        }

        public override void portA()
        {
        }

        public override void portB()
        {
            short lp = (short)((regs[PRB] | (short)(~regs[DDRB] & 0xff)) & 0x10);
            if (lp != this.lp)
            {
                m_player.lightpen();
            }
            this.lp = lp;
        }

        public C64cia1(sidPlayer player)
            : base(player.m_scheduler)
        {
            m_player = (player);
        }
        // only used for deserializing
        public C64cia1(sidPlayer player, BinaryReader reader, EventList events)
            : base(player.m_scheduler, reader, events)
        {
            m_player = player;
        }

        public override void reset()
        {
            lp = 0x10;
            base.reset();
        }

        // serializing
        public override void SaveToWriter(BinaryWriter writer)
        {
            base.SaveToWriter(writer);
            writer.Write(lp);
        }
        // deserializing
        protected override void LoadFromReader(BinaryReader reader)
        {
            base.LoadFromReader(reader);
            lp = reader.ReadInt16();
        }
    }
}