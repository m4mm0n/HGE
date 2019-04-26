using NAudio.Wave;

namespace HGE.IO.AudioEngine.sidPlayLib
{
    internal class sidWaveOut : WaveStream
    {
        private readonly sidPlayer p;
        
        public sidWaveOut(int sampleRate, int channels, sidPlayer player)
        {
            WaveFormat = new WaveFormat(sampleRate, 16, channels);
            p = player;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //var tmpBuf = new short[count];
            //for (var i = 0; i < buffer.Length; i++)
            //{
            //    tmpBuf[i] = (short) (buffer[i] << 8);
            //}
            
            //return (int) p.play(tmpBuf, count);
            return (int) p.play(buffer, count);
        }

        public override WaveFormat WaveFormat { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}
