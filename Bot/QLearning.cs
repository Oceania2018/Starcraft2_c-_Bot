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

        public Q_Learning(int width, int height)
        {
            
        }

        public void DefineModel(int width, int height)
        {
            INeuralNetwork network = NetworkManager.NewGraph(TensorInfo.Volume(width, height, 0), root =>
            {
                var fc1 = root.Pipeline(CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid),
                    CuDnnNetworkLayers.FullyConnected(16, ActivationType.Sigmoid),
                    CuDnnNetworkLayers.FullyConnected(8, ActivationType.Identity)
                    );
            });
        }
       
    }

}
