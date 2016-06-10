using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Mapgenix.Shapes;
using Mapgenix.Styles;
using Mapgenix.Canvas;
using Mapgenix.GSuite.Mvc.Properties;

namespace Mapgenix.GSuite.Mvc
{
    internal static class Validators
    {
        internal static void CheckParameterIsNotNull(Object objectToTest, string parameterName)
        {
            if (objectToTest == null)
            {
                string exceptionDescription = ExceptionDescription.ParameterIsNull;
                throw new ArgumentNullException(parameterName, exceptionDescription);
            }
        }

        internal static void CheckParameterIsNotNullOrEmpty(string stringToTest, string parameterName)
        {
            if (String.IsNullOrEmpty(stringToTest))
            {
                string exceptionDescription = ExceptionDescription.ParameterIsNullOrEmpty;
                throw new ArgumentNullException(parameterName, exceptionDescription);
            }
        }

        internal static void CheckValueIsGreaterOrEqualToZero(double value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, ExceptionDescription.TheValueShouldBeGreaterOrEqualToZero);
            }
        }

        internal static void CheckValueIsGreaterThanZero(double value, string parameterName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, ExceptionDescription.TheValueShouldBeGreaterThanZero);
            }
        }



        internal static void CheckJpegCompressionQuality(int quality, string paramterName)
        {
            if (quality < 1 || quality > 100)
            {
                throw new ArgumentOutOfRangeException(paramterName, ExceptionDescription.JpegImageCompressionShouldBeBetweenZeroAndOneHundred);
            }
        }

        internal static void NotSupport(string message)
        {
            throw new NotSupportedException(message);
        }

        internal static void CheckColumnNameIsInFeature(string columnName, IEnumerable<Feature> features)
        {
            bool exist = false;

            int featureCount = 0;
            foreach (Feature feature in features)
            {
                featureCount++;
                Dictionary<string, string> dict = feature.ColumnValues;

                foreach (KeyValuePair<string, string> item in dict)
                {
                    string name = item.Key;
                    if (columnName.ToUpperInvariant().Trim().Equals(name.ToUpperInvariant().Trim(), StringComparison.CurrentCulture))
                    {
                        exist = true;
                        break;
                    }
                }
            }

            if (featureCount == 0) { exist = true; }

            if (!exist)
            {
                throw new ArgumentException(ExceptionDescription.FieldNameIsNotInFeature, columnName);
            }
        }

        internal static void CheckClassBreaksAreValid(Collection<MarkerClassBreak> classBreaks)
        {
            double tmp = double.MinValue;
            for (int i = 1; i < classBreaks.Count - 1; i++)
            {
                if (classBreaks[i].Value <= tmp)
                {
                    throw new ArgumentException(ExceptionDescription.ClassBreaksIsValid);
                }
                else
                {
                    tmp = classBreaks[i].Value;
                }
            }
        }

        internal static void CheckMapUnitIsValid(GeographyUnit mapUnit)
        {
            switch (mapUnit)
            {
                case GeographyUnit.DecimalDegree: break;
                case GeographyUnit.Feet: break;
                case GeographyUnit.Meter: break;
                default: throw new ArgumentException(ExceptionDescription.MapUnitIsInvalid, "mapUnit");
            }
        }

        internal static void CheckPanDirectionIsValid(PanDirection panDirection)
        {
            switch (panDirection)
            {
                case PanDirection.Down: break;
                case PanDirection.Left: break;
                case PanDirection.LowerLeft: break;
                case PanDirection.LowerRight: break;
                case PanDirection.Right: break;
                case PanDirection.Up: break;
                case PanDirection.UpperLeft: break;
                case PanDirection.UpperRight: break;
                default: throw new ArgumentException(ExceptionDescription.PanDirectionIsInvalid, "panDirection");
            }
        }

        internal static void CheckBreakValueInclusionIsValid(BreakValueInclusion breakValueInclusion)
        {
            switch (breakValueInclusion)
            {
                case BreakValueInclusion.ExcludeValue: break;
                case BreakValueInclusion.IncludeValue: break;
                default: throw new ArgumentOutOfRangeException("breakValueInclusion", ExceptionDescription.EnumerationOutOfRange);
            }
        }

        internal static void CheckGoogleMapTypeIsValid(GoogleMapType mapType)
        {
            switch (mapType)
            {
                case GoogleMapType.Hybrid: break;
                case GoogleMapType.Normal: break;
                case GoogleMapType.Physical: break;
                case GoogleMapType.Satellite: break;
                default: throw new ArgumentOutOfRangeException("mapType", ExceptionDescription.EnumerationOutOfRange);
            }
        }

        internal static void CheckWebImageFormatIsValid(WebImageFormat webImageFormat)
        {
            switch (webImageFormat)
            {
                case WebImageFormat.Jpeg: break;
                case WebImageFormat.Png: break;
                default: throw new ArgumentOutOfRangeException("webImageFormat", ExceptionDescription.EnumerationOutOfRange);
            }
        }

        internal static void CheckTransitionEffectIsValid(TransitionEffect transitionEffect)
        {
            switch (transitionEffect)
            {
                case TransitionEffect.None: break;
                case TransitionEffect.Stretching: break;
                default: throw new ArgumentOutOfRangeException("transitionEffect", ExceptionDescription.EnumerationOutOfRange);
            }
        }

        internal static void CheckTileTypeIsValid(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.MultipleTile: break;
                case TileType.SingleTile: break;
                default: throw new ArgumentOutOfRangeException("tileType", ExceptionDescription.EnumerationOutOfRange);
            }
        }

        internal static void CheckOverlayEpsgProjectionGetAndSetValid(bool isBaseOverlay)
        {
            if (!isBaseOverlay)
            {
                throw new NotSupportedException(ExceptionDescription.EpsgProjectionSupportBaseOverlayOnly);
            }
        }

        internal static void CheckMapActualWidthAndHeightValid(double value)
        {
            if (Double.IsNaN(value))
            {
                throw new NotSupportedException(ExceptionDescription.ActualWidthAndHeightInvalid);
            }
        }

    }
}
