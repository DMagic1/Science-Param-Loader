#region license
/*The MIT License (MIT)
Science Modifier Toolbar- Addon for toolbar interface

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

using System.IO;
using System;

using ScienceParamModifier.Framework;
using UnityEngine;

namespace ScienceParamModifier.Toolbar
{

	class smToolbar : MonoBehaviour
	{
		private IButton scienceParamButton;

		private void Start()
		{
			setupToolbar();
		}

		private void setupToolbar()
		{
			if (!ToolbarManager.ToolbarAvailable) return;

			scienceParamButton = ToolbarManager.Instance.add("ScienceParamModifier", "SMToolbarID");

			if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/ScienceParamModifier/Icons/ScienceModifierToolbarIcon.png").Replace("\\", "/")))
				scienceParamButton.TexturePath = "ScienceParamModifier/Icons/ScienceModifierToolbarIcon";
			else
				scienceParamButton.TexturePath = "000_Toolbar/resize-cursor";

			scienceParamButton.ToolTip = "Science Value Modifier";
			scienceParamButton.OnClick += (e) =>
				{
					if (scienceModifierScenario.Instance == null)
						SM_MBE.LogFormatted("Science Value Modifier Scenario Not Loaded...");
					else if (scienceModifierScenario.Instance.scienceParamModifier == null)
						SM_MBE.LogFormatted("Science Value Modifier Window Not Loaded...");
					else
					{
						scienceModifierScenario.Instance.scienceParamModifier.Visible = !scienceModifierScenario.Instance.scienceParamModifier.Visible;
					}
				};
		}

		private void OnDestroy()
		{
			if (!ToolbarManager.ToolbarAvailable) return;
			if (scienceParamButton != null)
				scienceParamButton.Destroy();
		}
	}
}
