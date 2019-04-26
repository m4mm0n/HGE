using System;

namespace HGE.IO.AudioEngine.BASSlib
{
    [Flags]
    public enum BASSChannelType
    {
        // Token: 0x04000420 RID: 1056
        BASS_CTYPE_MUSIC_IT = 131076,

        // Token: 0x04000421 RID: 1057
        BASS_CTYPE_MUSIC_MO3 = 256,

        // Token: 0x04000422 RID: 1058
        BASS_CTYPE_MUSIC_MOD = 131072,

        // Token: 0x04000423 RID: 1059
        BASS_CTYPE_MUSIC_MTM = 131073,

        // Token: 0x04000424 RID: 1060
        BASS_CTYPE_MUSIC_S3M = 131074,

        // Token: 0x04000425 RID: 1061
        BASS_CTYPE_MUSIC_XM = 131075,

        // Token: 0x04000426 RID: 1062
        BASS_CTYPE_RECORD = 2,

        // Token: 0x04000427 RID: 1063
        BASS_CTYPE_SAMPLE = 1,

        // Token: 0x04000428 RID: 1064
        BASS_CTYPE_STREAM = 65536,

        // Token: 0x04000429 RID: 1065
        BASS_CTYPE_STREAM_AAC = 68352,

        // Token: 0x0400042A RID: 1066
        BASS_CTYPE_STREAM_AC3 = 69632,

        // Token: 0x0400042B RID: 1067
        BASS_CTYPE_STREAM_ADX = 126976,

        // Token: 0x0400042C RID: 1068
        BASS_CTYPE_STREAM_AIFF = 65542,

        // Token: 0x0400042D RID: 1069
        BASS_CTYPE_STREAM_AIX = 126977,

        // Token: 0x0400042E RID: 1070
        BASS_CTYPE_STREAM_ALAC = 69120,

        // Token: 0x0400042F RID: 1071
        BASS_CTYPE_STREAM_APE = 67328,

        // Token: 0x04000430 RID: 1072
        BASS_CTYPE_STREAM_CA = 65543,

        // Token: 0x04000431 RID: 1073
        BASS_CTYPE_STREAM_CD = 66048,

        // Token: 0x04000432 RID: 1074
        BASS_CTYPE_STREAM_DSD = 71424,

        // Token: 0x04000433 RID: 1075
        BASS_CTYPE_STREAM_FLAC = 67840,

        // Token: 0x04000434 RID: 1076
        BASS_CTYPE_STREAM_FLAC_OGG = 67841,

        // Token: 0x04000435 RID: 1077
        BASS_CTYPE_STREAM_MF = 65544,

        // Token: 0x04000436 RID: 1078
        BASS_CTYPE_STREAM_MIDI = 68864,

        // Token: 0x04000437 RID: 1079
        BASS_CTYPE_STREAM_MIXER = 67584,

        // Token: 0x04000438 RID: 1080
        BASS_CTYPE_STREAM_MP1 = 65539,

        // Token: 0x04000439 RID: 1081
        BASS_CTYPE_STREAM_MP2 = 65540,

        // Token: 0x0400043A RID: 1082
        BASS_CTYPE_STREAM_MP3 = 65541,

        // Token: 0x0400043B RID: 1083
        BASS_CTYPE_STREAM_MP4 = 68353,

        // Token: 0x0400043C RID: 1084
        BASS_CTYPE_STREAM_MPC = 68096,

        // Token: 0x0400043D RID: 1085
        BASS_CTYPE_STREAM_OFR = 67072,

        // Token: 0x0400043E RID: 1086
        BASS_CTYPE_STREAM_OGG = 65538,

        // Token: 0x0400043F RID: 1087
        BASS_CTYPE_STREAM_OPUS = 70144,

        // Token: 0x04000440 RID: 1088
        BASS_CTYPE_STREAM_REVERSE = 127489,

        // Token: 0x04000441 RID: 1089
        BASS_CTYPE_STREAM_SPLIT = 67585,

        // Token: 0x04000442 RID: 1090
        BASS_CTYPE_STREAM_SPX = 68608,

        // Token: 0x04000443 RID: 1091
        BASS_CTYPE_STREAM_TEMPO = 127488,

        // Token: 0x04000444 RID: 1092
        BASS_CTYPE_STREAM_TTA = 69376,

        // Token: 0x04000445 RID: 1093
        BASS_CTYPE_STREAM_VIDEO = 69888,

        // Token: 0x04000446 RID: 1094
        BASS_CTYPE_STREAM_WAV = 262144,

        // Token: 0x04000447 RID: 1095
        BASS_CTYPE_STREAM_WAV_FLOAT = 327683,

        // Token: 0x04000448 RID: 1096
        BASS_CTYPE_STREAM_WAV_PCM = 327681,

        // Token: 0x04000449 RID: 1097
        BASS_CTYPE_STREAM_WINAMP = 66560,

        // Token: 0x0400044A RID: 1098
        BASS_CTYPE_STREAM_WMA = 66304,

        // Token: 0x0400044B RID: 1099
        BASS_CTYPE_STREAM_WMA_MP3 = 66305,

        // Token: 0x0400044C RID: 1100
        BASS_CTYPE_STREAM_WV = 66816,

        // Token: 0x0400044D RID: 1101
        BASS_CTYPE_STREAM_WV_H = 66817,

        // Token: 0x0400044E RID: 1102
        BASS_CTYPE_STREAM_WV_L = 66818,

        // Token: 0x0400044F RID: 1103
        BASS_CTYPE_STREAM_WV_LH = 66819,

        // Token: 0x04000450 RID: 1104
        BASS_CTYPE_UNKNOWN = 0
    }
}