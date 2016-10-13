using System;
using System.Collections.Generic;
using UnityEngine;
using ScienceParamModifier.Toolbar;

namespace ScienceParamModifier
{
	public class smGameParameters : GameParameters.CustomParameterNode
	{
		private bool toolbar;

		[GameParameters.CustomStringParameterUI("", lines = 2, autoPersistance = false)]
		public string reload = "A re-load is required for some changes to take effect.";
		[GameParameters.CustomParameterUI("Edit Recovered Data Value", autoPersistance = true)]
		public bool editRecovered;
		[GameParameters.CustomStringParameterUI("Warning", lines = 3)]
		public string recoveryWarning = "The <b>Recovered Data Value</b> is used extensively by contracts and should not generally be altered.";
		[GameParameters.CustomParameterUI("Disable All Toolbars", autoPersistance = true)]
		public bool disableToolbars;
		[GameParameters.CustomStringParameterUI("Warning", lines = 2)]
		public string toolbarWarning = "This will only take effect if the <b>Use As Default</b> option is also selected";
		[GameParameters.CustomParameterUI("Use Stock Toolbar", toolTip = "Switch between stock and Blizzy's toolbar", autoPersistance = true)]
		public bool useStock = true;
		[GameParameters.CustomParameterUI("Use As Default Values", toolTip = "Use the current settings as the defaults for all new games", autoPersistance = false)]
		public bool useAsDefault;

		public smGameParameters()
		{
			if (HighLogic.LoadedScene != GameScenes.MAINMENU)
				return;

			if (smConfigLoad.TopNode == null)
				return;

			editRecovered = smConfigLoad.Settings.alterRecoveredData;
			disableToolbars = smConfigLoad.Settings.disableToolbar;
			useStock = smConfigLoad.Settings.stockToolbar;

			toolbar = ToolbarManager.ToolbarAvailable;
		}

		public override GameParameters.GameMode GameMode
		{
			get { return GameParameters.GameMode.CAREER | GameParameters.GameMode.SCIENCE; }
		}

		public override bool HasPresets
		{
			get { return false; }
		}

		public override string Section
		{
			get { return "DMagic Mods"; }
		}

		public override string Title
		{
			get { return "Science Param Modifier"; }
		}

		public override int SectionOrder
		{
			get { return 2; }
		}

		public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
		{
			if (member.Name == "useStock")
				return toolbar;
			else if (member.Name == "reload")
				return HighLogic.LoadedSceneIsGame;

			return true;
		}
	}
}
