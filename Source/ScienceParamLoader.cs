#region license
/* Science Param Loader
 * Module for altering Celestial Body Science Param values from a config file.
 *
 * Copyright (c) 2015, David Grandy <david.grandy@gmail.com>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation and/or other materials 
 * provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 */
#endregion

using UnityEngine;

namespace ScienceParamLoader
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	public class ScienceParamLoader : MonoBehaviour
	{
		private static bool run;

		private void Start()
		{
			if (!run)
				paramLoader();
		}

		private void paramLoader()
		{
			run = true;

			CelestialBody body = null;
			int i = 0;
			float f = 0;

			print("[Science Param Loader] Looking Up Custom Science Parameters Now...");

			foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("Custom_Science_Params"))
			{
				if (node == null)
					continue;

				foreach (ConfigNode bodyNode in node.GetNodes("Body_Science_Params"))
				{
					if (bodyNode == null)
						continue;

					if (!int.TryParse(bodyNode.GetValue("Body_Index"), out i))
					{
						print("[Science Param Loader] Invalid Celestial Body Index [" + bodyNode.GetValue("Body_Index") + "]; Skipping Config...");
						continue;
					}

					if (FlightGlobals.Bodies.Count <= i)
					{
						print("[Science Param Loader] No Body With Index [" + i + "]; Skipping Config...");
						continue;
					}

					body = FlightGlobals.Bodies[i];

					if (body == null)
					{
						print("[Science Param Loader] Celestial Body Value Null For Index [" + i + "]; Skipping Config...");
						continue;
					}

					if (float.TryParse(bodyNode.GetValue("LandedDataValue"), out f))
						body.scienceValues.LandedDataValue = f;
					if (float.TryParse(bodyNode.GetValue("SplashedDataValue"), out f))
						body.scienceValues.SplashedDataValue = f;
					if (float.TryParse(bodyNode.GetValue("FlyingLowDataValue"), out f))
						body.scienceValues.FlyingLowDataValue = f;
					if (float.TryParse(bodyNode.GetValue("FlyingHighDataValue"), out f))
						body.scienceValues.FlyingHighDataValue = f;
					if (float.TryParse(bodyNode.GetValue("InSpaceLowDataValue"), out f))
						body.scienceValues.InSpaceLowDataValue = f;
					if (float.TryParse(bodyNode.GetValue("InSpaceHighDataValue"), out f))
						body.scienceValues.InSpaceHighDataValue = f;
					if (float.TryParse(bodyNode.GetValue("RecoveredDataValue"), out f))
						body.scienceValues.RecoveryValue = f;
					if (float.TryParse(bodyNode.GetValue("FlyingAltitude"), out f))
						body.scienceValues.flyingAltitudeThreshold = f;
					if (float.TryParse(bodyNode.GetValue("SpaceAltitude"), out f))
						body.scienceValues.spaceAltitudeThreshold = f;
					print("[Science Param Loader] New Science Paramaters Set For [" + body.theName + "]");
				}
			}
		}

	}
}
