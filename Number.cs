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

        // Converts given Number into another numeral system
        public static string Convert(Number original, int targetSys)
        {
            double valueInDecimal = 0;
            double valueInFractionDecimal = 0;
            string originalValue = "";
            string originalFractionValue = "";

            int commaPosition = original.value.IndexOf(",");
            if (commaPosition == -1)
            {
                originalValue = original.value;
                originalFractionValue = "0";
            }
            else
            {
                originalValue = original.value.Substring(0, commaPosition);
                originalFractionValue = original.value.Substring(commaPosition + 1);
            }

            // Establishing value in decimal system

            if (original.system == 10)
            {
                valueInDecimal = Double.Parse(originalValue);
                valueInFractionDecimal = Double.Parse(originalFractionValue);
                double x = Math.Pow(10, originalFractionValue.Length);
                valueInFractionDecimal /= x;
            }
            else
            {
                double index = 0;

                // Iterating through numbers of original number
                for (int i = originalValue.Length - 1; i >= 0; i--)
                {
                    // Establishing value of current character

                    double symbolValue = 0;
                    byte asciiCode = Encoding.Default.GetBytes(originalValue)[i];

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

                index = -1;

                // Iterating through numbers of fraction
                for (int i = 0; i < originalFractionValue.Length; i++)
                {
                    // Establishing value of current character

                    double symbolValue = 0;
                    byte asciiCode = Encoding.Default.GetBytes(originalFractionValue)[i];

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

                    valueInFractionDecimal += symbolValue * Math.Pow((double)original.system, (double)index);
                    index--;
                }
            }

            // Converting value from decimal to target system

            string targetValue = "";
            int valueInDecimalInt = (int)valueInDecimal;
            if (valueInDecimalInt == 0)
            {
                targetValue = "0";
            }

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

            // Converting fraction
            if (valueInFractionDecimal != 0)
            {
                targetValue += ",";
                int index = 1;

                while (valueInFractionDecimal != 0)
                {
                    if (Math.Truncate(valueInFractionDecimal * targetSys) < 9)
                    {
                        targetValue += (Math.Truncate(valueInFractionDecimal * targetSys)).ToString();
                    }
                    else
                    {
                        char letterFromNumber = (char)(Math.Truncate(valueInFractionDecimal * targetSys) + 55);
                        targetValue += letterFromNumber.ToString();
                    }

                    valueInFractionDecimal *= targetSys;
                    valueInFractionDecimal -= Math.Floor(valueInFractionDecimal);

                    // Break
                    if (++index == 10)
                    {
                        targetValue = targetValue.TrimEnd('0');
                        break;
                    }
                }
            }

            return targetValue;
        }
    }
}