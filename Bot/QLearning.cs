using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    /// <summary>
    /// First iteration:
    /// 
    /// </summary>
    public class Q_Learning
    {
        private const string NetworkSaveName = "network";

        public const int RewardKill = 1;
        public const int RewardWin = 1;

        /// <summary>
        /// Default 2 
        /// </summary>
        public int OutputSize = 2;
        public delegate float[] GameStateCallback();
        public GameStateCallback GetGameStates;

        public float[] actionStates;

        private INeuralNetwork trainingNetwork;
        private float[,] QTable;
        private List<List<int>> QtableCopy = new List<List<int>>();
        List<float[]> StateMemory = new List<float[]>();
        private int Width, Height;

        public Q_Learning(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            QTable = new float[width, height];
            for(int i = 0; i < height; i++)
            {
                QtableCopy.Add(new List<int>());
            }
            if (this.OutputSize == 2)
                this.OutputSize = width * height;
            if(File.Exists(NetworkSaveName + NetworkLoader.NetworkFileExtension)){
                trainingNetwork = NetworkLoader.TryLoad(new FileInfo(NetworkSaveName + NetworkSaveName), ExecutionModePreference.Cuda);
            }
            else
            {
                DefineModel();
            }
        }

        public void DefineModel()
        {

            trainingNetwork = NetworkManager.NewSequential(TensorInfo.Volume(this.Width, this.Height, 1),
                CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid),
                CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid),
                CuDnnNetworkLayers.Softmax(OutputSize));

            /*trainingNetwork = NetworkManager.NewGraph(TensorInfo.Volume(this.Width, this.Height, 1), root =>
            {
                var fc1 = root.Layer(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid));
                var fc2 = fc1.Layer(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid));
                var model_out = fc2.Layer(CuDnnNetworkLayers.FullyConnected(OutputSize, ActivationType.Identity));
            });*/
            
        }

        public void AddAction(int[,] TakenActions)
        {
            
        }

        public int GetAction(float e_greedy = 0.9f)
        {
            actionStates = GetGameStates();
            StateMemory.Add(actionStates);
            try
            {
                if (ThreadSafeRandom.NextUniform() < e_greedy)
                {
                    float[] trainingValues = trainingNetwork.Forward(actionStates);
                    return (int)trainingValues.Max();
                }
                else
                {
                    return ThreadSafeRandom.NextInt(0, SmartActions.Actiomap.Count);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }


        public void Train()
        {
            float[,] data = new float[StateMemory.Count, 21];
            for(int x = 0; x < StateMemory.Count; x++)
            {
                for(int y = 0; y < 21; y++)
                {
                    data[x, y] = data[x, y];
                }
            }
            
            ITrainingDataset dataset = DatasetLoader.Training((data, null), data.Length * 21);
            TrainingSessionResult results = NetworkManager.TrainNetwork(trainingNetwork, dataset, TrainingAlgorithms.AdaMax(), 1);

        }
       
    }

}
