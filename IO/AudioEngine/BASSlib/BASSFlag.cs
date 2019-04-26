using System;

namespace HGE.IO.AudioEngine.BASSlib
{
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
}