namespace HGE.IO.AudioEngine.sidPlayLib.components.sid.resid
{
    public class sid_filter_t
    {
        public int[][] cutoff = new int[0x800][];

        public int points;

        public int Lthreshold, Lsteepness, Llp, Lbp, Lhp;
        public int Hthreshold, Hsteepness, Hlp, Hbp, Hhp;

        public sid_filter_t()
        {
            for (int i = 0; i <= cutoff.GetLength(0); i++)
            {
                cutoff[i] = new int[2];
            }
        }
    }
}