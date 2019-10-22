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


    public class RewardTypes
    {
        public const int EnemyUnityKilled = 1;
        public const int EnemyStructureKilled = 1;
        public const int UnitLost = -1;
        public const int StructureLost = -1;
        public const int GameLost = -1;
        public const int GameWon = 1;
    }

    /// <summary>
    /// First iteration:
    /// 
    /// </summary>
    public class Q_Learning
    {
        private const string NetworkSaveName = "network";

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
        private List<Tuple<int[], int[]>> ActionSpaceHistory = new List<Tuple<int[], int[]>>();

        public delegate int[] GetObs();
        public GetObs GetStates;
        
        public void DefineModel()
        {

            if (!SmartActions.Initialized)
                SmartActions.Init();

            var s = tf.global_variables_initializer();
            input1 = tf.placeholder(TF_DataType.TF_FLOAT, (1, 21));
            w = tf.Variable(tf.random_uniform(new[] { 21, SmartActions.Actiomap.Count }, 0.0f, 0.01f));
            Qout = tf.matmul(input1, w);
            predict = tf.argmax(Qout);

            nextQ = tf.placeholder(TF_DataType.TF_FLOAT, (1, SmartActions.Actiomap.Count));
            loss = tf.reduce_sum((tf.square(nextQ - Qout)));
            trainer = tf.train.GradientDescentOptimizer(0.1f);
            updateModel = trainer.minimize(loss);


        }

        public void Init()
        {
            sess = tf.Session();
            sess.run(tf.global_variables_initializer());
            
        }

        private Random rand = new Random();

        private bool nextAction = false;
        private int[] lastState;
        private NDArray allQ;
        private NDArray a;
        public float GetAction()
        {
            if (!nextAction)
            {
                var ress = sess.run((predict, Qout), (input1, np.identity(21)[1]));
                a = ress.Item1;
                allQ = ress.Item2;
                if (np.random.rand(1) < 0.1)
                {
                    ress.Item1[0] = rand.Next(0, SmartActions.Actiomap.Count - 1);
                }
                lastState = GetStates();
                nextAction = true;
                return ress.Item1;
            }
            else
            {
                var ress = sess.run(Qout, (input1, np.identity(16)[GetStates()]));
                var maxQ1 = np.max(ress);
                var targetQ = allQ;
                targetQ[0, ]
            }
        }



        public void Close()
        {
            Saver s = tf.train.Saver();
            s.save(sess, "model");
            sess.close();
            
        }
    }

}
