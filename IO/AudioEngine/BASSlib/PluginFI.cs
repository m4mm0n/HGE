namespace HGE.IO.AudioEngine.BASSlib
{
    /// <summary>
    /// Plugin FileName and Handle ID
    /// </summary>
    public struct PluginFI
    {
        public string FileName;
        public int HandleID;

        public PluginFI(string file, int handle)
        {
            FileName = file;
            HandleID = handle;
        }
    }
}
