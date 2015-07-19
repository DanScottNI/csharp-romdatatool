using System;
using System.Collections.Generic;
using System.Text;

namespace ROMDataWin
{
    class HelperMethods
    {
        public static string PadHex(int number)
        {
            string hexvalue = Convert.ToString(number, 16);

            if (hexvalue.Length == 1)
            {
                hexvalue = "0" + hexvalue;
            }

            return hexvalue.ToUpper();
        }
    }
}
