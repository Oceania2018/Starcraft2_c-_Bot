using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Delegates;
using NeuralNetworkNET.APIs.Datasets;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using NeuralNetworkNET.APIs.Settings;
using NeuralNetworkNET.APIs.Structs;
using NeuralNetworkNET.Helpers;
using NeuralNetworkNET.SupervisedLearning.Progress;


namespace Bot
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ActionPair
    {
        public float x;
        public float y;
    }

    /// <summary>
    /// First iteration:
    /// build or not to build workers
    /// </summary>
    public class Q_Learning
    {
        private const string NetworkSaveName = "network";

        public const int RewardKill = 1;
        public const int RewardWin = 1;

        /// <summary>
        /// Default 2 for testing the movement with this model
        /// </summary>
        public int OutputSize = 2;
        public delegate int[] GameStateCallback();
        public GameStateCallback GetGameStates;

        public float[] actionStates;

        private INeuralNetwork trainingNetwork;
        private List<ActionPair> actionHistory = new List<ActionPair>();
        private int Width, Height;

        public Q_Learning(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void DefineModel()
        {
            
            trainingNetwork = NetworkManager.NewGraph(TensorInfo.Volume(this.Width, this.Height, 1), root =>
            {
                var fc1 = root.Layer(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid));
                var fc2 = fc1.Layer(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid));
                var model_out = fc2.Layer(CuDnnNetworkLayers.FullyConnected(OutputSize, ActivationType.Identity));
            });
            
        }

        public void AddAction(ActionPair actionState)
        {
            this.actionHistory.Add(actionState);
        }

        public int GetAction(float e_greedy = 0.9f)
        {

            if(ThreadSafeRandom.NextUniform() < e_greedy)
            {
                float[] trainingValues = trainingNetwork.Forward(actionStates);
                return (int)trainingValues.Max();
            }
            else
            {
                return ThreadSafeRandom.NextInt(0, SmartActions.Actiomap.Count);
            }

            
        }


        public void Train()
        {
            float[,] xy = new float[actionHistory.Count, 2]; 
            for(int i = 0; i < actionHistory.Count; i++)
            {
                xy[i, 0] = actionHistory[i].x;
                xy[i, 1] = actionHistory[i].y;
            }
            ITrainingDataset dataset = DatasetLoader.Training((xy, null), actionHistory.Count);
            var results = NetworkManager.TrainNetwork(trainingNetwork, dataset, TrainingAlgorithms.AdaMax(), 20);


        }
       
    }

}
