#region license
/*The MIT License (MIT)
Science Values Node - An object to store persistent data from the config file

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

using ScienceParamModifier.Framework;

namespace ScienceParamModifier
{
	/// <summary>
	/// A storage object for loading and saving data from a config file
	/// </summary>
	public class ScienceConfigValuesNode : SM_ConfigNodeStorage
	{
		[Persistent]
		private bool disableToolbar = false;
		[Persistent]
		private bool disableSaveSpecificValues = false;
		[Persistent]
		private bool alterRecoveredData = false;
		[Persistent]
		private bool stockToolbar = true;
		[Persistent]
		private bool warnedToolbar = false;
		[Persistent]
		private bool warnedAlterRecovered = false;
		[Persistent]
		private List<bodyParamsContainer> bodyDefaltConfigs = new List<bodyParamsContainer>();

		private static Dictionary<string, bodyParamsContainer> masterBodyConfigs = new Dictionary<string, bodyParamsContainer>();

		public override void OnDecodeFromConfigNode()
		{
			try
			{
				masterBodyConfigs = bodyDefaltConfigs.ToDictionary(a => a.Body.bodyName, a => a);
			}
			catch (Exception e)
			{
				LogFormatted("Error while loading celestial body default container list; possibly a duplicate entry: {0}", e);
			}
		}

		public override void OnEncodeToConfigNode()
		{
			if (scienceModifierScenario.Instance != null)
			{
				disableToolbar = scienceModifierScenario.Instance.disableToolbar;
				stockToolbar = scienceModifierScenario.Instance.stockToolbar;
				alterRecoveredData = scienceModifierScenario.Instance.alterRecoveredData;
				warnedAlterRecovered = scienceModifierScenario.Instance.warnedAlterRecovered;
				warnedToolbar = scienceModifierScenario.Instance.warnedToolbar;
			}

			try
			{
				bodyDefaltConfigs = masterBodyConfigs.Values.ToList();
			}
			catch (Exception e)
			{
				LogFormatted("Error while saving celestial body default container list: {0}", e);
			}
		}

		internal ScienceConfigValuesNode(string filePath)
		{
			FilePath = filePath;

			Load();

			checkAllBodies();

			Save();
		}

		public static int ConfigCount
		{
			get { return masterBodyConfigs.Count; }
		}

		public static bodyParamsContainer getBodyConfig(int i)
		{
			if (ConfigCount > i)
				return masterBodyConfigs.ElementAtOrDefault(i).Value;
			else
				LogFormatted("Body Science Param Index Out Of Range...");

				return null;
		}

		public static List<bodyParamsContainer> getBodyConfigList()
		{
			return masterBodyConfigs.Values.ToList();
		}

		public static bodyParamsContainer getBodyConfig(string s)
		{
			if (masterBodyConfigs.ContainsKey(s))
				return masterBodyConfigs[s];
			else
				LogFormatted("No Celestial Body Default List Of Name: [{0}] Found...", s);

			return null;
		}

		private static bool addToBodyConfigList(bodyParamsContainer p)
		{
			if (!masterBodyConfigs.ContainsKey(p.Body.bodyName))
			{
				masterBodyConfigs.Add(p.Body.bodyName, p);
				return true;
			}
			else
			{
				LogFormatted("Celestial Body Default Container Dictionary Already Has Body Of This Name; Skipping...");
				return false;
			}
		}

		private void checkAllBodies()
		{
			foreach (CelestialBody b in FlightGlobals.Bodies)
			{
				bodyParamsContainer p = getBodyConfig(b.bodyName);

				if (p != null)
					continue;

				addToBodyConfigList(new bodyParamsContainer(b));
			}
		}

		public bool DisableToolbar
		{
			get { return disableToolbar; }
		}

		public bool DisableSaveSpecificValues
		{
			get { return disableSaveSpecificValues; }
		}

		public bool AlterRecoveredData
		{
			get { return alterRecoveredData; }
		}

		public bool StockToolbar
		{
			get { return stockToolbar; }
		}

		public bool WarnedAlterRecovered
		{
			get { return warnedAlterRecovered; }
		}

		public bool WarnedToolbar
		{
			get { return warnedToolbar; }
		}
	}
}
