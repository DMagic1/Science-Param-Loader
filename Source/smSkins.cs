#region license
/*The MIT License (MIT)
Science Modifier Skins - A simple MonoBehaviour to initialize skins and textures

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
using UnityEngine;

namespace ScienceParamModifier
{

	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	class smSkins : SM_MBE
	{
		internal static GUISkin smUnitySkin;

		internal static GUIStyle newWindowStyle;
		internal static GUIStyle resetBox;
		internal static GUIStyle dropDown;
		internal static GUIStyle resetButton;
		internal static GUIStyle configDropDown;
		internal static GUIStyle configHeader;
		internal static GUIStyle configClose;
		internal static GUIStyle configDropMenu;
		internal static GUIStyle configLabel;
		internal static GUIStyle configButton;
		internal static GUIStyle configTextBox;
		internal static GUIStyle configTextBoxBad;

		internal static Texture2D toolbarIcon;
		internal static Texture2D dropDownTex;

		protected override void OnGUIOnceOnly()
		{
			dropDownTex = GameDatabase.Instance.GetTexture("ScienceParamModifier/Icons/DropDownTex", false);
			toolbarIcon = GameDatabase.Instance.GetTexture("ScienceParamModifier/Icons/ScienceModifierAppIcon", false);

			smUnitySkin = SM_SkinsLibrary.CopySkin(SM_SkinsLibrary.DefSkinType.Unity);
			SM_SkinsLibrary.AddSkin("SMUnitySkin", smUnitySkin);

			newWindowStyle = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.window);
			newWindowStyle.name = "WindowStyle";
			newWindowStyle.fontSize = 14;
			newWindowStyle.fontStyle = FontStyle.Bold;
			newWindowStyle.padding = new RectOffset(0, 1, 20, 12);

			dropDown = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.box);
			dropDown.name = "DropDown";
			dropDown.normal.background = dropDownTex;

			resetBox = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.label);
			resetBox.name = "ResetBox";
			resetBox.fontSize = 16;
			resetBox.fontStyle = FontStyle.Bold;
			resetBox.normal.textColor = XKCDColors.VomitYellow;
			resetBox.wordWrap = true;
			resetBox.alignment = TextAnchor.UpperCenter;

			resetButton = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.button);
			resetButton.name = "ResetButton";
			resetButton.fontSize = 15;
			resetButton.fontStyle = FontStyle.Bold;
			resetButton.alignment = TextAnchor.MiddleCenter;

			configDropMenu = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.label);
			configDropMenu.name = "ConfigDropMenu";
			configDropMenu.fontSize = 13;
			configDropMenu.fontStyle = FontStyle.Bold;
			configDropMenu.padding = new RectOffset(2, 2, 2, 2);
			configDropMenu.normal.textColor = XKCDColors.White;
			configDropMenu.hover.textColor = XKCDColors.AlmostBlack;
			Texture2D menuBackground = new Texture2D(1, 1);
			menuBackground.SetPixel(1, 1, XKCDColors.OffWhite);
			menuBackground.Apply();
			configDropMenu.hover.background = menuBackground;
			configDropMenu.alignment = TextAnchor.MiddleLeft;
			configDropMenu.wordWrap = false;

			configDropDown = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.button);
			configDropDown.name = "ConfigDropDown";
			configDropDown.fontSize = 16;
			configDropDown.normal.textColor = XKCDColors.DustyOrange;
			configDropDown.alignment = TextAnchor.MiddleCenter;
			configDropDown.fontStyle = FontStyle.Bold;

			configHeader = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.label);
			configHeader.name = "ConfigHeader";
			configHeader.fontSize = 16;
			configHeader.normal.textColor = XKCDColors.DustyOrange;
			configHeader.alignment = TextAnchor.MiddleLeft;
			configHeader.fontStyle = FontStyle.Bold;

			configClose = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.button);
			configClose.name = "ConfigClose";
			configClose.normal.background = SM_SkinsLibrary.DefUnitySkin.label.normal.background;
			configClose.padding = new RectOffset(1, 1, 2, 2);
			configClose.normal.textColor = XKCDColors.DustyOrange;

			configLabel = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.label);
			configLabel.name = "ConfigLabel";
			configLabel.alignment = TextAnchor.MiddleRight;
			configLabel.fontStyle = FontStyle.Bold;
			configLabel.fontSize = 13;

			configButton = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.button);
			configButton.name = "ConfigButton";
			configButton.fontSize = 13;
			configButton.fontStyle = FontStyle.Bold;

			configTextBox = new GUIStyle(SM_SkinsLibrary.DefUnitySkin.textField);

			configTextBoxBad = new GUIStyle(configTextBox);
			configTextBoxBad.normal.textColor = XKCDColors.Red;
			configTextBoxBad.focused.textColor = XKCDColors.Red;

			SM_SkinsLibrary.List["SMUnitySkin"].window = new GUIStyle(newWindowStyle);
			SM_SkinsLibrary.List["SMUnitySkin"].button = new GUIStyle(configButton);
			SM_SkinsLibrary.List["SMUnitySkin"].label = new GUIStyle(configLabel);
			SM_SkinsLibrary.List["SMUnitySkin"].box = new GUIStyle(dropDown);
			SM_SkinsLibrary.List["SMUnitySkin"].textField = new GUIStyle(configTextBox);

			SM_SkinsLibrary.AddStyle("SMUnitySkin", newWindowStyle);
			SM_SkinsLibrary.AddStyle("SMUnitySkin", configButton);
			SM_SkinsLibrary.AddStyle("SMUnitySkin", configLabel);
			SM_SkinsLibrary.AddStyle("SMUnitySkin", dropDown);
			SM_SkinsLibrary.AddStyle("SMUnitySkin", configTextBox);
		}

	}
}
