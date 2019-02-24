using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    interface IGraficPoint
    {
        char SYMBOL { get; }
        int FOREGROUND { get; }
        int BACKGROUND { get; }
    }
}
