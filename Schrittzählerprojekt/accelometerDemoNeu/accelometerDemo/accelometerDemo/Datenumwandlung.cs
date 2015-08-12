using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accelometerDemo
{
    class Datenumwandlung
    {
        public short DoubleToShort(double dbl)
        {
            string dblstring = String.Format("{0,5:0.00}", dbl);
            byte indexflag = 0;
            int dblint = 0;
            if (dblstring.StartsWith("-"))
            {
                dblstring.Remove(0);
                indexflag = 1;
            }
            if (dblstring.Contains("."))
            {
                dblstring = dblstring.Remove(dblstring.IndexOf("."), 1);
            }
            if (dblstring.Length < 3)
            {
                dblstring = dblstring + "0";
            }
            if (dblstring.StartsWith("0"))
            {
                dblstring.Remove(0);
            }
            if (dblstring.StartsWith("0"))
            {
                dblstring.Remove(0);
            }
            if (indexflag == 1)
            {
                dblint = Convert.ToInt32(dblstring);
            }
            else
            {
                dblint = Convert.ToInt32(dblstring);
            }
            return Convert.ToInt16(dblint);
        }
    }
}
