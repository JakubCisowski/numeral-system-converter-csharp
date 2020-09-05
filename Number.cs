using System;
using System.Text;

namespace NumeralSystemConverter
{
    public class Number
    {
        private string value; // Value of the number is string, because it could contain letters, dot or comma
        private int system;

        public Number(string val, int sys)
        {
            this.value = val;
            this.system = sys;
        }

        public static string Convert(Number original, int targetSys)
        {
            // Establishing value in decimal system

            double valueInDecimal = 0;

            if (original.system == 10)
            {
                valueInDecimal = Double.Parse(original.value);
            }
            else
            {
                double index = 0;

                // Iterating through numbers of original number
                for (int i = original.value.Length - 1; i >= 0; i--)
                {
                    // Establishing value of current character

                    double symbolValue = 0;
                    byte asciiCode = Encoding.Default.GetBytes(original.value)[i];

                    // In case of number
                    if (asciiCode >= 48 && asciiCode <= 57)
                    {
                        symbolValue = (double)(asciiCode - 48);
                    }
                    // In case of upper case letter
                    else if (asciiCode >= 65 && asciiCode <= 90)
                    {
                        symbolValue = (double)(asciiCode - 55);
                    }
                    // In case of lower case letter
                    else if (asciiCode >= 97 && asciiCode <= 122)
                    {
                        symbolValue = (double)(asciiCode - 87);
                    }

                    valueInDecimal += symbolValue * Math.Pow((double)original.system, (double)index);
                    index++;
                }
            }

            // Converting value from decimal to target system
            string targetValue = "";
            int valueInDecimalInt = (int)valueInDecimal;

            while (valueInDecimalInt != 0)
            {
                if (valueInDecimalInt % targetSys < 9)
                {
                    targetValue = targetValue.Insert(0, (valueInDecimalInt % targetSys).ToString());
                }
                else
                {
                    char letterFromNumber = (char)(valueInDecimalInt % targetSys + 55);
                    targetValue = targetValue.Insert(0, letterFromNumber.ToString());
                }

                valueInDecimalInt /= targetSys;
            }

            return targetValue;
        }
    }
}