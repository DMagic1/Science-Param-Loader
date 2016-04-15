#region license
/*The MIT License (MIT)
Science Modifier Stock Toolbar- Addon for stock app launcher interface

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
using System.Collections;

using ScienceParamModifier.Framework;
using UnityEngine;
using KSP.UI.Screens;

namespace ScienceParamModifier.Toolbar
{
	public class smStockToolbar : SM_MBE
	{
		private ApplicationLauncherButton stockToolbarButton = null;

		protected override void Start()
		{
			setupToolbar();
		}

		private void setupToolbar()
		{
			StartCoroutine(addButton());
		}

		protected override void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherUnreadifying.Remove(removeButton);

			removeButton(HighLogic.LoadedScene);
		}

		IEnumerator addButton()
		{
			while (!ApplicationLauncher.Ready)
				yield return null;

			stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(toggle, toggle, null, null, null, null, (ApplicationLauncher.AppScenes)63, smSkins.toolbarIcon);

			GameEvents.onGUIApplicationLauncherUnreadifying.Add(removeButton);
		}

		private void removeButton(GameScenes scene)
		{
			if (stockToolbarButton != null)
			{
				ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
				stockToolbarButton = null;
			}
		}

		private void toggle()
		{
			if (scienceModifierScenario.Instance == null)
				LogFormatted("Science Modifier Scenario Not Loaded...");
			else if (scienceModifierScenario.Instance.scienceParamModifier == null)
				LogFormatted("Science Modifier Window Not Loaded...");
			else
			{
				scienceModifierScenario.Instance.scienceParamModifier.Visible = !scienceModifierScenario.Instance.scienceParamModifier.Visible;
			}
		}

	}
}
