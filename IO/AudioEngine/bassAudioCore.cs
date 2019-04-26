using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HGE.Events;
using HGE.Graphics;
using HGE.IO.AudioEngine.BASSlib;

namespace HGE.IO.AudioEngine
{
    public class bassAudioCore : IDisposable
    {
        private readonly float[] _fft = new float[2048];
        private readonly float[] _lastPeak = new float[2048];
        private readonly int _scaleFactorLinear = 9;
        private readonly float _scaleFactorLinearBoost = 0.05f;
        private readonly int _scaleFactorSqr = 4;
        private readonly float _scaleFactorSqrBoost = 0.005f;

        #region Private Delegate Pointers

        private readonly BASS_Free BassFree;
        private readonly BASS_PluginFree PluginFree;
        private readonly BASS_PluginLoad PluginLoad;
        private readonly BASS_ChannelPlay ChannelPlay;
        private readonly BASS_ChannelStop ChannelStop;
        private readonly BASS_ChannelBytes2Seconds GetBytes2Seconds;
        private readonly BASS_ChannelGetLength GetChanLength;
        private readonly BASS_ChannelGetLevel GetChannelLevel;
        private readonly BASS_ChannelGetTags GetChannelTags;
        private readonly BASS_ChannelGetData GetData;
        private readonly BASS_ChannelGetData2 GetData2;
        private readonly BASS_ChannelGetData3 GetData3;
        private readonly BASS_ChannelGetData4 GetData4;
        private readonly BASS_ChannelGetInfo GetInfo;
        private readonly BASS_ChannelIsActive GetIsActive;
        private readonly BASS_ChannelSeconds2Bytes GetSeconds2Bytes;
        private readonly BASS_Init InitBass;
        private readonly BASS_MusicLoad MusicLoadMemory;
        private readonly BASS_ChannelSetPosition SetChannelPos;
        private readonly BASS_StreamCreateFile StreamCreateFileMemory;
        private readonly BASS_StreamFree StreamFreeA;
        private readonly BASS_ChannelPause ChannelPause;
        private readonly BASS_ErrorCode GetErrorCode;
        private readonly BASS_SetConfig SetConfig;

        #endregion

        private readonly DynamicNativeLibrary memDll;

        private BASSData _maxFFT = BASSData.BASS_DATA_FFT4096;
        private int _maxFFTData = 4096;
        private int _maxFFTSampleIndex = 2047;
        private int _maxFrequencySpectrum = 2047;

        private double spp = 1.0;

        private List<PluginFI> loadedPlugins = new List<PluginFI>();

        public bassAudioCore()
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
                PluginFree =
                    (BASS_PluginFree) memDll.GetDelegateForFunction("BASS_PluginFree", typeof(BASS_PluginFree));
                PluginLoad =
                    (BASS_PluginLoad) memDll.GetDelegateForFunction("BASS_PluginLoad", typeof(BASS_PluginLoad));
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
            if (loadedPlugins.Count > 0)
            {
                foreach (var loadedPlugin in loadedPlugins)
                {
                    FreePlugin(loadedPlugin);
                }
            }
            Free();
            memDll?.Dispose();
        }

        public PluginFI LoadPlugin(string filename)
        {
            if (!filename.StartsWith(Directory.GetCurrentDirectory()))
                filename = Directory.GetCurrentDirectory() + "\\" + filename;

            var m = PluginLoad(filename, 0);
            if (m != 0)
            {
                var x = new PluginFI(filename, m);
                loadedPlugins.Add(x);
                return x;
            }
            throw new AudioEngineException(GetErrorCode(), "[LoadPlugin] Failed to load plugin: " + filename);
        }

        public void FreePlugin(PluginFI plugin)
        {
            if (loadedPlugins.Contains(plugin))
            {
                try
                {
                    PluginFree(plugin.HandleID);
                }
                catch (Exception innerEx)
                {
                    throw new AudioEngineException(GetErrorCode(), "[FreePlugin] Failed...", innerEx);
                }

                loadedPlugins.Remove(plugin);
            }
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

        public int LoadMusic(byte[] trackToLoad)
        {
            return MusicLoadMemory(true, trackToLoad, 0L, trackToLoad.Length,
                BASSFlag.BASS_MUSIC_LOOP | BASSFlag.BASS_MUSIC_RAMPS, 1);
        }

        public bool Stop(int handle)
        {
            return ChannelStop(handle);
        }

        public void Free()
        {
            BassFree();
        }

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
        private delegate bool BASS_PluginFree(int handle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int BASS_PluginLoad([MarshalAs(UnmanagedType.LPWStr)] [In] string fileName, BASSFlag flags);

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