using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;
using static Tensorflow.Binding;
using Tensorflow.Keras;
using Tensorflow.Keras.Layers;


namespace Bot
{
    /// <summary>
    /// First iteration:
    /// 
    /// </summary>
    public class Q_Learning
    {
        private const string NetworkSaveName = "network";
        private float obsSize;
        private float actionSpaceSize = 4;
        Tensorflow.Keras.Engine.Sequential model;
        public Q_Learning(float obsSpaceSize = 24)
        {
            this.obsSize = obsSpaceSize;
            
        }
        
        public void DefineModel()
        {

            Tensorflow.Tensor input1 = tf.placeholder(Tensorflow.TF_DataType.TF_FLOAT, (1, 16));
            var w = tf.Variable(tf.random_uniform(new int[16 * 4], 0, 0.01f));
            var Qout = tf.matmul(input1, w);
            var predict = tf.argmax(Qout);

            var nextQ = tf.placeholder(Tensorflow.TF_DataType.TF_FLOAT, (1, 4));
            var loss = tf.reduce_sum((tf.square(nextQ - Qout)));
            var trainer = tf.train.GradientDescentOptimizer(0.1f);
        }
    }

}
