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


    public class Q_Learning
    {
        private const string networkSaveName = "network";
        public int OutputSize = 8;

        public float[] actionStates;

        private INeuralNetwork trainingNetwork;

        public Q_Learning(int width, int height)
        {

        }

        public void DefineModel(int width, int height, float[] actions)
        {
            this.actionStates = actions;

            trainingNetwork = NetworkManager.NewGraph(TensorInfo.Volume(width, height, 0), root =>
            {
                var fc1 = root.Layer(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid));
                var fc2 = fc1.Layer(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid));
                var model_out = fc2.Layer(CuDnnNetworkLayers.FullyConnected(OutputSize, ActivationType.Identity));
            });
            
        }



        public int GetAction()
        {
            float[] trainingValues = trainingNetwork.Forward(actionStates);
            return 0;
        }
       
    }

}
