using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;

namespace Bot
{
    /// <summary>
    /// Components for ZEALOT fight
    /// </summary>
    public static class SmartActions
    {
        public delegate object SmartAction(object s);
        /// <summary>
        /// "Smart" actions
        /// Used to execute actions by neuralnetwork
        /// </summary>
        public static Dictionary<int, SmartAction> Actiomap = new Dictionary<int, SmartAction>();

        public static bool Initialized { get; private set; }

        private static object BuildPylon(object x)
        {
            if (Controller.CanAfford(Units.PYLON))
            {
                var resourceCenters = Controller.GetUnits(Units.ResourceCenters);
                foreach (var rc in resourceCenters)
                {
                    if (Controller.CanConstruct(Units.PROBE))
                        rc.Train(Units.PROBE);
                }
                List<Unit> worker = Controller.GetUnits(Units.PROBE);

                for(int i = 0; i < worker.Count; i++)
                {
                    if (worker[i].orders.Count == 0)
                    {
                        Controller.Construct(Units.PYLON);
                        break;
                    }

                }
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

        
    }
}
