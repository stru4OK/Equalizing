using System;
using System.Globalization;

namespace Equalizing
{
    class Utility
    {
        public static bool isCorrectDate(string date)
        {
            string format = "dd.MM.yyyy H:mm:ss";
            CultureInfo  provider = new CultureInfo("en-EN");

            try
            {
                DateTime.ParseExact(date, format, provider);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
