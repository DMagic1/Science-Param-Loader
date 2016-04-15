#region license
/*The MIT License (MIT)
Science Values Settings - A window with some configuration options

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
using System.Reflection;
using UnityEngine;
using ScienceParamModifier.Framework;
using ScienceParamModifier.Toolbar;

namespace ScienceParamModifier
{
	public class smSettingsWindow : SM_MBW
	{
		private const string lockID = "ScienceModifierSettingsLockID";
		private bool dropDown, recoveredPopup, toolbarPopup, savePopup;
		private bool controlLock;
		private bool stockToolbar = true;
		private bool alterRecovered, disableToolbar;
		private string version;
		private Rect ddRect;

		protected override void Awake()
		{
			Assembly assembly = AssemblyLoader.loadedAssemblies.GetByAssembly(Assembly.GetExecutingAssembly()).assembly;
			var ainfoV = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
			switch (ainfoV == null)
			{
				case true: version = ""; break;
				default: version = ainfoV.InformationalVersion; break;
			}

			WindowCaption = "SciParam Settings";
			WindowRect = new Rect(40, 80, 240, 160);
			WindowStyle = smSkins.newWindowStyle;
			Visible = false;
			DragEnabled = true;

			ClampToScreenOffset = new RectOffset(-125, -125, -70, -70);

			//Make sure our click-through control locks are disabled
			InputLockManager.RemoveControlLock(lockID);

			SM_SkinsLibrary.SetCurrent("SMUnitySkin");
		}

		protected override void Start()
		{
			base.Start();

			stockToolbar = scienceModifierScenario.Instance.stockToolbar;
			alterRecovered = scienceModifierScenario.Instance.alterRecoveredData;
			disableToolbar = scienceModifierScenario.Instance.disableToolbar;
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
				recoveredPopup = false;
				toolbarPopup = false;
				savePopup = false;
			}
		}

		protected override void DrawWindow(int id)
		{
			closeButton(id);
			versionLabel(id);

			windowConfig(id);

			drawPopup(id);
		}

		protected override void DrawWindowPost(int id)
		{
			if (stockToolbar != scienceModifierScenario.Instance.stockToolbar)
			{
				stockToolbar = scienceModifierScenario.Instance.stockToolbar;
				if (stockToolbar)
				{
					scienceModifierScenario.Instance.appLauncherButton = gameObject.AddComponent<smStockToolbar>();
					if (scienceModifierScenario.Instance.blizzyToolbarButton != null)
						Destroy(scienceModifierScenario.Instance.blizzyToolbarButton);
				}
				else
				{
					scienceModifierScenario.Instance.blizzyToolbarButton = gameObject.AddComponent<smToolbar>();
					if (scienceModifierScenario.Instance.appLauncherButton != null)
						Destroy(scienceModifierScenario.Instance.appLauncherButton);
				}
			}

			if (alterRecovered != scienceModifierScenario.Instance.alterRecoveredData)
			{
				alterRecovered = scienceModifierScenario.Instance.alterRecoveredData;
				if (alterRecovered)
				{
					if (!scienceModifierScenario.Instance.warnedAlterRecovered)
					{
						scienceModifierScenario.Instance.alterRecoveredData = false;
						alterRecovered = false;
						dropDown = true;
						recoveredPopup = true;
					}
				}
			}

			if (disableToolbar != scienceModifierScenario.Instance.disableToolbar)
			{
				disableToolbar = scienceModifierScenario.Instance.disableToolbar;
				if (disableToolbar)
				{
					if (!scienceModifierScenario.Instance.warnedToolbar)
					{
						if (disableToolbar)
						{
							scienceModifierScenario.Instance.disableToolbar = false;
							disableToolbar = false;
							dropDown = true;
							toolbarPopup = true;
						}
					}
					else
					{
						if (scienceModifierScenario.Instance.blizzyToolbarButton != null)
							Destroy(scienceModifierScenario.Instance.blizzyToolbarButton);
						if (scienceModifierScenario.Instance.appLauncherButton != null)
							Destroy(scienceModifierScenario.Instance.appLauncherButton);

					}
				}
				else
				{
					if (stockToolbar)
					{
						scienceModifierScenario.Instance.appLauncherButton = gameObject.AddComponent<smStockToolbar>();
						if (scienceModifierScenario.Instance.blizzyToolbarButton != null)
							Destroy(scienceModifierScenario.Instance.blizzyToolbarButton);
					}
					else
					{
						scienceModifierScenario.Instance.blizzyToolbarButton = gameObject.AddComponent<smToolbar>();
						if (scienceModifierScenario.Instance.appLauncherButton != null)
							Destroy(scienceModifierScenario.Instance.appLauncherButton);
					}
				}
			}

			if (dropDown && Event.current.type == EventType.mouseDown && !ddRect.Contains(Event.current.mousePosition))
				dropDown = false;
		}

		//Draw the close button in the upper right corner
		private void closeButton(int id)
		{
			Rect r = new Rect(WindowRect.width - 20, 0, 18, 18);
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

		//Draw all of the config option toggles and buttons
		private void windowConfig(int id)
		{
			if (!dropDown)
			{
				scienceModifierScenario.Instance.alterRecoveredData = GUILayout.Toggle(scienceModifierScenario.Instance.alterRecoveredData, " Edit Recovered Data Value");

				scienceModifierScenario.Instance.disableToolbar = GUILayout.Toggle(scienceModifierScenario.Instance.disableToolbar, " Disable All Toolbars");

				if (ToolbarManager.ToolbarAvailable && !scienceModifierScenario.Instance.disableToolbar)
					scienceModifierScenario.Instance.stockToolbar = GUILayout.Toggle(scienceModifierScenario.Instance.stockToolbar, " Use Stock Toolbar");

				if (GUILayout.Button("Save To Config"))
				{
					dropDown = !dropDown;
					savePopup = !savePopup;
				}
			}
			else
			{
				GUILayout.Toggle(scienceModifierScenario.Instance.alterRecoveredData, " Edit Recovered Data Value");

				GUILayout.Toggle(scienceModifierScenario.Instance.disableToolbar, " Disable All Toolbars");

				if (ToolbarManager.ToolbarAvailable && !scienceModifierScenario.Instance.disableToolbar)
					GUILayout.Toggle(scienceModifierScenario.Instance.stockToolbar, " Use Stock Toolbar");

				GUILayout.Label("Save To Config", smSkins.configButton);
			}
		}

		private void drawPopup(int id)
		{
			if (dropDown)
			{
				if (recoveredPopup)
				{
					ddRect = new Rect(5, 30, 230, 120);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 220, 80);
					GUI.Label(r, "Recovered Data value is used extensively by contract rewards", smSkins.resetBox);

					r.x += 10;
					r.y += 55;
					r.height = 30;
					scienceModifierScenario.Instance.warnedAlterRecovered = GUI.Toggle(r, scienceModifierScenario.Instance.warnedAlterRecovered, " Do not show this warning");

					r.x += 60;
					r.y += 25;
					r.width = 80;
					if (GUI.Button(r, "Confirm", smSkins.resetButton))
					{
						dropDown = false;
						recoveredPopup = false;
						scienceModifierScenario.Instance.alterRecoveredData = true;
						alterRecovered = true;
					}
				}

				else if (toolbarPopup)
				{
					ddRect = new Rect(5, 30, 230, 120);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 220, 80);
					GUI.Label(r, "Warning:\nToolbar icon can only be reactived from the config file", smSkins.resetBox);

					r.x += 10;
					r.y += 55;
					r.height = 30;
					scienceModifierScenario.Instance.warnedToolbar = GUI.Toggle(r, scienceModifierScenario.Instance.warnedToolbar, " Do not show this warning");

					r.x += 60;
					r.y += 25;
					r.width = 80;
					if (GUI.Button(r, "Confirm", smSkins.resetButton))
					{
						dropDown = false;
						toolbarPopup = false;
						scienceModifierScenario.Instance.disableToolbar = true;
						disableToolbar = true;
						if (scienceModifierScenario.Instance.blizzyToolbarButton != null)
							Destroy(scienceModifierScenario.Instance.blizzyToolbarButton);
						if (scienceModifierScenario.Instance.appLauncherButton != null)
							Destroy(scienceModifierScenario.Instance.appLauncherButton);
					}
				}

				else if (savePopup)
				{
					ddRect = new Rect(5, 70, 230, 80);
					GUI.Box(ddRect, "");
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 220, 45);
					GUI.Label(r, "Overwrite Default Config\nFile With Current Values?", smSkins.resetBox);

					r.x += 70;
					r.y += 40;
					r.width = 80;
					r.height = 30;
					if (GUI.Button(r, "Confirm", smSkins.resetButton))
					{
						dropDown = false;
						savePopup = false;
						scienceModifierScenario.Instance.SMNode.Save();
					}
				}

				else
					dropDown = false;
			}
		}
	}
}
