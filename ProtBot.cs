﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SC2APIProtocol;

namespace Bot
{

    [StructLayout(LayoutKind.Sequential]
    public struct GameState
    {
        public float scvsCount;
        public float scvs_indle;
        public float NexusCount;
        public float PylonCount;
        public float PylynCountComplete;
        public float GateWayCount;
        public float CompleteGatewayCount;
        public float ZealotCount;
        public float QueuedGatewaysCount;
        public float AvailableSupply;
        public float canAffordPylon;
        public float CanAffordGateway;
        public float CanAffordZealot;
        public float Enemyscv;
        public float EnemyscvIdle;
        public float enemyCommandCenter;
        public float SupplyDepoCount;
        public float CompletedSupplyDepo;
        public float Enemybarrack;
        public float EnemyCompletedBarrac;
        public float EnemyMarine;

        public float[] ToArray()
        {
            return new float[]
            {
                scvsCount,
                scvs_indle,
                NexusCount,
                PylonCount,
                PylynCountComplete,
                GateWayCount,
                CompleteGatewayCount,
                ZealotCount,
                QueuedGatewaysCount,
                AvailableSupply,
                canAffordPylon,
                CanAffordGateway,
                CanAffordZealot,
                Enemyscv,
                EnemyscvIdle,
                enemyCommandCenter,
                SupplyDepoCount,
                CompletedSupplyDepo,
                Enemybarrack,
                EnemyCompletedBarrac,
                EnemyMarine
            };
        }
    }

    internal class ProtBot : Bot
    {

        private Q_Learning learner;
        private bool initialized = false;

       
           
        public GameState GetGameState()
        {
            GameState ret;
            List<object> scvs = new List<object>(Controller.GetUnits(Units.PROBE));
            ret.scvsCount = scvs.Count;
            ret.scvs_indle = 0;
            for (int i = 0; i < scvs.Count; i++)
            {
                if (((Unit)scvs[i]).orders.Count == 0)
                {
                    ret.scvs_indle += 1;
                }
            }
            ret.NexusCount = Controller.GetUnits(Units.NEXUS).Count;
            ret.PylonCount = Controller.GetUnits(Units.PYLON).Count;
            ret.PylynCountComplete = Controller.GetUnits(Units.PYLON, Alliance.Self, onlyCompleted: true).Count;
            ret.GateWayCount = Controller.GetUnits(Units.GATEWAY).Count;
            List<Unit> completed = Controller.GetUnits(Units.GATEWAY, onlyCompleted: true);
            ret.CompleteGatewayCount = completed.Count;
            ret.ZealotCount = Controller.GetUnits(Units.ZEALOT).Count;

            List<object> Queue_Zelots = new List<object>();

            if (completed.Count > 0)
            {
                if (((Unit)completed[0]).orders != null)
                {
                    if (((Unit)completed[0]).orders.Count > 0)
                        ret.QueuedGatewaysCount = 0;
                    else
                        ret.QueuedGatewaysCount = ((Unit)completed[0]).orders.Count;
                }
                else
                {
                    ret.QueuedGatewaysCount = 0;
                }
            }
            else
            {
                ret.QueuedGatewaysCount = 0;
            }
            ret.AvailableSupply = (int)(Controller.maxSupply - Controller.obs.Observation.PlayerCommon.FoodUsed);
            ret.canAffordPylon = Controller.CanAfford(Units.PYLON) == true ? 1 : 0;
            ret.CanAffordGateway = Controller.CanAfford(Units.GATEWAY) == true ? 1 : 0;
            ret.CanAffordZealot = Controller.CanAfford(Units.ZEALOT) == true ? 1 : 0;

            List<Unit> enemyScvs = Controller.GetUnits(Units.SCV, Alliance.Enemy);

            ret.Enemyscv = enemyScvs.Count;
            ret.EnemyscvIdle = 0;

            for (int i = 0; i < enemyScvs.Count; i++)
            {
                if (enemyScvs[i].orders.Count == 0)
                {
                    ret.EnemyscvIdle += 1;
                }
            }
            ret.enemyCommandCenter = Controller.GetUnits(Units.COMMAND_CENTER, Alliance.Enemy).Count;
            ret.SupplyDepoCount = Controller.GetUnits(Units.SupplyDepots, Alliance.Enemy).Count;
            ret.CompletedSupplyDepo = Controller.GetUnits(Units.SupplyDepots, Alliance.Enemy, onlyCompleted: true).Count;
            ret.Enemybarrack = Controller.GetUnits(Units.BARRACKS, Alliance.Enemy).Count;
            ret.EnemyCompletedBarrac = Controller.GetUnits(Units.BARRACKS, Alliance.Enemy, onlyCompleted: true).Count;
            ret.EnemyMarine = Controller.GetUnits(Units.MARINE, Alliance.Enemy).Count;
            return ret;

        }

        public void Init()
        {
            SmartActions.Init();
            learner = new Q_Learning();
            initialized = true;
            learner.GetScore = GetScore;
            Logger.Info("QTable Initialized");
        }

        public float[] GetScore()
        {
            return new float[]
            {
                Controller.obs.Observation.Score.ScoreDetails.KilledValueUnits,
                Controller.obs.Observation.Score.ScoreDetails.KilledValueStructures,
                
            };
        }

        public IEnumerable<SC2APIProtocol.Action> OnFrame()
        {
            Controller.OpenFrame();
            if (!initialized)
                this.Init();
            

            if (Controller.frame % 10 == 0)
                Controller.DistributeWorkers();

            return Controller.CloseFrame();
        }
    }
}
