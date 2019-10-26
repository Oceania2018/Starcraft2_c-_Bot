using System;
using System.Threading.Tasks;
using SC2APIProtocol;
using NumSharp;
using Tensorflow;
using Tensorflow.Hub;
using static Tensorflow.Binding;

namespace Bot
{
    internal class Program
    {
        // Settings for your bot.
        private static readonly Bot bot = new ProtBot();
        private const Race race = Race.Protoss;

        // Settings for single player mode.
                private static string mapName = "AbyssalReefLE.SC2Map";
        //        private static string mapName = "AbiogenesisLE.SC2Map";
        //        private static string mapName = "FrostLE.SC2Map";
        //private static readonly string mapName = "AcidPlantLE.SC2Map";

        private static readonly Race opponentRace = Race.Random;
        private static readonly Difficulty opponentDifficulty = Difficulty.Easy;

        public static GameConnection gc;

        private  static Q_Learning lerner = new Q_Learning();
        
        public static void OnShutdown(object sender, ConsoleCancelEventArgs e)
        {
            lerner.Close();
            
        }

        static void Main(string[] args)
        {


            lerner.Init();
            lerner.DefineModel();
            (bot as ProtBot).learner = lerner;
            try
            {
                gc = new GameConnection();
                if(args.Length == 0)
                {
                    gc.readSettings();
                    while (Q_Learning.EpisodeCount < 100)
                    {
                        gc.RunSinglePlayer(bot, mapName, race, opponentRace, opponentDifficulty).Wait();
                    }
                }
                else
                {
                    gc.RunLadder(bot, race, args).Wait();
                }
            }catch(Exception ex)
            {
                Logger.Info(ex.ToString());
            }

            Logger.Info("Terminated");

        }
    }
}
