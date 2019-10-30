#region license
/*The MIT License (MIT)
Science Param Modifier - Addon to edit celestial body science values

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
using System.Reflection;

using ScienceParamModifier.Framework;
using ScienceParamModifier.Toolbar;
using UnityEngine;

namespace ScienceParamModifier
{
	class scienceParamModifier : SM_MBW
	{
		private const string lockID = "ScienceModifierLockID";
		private bool dropDown, bSelection, defaultPopup, stockPopup, savePopup;
		private bool controlLock;
		private string version;
		private Rect ddRect;
		private Vector2 bScroll;
		private float landedVal, splashVal, flyingLowVal, flyingHighVal, spaceLowVal, spaceHighVal, recoveredVal, flyingAltVal, spaceAltVal;
		private string landed, splash, flyingLow, flyingHigh, spaceLow, spaceHigh, recovered, flyingAlt, spaceAlt;

		private List<bodyParamsContainer> paramsList = new List<bodyParamsContainer>();

		private bodyParamsContainer currentBody;
		private paramSet currentSet;

		protected override void Awake()
		{
			Assembly assembly = AssemblyLoader.loadedAssemblies.GetByAssembly(Assembly.GetExecutingAssembly()).assembly;
			var ainfoV = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
			switch (ainfoV == null)
			{
				case true: version = ""; break;
				default: version = ainfoV.InformationalVersion; break;
			}

			WindowCaption = "Science Params Modifier";
			WindowRect = new Rect(40, 80, 220, 260);
			WindowOptions = new GUILayoutOption[1] { GUILayout.Height(260) };
			WindowStyle = smSkins.newWindowStyle;
			Visible = false;
			DragEnabled = true;

			ClampToScreenOffset = new RectOffset(-250, -250, -200, -200);

			//Make sure our click-through control locks are disabled
			InputLockManager.RemoveControlLock(lockID);

			SM_SkinsLibrary.SetCurrent("SMUnitySkin");
		}

		protected override void Start()
		{
			base.Start();

			paramsList = ScienceConfigValuesNode.getBodyConfigList();

			setCurrentPlanet(paramsList[0]);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			unlockControls();
		}

		private void unlockControls()
		{
			InputLockManager.RemoveControlLock(lockID);
			controlLock = false;
		}

		protected override void DrawWindowPre(int id)
		{
			//Prevent click through from activating part options
			if (HighLogic.LoadedSceneIsFlight)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && GUIUtility.hotControl == 0 && Input.GetMouseButton(0))
				{
					foreach (var window in GameObject.FindObjectsOfType(typeof(UIPartActionWindow)).OfType<UIPartActionWindow>().Where(p => p.Display == UIPartActionWindow.DisplayType.Selected))
					{
						window.enabled = false;
						window.displayDirty = true;
					}
				}
			}

			//Lock space center click through
			if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.KSC_FACILITIES | ControlTypes.KSC_UI, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
				{
					unlockControls();
				}
			}

			//Lock tracking scene click through
			if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.TRACKINGSTATION_ALL, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
				{
					unlockControls();
				}
			}

			if (!dropDown)
			{
				dropDown = false;
				bSelection = false;
				defaultPopup = false;
				stockPopup = false;
				savePopup = false;
			}
		}

		protected override void DrawWindow(int id)
		{
			closeButton(id);						/* Draw the close button */
			versionLabel(id);						/* Draw the version label */

			bodySelecetionMenu(id);					/* Drop down menu and label for the current body */
			editScienceValues(id);					/* Science multiplier sliders */
			drawButtons(id);						/* Draw buttons for saving and resetting values*/

			dropDownMenu(id);						/* Draw the drop down menus when open */
		}

		protected override void DrawWindowPost(int id)
		{
			if (dropDown && Event.current.type == EventType.MouseDown && !ddRect.Contains(Event.current.mousePosition))
				dropDown = false;
		}

		//Draw the close button in the upper right corner
		private void closeButton(int id)
		{
			Rect r = new Rect(WindowRect.width - 26, 0, 22, 22);

			if (GUI.Button(r, "✖", smSkins.configClose))
			{
				unlockControls();
				Visible = false;
			}
		}

		//Draw the version number label
		private void versionLabel(int id)
		{
			Rect r = new Rect(2, 2, 30, 20);
			GUI.Label(r, version);
		}

		//Select Celestial Body to edit
		private void bodySelecetionMenu(int id)
		{
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				if(GUILayout.Button("Celestial Body: ", smSkins.configDropDown, GUILayout.MaxWidth(140)))
				{
					dropDown = !dropDown;
					bSelection = !bSelection;
				}

				GUILayout.Space(10);

				if (currentBody != null)
					GUILayout.Label(currentBody.Body.bodyName, smSkins.configHeader, GUILayout.Width(120));
				else
					GUILayout.Label("Unknown", smSkins.configHeader, GUILayout.Width(120));
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		//Sliders to control science values
		private void editScienceValues(int id)
		{
			GUILayout.Space(10);
			drawValueGroup("Landed Data: ", currentSet.LandedData, ref landedVal, 0.1f, 50, ref landed, 1);
			if (currentBody.Body.ocean)
				drawValueGroup("Splashed Data: ", currentSet.SplashedData, ref splashVal, 0.1f, 50, ref splash, 1);
			if (currentBody.Body.atmosphere)
			{
				drawValueGroup("Flying Low Data: ", currentSet.FlyingLowData, ref flyingLowVal, 0.1f, 50, ref flyingLow, 1);
				drawValueGroup("Flying High Data: ", currentSet.FlyingHighData, ref flyingHighVal, 0.1f, 50, ref flyingHigh, 1);
			}
			drawValueGroup("Low Orbit Data: ", currentSet.SpaceLowData, ref spaceLowVal, 0.1f, 50, ref spaceLow, 1);
			drawValueGroup("High Orbit Data: ", currentSet.SpaceHighData, ref spaceHighVal, 0.1f, 50, ref spaceHigh, 1);
			if (scienceModifierScenario.Instance.Settings.editRecovered)
				drawValueGroup("Recovery Data: ", currentSet.RecoveredData, ref recoveredVal, 0.1f, 50, ref recovered, 1);
			if (currentBody.Body.atmosphere)
				drawValueGroup("Flying Threshold: ", currentSet.FlyingThreshold, ref flyingAltVal, 100, currentBody.MaxFlying, ref flyingAlt, 0, "m", 10);
			drawValueGroup("Space Threshold: ", currentSet.SpaceThreshold, ref spaceAltVal, currentBody.MinSpace, currentBody.MaxSpace, ref spaceAlt, 0, "m", 10);
		}

		//Reset and save buttons
		private void drawButtons(int id)
		{
			GUILayout.FlexibleSpace();
			if (!dropDown)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Space(20);
					if (GUILayout.Button("Apply Values"))
					{
						applyValues();
						setCurrentPlanet(currentBody);
					}
					GUILayout.Space(20);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Default Values", GUILayout.Width(100)))
					{
						dropDown = !dropDown;
						defaultPopup = !defaultPopup;
					}
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Stock Values", GUILayout.Width(100)))
					{
						dropDown = !dropDown;
						stockPopup = !stockPopup;
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Space(20);
					if (GUILayout.Button("Save To Config"))
					{
						dropDown = !dropDown;
						savePopup = !savePopup;
					}
					GUILayout.Space(20);
				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUILayout.Label("Apply Values", smSkins.configButton);
					GUILayout.Space(20);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("Default Values", smSkins.configButton, GUILayout.Width(100));
					GUILayout.FlexibleSpace();
					GUILayout.Label("Stock Values", smSkins.configButton, GUILayout.Width(100));
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUILayout.Label("Save To Config", smSkins.configButton);
					GUILayout.Space(20);
				GUILayout.EndHorizontal();
			}
		}

		//Handle all of the drop down menus and pop up windows here
		//Only 1 can be active at a time
		private void dropDownMenu(int id)
		{
			if (dropDown)
			{
				if (bSelection)
				{
					ddRect = new Rect(10, 55, 140, 180);
					GUI.Box(ddRect, "");

					for (int i = 0; i < paramsList.Count; i++)
					{
						bodyParamsContainer b = paramsList[i];

						if (b == null)
							continue;

						bScroll = GUI.BeginScrollView(ddRect, bScroll, new Rect(0, 0, 120, 25 * FlightGlobals.Bodies.Count));

						Rect r = new Rect(2, (25 * i) + 2, 120, 25);

						if (GUI.Button(r, b.Body.bodyName, smSkins.configDropMenu))
						{
							setCurrentPlanet(b);
							bSelection = false;
							dropDown = false;
						}
						GUI.EndScrollView();
					}
				}

				else if (defaultPopup)
				{
					ddRect = new Rect(20, WindowRect.height - 206, 150, 120);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 140, 90);
					GUI.Label(r, "Science Values\nFor:<b>" + currentBody.Body.bodyName + "</b>\nWill Be Reset\nTo Default", smSkins.resetBox);

					r.x += 30;
					r.y += 75;
					r.width = 80;
					r.height = 30;
					if (GUI.Button(r, "Confirm", smSkins.resetButton))
					{
						dropDown = false;
						defaultPopup = false;
						defaultValues();
					}
				}

				else if (stockPopup)
				{
					ddRect = new Rect(20, WindowRect.height - 206, 150, 120);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 140, 90);
					GUI.Label(r, "Science Values\nFor:<b>" + currentBody.Body.bodyName + "</b>\nWill Be Reset\nTo Stock", smSkins.resetBox);

					r.x += 30;
					r.y += 75;
					r.width = 80;
					r.height = 30;
					if (GUI.Button(r, "Confirm", smSkins.resetButton))
					{
						dropDown = false;
						stockPopup = false;
						stockValues();
					}
				}

				else if (savePopup)
				{
					ddRect = new Rect(20, WindowRect.height - 206, 150, 120);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 140, 90);
					GUI.Label(r, "Overwrite Default\nConfig File With\nCurrent Values?", smSkins.resetBox);

					r.x += 30;
					r.y += 75;
					r.width = 80;
					r.height = 30;
					if (GUI.Button(r, "Confirm", smSkins.resetButton))
					{
						dropDown = false;
						savePopup = false;
						if (smConfigLoad.TopNode != null)
							smConfigLoad.TopNode.Save();
					}
				}

				else
					dropDown = false;
			}
		}

		private void drawValueGroup(string title, float currentValue, ref float newValue, float min, float max, ref string textField, int precision, string units = "", int charLimit = 4)
		{
			GUILayout.BeginHorizontal();
				GUILayout.Label(title + currentValue.ToString("N" + precision) + units, GUILayout.Width(140));

				GUILayout.FlexibleSpace();

				newValue = drawInputField(newValue, min, max, ref textField, charLimit);
				GUILayout.Space(10);
			GUILayout.EndHorizontal();
		}

		private float drawInputField(float f, float min, float max, ref string s, int charLimit)
		{
			float newValue = f;

			GUIStyle style = smSkins.configTextBox;

			if (!float.TryParse(s, out newValue))
			{
				style = smSkins.configTextBoxBad;
				newValue = f;
			}

			if (newValue < min)
			{
				newValue = min;
				style = smSkins.configTextBoxBad;
			}
			else if (newValue > max)
			{
				newValue = max;
				style = smSkins.configTextBoxBad;
			}

			s = GUILayout.TextField(s, charLimit, style, GUILayout.Width(80));

			return newValue;
		}

		private void setCurrentPlanet(bodyParamsContainer B)
		{
			currentBody = B;
			currentSet = currentBody.AdjustedParams;

			landedVal = currentSet.LandedData;
			splashVal = currentSet.SplashedData;
			flyingLowVal = currentSet.FlyingLowData;
			flyingHighVal = currentSet.FlyingHighData;
			spaceLowVal = currentSet.SpaceLowData;
			spaceHighVal = currentSet.SpaceHighData;
			recoveredVal = currentSet.RecoveredData;
			flyingAltVal = currentSet.FlyingThreshold;
			spaceAltVal = currentSet.SpaceThreshold;

			landed = landedVal.ToString("F1");
			splash = splashVal.ToString("F1");
			flyingLow = flyingLowVal.ToString("F1");
			flyingHigh = flyingHighVal.ToString("F1");
			spaceLow = spaceLowVal.ToString("F1");
			spaceHigh = spaceHighVal.ToString("F1");
			recovered = recoveredVal.ToString("F1");
			flyingAlt = flyingAltVal.ToString("F0");
			spaceAlt = spaceAltVal.ToString("F0");
		}

		private void defaultValues()
		{
			currentBody.resetToDefault();

			setCurrentPlanet(currentBody);

			applyValues();
		}

		private void stockValues()
		{
			currentBody.resetToStock();

			setCurrentPlanet(currentBody);

			applyValues();
		}

		private void applyValues()
		{
			currentBody.setNewParamValue(landedVal, scienceParamType.landed);
			currentBody.setNewParamValue(splashVal, scienceParamType.splashed);
			currentBody.setNewParamValue(flyingLowVal, scienceParamType.flyingLow);
			currentBody.setNewParamValue(flyingHighVal, scienceParamType.flyingHigh);
			currentBody.setNewParamValue(spaceLowVal, scienceParamType.spaceLow);
			currentBody.setNewParamValue(spaceHighVal, scienceParamType.spaceHigh);
			currentBody.setNewParamValue(flyingAltVal, scienceParamType.flyingAltitude);
			currentBody.setNewParamValue(spaceAltVal, scienceParamType.spaceAltitude);

			if (scienceModifierScenario.Instance.Settings.editRecovered)
				currentBody.setNewParamValue(recoveredVal, scienceParamType.recovered);
		}

	}
}
