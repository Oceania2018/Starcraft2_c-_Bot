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
        Graph NetworkGraph;
        Tensor input1;
        RefVariable w;
        Tensor Qout;
        Tensor predict;
        Tensor nextQ;
        //Tensor loss;
        //Optimizer trainer;
        Operation updateModel;
        private bool BuildModel = false;

        private Session sess;
        private List<Tuple<int[], int[]>> ActionSpaceHistory = new List<Tuple<int[], int[]>>();
        private List<Tuple<float[], GameState>> spaceHistory = new List<Tuple<float[], GameState>>();

        public delegate GameState GetObs();
        public Func<float[]> GetScore; 
        public GetObs GetStates;

        public static int EpisodeCount = 0;

        private Graph DefineModel()
        {
            var graph = tf.Graph().as_default();

            input1 = tf.placeholder(TF_DataType.TF_FLOAT, (1, 21));
            w = tf.Variable(tf.random_uniform(new[] { 21, SmartActions.Actiomap.Count }, 0.0f, 0.01f));
            Qout = tf.matmul(input1, w);
            predict = tf.argmax(Qout, 1);

            nextQ = tf.placeholder(TF_DataType.TF_FLOAT, (1, SmartActions.Actiomap.Count));
            Tensor loss = tf.reduce_sum((tf.square(nextQ - Qout)));
            Optimizer trainer = tf.train.GradientDescentOptimizer(0.1f);
            updateModel = trainer.minimize(loss);
            BuildModel = true;
            

            return graph;
        }

        public void Init()
        {
            np.random.seed(4);
            this.NetworkGraph = this.DefineModel();
            sess = new Session(NetworkGraph);
            var init = tf.global_variables_initializer();
            sess.run(init);
            tf.train.export_meta_graph("asd.meta.txt", as_text: true);

        }

        public float GetRewards()
        {

            float rewardAmount = 0;

            GameState state = GetStates();
            GameState lastspace = spaceHistory.Last().Item2;
            float[] score = GetScore();
            if (state.EnemyMarine < lastspace.EnemyMarine)
            {
                rewardAmount += RewardTypes.EnemyUnityKilled;
            }
            if (score[1] > spaceHistory.Last().Item1[1])
            {
                rewardAmount += 1;
            }


            return rewardAmount;
        }

        private Random rand = new Random();

        private bool nextAction = false;
        private GameState lastState;
        private NDArray allQ;
        private NDArray a;

        private double e = 0.1f;
        int cc = 1;
        public float GetAction()
        {
            tf.train.export_meta_graph(cc++ + "asdd.meta.txt", as_text: true);
            
            spaceHistory.Add(new Tuple<float[], GameState>(GetStates().ToArray(), GetStates()));
            if (!nextAction)
            {

                float[] s = GetStates().ToArray();
                var ress = sess.run((this.predict, this.Qout), (this.input1, np.identity(s.Length)[s]));
                a = ress.Item1;
                allQ = ress.Item2;
                
                if (rand.NextDouble() < e)
                {
                    ress.Item1[0] = rand.Next(0, SmartActions.Actiomap.Count - 1);
                }
                lastState = GetStates();
                nextAction = !nextAction;
                return (float)ress.Item1[0];
            }
            else
            {
                float[] y = GetStates().ToArray();
                var ress = sess.run(Qout, (input1, np.identity(y.Length)[y]));
                NDArray maxQ1 = np.max(ress);
                NDArray targetQ = allQ;
                float reward = GetRewards() * 0.99f * maxQ1;
                targetQ.SetAtIndex(reward, a[0]);
                float[] s = GetStates().ToArray();
                var d = sess.run((updateModel, w._AsTensor()),  new FeedItem(input1, np.identity(s.Length)[s]), new FeedItem(nextQ, targetQ));
                nextAction = !nextAction;

                e = 1.0 / ((EpisodeCount / 50) + 10);

                return 0;
            }
        }



        public void Close()
        {
            Saver s = tf.train.Saver();
            s.save(sess, "model");
            sess.close();
            sess.Dispose();
            
        }
    }

}
