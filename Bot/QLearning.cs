using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;
using TensorFlow;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;
using KerasSharp;
using KerasSharp.Activations;
using KerasSharp.Backends;
using KerasSharp.Initializers;
using KerasSharp.Losses;
using KerasSharp.Metrics;
using KerasSharp.Models;
using KerasSharp.Optimizers;

using static KerasSharp.Backends.Current;


namespace Bot
{

    public class Q_Learning
    {

        Model model;

        public Q_Learning(List<int> actions)
        {
            Switch("KerasSharp.Backends.TensorFlowBackend");
            model = new Sequential();
        }

       
    }

}
