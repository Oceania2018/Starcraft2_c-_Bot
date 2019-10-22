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
        private static readonly Bot bot = new RaxBot();
        private const Race race = Race.Terran;

        // Settings for single player mode.
        //        private static string mapName = "AbyssalReefLE.SC2Map";
        //        private static string mapName = "AbiogenesisLE.SC2Map";
        //        private static string mapName = "FrostLE.SC2Map";
        private static readonly string mapName = "(2)16-BitLE.SC2Map";

        private static readonly Race opponentRace = Race.Random;
        private static readonly Difficulty opponentDifficulty = Difficulty.Easy;

        public static GameConnection gc;

        static void Main(string[] args)
        {
            Q_Learning lerner = new Q_Learning();
            lerner.Init();
            lerner.DefineModel();
            lerner.Close();


        }
    }
}
