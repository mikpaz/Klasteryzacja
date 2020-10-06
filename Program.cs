using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace klasteryzacja
{
    class Program
    {
        static void Main(string[] args)
        {
            // Loading data from file
            double[][] data = loadData();
            // Number of possible classes
            int numClasses = 3;
            int numFeatures = 4;
            double[] toPredict = {1.2, 2.5, 5.2, 3.0};
            Console.Write("Flower values to predict: (");
            for (int i = 0; i < toPredict.Length - 1; i++)
            {
                Console.Write(toPredict[i] + ", ");
            }
            Console.WriteLine(toPredict[3] + ")");
            int k = 6;
            Console.WriteLine("k = " + k);

            int predicted = classify(toPredict, data, numClasses, numFeatures, k);
            Console.WriteLine("Predicted class = " + predicted);
            Console.WriteLine("Predicted flower type: " + intToFlowerName(predicted));
        }

        static double flowerNameToInt(string name)
        {
            double binaryName = 0;
            // Every flower name has assigned specific binary value
            if (name == "Iris-setosa") binaryName = 0;
            else if (name == "Iris-versicolor") binaryName = 1;
            else if (name == "Iris-virginica") binaryName = 2;
            return binaryName;
        }

        static double[][] loadData()
        {
            string[] lines = File.ReadAllLines(@"C:\Users\pazer\GoogleDrive\Studia\Sem4\SystemySztucznejInteligencji\klasteryzacja\bazaIris.txt");
            double[][] data = new double[lines.Length][];
            
            for (int i = 0; i < lines.Length; i++)
            {
                // Database is using , sign as separator
                string[] tmp = lines[i].Split(',');
                
                // Creating new array in each line of original array
                data[i] = new double[tmp.Length];
                for (int j = 0; j < tmp.Length; j++)
                {
                    // Converting flower names to binary code
                    if (j == tmp.Length - 1)
                    {
                        double flowerName = flowerNameToInt(tmp[j]);
                        data[i][j] = flowerName;
                    }
                    // Converting other fields of database from string do double
                    else
                        data[i][j] = Convert.ToDouble(tmp[j]);
                }
            }
            return data;
        }

        static double distance(double[] toPredict, double[] data)
        {
            // Calculatin distance according to given formula
            double sum = 0;
            for (int i = 0; i < toPredict.Length; ++i)
                sum += (toPredict[i] - data[i]) * (toPredict[i] - data[i]);
            double distance = Math.Sqrt(sum);
            return distance;
        }

        static int vote(IndexAndDistance[] info, double[][] data, int numClasses, int k)
        {
            int[] votes = new int[numClasses];
            for (int i = 0; i < k; ++i)
            {
                int idx = info[i].idx;
                int c = (int)data[idx][4];
                ++votes[c];
            }
            int mostVotes = 0;
            int classWithMostVotes = 0;
            for (int j = 0; j < numClasses; ++j)
            {
                if (votes[j] > mostVotes)
                {
                    mostVotes = votes[j];
                    classWithMostVotes = j;
                }
            }
            return classWithMostVotes;
        }

        static int classify(double[] toPredict, double[][] data, int numClasses, int numFeatures, int k)
        {
            int n = data.Length;
            IndexAndDistance[] info = new IndexAndDistance[n];
            for (int i = 0; i < n; ++i)
            {
                IndexAndDistance curr = new IndexAndDistance();
                double dist = distance(toPredict, data[i]);
                curr.idx = i;
                curr.dist = dist;
                info[i] = curr;
            }
            Array.Sort(info);  // sort by distance
            Console.WriteLine("Nearest Neighbour -> Distance -> Class");
            for (int i = 1; i <= k; i++)
            {
                double possibleClass = data[info[i].idx][4];
                string distance = info[i].dist.ToString("N2");
                Console.Write("(");
                for (int j = 0; j < numFeatures; j++)
                {
                    if (j == numFeatures - 1) Console.Write(data[info[i].idx][j]);
                    else Console.Write(data[info[i].idx][j] + ", ");
                }
                Console.Write(") -> " + distance + " -> " + possibleClass + "\n");
            }
            int result = vote(info, data, numClasses, k);
            return result;
        }

        static string intToFlowerName(int intName)
        {
            string flowerName = "";
            if (intName == 0) flowerName = "Iris-setosa";
            else if (intName == 1) flowerName = "Iris-versicolor";
            else if (intName == 2) flowerName = "Iris-virginica";
            return flowerName;
        }


        public class IndexAndDistance : IComparable<IndexAndDistance>
        {
            public int idx;
            public double dist;
            public int CompareTo(IndexAndDistance other)
            {
                if (this.dist < other.dist) return -1;
                else if (this.dist > other.dist) return +1;
                else return 0;
            }
        }
    }
}
