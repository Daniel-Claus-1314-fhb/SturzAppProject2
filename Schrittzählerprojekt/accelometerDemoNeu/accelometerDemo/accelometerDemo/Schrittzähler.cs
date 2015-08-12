using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accelometerDemo
{
    class Schrittzähler
    {
        Datenumwandlung dtu = new Datenumwandlung();
        short[] dataToVectors(short[] data)
        {
            short help = 0;
            List<short> newdata = new List<short>();
            for (int i = 0; i < data.Length / 3; i++)
            {
                //double test = Math.Sqrt(Math.Pow(data[3 * i], 2) + Math.Pow(data[3 * i + 1], 2) + Math.Pow(data[3 * i + 2], 2));
                help = dtu.DoubleToShort((Math.Sqrt(Math.Pow((double)data[3 * i] / 100, 2) + Math.Pow((double)data[3 * i + 1] / 100, 2) + Math.Pow((double)data[3 * i + 2] / 100, 2))));
                newdata.Add(help);
            }
            
            return newdata.ToArray();
        }
        public int CountSteps(short[] data)
        {
            data = dataToVectors(data);
            List<short> newdata = new List<short>();
            List<short> indexList = new List<short>();
            for (short i = 1; i < data.Length - 1; i++)
            { 
                if(((data[i] - data[i-1]) * (data[i] - data[i+1])) > 0 )
                {
                    newdata.Add(data[i]);
                    indexList.Add(i);
                }
            }
            short[] newdataA = newdata.ToArray();
            short[] indexListA = indexList.ToArray();
            return FilterThreshold(newdataA, indexListA, 125);
        }

        int FilterThreshold(short[] data, short[] indexList, short threshold)
        {
            List<short> newdata = new List<short>();
            List<short> newindexList = new List<short>();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > threshold)
                {
                    newdata.Add(data[i]);
                    newindexList.Add(indexList[i]);
                }
            }
            short[] newdataA = newdata.ToArray();
            short[] indexListA = newindexList.ToArray();
            return FilterDistance(newdataA, indexListA, 30);
        }

        int FilterDistance(short[] data, short[] indexList, short distance)
        {
            List<short> newdata = new List<short>();
            List<short> newindexList = new List<short>();
            short help = (short)((int)(distance + 1)* -1);
            int counter = 0;
            for (int i = 0; i < indexList.Length; i++)
            {
                if (indexList[i] - help > distance)
                {
                    counter++;
                    help = indexList[i];
                }
            }
            return counter;
        }

    }
}
