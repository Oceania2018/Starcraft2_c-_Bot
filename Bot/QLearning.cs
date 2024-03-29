﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;
using TensorFlow;


namespace Bot
{

    public class Q_Learning
    {
        private TFGraph graf;
        private Variable va;
        private TFOutput Qout;
        private TFOutput Predict;
        private TFOutput nextQ;
        private TFOutput Loss;
        private SGD Trainer;
        private TFOperation[] UpdateModel;

        private double y = 0.99;
        private double e = 0.1;
        private int n_episodes = 2000;

        public Q_Learning(List<int> actions)
        {
            
            graf = new TFGraph();
            TFOutput s = graf.Placeholder(TFDataType.Float, new TFShape(1, 16));
            va = graf.Variable(graf.RandomUniform(new TFShape(16, 4), 0, 0.01));
            Qout = graf.MatMul(s, va);
            Predict = graf.ArgMax(Qout, graf.Constant(1, new TFShape(1)));

            nextQ = graf.Placeholder(TFDataType.Float, new TFShape(1, 4));
            Loss = graf.ReduceSum(graf.SquaredDifference(nextQ, Qout));
            Trainer = new SGD(graf, 0.1f);
            UpdateModel = Trainer.Minimize(Loss);
        }

        public void Run()
        {
            List<object> jList = new List<object>();
            List<object> rList = new List<object>();

            using (TFSession session = new TFSession(graf))
            {
                session.Run()
            }
        }

       
    }

}
