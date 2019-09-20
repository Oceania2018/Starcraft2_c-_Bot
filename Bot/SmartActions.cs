using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;

namespace Bot
{
    public static class SmartActions
    {
        public delegate object SmartAction(object s);
        public static Dictionary<int, SmartAction> Actionap = new Dictionary<int, SmartAction>();
    }
}
