namespace HGE.IO.AudioEngine.sidPlayLib.components.mos6510
{
    public class ProcessorCycle
    {
        public delegate void FunctionDelegate();

        internal FunctionDelegate func;

        internal bool nosteal;

        internal ProcessorCycle()
        {
            func = null;
            nosteal = false;
        }
    }
}