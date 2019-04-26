using System;

namespace HGE.IO.AudioEngine.sidPlayLib.components.sid.resid
{
    /// <summary>
    /// Class for plotting integers into an array
    /// 
    /// @author Ken Händel
    /// </summary>
    public class PointPlotter
    {
        protected int[] f;

        public PointPlotter(int[] arr)
        {
            this.f = arr;
        }

        internal void plot(double x, double y)
        {
            f[(int)x] = Math.Max(0, (int)y);
        }
    }
}