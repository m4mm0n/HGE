using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HGE.Graphics;

namespace HGE.IO
{
    public class AudioEngine : IDisposable
    {
        // Token: 0x0200006C RID: 108
        public enum BASSActive
        {
            // Token: 0x04000386 RID: 902
            BASS_ACTIVE_STOPPED,

            // Token: 0x04000387 RID: 903
            BASS_ACTIVE_PLAYING,

            // Token: 0x04000388 RID: 904
            BASS_ACTIVE_STALLED,

            // Token: 0x04000389 RID: 905
            BASS_ACTIVE_PAUSED
        }

        // Token: 0x02000070 RID: 112
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

        // Token: 0x02000073 RID: 115
        public enum BASSConfig
        {
            // Token: 0x04000468 RID: 1128
            BASS_CONFIG_BUFFER,

            // Token: 0x04000469 RID: 1129
            BASS_CONFIG_UPDATEPERIOD,

            // Token: 0x0400046A RID: 1130
            BASS_CONFIG_GVOL_SAMPLE = 4,

            // Token: 0x0400046B RID: 1131
            BASS_CONFIG_GVOL_STREAM,

            // Token: 0x0400046C RID: 1132
            BASS_CONFIG_GVOL_MUSIC,

            // Token: 0x0400046D RID: 1133
            BASS_CONFIG_CURVE_VOL,

            // Token: 0x0400046E RID: 1134
            BASS_CONFIG_CURVE_PAN,

            // Token: 0x0400046F RID: 1135
            BASS_CONFIG_FLOATDSP,

            // Token: 0x04000470 RID: 1136
            BASS_CONFIG_3DALGORITHM,

            // Token: 0x04000471 RID: 1137
            BASS_CONFIG_NET_TIMEOUT,

            // Token: 0x04000472 RID: 1138
            BASS_CONFIG_NET_BUFFER,

            // Token: 0x04000473 RID: 1139
            BASS_CONFIG_PAUSE_NOPLAY,

            // Token: 0x04000474 RID: 1140
            BASS_CONFIG_NET_PREBUF = 15,

            // Token: 0x04000475 RID: 1141
            BASS_CONFIG_NET_AGENT,

            // Token: 0x04000476 RID: 1142
            BASS_CONFIG_NET_PROXY,

            // Token: 0x04000477 RID: 1143
            BASS_CONFIG_NET_PASSIVE,

            // Token: 0x04000478 RID: 1144
            BASS_CONFIG_REC_BUFFER,

            // Token: 0x04000479 RID: 1145
            BASS_CONFIG_NET_PLAYLIST = 21,

            // Token: 0x0400047A RID: 1146
            BASS_CONFIG_MUSIC_VIRTUAL,

            // Token: 0x0400047B RID: 1147
            BASS_CONFIG_VERIFY,

            // Token: 0x0400047C RID: 1148
            BASS_CONFIG_UPDATETHREADS,

            // Token: 0x0400047D RID: 1149
            BASS_CONFIG_DEV_BUFFER = 27,

            // Token: 0x0400047E RID: 1150
            BASS_CONFIG_VISTA_TRUEPOS = 30,

            // Token: 0x0400047F RID: 1151
            BASS_CONFIG_MP3_ERRORS = 35,

            // Token: 0x04000480 RID: 1152
            BASS_CONFIG_DEV_DEFAULT,

            // Token: 0x04000481 RID: 1153
            BASS_CONFIG_NET_READTIMEOUT,

            // Token: 0x04000482 RID: 1154
            BASS_CONFIG_VISTA_SPEAKERS,

            // Token: 0x04000483 RID: 1155
            BASS_CONFIG_HANDLES = 41,

            // Token: 0x04000484 RID: 1156
            BASS_CONFIG_UNICODE,

            // Token: 0x04000485 RID: 1157
            BASS_CONFIG_SRC,

            // Token: 0x04000486 RID: 1158
            BASS_CONFIG_SRC_SAMPLE,

            // Token: 0x04000487 RID: 1159
            BASS_CONFIG_ASYNCFILE_BUFFER,

            // Token: 0x04000488 RID: 1160
            BASS_CONFIG_OGG_PRESCAN = 47,

            // Token: 0x04000489 RID: 1161
            BASS_CONFIG_MF_VIDEO,

            // Token: 0x0400048A RID: 1162
            BASS_CONFIG_VERIFY_NET = 52,

            // Token: 0x0400048B RID: 1163
            BASS_CONFIG_AC3_DYNRNG = 65537,

            // Token: 0x0400048C RID: 1164
            BASS_CONFIG_WMA_PREBUF = 65793,

            // Token: 0x0400048D RID: 1165
            BASS_CONFIG_WMA_BASSFILE = 65795,

            // Token: 0x0400048E RID: 1166
            BASS_CONFIG_WMA_NETSEEK,

            // Token: 0x0400048F RID: 1167
            BASS_CONFIG_WMA_VIDEO,

            // Token: 0x04000490 RID: 1168
            BASS_CONFIG_WMA_ASYNC = 65807,

            // Token: 0x04000491 RID: 1169
            BASS_CONFIG_CD_FREEOLD = 66048,

            // Token: 0x04000492 RID: 1170
            BASS_CONFIG_CD_RETRY,

            // Token: 0x04000493 RID: 1171
            BASS_CONFIG_CD_AUTOSPEED,

            // Token: 0x04000494 RID: 1172
            BASS_CONFIG_CD_SKIPERROR,

            // Token: 0x04000495 RID: 1173
            BASS_CONFIG_CD_CDDB_SERVER,

            // Token: 0x04000496 RID: 1174
            BASS_CONFIG_ENCODE_PRIORITY = 66304,

            // Token: 0x04000497 RID: 1175
            BASS_CONFIG_ENCODE_QUEUE,

            // Token: 0x04000498 RID: 1176
            BASS_CONFIG_ENCODE_ACM_LOAD,

            // Token: 0x04000499 RID: 1177
            BASS_CONFIG_ENCODE_CAST_TIMEOUT = 66320,

            // Token: 0x0400049A RID: 1178
            BASS_CONFIG_ENCODE_CAST_PROXY,

            // Token: 0x0400049B RID: 1179
            BASS_CONFIG_MIDI_COMPACT = 66560,

            // Token: 0x0400049C RID: 1180
            BASS_CONFIG_MIDI_VOICES,

            // Token: 0x0400049D RID: 1181
            BASS_CONFIG_MIDI_AUTOFONT,

            // Token: 0x0400049E RID: 1182
            BASS_CONFIG_MIDI_DEFFONT,

            // Token: 0x0400049F RID: 1183
            BASS_CONFIG_MIDI_IN_PORTS,

            // Token: 0x040004A0 RID: 1184
            BASS_CONFIG_MIXER_FILTER = 67072,

            // Token: 0x040004A1 RID: 1185
            BASS_CONFIG_MIXER_BUFFER,

            // Token: 0x040004A2 RID: 1186
            BASS_CONFIG_MIXER_POSEX,

            // Token: 0x040004A3 RID: 1187
            BASS_CONFIG_SPLIT_BUFFER = 67088,

            // Token: 0x040004A4 RID: 1188
            BASS_CONFIG_MP4_VIDEO = 67328,

            // Token: 0x040004A5 RID: 1189
            BASS_CONFIG_AAC_PRESCAN = 67330,

            // Token: 0x040004A6 RID: 1190
            BASS_CONFIG_AAC_MP4 = 67329,

            // Token: 0x040004A7 RID: 1191
            BASS_CONFIG_WINAMP_INPUT_TIMEOUT = 67584,

            // Token: 0x040004A8 RID: 1192
            BASS_CONFIG_DSD_FREQ = 67584,

            // Token: 0x040004A9 RID: 1193
            BASS_CONFIG_DSD_GAIN
        }

        public enum BASSData
        {
            BASS_DATA_AVAILABLE = 0,
            BASS_DATA_FFT_INDIVIDUAL = 16,
            BASS_DATA_FFT_NOWINDOW = 32,
            BASS_DATA_FFT_REMOVEDC = 64,
            BASS_DATA_FFT_COMPLEX = 128,
            BASS_DATA_FIXED = 536870912,
            BASS_DATA_FLOAT = 1073741824,
            BASS_DATA_FFT256 = -2147483648,
            BASS_DATA_FFT512 = -2147483647,
            BASS_DATA_FFT1024 = -2147483646,
            BASS_DATA_FFT2048 = -2147483645,
            BASS_DATA_FFT4096 = -2147483644,
            BASS_DATA_FFT8192 = -2147483643,
            BASS_DATA_FFT16384 = -2147483642,
            BASS_DATA_FFT32768 = -2147483641
        }

        // Token: 0x0200006D RID: 109
        [Flags]
        public enum BASSFlag
        {
            // Token: 0x0400038B RID: 907
            BASS_AAC_FRAME960 = 4096,

            // Token: 0x0400038C RID: 908
            BASS_AAC_STEREO = 4194304,

            // Token: 0x0400038D RID: 909
            BASS_AC3_DOWNMIX_2 = 512,

            // Token: 0x0400038E RID: 910
            BASS_AC3_DOWNMIX_4 = 1024,

            // Token: 0x0400038F RID: 911
            BASS_AC3_DOWNMIX_DOLBY = 1536,

            // Token: 0x04000390 RID: 912
            BASS_AC3_DYNAMIC_RANGE = 2048,

            // Token: 0x04000391 RID: 913
            BASS_ASYNCFILE = 1073741824,

            // Token: 0x04000392 RID: 914
            BASS_CD_C2ERRORS = 2048,

            // Token: 0x04000393 RID: 915
            BASS_CD_SUBCHANNEL = 512,

            // Token: 0x04000394 RID: 916
            BASS_CD_SUBCHANNEL_NOHW = 1024,

            // Token: 0x04000395 RID: 917
            BASS_DEFAULT = 0,

            // Token: 0x04000396 RID: 918
            BASS_DSD_DOP = 1024,

            // Token: 0x04000397 RID: 919
            BASS_DSD_RAW = 512,

            // Token: 0x04000398 RID: 920
            BASS_DSHOW_NOAUDIO_PROC = 524288,

            // Token: 0x04000399 RID: 921
            BASS_DSHOW_STREAM_AUTODVD = 67108864,

            // Token: 0x0400039A RID: 922
            BASS_DSHOW_STREAM_LOOP = 134217728,

            // Token: 0x0400039B RID: 923
            BASS_DSHOW_STREAM_MIX = 16777216,

            // Token: 0x0400039C RID: 924
            BASS_DSHOW_STREAM_VIDEOPROC = 131072,

            // Token: 0x0400039D RID: 925
            BASS_FX_BPM_BKGRND = 1,

            // Token: 0x0400039E RID: 926
            BASS_FX_BPM_MULT2 = 2,

            // Token: 0x0400039F RID: 927
            BASS_FX_FREESOURCE = 65536,

            // Token: 0x040003A0 RID: 928
            BASS_FX_TEMPO_ALGO_CUBIC = 1024,

            // Token: 0x040003A1 RID: 929
            BASS_FX_TEMPO_ALGO_LINEAR = 512,

            // Token: 0x040003A2 RID: 930
            BASS_FX_TEMPO_ALGO_SHANNON = 2048,

            // Token: 0x040003A3 RID: 931
            BASS_MIDI_DECAYEND = 4096,

            // Token: 0x040003A4 RID: 932
            BASS_MIDI_DECAYSEEK = 16384,

            // Token: 0x040003A5 RID: 933
            BASS_MIDI_FONT_MMAP = 131072,

            // Token: 0x040003A6 RID: 934
            BASS_MIDI_FONT_NOFX = 524288,

            // Token: 0x040003A7 RID: 935
            BASS_MIDI_FONT_XGDRUMS = 262144,

            // Token: 0x040003A8 RID: 936
            BASS_MIDI_NOCROP = 32768,

            // Token: 0x040003A9 RID: 937
            BASS_MIDI_NOFX = 8192,

            // Token: 0x040003AA RID: 938
            BASS_MIDI_NOSYSRESET = 2048,

            // Token: 0x040003AB RID: 939
            BASS_MIDI_NOTEOFF1 = 65536,

            // Token: 0x040003AC RID: 940
            BASS_MIDI_PACK_16BIT = 2,

            // Token: 0x040003AD RID: 941
            BASS_MIDI_PACK_NOHEAD = 1,

            // Token: 0x040003AE RID: 942
            BASS_MIDI_SINCINTER = 8388608,

            // Token: 0x040003AF RID: 943
            BASS_MIXER_BUFFER = 8192,

            // Token: 0x040003B0 RID: 944
            BASS_MIXER_DOWNMIX = 4194304,

            // Token: 0x040003B1 RID: 945
            BASS_MIXER_END = 65536,

            // Token: 0x040003B2 RID: 946
            BASS_MIXER_LIMIT = 16384,

            // Token: 0x040003B3 RID: 947
            BASS_MIXER_MATRIX = 65536,

            // Token: 0x040003B4 RID: 948
            BASS_MIXER_NONSTOP = 131072,

            // Token: 0x040003B5 RID: 949
            BASS_MIXER_NORAMPIN = 8388608,

            // Token: 0x040003B6 RID: 950
            BASS_MIXER_PAUSE = 131072,

            // Token: 0x040003B7 RID: 951
            BASS_MIXER_POSEX = 8192,

            // Token: 0x040003B8 RID: 952
            BASS_MIXER_RESUME = 4096,

            // Token: 0x040003B9 RID: 953
            BASS_MUSIC_3D = 8,

            // Token: 0x040003BA RID: 954
            BASS_MUSIC_AUTOFREE = 262144,

            // Token: 0x040003BB RID: 955
            BASS_MUSIC_DECODE = 2097152,

            // Token: 0x040003BC RID: 956
            BASS_MUSIC_FLOAT = 256,

            // Token: 0x040003BD RID: 957
            BASS_MUSIC_FT2MOD = 8192,

            // Token: 0x040003BE RID: 958
            BASS_MUSIC_FT2PAN = 8192,

            // Token: 0x040003BF RID: 959
            BASS_MUSIC_FX = 128,

            // Token: 0x040003C0 RID: 960
            BASS_MUSIC_LOOP = 4,

            // Token: 0x040003C1 RID: 961
            BASS_MUSIC_MONO = 2,

            // Token: 0x040003C2 RID: 962
            BASS_MUSIC_NONINTER = 65536,

            // Token: 0x040003C3 RID: 963
            BASS_MUSIC_NOSAMPLE = 1048576,

            // Token: 0x040003C4 RID: 964
            BASS_MUSIC_POSRESET = 32768,

            // Token: 0x040003C5 RID: 965
            BASS_MUSIC_POSRESETEX = 4194304,

            // Token: 0x040003C6 RID: 966
            BASS_MUSIC_PRESCAN = 131072,

            // Token: 0x040003C7 RID: 967
            BASS_MUSIC_PT1MOD = 16384,

            // Token: 0x040003C8 RID: 968
            BASS_MUSIC_RAMP = 512,

            // Token: 0x040003C9 RID: 969
            BASS_MUSIC_RAMPS = 1024,

            // Token: 0x040003CA RID: 970
            BASS_MUSIC_SINCINTER = 8388608,

            // Token: 0x040003CB RID: 971
            BASS_MUSIC_STOPBACK = 524288,

            // Token: 0x040003CC RID: 972
            BASS_MUSIC_SURROUND = 2048,

            // Token: 0x040003CD RID: 973
            BASS_MUSIC_SURROUND2 = 4096,

            // Token: 0x040003CE RID: 974
            BASS_RECORD_AGC = 16384,

            // Token: 0x040003CF RID: 975
            BASS_RECORD_ECHOCANCEL = 8192,

            // Token: 0x040003D0 RID: 976
            BASS_RECORD_PAUSE = 32768,

            // Token: 0x040003D1 RID: 977
            BASS_SAMPLE_3D = 8,

            // Token: 0x040003D2 RID: 978
            BASS_SAMPLE_8BITS = 1,

            // Token: 0x040003D3 RID: 979
            BASS_SAMPLE_FLOAT = 256,

            // Token: 0x040003D4 RID: 980
            BASS_SAMPLE_FX = 128,

            // Token: 0x040003D5 RID: 981
            BASS_SAMPLE_LOOP = 4,

            // Token: 0x040003D6 RID: 982
            BASS_SAMPLE_MONO = 2,

            // Token: 0x040003D7 RID: 983
            BASS_SAMPLE_MUTEMAX = 32,

            // Token: 0x040003D8 RID: 984
            BASS_SAMPLE_OVER_DIST = 196608,

            // Token: 0x040003D9 RID: 985
            BASS_SAMPLE_OVER_POS = 131072,

            // Token: 0x040003DA RID: 986
            BASS_SAMPLE_OVER_VOL = 65536,

            // Token: 0x040003DB RID: 987
            BASS_SAMPLE_SOFTWARE = 16,

            // Token: 0x040003DC RID: 988
            BASS_SAMPLE_VAM = 64,

            // Token: 0x040003DD RID: 989
            BASS_SPEAKER_CENLFE = 50331648,

            // Token: 0x040003DE RID: 990
            BASS_SPEAKER_CENTER = 318767104,

            // Token: 0x040003DF RID: 991
            BASS_SPEAKER_FRONT = 16777216,

            // Token: 0x040003E0 RID: 992
            BASS_SPEAKER_FRONTLEFT = 285212672,

            // Token: 0x040003E1 RID: 993
            BASS_SPEAKER_FRONTRIGHT = 553648128,

            // Token: 0x040003E2 RID: 994
            BASS_SPEAKER_LEFT = 268435456,

            // Token: 0x040003E3 RID: 995
            BASS_SPEAKER_LFE = 587202560,

            // Token: 0x040003E4 RID: 996
            BASS_SPEAKER_PAIR1 = 16777216,

            // Token: 0x040003E5 RID: 997
            BASS_SPEAKER_PAIR10 = 167772160,

            // Token: 0x040003E6 RID: 998
            BASS_SPEAKER_PAIR11 = 184549376,

            // Token: 0x040003E7 RID: 999
            BASS_SPEAKER_PAIR12 = 201326592,

            // Token: 0x040003E8 RID: 1000
            BASS_SPEAKER_PAIR13 = 218103808,

            // Token: 0x040003E9 RID: 1001
            BASS_SPEAKER_PAIR14 = 234881024,

            // Token: 0x040003EA RID: 1002
            BASS_SPEAKER_PAIR15 = 251658240,

            // Token: 0x040003EB RID: 1003
            BASS_SPEAKER_PAIR2 = 33554432,

            // Token: 0x040003EC RID: 1004
            BASS_SPEAKER_PAIR3 = 50331648,

            // Token: 0x040003ED RID: 1005
            BASS_SPEAKER_PAIR4 = 67108864,

            // Token: 0x040003EE RID: 1006
            BASS_SPEAKER_PAIR5 = 83886080,

            // Token: 0x040003EF RID: 1007
            BASS_SPEAKER_PAIR6 = 100663296,

            // Token: 0x040003F0 RID: 1008
            BASS_SPEAKER_PAIR7 = 117440512,

            // Token: 0x040003F1 RID: 1009
            BASS_SPEAKER_PAIR8 = 134217728,

            // Token: 0x040003F2 RID: 1010
            BASS_SPEAKER_PAIR9 = 150994944,

            // Token: 0x040003F3 RID: 1011
            BASS_SPEAKER_REAR = 33554432,

            // Token: 0x040003F4 RID: 1012
            BASS_SPEAKER_REAR2 = 67108864,

            // Token: 0x040003F5 RID: 1013
            BASS_SPEAKER_REAR2LEFT = 335544320,

            // Token: 0x040003F6 RID: 1014
            BASS_SPEAKER_REAR2RIGHT = 603979776,

            // Token: 0x040003F7 RID: 1015
            BASS_SPEAKER_REARLEFT = 301989888,

            // Token: 0x040003F8 RID: 1016
            BASS_SPEAKER_REARRIGHT = 570425344,

            // Token: 0x040003F9 RID: 1017
            BASS_SPEAKER_RIGHT = 536870912,

            // Token: 0x040003FA RID: 1018
            BASS_SPLIT_SLAVE = 4096,

            // Token: 0x040003FB RID: 1019
            BASS_STREAM_AUTOFREE = 262144,

            // Token: 0x040003FC RID: 1020
            BASS_STREAM_BLOCK = 1048576,

            // Token: 0x040003FD RID: 1021
            BASS_STREAM_DECODE = 2097152,

            // Token: 0x040003FE RID: 1022
            BASS_STREAM_PRESCAN = 131072,

            // Token: 0x040003FF RID: 1023
            BASS_STREAM_RESTRATE = 524288,

            // Token: 0x04000400 RID: 1024
            BASS_STREAM_STATUS = 8388608,

            // Token: 0x04000401 RID: 1025
            BASS_UNICODE = -2147483648,

            // Token: 0x04000402 RID: 1026
            BASS_WV_STEREO = 4194304
        }

        // Token: 0x0200006E RID: 110
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

        // Token: 0x02000072 RID: 114
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

        public enum BASSTag
        {
            // Token: 0x04000361 RID: 865
            BASS_TAG_ID3,

            // Token: 0x04000362 RID: 866
            BASS_TAG_ID3V2,

            // Token: 0x04000363 RID: 867
            BASS_TAG_OGG,

            // Token: 0x04000364 RID: 868
            BASS_TAG_HTTP,

            // Token: 0x04000365 RID: 869
            BASS_TAG_ICY,

            // Token: 0x04000366 RID: 870
            BASS_TAG_META,

            // Token: 0x04000367 RID: 871
            BASS_TAG_APE,

            // Token: 0x04000368 RID: 872
            BASS_TAG_MP4,

            // Token: 0x04000369 RID: 873
            BASS_TAG_WMA,

            // Token: 0x0400036A RID: 874
            BASS_TAG_VENDOR,

            // Token: 0x0400036B RID: 875
            BASS_TAG_LYRICS3,

            // Token: 0x0400036C RID: 876
            BASS_TAG_WMA_META,

            // Token: 0x0400036D RID: 877
            BASS_TAG_CA_CODEC = 11,

            // Token: 0x0400036E RID: 878
            BASS_TAG_FLAC_CUE,

            // Token: 0x0400036F RID: 879
            BASS_TAG_WMA_CODEC = 12,

            // Token: 0x04000370 RID: 880
            BASS_TAG_MF,

            // Token: 0x04000371 RID: 881
            BASS_TAG_WAVEFORMAT,

            // Token: 0x04000372 RID: 882
            BASS_TAG_RIFF_INFO = 256,

            // Token: 0x04000373 RID: 883
            BASS_TAG_RIFF_BEXT,

            // Token: 0x04000374 RID: 884
            BASS_TAG_RIFF_CART,

            // Token: 0x04000375 RID: 885
            BASS_TAG_RIFF_DISP,

            // Token: 0x04000376 RID: 886
            BASS_TAG_APE_BINARY = 4096,

            // Token: 0x04000377 RID: 887
            BASS_TAG_MUSIC_NAME = 65536,

            // Token: 0x04000378 RID: 888
            BASS_TAG_MUSIC_MESSAGE,

            // Token: 0x04000379 RID: 889
            BASS_TAG_MUSIC_ORDERS,

            // Token: 0x0400037A RID: 890
            BASS_TAG_MUSIC_AUTH,

            // Token: 0x0400037B RID: 891
            BASS_TAG_MUSIC_INST = 65792,

            // Token: 0x0400037C RID: 892
            BASS_TAG_MUSIC_SAMPLE = 66304,

            // Token: 0x0400037D RID: 893
            BASS_TAG_MIDI_TRACK = 69632,

            // Token: 0x0400037E RID: 894
            BASS_TAG_ADX_LOOP = 73728,

            // Token: 0x0400037F RID: 895
            BASS_TAG_FLAC_PICTURE = 73728,

            // Token: 0x04000380 RID: 896
            BASS_TAG_DSD_ARTIST = 77824,

            // Token: 0x04000381 RID: 897
            BASS_TAG_DSD_TITLE,

            // Token: 0x04000382 RID: 898
            BASS_TAG_DSD_COMMENT = 78080,

            // Token: 0x04000383 RID: 899
            BASS_TAG_HLS_EXTINF = 81920,

            // Token: 0x04000384 RID: 900
            BASS_TAG_UNKNOWN = -1
        }

        private readonly float[] _fft = new float[2048];
        private readonly float[] _lastPeak = new float[2048];
        private readonly int _scaleFactorLinear = 9;
        private readonly float _scaleFactorLinearBoost = 0.05f;
        private readonly int _scaleFactorSqr = 4;
        private readonly float _scaleFactorSqrBoost = 0.005f;

        private readonly BASS_Free BassFree;

        // Token: 0x040001FE RID: 510
        private readonly BASS_ChannelPlay ChannelPlay;

        // Token: 0x040001FF RID: 511
        private readonly BASS_ChannelStop ChannelStop;

        // Token: 0x0400020B RID: 523
        private readonly BASS_ChannelBytes2Seconds GetBytes2Seconds;

        // Token: 0x04000206 RID: 518
        private readonly BASS_ChannelGetLength GetChanLength;

        // Token: 0x0400020E RID: 526
        private readonly BASS_ChannelGetLevel GetChannelLevel;

        // Token: 0x0400020F RID: 527
        private readonly BASS_ChannelGetTags GetChannelTags;

        // Token: 0x04000201 RID: 513
        private readonly BASS_ChannelGetData GetData;

        // Token: 0x04000202 RID: 514
        private readonly BASS_ChannelGetData2 GetData2;

        // Token: 0x04000203 RID: 515
        private readonly BASS_ChannelGetData3 GetData3;

        // Token: 0x04000204 RID: 516
        private readonly BASS_ChannelGetData4 GetData4;

        // Token: 0x04000205 RID: 517
        private readonly BASS_ChannelGetInfo GetInfo;

        // Token: 0x04000208 RID: 520
        private readonly BASS_ChannelIsActive GetIsActive;

        // Token: 0x04000207 RID: 519
        private readonly BASS_ChannelSeconds2Bytes GetSeconds2Bytes;

        // Token: 0x04000211 RID: 529
        private readonly BASS_Init InitBass;
        private readonly DynamicNativeLibrary memDll;

        // Token: 0x04000200 RID: 512
        private readonly BASS_MusicLoad MusicLoadMemory;

        // Token: 0x0400020A RID: 522
        private readonly BASS_ChannelSetPosition SetChannelPos;

        // Token: 0x0400020C RID: 524
        private readonly BASS_StreamCreateFile StreamCreateFileMemory;

        // Token: 0x0400020D RID: 525
        private readonly BASS_StreamFree StreamFreeA;
        private BASSData _maxFFT = BASSData.BASS_DATA_FFT4096;
        private int _maxFFTData = 4096;
        private int _maxFFTSampleIndex = 2047;
        private int _maxFrequencySpectrum = 2047;

        // Token: 0x040001FD RID: 509
        private BASS_ChannelPause ChannelPause;

        // Token: 0x04000210 RID: 528
        private BASS_ErrorCode GetErrorCode;

        // Token: 0x04000209 RID: 521
        private BASS_SetConfig SetConfig;
        private double spp = 1.0;

        public AudioEngine()
        {
            for (var i = 0; i < _lastPeak.Length; i++) _lastPeak[i] = 0f;

            memDll = new DynamicNativeLibrary(Assembly.GetExecutingAssembly().FindEmbedded("bass.dll"));
            InitBass = (BASS_Init) memDll.GetDelegateForFunction("BASS_Init", typeof(BASS_Init));

            if (InitBass(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero, IntPtr.Zero))
            {
                BassFree = (BASS_Free) memDll.GetDelegateForFunction("BASS_Free", typeof(BASS_Free));
                ChannelPause =
                    (BASS_ChannelPause) memDll.GetDelegateForFunction("BASS_ChannelPause", typeof(BASS_ChannelPause));
                ChannelStop =
                    (BASS_ChannelStop) memDll.GetDelegateForFunction("BASS_ChannelStop", typeof(BASS_ChannelStop));
                ChannelPlay =
                    (BASS_ChannelPlay) memDll.GetDelegateForFunction("BASS_ChannelPlay", typeof(BASS_ChannelPlay));
                MusicLoadMemory =
                    (BASS_MusicLoad) memDll.GetDelegateForFunction("BASS_MusicLoad", typeof(BASS_MusicLoad));
                GetData = (BASS_ChannelGetData) memDll.GetDelegateForFunction("BASS_ChannelGetData",
                    typeof(BASS_ChannelGetData));
                GetData2 = (BASS_ChannelGetData2) memDll.GetDelegateForFunction("BASS_ChannelGetData",
                    typeof(BASS_ChannelGetData2));
                GetData3 = (BASS_ChannelGetData3) memDll.GetDelegateForFunction("BASS_ChannelGetData",
                    typeof(BASS_ChannelGetData3));
                GetData4 = (BASS_ChannelGetData4) memDll.GetDelegateForFunction("BASS_ChannelGetData",
                    typeof(BASS_ChannelGetData4));
                GetInfo = (BASS_ChannelGetInfo) memDll.GetDelegateForFunction("BASS_ChannelGetInfo",
                    typeof(BASS_ChannelGetInfo));
                GetChanLength =
                    (BASS_ChannelGetLength) memDll.GetDelegateForFunction("BASS_ChannelGetLength",
                        typeof(BASS_ChannelGetLength));
                GetSeconds2Bytes =
                    (BASS_ChannelSeconds2Bytes) memDll.GetDelegateForFunction("BASS_ChannelSeconds2Bytes",
                        typeof(BASS_ChannelSeconds2Bytes));
                SetConfig = (BASS_SetConfig) memDll.GetDelegateForFunction("BASS_SetConfig", typeof(BASS_SetConfig));
                GetIsActive =
                    (BASS_ChannelIsActive) memDll.GetDelegateForFunction("BASS_ChannelIsActive",
                        typeof(BASS_ChannelIsActive));
                SetChannelPos = (BASS_ChannelSetPosition) memDll.GetDelegateForFunction("BASS_ChannelSetPosition",
                    typeof(BASS_ChannelSetPosition));
                GetBytes2Seconds =
                    (BASS_ChannelBytes2Seconds) memDll.GetDelegateForFunction("BASS_ChannelBytes2Seconds",
                        typeof(BASS_ChannelBytes2Seconds));
                StreamCreateFileMemory =
                    (BASS_StreamCreateFile) memDll.GetDelegateForFunction("BASS_StreamCreateFile",
                        typeof(BASS_StreamCreateFile));
                StreamFreeA =
                    (BASS_StreamFree) memDll.GetDelegateForFunction("BASS_StreamFree", typeof(BASS_StreamFree));
                GetChannelLevel =
                    (BASS_ChannelGetLevel) memDll.GetDelegateForFunction("BASS_ChannelGetLevel",
                        typeof(BASS_ChannelGetLevel));
                GetChannelTags =
                    (BASS_ChannelGetTags) memDll.GetDelegateForFunction("BASS_ChannelGetTags",
                        typeof(BASS_ChannelGetTags));
                GetErrorCode =
                    (BASS_ErrorCode) memDll.GetDelegateForFunction("BASS_ErrorGetCode", typeof(BASS_ErrorCode));
            }
        }

        public BASSData MaxFFT
        {
            get => _maxFFT;
            set
            {
                switch (value)
                {
                    case BASSData.BASS_DATA_FFT512:
                        _maxFFTData = 1024;
                        _maxFFT = value;
                        _maxFFTSampleIndex = 255;
                        break;
                    case BASSData.BASS_DATA_FFT1024:
                        _maxFFTData = 1024;
                        _maxFFT = value;
                        _maxFFTSampleIndex = 511;
                        break;
                    case BASSData.BASS_DATA_FFT2048:
                        _maxFFTData = 2048;
                        _maxFFT = value;
                        _maxFFTSampleIndex = 1023;
                        break;
                    case BASSData.BASS_DATA_FFT4096:
                        _maxFFTData = 4096;
                        _maxFFT = value;
                        _maxFFTSampleIndex = 2047;
                        break;
                    case BASSData.BASS_DATA_FFT8192:
                        _maxFFTData = 8192;
                        _maxFFT = value;
                        _maxFFTSampleIndex = 4095;
                        break;
                    default:
                        _maxFFTData = 4096;
                        _maxFFT = BASSData.BASS_DATA_FFT4096;
                        _maxFFTSampleIndex = 2047;
                        break;
                }

                if (_maxFrequencySpectrum > _maxFFTSampleIndex) _maxFrequencySpectrum = _maxFFTSampleIndex;
            }
        }

        public int MaxFrequencySpectrum
        {
            get => _maxFrequencySpectrum;
            set
            {
                if (value > _maxFFTSampleIndex) _maxFrequencySpectrum = _maxFFTSampleIndex;
                if (value < 1)
                {
                    _maxFrequencySpectrum = 1;
                    return;
                }

                _maxFrequencySpectrum = value;
            }
        }

        public void Dispose()
        {
            Free();
            memDll?.Dispose();
        }

        public float GetPeakFreq(int channel)
        {
            var m_fft = new float[2048];
            GetData(channel, m_fft, (int) BASSData.BASS_DATA_FFT4096);
            var m_peak = 0f;
            var m_peaki = 0;
            for (var a = 2; a < 2047; a++)
                if (m_peak < m_fft[a])
                {
                    // found peak
                    m_peak = m_fft[a];
                    m_peaki = a;
                }

            if (m_peaki == 0) return 0; // no sound
            m_peak = (float) (m_peaki + 0.8721 *
                              Math.Sin((m_fft[m_peaki + 1] - m_fft[m_peaki - 1]) / m_fft[m_peaki] * 0.7632)
                ); // tweak the bin
            var ci = new BASS_CHANNELINFO_INTERNAL();
            GetInfo(channel, ref ci);

            return m_peak * ci.freq / 4096;
        }

        public void DrawSpectrum(Graphics2DGL g, int playHandle, int width, int height, Pixel color1, Pixel color2,
            int lineW, int distance, int peakD, bool fullSpec = true)
        {
            if (GetData(playHandle, _fft, (int) MaxFFT) > 0)
                DrawSpectrumLinePeak(g, width, height, color1, color2, lineW, distance, peakD, Pixel.BLACK, true,
                    fullSpec);
        }

        public bool Play(int handle)
        {
            return ChannelPlay(handle, true);
        }

        // Token: 0x060002D0 RID: 720 RVA: 0x000178A3 File Offset: 0x00015AA3
        public int LoadMusic(byte[] trackToLoad)
        {
            return MusicLoadMemory(true, trackToLoad, 0L, trackToLoad.Length,
                BASSFlag.BASS_MUSIC_LOOP | BASSFlag.BASS_MUSIC_RAMPS, 1);
        }

        // Token: 0x060002D1 RID: 721 RVA: 0x000178B9 File Offset: 0x00015AB9
        public bool Stop(int handle)
        {
            return ChannelStop(handle);
        }

        // Token: 0x060002D2 RID: 722 RVA: 0x000178C7 File Offset: 0x00015AC7
        public void Free()
        {
            BassFree();
        }

        // Token: 0x060002D3 RID: 723 RVA: 0x000178D8 File Offset: 0x00015AD8
        public string GetTrackName(int handle)
        {
            var getChannelTags = GetChannelTags;
            var intPtr = getChannelTags != null ? getChannelTags(handle, BASSTag.BASS_TAG_MUSIC_NAME) : (IntPtr) 0;
            if (intPtr != IntPtr.Zero) return "Track: " + IntPtrAsStringAnsi(intPtr);
            return "";
        }

        // Token: 0x060002D4 RID: 724 RVA: 0x00017923 File Offset: 0x00015B23
        public bool ChannelSetPosition(int handle, long pos)
        {
            return SetChannelPos(handle, pos, BASSMode.BASS_POS_BYTES);
        }

        // Token: 0x060002D5 RID: 725 RVA: 0x00017933 File Offset: 0x00015B33
        public long ChannelSeconds2Bytes(int handle, double pos)
        {
            return GetSeconds2Bytes(handle, pos);
        }

        // Token: 0x060002D6 RID: 726 RVA: 0x00017942 File Offset: 0x00015B42
        public long ChannelGetLength(int handle)
        {
            return GetChanLength(handle, BASSMode.BASS_POS_BYTES);
        }

        // Token: 0x060002D7 RID: 727 RVA: 0x00017951 File Offset: 0x00015B51
        public BASSActive ChannelIsActive(int handle)
        {
            return GetIsActive(handle);
        }

        // Token: 0x060002D8 RID: 728 RVA: 0x0001795F File Offset: 0x00015B5F
        public int ChannelGetData(int handle, [In] [Out] float[] buffer, int length)
        {
            return GetData(handle, buffer, length);
        }

        // Token: 0x060002D9 RID: 729 RVA: 0x0001796F File Offset: 0x00015B6F
        public int ChannelGetData(int handle, [In] [Out] short[] buffer, int length)
        {
            return GetData4(handle, buffer, length);
        }

        // Token: 0x060002DA RID: 730 RVA: 0x0001797F File Offset: 0x00015B7F
        public int ChannelGetData(int handle, [In] [Out] byte[] buffer, int length)
        {
            return GetData2(handle, buffer, length);
        }

        // Token: 0x060002DB RID: 731 RVA: 0x0001798F File Offset: 0x00015B8F
        public int ChannelGetData(int handle, [In] [Out] IntPtr buffer, int length)
        {
            return GetData3(handle, buffer, length);
        }

        // Token: 0x060002DC RID: 732 RVA: 0x0001799F File Offset: 0x00015B9F
        public double ChannelBytes2Seconds(int handle, long pos)
        {
            return GetBytes2Seconds(handle, pos);
        }

        // Token: 0x060002DD RID: 733 RVA: 0x000179B0 File Offset: 0x00015BB0
        public bool ChannelGetInfo(int handle, BASS_CHANNELINFO info)
        {
            var expr_0C = GetInfo(handle, ref info._internal);
            if (expr_0C)
            {
                info.chans = info._internal.chans;
                info.ctype = info._internal.ctype;
                info.flags = info._internal.flags;
                info.freq = info._internal.freq;
                info.origres = info._internal.origres;
                info.plugin = info._internal.plugin;
                info.sample = info._internal.sample;
                if ((info.flags & BASSFlag.BASS_UNICODE) != BASSFlag.BASS_DEFAULT)
                {
                    info.filename = Marshal.PtrToStringUni(info._internal.filename);
                    return expr_0C;
                }

                info.filename = IntPtrAsStringAnsi(info._internal.filename);
            }

            return expr_0C;
        }

        // Token: 0x060002DE RID: 734 RVA: 0x00017A8B File Offset: 0x00015C8B
        public int StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
        {
            return StreamCreateFileMemory(true, memory, offset, length, flags);
        }

        // Token: 0x060002DF RID: 735 RVA: 0x00017A9E File Offset: 0x00015C9E
        public bool StreamFree(int handle)
        {
            return StreamFreeA(handle);
        }

        // Token: 0x060002E0 RID: 736 RVA: 0x00017AAC File Offset: 0x00015CAC
        public int ChannelGetLevel(int handle)
        {
            return GetChannelLevel(handle);
        }


        private int IntPtrNullTermLength(IntPtr p)
        {
            var num = 0;
            while (Marshal.ReadByte(p, num) != 0) num++;
            return num;
        }

        public string IntPtrAsStringUtf8orLatin1(IntPtr utf8Ptr, out int len)
        {
            len = 0;
            if (utf8Ptr != IntPtr.Zero)
            {
                len = IntPtrNullTermLength(utf8Ptr);
                if (len != 0)
                {
                    var array = new byte[len];
                    Marshal.Copy(utf8Ptr, array, 0, len);
                    var text = Encoding.Default.GetString(array, 0, len);
                    try
                    {
                        var @string = new UTF8Encoding(false, true).GetString(array, 0, len);
                        if (@string.Length < text.Length) return @string;
                    }
                    catch
                    {
                    }

                    return text;
                }
            }

            return null;
        }

        public string IntPtrAsStringAnsi(IntPtr ansiPtr)
        {
            var num = 0;
            return IntPtrAsStringUtf8orLatin1(ansiPtr, out num);
        }

        private void DrawSpectrumLinePeak(Graphics2DGL g, int width, int height, Pixel p1, Pixel p2, int lineWidth,
            int distance, int peakDelay, Pixel background, bool linear, bool fullSpectrum = true)
        {
            g.FillRect(0, 0, width, height, background);
            var num = 0f;
            var num2 = 0f;
            spp = MaxFrequencySpectrum / (double) height;
            var num3 = MaxFrequencySpectrum + 1;

            if (!fullSpectrum)
            {
                spp = 1.0;
                num3 = width + 1;
                if (num3 > _maxFFTSampleIndex + 1)
                    num3 = _maxFFTSampleIndex + 1;
            }

            var num4 = 0;
            var num5 = 1f + num4 * (linear ? _scaleFactorLinearBoost : _scaleFactorSqrBoost);

            for (var i = 1; i < num3; i++)
            {
                var num6 = linear
                    ? _fft[i] * _scaleFactorLinear * num5 * height
                    : (float) Math.Sqrt(_fft[i] * num5) * _scaleFactorSqr * height - 4f;
                num2 += num6;
                //var num7 = (float) Math.Round((double) i / spp) - 1f;
                var num7 = (float) Math.Round(i / spp) - 1f;
                if ((int) num7 % (distance + lineWidth) == 0 && num7 > num)
                {
                    num2 /= distance + lineWidth;
                    if (num2 > height) num2 = height;
                    if (num2 < 0f) num2 = 0f;

                    var num8 = num2;
                    if (_lastPeak[(int) num7] < num8)
                        _lastPeak[(int) num7] = num8;
                    else
                        _lastPeak[(int) num7] = (num8 + peakDelay * _lastPeak[(int) num7]) / (peakDelay + 1);

                    //g.DrawLine((int) (num + (float) (lineWidth / 2) + 1f), (int) ((float) height - 1f),
                    //    (int) (num + (float) (lineWidth / 2) + 1f), (int) ((float) height - 1f - num2), p1);
                    g.FillRect((int) (num + 1f), (int) (height - 1f - num2), lineWidth,
                        (int) (height - 1f) - (int) (height - 1f - num2), p1);


                    var num9 = 1f;
                    g.DrawLine((int) (num + 1f), (int) (height - 1f / 2f - _lastPeak[(int) num7]),
                        (int) (num + lineWidth + num9 + 1f),
                        (int) (height - 1f / 2f - _lastPeak[(int) num7]), p2);

                    num = num7;
                    num2 = 0f;
                    num4++;
                    num5 = 1f + num4 * (linear ? _scaleFactorLinearBoost : _scaleFactorSqrBoost);
                }
            }
        }

        public static string BASSChannelTypeToString(BASSChannelType ctype)
        {
            var str = "???";
            if ((ctype & BASSChannelType.BASS_CTYPE_STREAM_WAV) > BASSChannelType.BASS_CTYPE_UNKNOWN)
                ctype = BASSChannelType.BASS_CTYPE_STREAM_WAV;
            if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_AAC)
                if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_WINAMP)
                {
                    if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_MF)
                        switch (ctype)
                        {
                            case BASSChannelType.BASS_CTYPE_UNKNOWN:
                                return "Unknown";
                            case BASSChannelType.BASS_CTYPE_SAMPLE:
                                return "Sample";
                            case BASSChannelType.BASS_CTYPE_RECORD:
                                return "Recording";
                            default:
                                if (ctype == BASSChannelType.BASS_CTYPE_MUSIC_MO3) return "MO3";
                                switch (ctype)
                                {
                                    case BASSChannelType.BASS_CTYPE_STREAM:
                                        return "Custom Stream";
                                    case BASSChannelType.BASS_CTYPE_SAMPLE | BASSChannelType.BASS_CTYPE_STREAM:
                                        return str;
                                    case BASSChannelType.BASS_CTYPE_STREAM_OGG:
                                        return "OGG";
                                    case BASSChannelType.BASS_CTYPE_STREAM_MP1:
                                        return "MP1";
                                    case BASSChannelType.BASS_CTYPE_STREAM_MP2:
                                        return "MP2";
                                    case BASSChannelType.BASS_CTYPE_STREAM_MP3:
                                        return "MP3";
                                    case BASSChannelType.BASS_CTYPE_STREAM_AIFF:
                                        return "AIFF";
                                    case BASSChannelType.BASS_CTYPE_STREAM_CA:
                                        return "CoreAudio";
                                    case BASSChannelType.BASS_CTYPE_STREAM_MF:
                                        return "MF";
                                    default:
                                        return str;
                                }

                                break;
                        }

                    if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_WMA)
                    {
                        if (ctype == BASSChannelType.BASS_CTYPE_STREAM_CD) return "CDA";
                        if (ctype != BASSChannelType.BASS_CTYPE_STREAM_WMA) return str;
                        return "WMA";
                    }

                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_WMA_MP3) return "MP3";
                    if (ctype != BASSChannelType.BASS_CTYPE_STREAM_WINAMP) return str;
                    return "Winamp";
                }
                else if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_MIXER)
                {
                    if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_OFR)
                        switch (ctype)
                        {
                            case BASSChannelType.BASS_CTYPE_STREAM_WV:
                                return "Wavpack";
                            case BASSChannelType.BASS_CTYPE_STREAM_WV_H:
                                return "Wavpack";
                            case BASSChannelType.BASS_CTYPE_STREAM_WV_L:
                                return "Wavpack";
                            case BASSChannelType.BASS_CTYPE_STREAM_WV_LH:
                                return "Wavpack";
                            default:
                                if (ctype != BASSChannelType.BASS_CTYPE_STREAM_OFR) return str;
                                return "Optimfrog";
                        }

                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_APE) return "APE";
                    if (ctype != BASSChannelType.BASS_CTYPE_STREAM_MIXER) return str;
                    return "Mixer";
                }
                else if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_FLAC)
                {
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_SPLIT) return "Splitter";
                    if (ctype != BASSChannelType.BASS_CTYPE_STREAM_FLAC) return str;
                    return "FLAC";
                }
                else
                {
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_MPC) return "MPC";
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_AAC) return "AAC";
                    return str;
                }

            if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_VIDEO)
                if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_MIDI)
                {
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_MP4) return "MP4";
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_SPX) return "Speex";
                    if (ctype != BASSChannelType.BASS_CTYPE_STREAM_MIDI) return str;
                    return "MIDI";
                }
                else if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_TTA)
                {
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_ALAC) return "ALAC";
                    if (ctype != BASSChannelType.BASS_CTYPE_STREAM_TTA) return str;
                    return "TTA";
                }
                else
                {
                    if (ctype == BASSChannelType.BASS_CTYPE_STREAM_AC3) return "AC3";
                    if (ctype != BASSChannelType.BASS_CTYPE_STREAM_VIDEO) return str;
                    return "Video";
                }

            if (ctype > BASSChannelType.BASS_CTYPE_STREAM_AIX)
            {
                if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_WAV)
                    switch (ctype)
                    {
                        case BASSChannelType.BASS_CTYPE_MUSIC_MOD:
                            return "MOD";
                        case BASSChannelType.BASS_CTYPE_MUSIC_MTM:
                            return "MTM";
                        case BASSChannelType.BASS_CTYPE_MUSIC_S3M:
                            return "S3M";
                        case BASSChannelType.BASS_CTYPE_MUSIC_XM:
                            return "XM";
                        case BASSChannelType.BASS_CTYPE_MUSIC_IT:
                            return "IT";
                        default:
                            if (ctype != BASSChannelType.BASS_CTYPE_STREAM_WAV) return str;
                            break;
                    }
                else if (ctype != BASSChannelType.BASS_CTYPE_STREAM_WAV_PCM &&
                         ctype != BASSChannelType.BASS_CTYPE_STREAM_WAV_FLOAT) return str;
                return "WAV";
            }

            if (ctype <= BASSChannelType.BASS_CTYPE_STREAM_DSD)
            {
                if (ctype == BASSChannelType.BASS_CTYPE_STREAM_OPUS) return "OPUS";
                if (ctype != BASSChannelType.BASS_CTYPE_STREAM_DSD) return str;
                return "DSD";
            }

            if (ctype == BASSChannelType.BASS_CTYPE_STREAM_ADX) return "ADX";
            if (ctype != BASSChannelType.BASS_CTYPE_STREAM_AIX) return str;
            return "AIX";
        }

        public static string ChannelNumberToString(int chans)
        {
            var str = chans.ToString();
            switch (chans)
            {
                case 1:
                    return "Mono";
                case 2:
                    return "Stereo";
                case 3:
                    return "2.1";
                case 4:
                    return "2.2";
                case 5:
                    return "4.1";
                case 6:
                    return "5.1";
                case 7:
                    return "5.2";
                case 8:
                    return "7.1";
                default:
                    return str;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_ErrorCode();

        // Token: 0x02000056 RID: 86
        // (Invoke) Token: 0x06000324 RID: 804
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_MusicLoad([MarshalAs(UnmanagedType.Bool)] bool mem, byte[] memory, long offset,
            int length, BASSFlag flags, int freq);

        // Token: 0x02000057 RID: 87
        // (Invoke) Token: 0x06000328 RID: 808
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_ChannelPlay(int handle, [MarshalAs(UnmanagedType.Bool)] bool restart);

        // Token: 0x02000058 RID: 88
        // (Invoke) Token: 0x0600032C RID: 812
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, IntPtr clsid);

        // Token: 0x02000059 RID: 89
        // (Invoke) Token: 0x06000330 RID: 816
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_ChannelPause(int handle);

        // Token: 0x0200005A RID: 90
        // (Invoke) Token: 0x06000334 RID: 820
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_ChannelStop(int handle);

        // Token: 0x0200005B RID: 91
        // (Invoke) Token: 0x06000338 RID: 824
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_Free();

        // Token: 0x0200005C RID: 92
        // (Invoke) Token: 0x0600033C RID: 828
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate long BASS_ChannelSeconds2Bytes(int handle, double pos);

        // Token: 0x0200005D RID: 93
        // (Invoke) Token: 0x06000340 RID: 832
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_ChannelGetData(int handle, [In] [Out] float[] buffer, int length);

        // Token: 0x0200005E RID: 94
        // (Invoke) Token: 0x06000344 RID: 836
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_ChannelGetData2(int handle, [In] [Out] byte[] buffer, int length);

        // Token: 0x0200005F RID: 95
        // (Invoke) Token: 0x06000348 RID: 840
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_ChannelGetData3(int handle, IntPtr buffer, int length);

        // Token: 0x02000060 RID: 96
        // (Invoke) Token: 0x0600034C RID: 844
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_ChannelGetData4(int handle, [In] [Out] short[] buffer, int length);

        // Token: 0x02000061 RID: 97
        // (Invoke) Token: 0x06000350 RID: 848
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_ChannelGetInfo(int handle, [In] [Out] ref BASS_CHANNELINFO_INTERNAL info);

        // Token: 0x02000062 RID: 98
        // (Invoke) Token: 0x06000354 RID: 852
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate long BASS_ChannelGetLength(int handle, BASSMode mode);

        // Token: 0x02000063 RID: 99
        // (Invoke) Token: 0x06000358 RID: 856
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_SetConfig(BASSConfig option, int newvalue);

        // Token: 0x02000064 RID: 100
        // (Invoke) Token: 0x0600035C RID: 860
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate BASSActive BASS_ChannelIsActive(int handle);

        // Token: 0x02000065 RID: 101
        // (Invoke) Token: 0x06000360 RID: 864
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_ChannelSetPosition(int handle, long pos, BASSMode mode);

        // Token: 0x02000066 RID: 102
        // (Invoke) Token: 0x06000364 RID: 868
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate double BASS_ChannelBytes2Seconds(int handle, long pos);

        // Token: 0x02000067 RID: 103
        // (Invoke) Token: 0x06000368 RID: 872
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_StreamCreateFile([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset,
            long length, BASSFlag flags);

        // Token: 0x02000068 RID: 104
        // (Invoke) Token: 0x0600036C RID: 876
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool BASS_StreamFree(int handle);

        // Token: 0x02000069 RID: 105
        // (Invoke) Token: 0x06000370 RID: 880
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_ChannelGetLevel(int handle);

        // Token: 0x0200006A RID: 106
        // (Invoke) Token: 0x06000374 RID: 884
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr BASS_ChannelGetTags(int handle, BASSTag tags);

        public sealed class BASS_CHANNELINFO
        {
            // Token: 0x040004AA RID: 1194
            internal BASS_CHANNELINFO_INTERNAL _internal;

            // Token: 0x040004AB RID: 1195
            public int chans;

            // Token: 0x040004AC RID: 1196
            public BASSChannelType ctype;

            // Token: 0x040004AD RID: 1197
            public string filename = string.Empty;

            // Token: 0x040004AE RID: 1198
            public BASSFlag flags;

            // Token: 0x040004AF RID: 1199
            public int freq;

            // Token: 0x040004B0 RID: 1200
            public int origres;

            // Token: 0x040004B1 RID: 1201
            public int plugin;

            // Token: 0x040004B2 RID: 1202
            public int sample;

            // Token: 0x17000032 RID: 50
            // (get) Token: 0x06000378 RID: 888 RVA: 0x0001867A File Offset: 0x0001687A
            public bool Is32bit => (flags & BASSFlag.BASS_MUSIC_FLOAT) > BASSFlag.BASS_DEFAULT;

            // Token: 0x17000033 RID: 51
            // (get) Token: 0x06000379 RID: 889 RVA: 0x0001868B File Offset: 0x0001688B
            public bool Is8bit => (flags & BASSFlag.BASS_FX_BPM_BKGRND) > BASSFlag.BASS_DEFAULT;

            // Token: 0x17000034 RID: 52
            // (get) Token: 0x0600037A RID: 890 RVA: 0x00018698 File Offset: 0x00016898
            public bool IsDecodingChannel => (flags & BASSFlag.BASS_MUSIC_DECODE) > BASSFlag.BASS_DEFAULT;

            // Token: 0x06000377 RID: 887 RVA: 0x00018600 File Offset: 0x00016800
            public override string ToString()
            {
                return string.Format("{0}, {1}Hz, {2}, {3}bit", BASSChannelTypeToString(ctype),
                    freq,
                    ChannelNumberToString(chans),
                    origres == 0 ? (Is32bit ? 32 : (Is8bit ? 8 : 16)) : origres);
            }
        }

        // Token: 0x02000071 RID: 113
        [Serializable]
        public struct BASS_CHANNELINFO_INTERNAL
        {
            // Token: 0x04000451 RID: 1105
            public int freq;

            // Token: 0x04000452 RID: 1106
            public int chans;

            // Token: 0x04000453 RID: 1107
            public BASSFlag flags;

            // Token: 0x04000454 RID: 1108
            public BASSChannelType ctype;

            // Token: 0x04000455 RID: 1109
            public int origres;

            // Token: 0x04000456 RID: 1110
            public int plugin;

            // Token: 0x04000457 RID: 1111
            public int sample;

            // Token: 0x04000458 RID: 1112
            public IntPtr filename;
        }
    }
}