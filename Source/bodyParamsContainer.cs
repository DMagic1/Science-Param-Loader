#region license
/*The MIT License (MIT)
Science Values Container - An object to store persistent data about science param values

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
using System.Linq;
using ScienceParamModifier.Framework;

namespace ScienceParamModifier
{
	public class bodyParamsContainer : SM_ConfigNodeStorage
	{
		[Persistent]
		private string bodyName = "";
		[Persistent]
		private paramSet adjustedParams;

		private paramSet defaultParams;
		private paramSet stockParams;

		private CelestialBody body;
		private CelestialBodyScienceParams scienceParams;

		private float maxFlying;
		private float minSpace;
		private float maxSpace;

		public bodyParamsContainer()
		{

		}

		public bodyParamsContainer(CelestialBody b)
		{
			body = b;
			bodyName = b.bodyName;

			scienceParams = b.scienceValues;

			stockParams = setStockValues();
			adjustedParams = setStockValues();
			defaultParams = setStockValues();

			setThresholdLimits();
		}

		public override void OnDecodeFromConfigNode()
		{
			loadFromNode();
		}

		public override void OnEncodeToConfigNode()
		{
			defaultParams = setDefaultValues();
		}

		private void loadFromNode()
		{
			body = FlightGlobals.Bodies.FirstOrDefault(b => b.bodyName == bodyName);

			if (body == null)
				return;

			scienceParams = body.scienceValues;

			stockParams = setStockValues();
			defaultParams = setDefaultValues();

			setThresholdLimits();
		}

		private paramSet setDefaultValues()
		{
			return new paramSet(adjustedParams);
		}

		private paramSet setStockValues()
		{
			return new paramSet(scienceParams);
		}

		private void setThresholdLimits()
		{
			if (body.atmosphere)
			{
				maxFlying = (float)(body.atmosphereDepth - 1000);
				minSpace = (float)(body.atmosphereDepth + 1000);
			}
			else
			{
				maxFlying = 100000;
				minSpace = 1000;
			}
			maxSpace = (float)(body.sphereOfInfluence - 1000);
		}

		public CelestialBody Body
		{
			get { return body; }
		}

		public float MaxFlying
		{
			get { return maxFlying; }
		}

		public float MinSpace
		{
			get { return minSpace; }
		}

		public float MaxSpace
		{
			get { return maxSpace; }
		}

		public paramSet AdjustedParams
		{
			get { return adjustedParams; }
		}

		public void resetToDefault()
		{
			float recovered = adjustedParams.RecoveredData;
			adjustedParams = new paramSet(defaultParams);

			if (ScienceParamSettings.Instance != null && !ScienceParamSettings.Instance.alterRecoveredData)
				setNewParamValue(recovered, scienceParamType.recovered);
		}

		public void resetToStock()
		{
			adjustedParams = new paramSet(stockParams);
		}

		public void setNewParamValue(float value, scienceParamType type)
		{
			switch (type)
			{
				case scienceParamType.landed:
					if (value > 0 && value <= 50)
					{
						adjustedParams.LandedData = value;
						scienceParams.LandedDataValue = value;
					}
					break;
				case scienceParamType.splashed:
					if (value > 0 && value <= 50)
					{
						adjustedParams.SplashedData = value;
						scienceParams.SplashedDataValue= value;
					}
					break;
				case scienceParamType.flyingLow:
					if (value > 0 && value <= 50)
					{
						adjustedParams.FlyingLowData = value;
						scienceParams.FlyingLowDataValue = value;
					}
					break;
				case scienceParamType.flyingHigh:
					if (value > 0 && value <= 50)
					{
						adjustedParams.FlyingHighData = value;
						scienceParams.FlyingHighDataValue = value;
					}
					break;
				case scienceParamType.spaceLow:
					if (value > 0 && value <= 50)
					{
						adjustedParams.SpaceLowData = value;
						scienceParams.InSpaceLowDataValue = value;
					}
					break;
				case scienceParamType.spaceHigh:
					if (value > 0 && value <= 50)
					{
						adjustedParams.SpaceHighData = value;
						scienceParams.InSpaceHighDataValue = value;
					}
					break;
				case scienceParamType.recovered:
					if (value > 0 && value <= 50)
					{
						adjustedParams.RecoveredData = value;
						scienceParams.RecoveryValue = value;
					}
					break;
				case scienceParamType.flyingAltitude:
					if (value >= 100 && value <= maxFlying)
					{
						adjustedParams.FlyingThreshold = value;
						scienceParams.flyingAltitudeThreshold = value;
					}
					break;
				case scienceParamType.spaceAltitude:
					if (value >= minSpace && value <= maxSpace)
					{
						adjustedParams.SpaceThreshold = value;
						scienceParams.spaceAltitudeThreshold = value;
					}
					break;
			}
		}

	}
}
