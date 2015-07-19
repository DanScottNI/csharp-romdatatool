using System;
using System.Collections.Generic;
using System.Text;

namespace ROMDataLib
{
    interface IGameExport
    {
        void Export(ref Game game, string filename);
    }
}
