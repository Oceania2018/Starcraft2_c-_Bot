using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;

namespace Bot
{
    internal class ProtBot : Bot
    {

        private Q_Learning learner;

        public int[] GetGameState()
        {
            int[] ret = new int[21];
            List<object> scvs = new List<object>(Controller.GetUnits(Units.PROBE));
            ret[0] = scvs.Count;
            ret[1] = 0;
            for (int i = 0; i < scvs.Count; i++)
            {
                if (((Unit)scvs[i]).orders.Count == 0)
                {
                    ret[1] += 1;
                }
            }
            ret[2] = Controller.GetUnits(Units.NEXUS).Count;
            ret[3] = Controller.GetUnits(Units.PYLON).Count;
            ret[4] = Controller.GetUnits(Units.PYLON, Alliance.Self, onlyCompleted: true).Count;
            ret[5] = Controller.GetUnits(Units.GATEWAY).Count;
            List<Unit> completed = Controller.GetUnits(Units.GATEWAY, onlyCompleted: true);
            ret[6] = completed.Count;
            ret[7] = Controller.GetUnits(Units.ZEALOT).Count;

            List<object> Queue_Zelots = new List<object>();
            if (((Unit)completed[0]).orders.Count > 0)
                ret[8] = 0;
            else
                ret[8] = ((Unit)completed[0]).orders.Count;
            ret[9] = (int)(Controller.maxSupply - Controller.obs.Observation.PlayerCommon.FoodUsed);
            ret[10] = Controller.CanAfford(Units.PYLON) == true ? 1 : 0;
            ret[11] = Controller.CanAfford(Units.GATEWAY) == true ? 1 : 0;
            ret[12] = Controller.CanAfford(Units.ZEALOT) == true ? 1 : 0;

            List<Unit> enemyScvs = Controller.GetUnits(Units.SCV, Alliance.Enemy);

            ret[13] = enemyScvs.Count;
            ret[14] = 0;

            for (int i = 0; i < enemyScvs.Count; i++)
            {
                if (enemyScvs[i].orders.Count == 0)
                {
                    ret[14] += 1;
                }
            }
            ret[15] = Controller.GetUnits(Units.COMMAND_CENTER, Alliance.Enemy).Count;
            ret[16] = Controller.GetUnits(Units.SupplyDepots, Alliance.Enemy).Count;
            ret[17] = Controller.GetUnits(Units.SupplyDepots, Alliance.Enemy, onlyCompleted: true).Count;
            ret[18] = Controller.GetUnits(Units.BARRACKS, Alliance.Enemy).Count;
            ret[19] = Controller.GetUnits(Units.BARRACKS, Alliance.Enemy, onlyCompleted: true).Count;
            ret[20] = Controller.GetUnits(Units.MARINE, Alliance.Enemy).Count;
            return ret;

        }

        public void Init()
        {
            learner = new Q_Learning(GetGameState().Length, SmartActions.Actiomap.Count);
            learner.GetGameStates += GetGameState;
        }

        public IEnumerable<SC2APIProtocol.Action> OnFrame()
        {
            Controller.OpenFrame();

            List<Unit> resourceCenters = Controller.GetUnits(Units.ResourceCenters);

            foreach(var rc in resourceCenters)
            {
                if (Controller.CanConstruct(Units.PROBE))
                {
                    rc.Train(Units.PROBE);
                }
            }

            if(Controller.maxSupply - Controller.currentSupply <= 5)
            {
                if (Controller.CanConstruct(Units.PYLON))
                    if (Controller.GetPendingCount(Units.PYLON) == 0)
                        Controller.Construct(Units.PYLON);
            }

            if (Controller.frame % 10 == 0)
                Controller.DistributeWorkers();

            if (Controller.CanConstruct(Units.GATEWAY))
                if (Controller.GetTotalCount(Units.GATEWAY) < 4)
                    Controller.Construct(Units.GATEWAY);

            if (Controller.CanConstruct(Units.CYBERNETICS_CORE))
                Controller.Construct(Units.CYBERNETICS_CORE);

            /*if(Controller.)

            if(Controller.CanConstruct(Units.ASSIMILATOR))*/
                

            List<Unit> units = Controller.GetUnits(Units.Structures);
            foreach(var i in units)
            {
                if(i.unitType == Units.CYBERNETICS_CORE)
                {

                }
            }

            return Controller.CloseFrame();
        }
    }
}
