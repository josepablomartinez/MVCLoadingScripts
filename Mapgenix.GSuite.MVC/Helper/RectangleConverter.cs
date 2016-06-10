using System;
using System.Globalization;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    internal static class RectangleConverter
    {
        internal static string ConvertRectangleToString(RectangleShape rectangle)
        {
            string rectangleString = string.Empty;
            if (rectangle != null)
            {
                rectangleString += rectangle.LowerLeftPoint.X.ToString(CultureInfo.InvariantCulture) + ",";
                rectangleString += rectangle.LowerLeftPoint.Y.ToString(CultureInfo.InvariantCulture) + ",";
                rectangleString += rectangle.UpperRightPoint.X.ToString(CultureInfo.InvariantCulture) + ",";
                rectangleString += rectangle.UpperRightPoint.Y.ToString(CultureInfo.InvariantCulture);
            }
            return rectangleString;
        }

        internal static RectangleShape ConvertStringToRectangle(string rectangleString)
        {
            try
            {
                string[] rectangleStrings = rectangleString.Split(',');
                double left = double.Parse(rectangleStrings[0], CultureInfo.InvariantCulture);
                double bottom = double.Parse(rectangleStrings[1], CultureInfo.InvariantCulture);
                double right = double.Parse(rectangleStrings[2], CultureInfo.InvariantCulture);
                double top = double.Parse(rectangleStrings[3], CultureInfo.InvariantCulture);
                return new RectangleShape(left, top, right, bottom);
            }
            catch
            {
                throw new ArgumentException("Invalid rectangle string.", "rectangleString");
            }
        }
    }
}
