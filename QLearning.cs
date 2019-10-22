using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;
using Tensorflow;
using NumSharp;
using static Tensorflow.Binding;


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

        /******/
        Tensor input1;
        RefVariable w;
        Tensor Qout;
        Tensor predict;
        Tensor nextQ;
        Tensor loss;
        Optimizer trainer;
        Operation updateModel;

        private Session sess;

        public Q_Learning(float obsSpaceSize = 24)
        {
            this.obsSize = obsSpaceSize;
            
        }
        
        public void DefineModel()
        {
            var s = tf.global_variables_initializer();
            input1 = tf.placeholder(TF_DataType.TF_FLOAT, (1, 16));
            w = tf.Variable(tf.random_uniform(new[] { 16, 4 }, 0.0f, 0.01f));
            Qout = tf.matmul(input1, w);
            predict = tf.argmax(Qout);

            nextQ = tf.placeholder(TF_DataType.TF_FLOAT, (1, 4));
            loss = tf.reduce_sum((tf.square(nextQ - Qout)));
            trainer = tf.train.GradientDescentOptimizer(0.1f);
            updateModel = trainer.minimize(loss);
        }

        public void Init()
        {
            sess = tf.Session();    
        }

        public float GetAction(float y)
        {
            var ress = sess.run((predict, Qout), (input1, np.identity(16)[1]));
            if(np.random.rand(1) < 0.1)
            {
                
            }

        }

        public void Close()
        {
            sess.close();
            
        }
    }

}
