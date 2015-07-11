#region license
/*The MIT License (MIT)
Science Param Set - An object to store a set of science values

Copyright (c) 2015 DMagic

KSP Plugin Framework by TriggerAu, 2014: http://forum.kerbalspaceprogram.com/threads/66503-KSP-Plugin-Framework

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using ScienceParamModifier.Framework;

namespace ScienceParamModifier
{
	public class paramSet : SM_ConfigNodeStorage
	{
		[Persistent]
		private float landedData = 1;
		[Persistent]
		private float splashedData = 1;
		[Persistent]
		private float flyingLowData = 1;
		[Persistent]
		private float flyingHighData = 1;
		[Persistent]
		private float spaceLowData = 1;
		[Persistent]
		private float spaceHighData = 1;
		[Persistent]
		private float recoveredData = 1;
		[Persistent]
		private float flyingThreshold = 1;
		[Persistent]
		private float spaceThreshold = 1;

		public paramSet()
		{

		}

		public paramSet(CelestialBodyScienceParams p)
		{
			landedData = p.LandedDataValue;
			splashedData = p.SplashedDataValue;
			flyingLowData = p.FlyingLowDataValue;
			flyingHighData = p.FlyingHighDataValue;
			spaceLowData = p.InSpaceLowDataValue;
			spaceHighData = p.InSpaceHighDataValue;
			recoveredData = p.RecoveryValue;
			flyingThreshold = p.flyingAltitudeThreshold;
			spaceThreshold = p.spaceAltitudeThreshold;
		}

		public paramSet(paramSet copy)
		{
			LandedData = copy.LandedData;
			SplashedData = copy.SplashedData;
			FlyingLowData = copy.FlyingLowData;
			FlyingHighData = copy.FlyingHighData;
			SpaceLowData = copy.SpaceLowData;
			SpaceHighData = copy.SpaceHighData;
			RecoveredData = copy.RecoveredData;
			FlyingThreshold = copy.FlyingThreshold;
			SpaceThreshold = copy.SpaceThreshold;
		}

		public float LandedData
		{
			get { return landedData; }
			set { landedData = value; }
		}
		public float SplashedData
		{
			get { return splashedData; }
			set { splashedData = value; }
		}
		public float FlyingLowData
		{
			get { return flyingLowData; }
			set { flyingLowData = value; }
		}
		public float FlyingHighData
		{
			get { return flyingHighData; }
			set { flyingHighData = value; }
		}
		public float SpaceLowData
		{
			get { return spaceLowData; }
			set { spaceLowData = value; }
		}
		public float SpaceHighData
		{
			get { return spaceHighData; }
			set { spaceHighData = value; }
		}
		public float RecoveredData
		{
			get { return recoveredData; }
			set { recoveredData = value; }
		}
		public float FlyingThreshold
		{
			get { return flyingThreshold; }
			set { flyingThreshold = value; }
		}
		public float SpaceThreshold
		{
			get { return spaceThreshold; }
			set { spaceThreshold = value; }
		}
	}

	public enum scienceParamType
	{
		landed,
		splashed,
		flyingLow,
		flyingHigh,
		spaceLow,
		spaceHigh,
		recovered,
		flyingAltitude,
		spaceAltitude,
	}
}
