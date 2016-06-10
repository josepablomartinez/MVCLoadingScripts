using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    internal static class MapUtilities
    {
        const int DotsPerInch = 96;
        private static object _lockObject = new object();
        static Dictionary<string, double> _inchesPerUnit = InitializeInchedPerUnit();

        static Dictionary<string, double> InitializeInchedPerUnit()
        {
            Dictionary<string, double> inchesPerUnits = new Dictionary<string, double>();
            inchesPerUnits.Add("DecimalDegree", 4374754);
            inchesPerUnits.Add("Feet", 12.0);
            inchesPerUnits.Add("Meter", 39.3701);
            return inchesPerUnits;
        }

        internal static double GetResolutionFromScale(double scale, GeographyUnit unit)
        {
            //Validators.CheckMapUnitIsValid(unit);

            double resolution = scale / (_inchesPerUnit[unit.ToString()] * DotsPerInch);

            return resolution;
        }

        internal static int GetZoomFromScale(double scale, Collection<double> zoomLevelSets)
        {
            int zoomLevel = 0;
            int counter = 0;

            foreach (double zoomLevelScale in zoomLevelSets)
            {
                if (counter == 0)
                {
                    counter++;
                }
                else if (scale <= zoomLevelScale)
                {
                    counter++;
                    zoomLevel++;
                }
                else
                {
                    break;
                }
            }

            return zoomLevel;
        }

        internal static void WriteDebugInfo()
        {
            WriteDebugInfo("", 0, -1);
        }

        internal static void WriteDebugInfo(string information, int tabCount)
        {
            WriteDebugInfo(information, tabCount, -1);
        }

        internal static void WriteDebugInfo(string information, int tabCount, long milliSeconds)
        {
            if (milliSeconds >= 0)
            {
                information = milliSeconds + "ms: " + information;
            }

            for (int i = 0; i < tabCount; i++)
            {
                information = "\t" + information;
            }
            string logFileFullPath = ConfigurationManager.AppSettings.Get("LogFileForWebEdition");
            if (!string.IsNullOrEmpty(logFileFullPath))
            {
                lock (_lockObject)
                {
                    using (TextWriter debugStreamWriter = new StreamWriter(logFileFullPath, true))
                    {
                        debugStreamWriter.WriteLine(information);
                        debugStreamWriter.Flush();
                        debugStreamWriter.Close();
                    }
                }
            }
        }
    }
}
