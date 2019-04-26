using System;
using System.IO;
using HGE.Events;
using HGE.IO.AudioEngine.sidPlayLib;
using HGE.IO.AudioEngine.sidPlayLib.common;
using HGE.IO.AudioEngine.sidPlayLib.components.sidtune;
using NAudio.Wave;

namespace HGE.IO.AudioEngine
{
    public class sidPlayAudioCore : IDisposable
    {
        private sidPlayer player;
        private SidTune sidTune;

        private readonly IWavePlayer wavePlayer;

        public bool IsTuneLoaded { get; private set; }
        public bool IsTuneFaulty { get; private set; }

        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Copyright { get; private set; }

        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }

        public sidPlayAudioCore(byte[] sidFileBytes, int sampleRate = 44100, int channels = 2)
        {
            try
            {
                sidTune = new SidTune(new MemoryStream(sidFileBytes));
                sidTune.selectSong(0);

            }
            catch (Exception ex)
            {
                throw new AudioEngineException(0, "[sidPlayAudioCore] Exception when loading the tune...", ex);
            }

            player = new sidPlayer();

            Title = sidTune.Info.InfoString1;
            Author = sidTune.Info.InfoString2;
            Copyright = sidTune.Info.InfoString3;

            var tmpTuneLoaded = player.load(sidTune);
            IsTuneLoaded = tmpTuneLoaded == 0;
            IsTuneFaulty = tmpTuneLoaded == -2;

            var sidCfg = player.config();
            sidCfg.clockForced = false;
            sidCfg.clockSpeed = SID2Types.sid2_clock_t.SID2_CLOCK_CORRECT;
            sidCfg.clockDefault = SID2Types.sid2_clock_t.SID2_CLOCK_CORRECT;
            sidCfg.frequency = sampleRate;
            sidCfg.playback =
                channels != 1 ? SID2Types.sid2_playback_t.sid2_stereo : SID2Types.sid2_playback_t.sid2_mono;
            sidCfg.precision = 16;

            sidCfg.sidModel = SID2Types.sid2_model_t.SID2_MODEL_CORRECT;
            sidCfg.sidDefault = SID2Types.sid2_model_t.SID2_MODEL_CORRECT;
            sidCfg.sidSamples = true;
            sidCfg.environment = SID2Types.sid2_env_t.sid2_envR;
            sidCfg.forceDualSids = false;
            sidCfg.optimisation = 2;

            if (player.config(sidCfg) != 0)
            {
                Dispose();
                return;
            }

            try
            {
                wavePlayer = new WaveOut();
                wavePlayer.Init(new sidWaveOut(sampleRate, channels, player));
            }
            catch (NAudio.MmException ex)
            {
                throw new AudioEngineException(0, "[sidPlayAudioCore] NAudio initialization exception!", ex);
            }

            IsPlaying = false;
            IsPaused = false;
        }

        public void Pause()
        {
            if (IsPlaying && !IsPaused)
            {
                player.pause();
                IsPaused = true;
                wavePlayer?.Pause();
            }
        }

        public void Resume()
        {
            if (IsPlaying && IsPaused)
            {
                player.resume();
                IsPaused = false;
                wavePlayer?.Play();
            }
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                player.stop();
                IsPlaying = false;
                IsPaused = false;
                wavePlayer?.Stop();
            }
        }

        public void Play()
        {
            if (!IsPlaying && !IsPaused)
            {
                player.start();
                IsPlaying = true;
                IsPaused = false;
                wavePlayer?.Play();
            }
        }

        public void Dispose()
        {
            Stop();
            wavePlayer?.Dispose();
            sidTune = null;
            player = null;
        }
    }
}
