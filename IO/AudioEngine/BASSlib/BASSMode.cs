using System;

namespace HGE.IO.AudioEngine.BASSlib
{
    [Flags]
    public enum BASSMode
    {
        // Token: 0x0400045A RID: 1114
        BASS_POS_BYTES = 0,

        // Token: 0x0400045B RID: 1115
        BASS_POS_MUSIC_ORDERS = 1,

        // Token: 0x0400045C RID: 1116
        BASS_POS_MIDI_TICK = 2,

        // Token: 0x0400045D RID: 1117
        BASS_POS_OGG = 3,

        // Token: 0x0400045E RID: 1118
        BASS_POS_CD_TRACK = 4,

        // Token: 0x0400045F RID: 1119
        BASS_POS_INEXACT = 134217728,

        // Token: 0x04000460 RID: 1120
        BASS_MUSIC_POSRESET = 32768,

        // Token: 0x04000461 RID: 1121
        BASS_MUSIC_POSRESETEX = 4194304,

        // Token: 0x04000462 RID: 1122
        BASS_MIXER_NORAMPIN = 8388608,

        // Token: 0x04000463 RID: 1123
        BASS_POS_DECODE = 268435456,

        // Token: 0x04000464 RID: 1124
        BASS_POS_DECODETO = 536870912,

        // Token: 0x04000465 RID: 1125
        BASS_POS_SCAN = 1073741824,

        // Token: 0x04000466 RID: 1126
        BASS_MIDI_DECAYSEEK = 16384
    }
}