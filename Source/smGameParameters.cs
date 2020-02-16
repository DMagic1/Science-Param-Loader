using System;
using System.Collections.Generic;
using UnityEngine;
using ScienceParamModifier.Toolbar;
using KSP.Localization;

namespace ScienceParamModifier
{
	public class smGameParameters : GameParameters.CustomParameterNode
	{
		private bool toolbar;

		[GameParameters.CustomStringParameterUI("", lines = 2, autoPersistance = false)]
		public string reload = Localizer.Format("#DMagic_ScienceParamModifier_ReloadRequired");//"A re-load is required for some changes to take effect."
		[GameParameters.CustomParameterUI("#DMagic_ScienceParamModifier_editRecovered", toolTip = "#DMagic_ScienceParamModifier_editRecovered_tip", autoPersistance = true)]//Edit Recovered Data Value--Edit the recovered data value; activating this is not recommended as the value is used by the contract system
		public bool editRecovered;
		[GameParameters.CustomStringParameterUI("#DMagic_ScienceParamModifier_Warning", lines = 3)]//Warning
		public string recoveryWarning = Localizer.Format("#DMagic_ScienceParamModifier_recoveryWarning");//"The <b>Recovered Data Value</b> is used extensively by contracts and should not generally be altered."
		[GameParameters.CustomParameterUI("#DMagic_ScienceParamModifier_disableToolbars", toolTip = "#DMagic_ScienceParamModifier_disableToolbars_tip", autoPersistance = true)]//Disable All Toolbars--Disable all in-game toolbars and windows
		public bool disableToolbars;
		[GameParameters.CustomStringParameterUI("#DMagic_ScienceParamModifier_Warning", lines = 2)]//Warning
		public string toolbarWarning = Localizer.Format("#DMagic_ScienceParamModifier_toolbarWarning");//"This will only take effect if the <b>Use As Default</b> option is also selected"
		[GameParameters.CustomParameterUI("#DMagic_ScienceParamModifier_useStock", toolTip = "#DMagic_ScienceParamModifier_useStock_tip", autoPersistance = true)]//Use Stock Toolbar--Switch between stock and Blizzy's toolbar
		public bool useStock = true;
		[GameParameters.CustomParameterUI("#DMagic_ScienceParamModifier_useAsDefault", toolTip = "#DMagic_ScienceParamModifier_useAsDefault_tip", autoPersistance = false)]//Use As Default Values--Use the current settings as the defaults for all new games
		public bool useAsDefault;

		public smGameParameters()
		{
			toolbar = ToolbarManager.ToolbarAvailable;

			if (HighLogic.LoadedScene != GameScenes.MAINMENU)
				return;

			if (ScienceParamSettings.Instance == null)
				return;

			editRecovered = ScienceParamSettings.Instance.alterRecoveredData;
			disableToolbars = ScienceParamSettings.Instance.disableToolbar;
			useStock = ScienceParamSettings.Instance.stockToolbar;
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
			get { return "Science Param Modifier"; }
		}

		public override string DisplaySection
		{
			get { return Localizer.Format("#DMagic_ScienceParamModifier_DisplaySection"); }//"Science Param Modifier"
		}

		public override string Title
		{
			get { return Localizer.Format("#DMagic_ScienceParamModifier_DisplaySection"); }//"Science Param Modifier"
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
