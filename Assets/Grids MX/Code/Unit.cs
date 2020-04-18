using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace mx
{
	namespace Grids
	{
		public enum Unit
		{
			Meter,
			Decimeter,
			Centimeter,
			Millimeter,
			Decameter,
			Hectometer,
			Kilometer,
			Foot,
			Inch,
			Yard,
			Mile,
			Attoparsec
		}

		public static class UnitUtil
		{
			private const string ABBREV_METER = "m";
			private const string ABBREV_DECIMETER = "dm";
			private const string ABBREV_CENTIMETER = "cm";
			private const string ABBREV_MILLIMETER = "mm";
			private const string ABBREV_DECAMETER = "dam";
			private const string ABBREV_HECTOMETER = "hm";
			private const string ABBREV_KILOMETER = "km";
			private const string ABBREV_FOOT = "ft";
			private const string ABBREV_INCH = "in";
			private const string ABBREV_YARD = "yd";
			private const string ABBREV_MILE = "mi";
			private const string ABBREV_ATTOPARSEC = "apc";

			public static double Convert(Unit from, Unit to, double measurement)
			{
				if (from == to)
				{
					return measurement;
				}

				if (from != Unit.Meter)
				{
					measurement /= MeterToXMultiplier(from);
				}

				if (to != Unit.Meter)
				{
					measurement *= MeterToXMultiplier(to);
				}

				return measurement;
			}

			private static double MeterToXMultiplier(Unit unit)
			{
				switch (unit)
				{
					case Unit.Meter:		return 1.0;
					case Unit.Decimeter:	return 10.0;
					case Unit.Centimeter:	return 100.0;
					case Unit.Millimeter:	return 1000.0;
					case Unit.Decameter:	return 0.1;
					case Unit.Hectometer:	return 0.01;
					case Unit.Kilometer:	return 0.001;
					case Unit.Foot:			return 3.28084;
					case Unit.Inch:			return 39.37008;
					case Unit.Yard:			return 1.09361;
					case Unit.Mile:			return 0.000621371;
					case Unit.Attoparsec:	return 32.4078;
					default:
						Debug.LogError(string.Format("GridsMX -- Unknown Unit: {0}", unit));
						return 1.0;
				}
			}

			public static string Abbreviation(Unit unit)
			{
				switch (unit)
				{
					case Unit.Meter:		return ABBREV_METER;
					case Unit.Decimeter:	return ABBREV_DECIMETER;
					case Unit.Centimeter:	return ABBREV_CENTIMETER;
					case Unit.Millimeter:	return ABBREV_MILLIMETER;
					case Unit.Decameter:	return ABBREV_DECAMETER;
					case Unit.Hectometer:	return ABBREV_HECTOMETER;
					case Unit.Kilometer:	return ABBREV_KILOMETER;
					case Unit.Foot:			return ABBREV_FOOT;
					case Unit.Inch:			return ABBREV_INCH;
					case Unit.Yard:			return ABBREV_YARD;
					case Unit.Mile:			return ABBREV_MILE;
					case Unit.Attoparsec:	return ABBREV_ATTOPARSEC;
					default:
						Debug.LogError(string.Format("GridsMX -- Unknown Unit: {0}", unit));
						return "u";
				}
			}

#if UNITY_EDITOR
			public static double DrawConvertedSizeField(string label, double size, Unit storeAs, Unit displayAs, float minWidth)
			{
				size = Convert(storeAs, displayAs, size);
#if UNITY_5
				size = EditorGUILayout.DoubleField(label, size, GUILayout.MinWidth(minWidth));
#else
				size = EditorGUILayout.FloatField(label, (float)size, GUILayout.MinWidth(minWidth));
#endif
				return Convert(displayAs, storeAs, size);
			}

			public static double DrawConvertedSizeField(double size, Unit storeAs, Unit displayAs, float minWidth)
			{
				size = Convert(storeAs, displayAs, size);
#if UNITY_5
				size = EditorGUILayout.DoubleField(size, GUILayout.MinWidth(minWidth));
#else
				size = EditorGUILayout.FloatField((float)size, GUILayout.MinWidth(minWidth));
#endif
				return Convert(displayAs, storeAs, size);
			}
#endif
		}
	}
}