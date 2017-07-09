#region license
/*The MIT License (MIT)
ScienceParamSettings - A persistent storage module for saving settings to disk
Copyright (c) 2016 DMagic

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
using System.Reflection;
using System.IO;
using UnityEngine;
using ScienceParamModifier.Framework;

namespace ScienceParamModifier
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class ScienceParamSettings : SM_MBE
	{
		private static ScienceParamSettings instance;

		[Persistent]
		public bool disableToolbar = false;
		[Persistent]
		public bool alterRecoveredData = false;
		[Persistent]
		public bool stockToolbar = true;

		private smGameParameters settings;
		private const string fileName = "PluginData/ScienceParamSettings.cfg";
		private string fullPath;

		public static ScienceParamSettings Instance
		{
			get { return instance; }
		}

		protected override void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);

			instance = this;

			fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName).Replace("\\", "/");
			GameEvents.OnGameSettingsApplied.Add(SettingsApplied);

			if (Load())
				LogFormatted("Settings file loaded");
			else
			{
				if (Save())
					LogFormatted("New Settings file generated at:\n{0}", fullPath);
			}
		}
		
		protected override void OnDestroy()
		{
			GameEvents.OnGameSettingsApplied.Remove(SettingsApplied);
		}

		public void SettingsApplied()
		{
			if (HighLogic.CurrentGame != null)
				settings = HighLogic.CurrentGame.Parameters.CustomParams<smGameParameters>();

			if (settings == null)
				return;

			if (settings.useAsDefault)
			{
				disableToolbar = settings.disableToolbars;
				alterRecoveredData = settings.editRecovered;
				stockToolbar = settings.useStock;

				if (Save())
					LogFormatted("Settings file saved");
			}
		}

		public bool Load()
		{
			bool b = false;

			try
			{
				if (File.Exists(fullPath))
				{
					ConfigNode node = ConfigNode.Load(fullPath);
					ConfigNode unwrapped = node.GetNode(GetType().Name);
					ConfigNode.LoadObjectFromConfig(this, unwrapped);
					b = true;
				}
				else
				{
					LogFormatted("Settings file could not be found [{0}]", fullPath);
					b = false;
				}
			}
			catch (Exception e)
			{
				LogFormatted("Error while loading settings file from [{0}]\n{1}", fullPath, e);
				b = false;
			}

			return b;
		}

		public bool Save()
		{
			bool b = false;

			try
			{
				ConfigNode node = AsConfigNode();
				ConfigNode wrapper = new ConfigNode(GetType().Name);
				wrapper.AddNode(node);
				wrapper.Save(fullPath);
				b = true;
			}
			catch (Exception e)
			{
				LogFormatted("Error while saving settings file at [{0}]\n{1}", fullPath, e);
				b = false;
			}

			return b;
		}

		private ConfigNode AsConfigNode()
		{
			try
			{
				ConfigNode node = new ConfigNode(GetType().Name);

				node = ConfigNode.CreateConfigFromObject(this, node);
				return node;
			}
			catch (Exception e)
			{
				LogFormatted("Failed to generate settings file node...\n{0}", e);
				return new ConfigNode(GetType().Name);
			}
		}
	}
}
