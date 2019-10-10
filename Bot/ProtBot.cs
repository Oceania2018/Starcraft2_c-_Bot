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
        private bool initialized = false;

        public float[] GetGameState()
        {
            float[] ret = new float[21];
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

            if (completed.Count > 0)
            {
                if (((Unit)completed[0]).orders != null)
                {
                    if (((Unit)completed[0]).orders.Count > 0)
                        ret[8] = 0;
                    else
                        ret[8] = ((Unit)completed[0]).orders.Count;
                }
                else
                {
                    ret[8] = 0;
                }
            }
            else
            {
                ret[8] = 0;
            }
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
            SmartActions.Init();
            learner = new Q_Learning(GetGameState().Length, SmartActions.Actiomap.Count);
            learner.GetGameStates += GetGameState;
            
            initialized = true;
            Logger.Info("QTable Initialized");
        }

        public IEnumerable<SC2APIProtocol.Action> OnFrame()
        {
            Controller.OpenFrame();
            if (!initialized)
                this.Init();


            int Action = learner.GetAction();

            SmartActions.Actiomap[Action](null);
            if (Controller.frame % 10 == 0)
                Controller.DistributeWorkers();

            learner.Train();
            return Controller.CloseFrame();
        }
    }
}
