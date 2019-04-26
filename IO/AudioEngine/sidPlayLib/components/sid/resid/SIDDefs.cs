namespace HGE.IO.AudioEngine.sidPlayLib.components.sid.resid
{
    public static class SIDDefs
    {
        public enum chip_model
        {
            MOS6581,
            MOS8580
        }
        public enum sampling_method
        {
            SAMPLE_FAST,
            SAMPLE_INTERPOLATE,
            SAMPLE_RESAMPLE_INTERPOLATE,
            SAMPLE_RESAMPLE_FAST
        }
    }
}