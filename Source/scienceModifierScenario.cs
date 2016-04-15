#region license
/*The MIT License (MIT)
Science Modifier Scenario : A scenario module to store data for the addon and control save/load

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
using System.Collections.Generic;
using System.Linq;

using ScienceParamModifier.Toolbar;
using ScienceParamModifier.Framework;
using UnityEngine;

namespace ScienceParamModifier
{
	[KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames | ScenarioCreationOptions.AddToNewCareerGames | ScenarioCreationOptions.AddToExistingScienceSandboxGames | ScenarioCreationOptions.AddToNewScienceSandboxGames, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER)]
	public class scienceModifierScenario : ScenarioModule
	{
		public static scienceModifierScenario Instance
		{
			get { return instance; }
		}

		private static scienceModifierScenario instance;

		public bool alterRecoveredData = false;
		public bool stockToolbar = true;
		public bool disableToolbar = false;
		public bool warnedToolbar = false;
		public bool warnedAlterRecovered = false;
		private bool disableSaveLoading = false;

		internal smStockToolbar appLauncherButton;
		internal smToolbar blizzyToolbarButton;
		internal scienceParamModifier scienceParamModifier;

		private ScienceConfigValuesNode smNode;

		public ScienceConfigValuesNode SMNode
		{
			get { return smNode; }
		}

		private void Start()
		{
			instance = this;

			if (!disableToolbar)
			{
				if (stockToolbar || !ToolbarManager.ToolbarAvailable)
				{
					appLauncherButton = gameObject.AddComponent<smStockToolbar>();
					if (blizzyToolbarButton != null)
						Destroy(blizzyToolbarButton);
				}
				else if (ToolbarManager.ToolbarAvailable && !stockToolbar)
				{
					blizzyToolbarButton = gameObject.AddComponent<smToolbar>();
					if (appLauncherButton != null)
						Destroy(appLauncherButton);
				}
			}
		}

		private void OnDestroy()
		{
			if (scienceParamModifier != null)
				Destroy(scienceParamModifier);
			if (appLauncherButton != null)
				Destroy(appLauncherButton);
			if (blizzyToolbarButton != null)
				Destroy(blizzyToolbarButton);
		}

		#region Save/Load

		public override void OnLoad(ConfigNode node)
		{
			smNode = smConfigLoad.TopNode;
			if (smNode == null)
				smNode = new ScienceConfigValuesNode(smConfigLoad.fileName);

			if (smNode != null)
			{
				disableSaveLoading = smNode.DisableSaveSpecificValues;
				alterRecoveredData = smNode.AlterRecoveredData;
				stockToolbar = smNode.StockToolbar;
				disableToolbar = smNode.DisableToolbar;
				warnedAlterRecovered = smNode.WarnedAlterRecovered;
				warnedToolbar = smNode.WarnedToolbar;
			}

			if (!disableSaveLoading)
			{
				node.TryGetValue("alterRecoveredData", ref alterRecoveredData);
				node.TryGetValue("stockToolbar", ref stockToolbar);
				node.TryGetValue("warnedAlterRecovered", ref warnedAlterRecovered);
				node.TryGetValue("warnedToolbar", ref warnedToolbar);

				try
				{
					ConfigNode paramNodes = node.GetNode("Body_Science_Params");

					if (paramNodes != null)
					{
						foreach (ConfigNode paramNode in paramNodes.GetNodes("Body_Param"))
						{
							if (paramNode == null)
								continue;

							string bodyName = paramNode.GetValue("BodyName");
							string valuesString = paramNode.GetValue("ParamValues");
							stringParse(valuesString, bodyName);
						}
					}
				}
				catch (Exception e)
				{
					SM_MBE.LogFormatted("Body Science Param List Cannot Be Generated Or Loaded: {0}", e);
				}

			}
			else
				SM_MBE.LogFormatted("All save-specific settings disabled; values loaded from config file...");

			//Start the window object
			if (!disableToolbar)
			{
				try
				{
					scienceParamModifier = gameObject.AddComponent<scienceParamModifier>();
				}
				catch (Exception e)
				{
					SM_MBE.LogFormatted("Science Param Modifier Windows Cannot Be Started: {0}", e);
				}
			}
		}

		public override void OnSave(ConfigNode node)
		{
			if (!disableSaveLoading)
			{
				node.AddValue("alterRecoveredData", alterRecoveredData);
				node.AddValue("stockToolbar", stockToolbar);
				node.AddValue("warnedAlterRecovered", warnedAlterRecovered);
				node.AddValue("warnedToolbar", warnedToolbar);

				try
				{
					ConfigNode paramNodes = new ConfigNode("Body_Science_Params");

					for (int i = 0; i < ScienceConfigValuesNode.ConfigCount; i++)
					{
						bodyParamsContainer b = ScienceConfigValuesNode.getBodyConfig(i);

						if (b == null)
							continue;

						ConfigNode paramNode = new ConfigNode("Body_Param");

						paramNode.AddValue("BodyName", b.Body.bodyName);
						paramNode.AddValue("ParamValues", stringConcat(b));

						paramNodes.AddNode(paramNode);
					}

					node.AddNode(paramNodes);
				}
				catch (Exception e)
				{
					SM_MBE.LogFormatted("Science Params Cannot Be Saved: {0}", e);
				}
			}
		}

		#endregion

		#region Save/Load Utilities

		private string stringConcat(bodyParamsContainer b)
		{
			string[] s = new string[9];
			s[0] = b.AdjustedParams.LandedData.ToString("F3");
			s[1] = b.AdjustedParams.SplashedData.ToString("F3");
			s[2] = b.AdjustedParams.FlyingLowData.ToString("F3");
			s[3] = b.AdjustedParams.FlyingHighData.ToString("F3");
			s[4] = b.AdjustedParams.SpaceLowData.ToString("F3");
			s[5] = b.AdjustedParams.SpaceHighData.ToString("F3");
			s[6] = b.AdjustedParams.RecoveredData.ToString("F3");
			s[7] = b.AdjustedParams.FlyingThreshold.ToString("F1");
			s[8] = b.AdjustedParams.SpaceThreshold.ToString("F1");
			return string.Join(",", s);
		}

		private void stringParse(string s, string bodyName)
		{
			bodyParamsContainer b = ScienceConfigValuesNode.getBodyConfig(bodyName);

			if (b == null)
			{
				SM_MBE.LogFormatted("Celestial Body Science Param Container Not Found; Removing Planet From List");
				return;
			}

			string[] a = s.Split(',');

			b.setNewParamValue(stringFloatParse(a[0]), scienceParamType.landed);
			b.setNewParamValue(stringFloatParse(a[1]), scienceParamType.splashed);
			b.setNewParamValue(stringFloatParse(a[2]), scienceParamType.flyingLow);
			b.setNewParamValue(stringFloatParse(a[3]), scienceParamType.flyingHigh);
			b.setNewParamValue(stringFloatParse(a[4]), scienceParamType.spaceLow);
			b.setNewParamValue(stringFloatParse(a[5]), scienceParamType.spaceHigh);
			b.setNewParamValue(stringFloatParse(a[6]), scienceParamType.recovered);
			b.setNewParamValue(stringFloatParse(a[7]), scienceParamType.flyingAltitude);
			b.setNewParamValue(stringFloatParse(a[8]), scienceParamType.spaceAltitude);
		}

		private float stringFloatParse(string s)
		{
			float f;

			if (float.TryParse(s, out f))
				return f;

			return 1;
		}

		#endregion
	}
}
