using System;

namespace HGE.IO.AudioEngine.BASSlib
{
    [Flags]
    public enum BASSInit
    {
        // Token: 0x04000404 RID: 1028
        BASS_DEVICE_DEFAULT = 0,

        // Token: 0x04000405 RID: 1029
        BASS_DEVICE_8BITS = 1,

        // Token: 0x04000406 RID: 1030
        BASS_DEVICE_MONO = 2,

        // Token: 0x04000407 RID: 1031
        BASS_DEVICE_3D = 4,

        // Token: 0x04000408 RID: 1032
        BASS_DEVICE_LATENCY = 256,

        // Token: 0x04000409 RID: 1033
        BASS_DEVICE_CPSPEAKERS = 1024,

        // Token: 0x0400040A RID: 1034
        BASS_DEVICE_SPEAKERS = 2048,

        // Token: 0x0400040B RID: 1035
        BASS_DEVICE_NOSPEAKER = 4096,

        // Token: 0x0400040C RID: 1036
        BASS_DEVIDE_DMIX = 8192,

        // Token: 0x0400040D RID: 1037
        BASS_DEVICE_FREQ = 16384,

        // Token: 0x0400040E RID: 1038
        BASS_DEVICE_STEREO = 32768
    }
}