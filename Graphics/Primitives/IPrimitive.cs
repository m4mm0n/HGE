using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGE.Graphics.Primitives
{
    public interface IPrimitive
    {
        void Draw(Graphics2DGL core, params object[] parameters);
        void Fill(Graphics2DGL core, params object[] parameters);
    }
}
