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
using UnityEngine;
using ScienceParamModifier.Framework;

namespace ScienceParamModifier
{
	public class ScienceParamSettings : SM_ConfigNodeStorage
	{
		[Persistent]
		public bool disableToolbar = false;
		[Persistent]
		public bool alterRecoveredData = false;
		[Persistent]
		public bool stockToolbar = true;

		private smGameParameters settings;

		private static ScienceParamSettings instance;

		public static ScienceParamSettings Instance
		{
			get { return instance; }
		}

		public ScienceParamSettings(string file)
		{
			if (instance != null)
				return;

			instance = this;

			FilePath = file;

			GameEvents.OnGameSettingsApplied.Add(SettingsApplied);

			if (Load())
				LogFormatted("Settings file loaded");
		}

		public void SettingsApplied()
		{
			if (HighLogic.CurrentGame != null)
				settings = HighLogic.CurrentGame.Parameters.CustomParams<smGameParameters>();

			if (settings == null)
				return;

			if (settings.useAsDefault)
			{
				if (Save())
					LogFormatted("Settings file saved");
			}
		}

		public override void OnEncodeToConfigNode()
		{
			if (settings == null)
				return;

			disableToolbar = settings.disableToolbars;
			alterRecoveredData = settings.editRecovered;
			stockToolbar = settings.useStock;
		}
	}
}
