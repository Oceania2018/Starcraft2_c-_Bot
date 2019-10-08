using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;

namespace Bot
{
    /// <summary>
    /// Components for stalker fight
    /// </summary>
    public static class SmartActions
    {
        public delegate object SmartAction(object s);
        /// <summary>
        /// "Smart" actions
        /// Used to execute actions by neuralnetwork
        /// </summary>
        public static Dictionary<int, SmartAction> Actiomap = new Dictionary<int, SmartAction>();


        private static object BuildPylon(object x)
        {
            if (Controller.CanAfford(Units.PYLON))
            {
                Controller.Construct(Units.PYLON);
                return true;
            }
            return false;
        }

        private static object BuildGateway(object x)
        {
            if (Controller.CanAfford(Units.GATEWAY))
            {
                Controller.Construct(Units.GATEWAY);
                return true;
            }
            return false;
        }

        private static object BuildZealot(object x)
        {
            int count = 0;
            foreach(Unit bar in Controller.GetUnits(Units.GATEWAY, onlyCompleted: true))
            {
                bar.Train(Units.ZEALOT);
                count++;
            }
            return count;
        }

        private static object AttackCommand(object x)
        {
            List<Unit> army = Controller.GetUnits(Units.ArmyUnits);
            if(army.Count > 0)
            {
                Controller.Attack(army, Controller.enemyLocations[0]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initialize smart actions
        /// </summary>
        /// <remarks>
        /// Order:
        /// 0-x build commands
        /// x-y unit commands (ALL UNITS)
        /// 0 Build pylon
        /// </remarks>
        public static void Init()
        {
            Actiomap.Add(0, BuildPylon);
            Actiomap.Add(1, BuildGateway);
            Actiomap.Add(2, BuildZealot);
            Actiomap.Add(3, AttackCommand);
        }
    }
}
