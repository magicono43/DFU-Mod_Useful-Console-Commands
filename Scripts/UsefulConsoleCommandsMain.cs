// Project:         UsefulConsoleCommands mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    4/14/2022, 9:00 AM
// Last Edit:		5/11/2022, 11:30 PM
// Version:			1.00
// Special Thanks:  Interkarma, Jefetienne, Hazelnut, Kab the Bird Ranger, Macadaynu, Ralzar, Billyloist, Extract
// Modifier:

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using UnityEngine;
using System;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop;
using Wenzil.Console;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Questing;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;

namespace UsefulConsoleCommands
{
    public class UsefulConsoleCommandsMain : MonoBehaviour
	{
        static UsefulConsoleCommandsMain instance;

        public static UsefulConsoleCommandsMain Instance
        {
            get { return instance ?? (instance = FindObjectOfType<UsefulConsoleCommandsMain>()); }
        }

        static Mod mod;

        public static bool RolePlayRealismBandagingModule { get; set; }
        public static bool RepairToolsCheck { get; set; }
        public static bool RealisticWagonCheck { get; set; }
        public static bool ClimatesAndCaloriesCheck { get; set; }

        static PlayerEntity player = GameManager.Instance.PlayerEntity;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            instance = new GameObject("UsefulConsoleCommands").AddComponent<UsefulConsoleCommandsMain>(); // Add script to the scene.

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Useful Console Commands");

            mod.LoadSettings();

            RegisterModConsoleCommands(); // Main bulk of mod features are in these added console commands.

            Debug.Log("Finished mod init: Useful Console Commands");
        }

        #region Settings

        static void LoadSettings(ModSettings modSettings, ModSettingsChange change)
        {
            Mod roleplayRealism = ModManager.Instance.GetMod("roleplayrealism");
            RolePlayRealismBandagingModule = false;
            if (roleplayRealism != null)
            {
                ModSettings rolePlayRealismSettings = roleplayRealism.GetSettings();
                RolePlayRealismBandagingModule = rolePlayRealismSettings.GetBool("Modules", "bandaging");
            }

            Mod repairTools = ModManager.Instance.GetMod("repairtools");
            RepairToolsCheck = false;
            if (repairTools != null)
            {
                RepairToolsCheck = true;
            }

            Mod realisticWagon = ModManager.Instance.GetMod("Realistic Wagon");
            RealisticWagonCheck = false;
            if (realisticWagon != null)
            {
                RealisticWagonCheck = true;
            }

            Mod climatesAndCalories = ModManager.Instance.GetMod("Climates & Calories"); // Test to make sure this works tomorrow and the continue on, hopefully feeling better tomorrow as well.
            ClimatesAndCaloriesCheck = false;
            if (climatesAndCalories != null)
            {
                ClimatesAndCaloriesCheck = true;
            }
        }

        #endregion

        public static bool GetRolePlayRealismBandagingCheck()
        {
            return RolePlayRealismBandagingModule;
        }

        public static bool GetRepairToolsCheck()
        {
            return RepairToolsCheck;
        }

        public static bool GetRealisticWagonCheck()
        {
            return RealisticWagonCheck;
        }

        public static bool GetClimatesAndCaloriesCheckCheck()
        {
            return ClimatesAndCaloriesCheck;
        }

        public static void RegisterModConsoleCommands()
        {
            Debug.Log("[UsefulConsoleCommands] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ChangePlayerAttribute.command, ChangePlayerAttribute.description, ChangePlayerAttribute.usage, ChangePlayerAttribute.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ChangePlayerSkill.command, ChangePlayerSkill.description, ChangePlayerSkill.usage, ChangePlayerSkill.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ChangeGender.command, ChangeGender.description, ChangeGender.usage, ChangeGender.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ChangeRace.command, ChangeRace.description, ChangeRace.usage, ChangeRace.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ChangeFace.command, ChangeFace.description, ChangeFace.usage, ChangeFace.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(OpenShop.command, OpenShop.description, OpenShop.usage, OpenShop.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(CleanupCorpses.command, CleanupCorpses.description, CleanupCorpses.usage, CleanupCorpses.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(EmptyInventory.command, EmptyInventory.description, EmptyInventory.usage, EmptyInventory.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(LetMeSleep.command, LetMeSleep.description, LetMeSleep.usage, LetMeSleep.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(FastFlying.command, FastFlying.description, FastFlying.usage, FastFlying.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ClearMagic.command, ClearMagic.description, ClearMagic.usage, ClearMagic.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ViewNPCReputation.command, ViewNPCReputation.description, ViewNPCReputation.usage, ViewNPCReputation.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(CreateInfiniteTorch.command, CreateInfiniteTorch.description, CreateInfiniteTorch.usage, CreateInfiniteTorch.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(QuestTaskToggle.command, QuestTaskToggle.description, QuestTaskToggle.usage, QuestTaskToggle.Execute); // tested
                //ConsoleCommandsDatabase.RegisterCommand(QuestTesting1.command, QuestTesting1.description, QuestTesting1.usage, QuestTesting1.Execute);
                //ConsoleCommandsDatabase.RegisterCommand(TeleportTo.command, TeleportTo.description, TeleportTo.usage, TeleportTo.Execute);
                //ConsoleCommandsDatabase.RegisterCommand(ListRegions.command, ListRegions.description, ListRegions.usage, ListRegions.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering UsefulConsoleCommands Console commands: {0}", e.Message));
            }
        }

        private static class ChangePlayerAttribute
        {
            public static readonly string command = "setattrib";
            public static readonly string description = "Changes the specified attribute's value, between 1 and 1000.";
            public static readonly string usage = "setattrib [attribute] [n]; try something like: 'setattrib strength 75' or 'setattrib per 30' or 'setattrib 4 95' or even 'setattrib all 60'";

            public static string Execute(params string[] args)
            {
                if (args.Length < 2) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (!int.TryParse(args[1], out int n))
                    return string.Format("`{0}` is not a number, please use a number for [n].", args[1]);
                if (n < 1 || n > 1000)
                    return "Invalid amount, [n] must be a value between 1 and 1000."; // May change this if attribute values above 1000 are allowed and don't cause major issues.

                if (player != null)
                {
                    switch (args[0])
                    {
                        case "strength":
                        case "str":
                        case "0":
                            playerEntity.Stats.SetPermanentStatValue(0, n); // Possibly try and make this switch statement more compact vertically later, if it still allows it to be easily human readable.
                            return string.Format("Strength was set to {0}.", n);
                        case "intelligence":
                        case "int":
                        case "1":
                            playerEntity.Stats.SetPermanentStatValue(1, n);
                            return string.Format("Intelligence was set to {0}.", n);
                        case "willpower":
                        case "wil":
                        case "2":
                        case "will":
                            playerEntity.Stats.SetPermanentStatValue(2, n);
                            return string.Format("Willpower was set to {0}.", n);
                        case "agility":
                        case "agi":
                        case "3":
                            playerEntity.Stats.SetPermanentStatValue(3, n);
                            return string.Format("Agility was set to {0}.", n);
                        case "endurance":
                        case "end":
                        case "4":
                            playerEntity.Stats.SetPermanentStatValue(4, n);
                            return string.Format("Endurance was set to {0}.", n);
                        case "personality":
                        case "per":
                        case "5":
                            playerEntity.Stats.SetPermanentStatValue(5, n);
                            return string.Format("Personality was set to {0}.", n);
                        case "speed":
                        case "spe":
                        case "6":
                            playerEntity.Stats.SetPermanentStatValue(6, n);
                            return string.Format("Speed was set to {0}.", n);
                        case "luck":
                        case "luc":
                        case "7":
                            playerEntity.Stats.SetPermanentStatValue(7, n);
                            return string.Format("Luck was set to {0}.", n);
                        case "all":
                            for (int i = 0; i < 8; i++)
                                playerEntity.Stats.SetPermanentStatValue(i, n);
                            return string.Format("All attributes were set to {0}.", n);
                        default:
                            return "Invalid attribute, try something like: 'setattrib strength 75' or 'setattrib per 30' or 'setattrib 4 95' or even 'setattrib all 60'";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ChangePlayerSkill
        {
            public static readonly string command = "setskill";
            public static readonly string description = "Changes the specified skill's value, between 1 and 1000.";
            public static readonly string usage = "setskill [skill] [n] ['xp' if you want tallies and not levels]; try something like: 'setskill bluntweapon 75' or 'setskill jump 30' or 'setskill 8 95' or even 'setskill all 60'. Also 'setskill swimming 200 xp'";

            public static string Execute(params string[] args)
            {
                if (args.Length < 2) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (!int.TryParse(args[1], out int n))
                    return string.Format("`{0}` is not a number, please use a number for [n].", args[1]);
                if (args.Length == 2 && (n < 1 || n > 1000))
                    return "Invalid amount, [n] must be a value between 1 and 1000."; // May change this if skill values above 1000 are allowed and don't cause major issues.

                if (args.Length > 2 && args[2] != "xp")
                    return "Invalid entry, see usage notes.";

                if (player != null)
                {
                    switch (args[0])
                    {
                        case "medical":
                        case "med":
                        case "0":
                        case "medic":
                        case "doctor":
                        case "first-aid":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Medical, (short)n);
                                return string.Format("Medical skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(0, (short)n); // Possibly try and make this switch statement more compact vertically later, if it still allows it to be easily human readable.
                            return string.Format("Medical skill was set to {0}.", n);
                        case "etiquette":
                        case "eti":
                        case "1":
                        case "posh":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Etiquette, (short)n);
                                return string.Format("Etiquette skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(1, (short)n);
                            return string.Format("Etiquette skill was set to {0}.", n);
                        case "streetwise":
                        case "str":
                        case "2":
                        case "sw":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Streetwise, (short)n);
                                return string.Format("Streetwise skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(2, (short)n);
                            return string.Format("Streetwise skill was set to {0}.", n);
                        case "jumping":
                        case "jum":
                        case "3":
                        case "jump":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Jumping, (short)n);
                                return string.Format("Jumping skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(3, (short)n);
                            return string.Format("Jumping skill was set to {0}.", n);
                        case "orcish":
                        case "orc":
                        case "4":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Orcish, (short)n);
                                return string.Format("Orcish skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(4, (short)n);
                            return string.Format("Orcish skill was set to {0}.", n);
                        case "harpy":
                        case "har":
                        case "5":
                        case "harp":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Harpy, (short)n);
                                return string.Format("Harpy skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(5, (short)n);
                            return string.Format("Harpy skill was set to {0}.", n);
                        case "giantish":
                        case "gia":
                        case "6":
                        case "giant":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Giantish, (short)n);
                                return string.Format("Giantish skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(6, (short)n);
                            return string.Format("Giantish skill was set to {0}.", n);
                        case "dragonish":
                        case "dra":
                        case "7":
                        case "dragon":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Dragonish, (short)n);
                                return string.Format("Dragonish skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(7, (short)n);
                            return string.Format("Dragonish skill was set to {0}.", n);
                        case "nymph":
                        case "nym":
                        case "8":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Nymph, (short)n);
                                return string.Format("Nymph skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(8, (short)n);
                            return string.Format("Nymph skill was set to {0}.", n);
                        case "daedric":
                        case "dae":
                        case "9":
                        case "demon":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Daedric, (short)n);
                                return string.Format("Daedric skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(9, (short)n);
                            return string.Format("Daedric skill was set to {0}.", n);
                        case "spriggan":
                        case "spr":
                        case "10":
                        case "tree":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Spriggan, (short)n);
                                return string.Format("Spriggan skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(10, (short)n);
                            return string.Format("Spriggan skill was set to {0}.", n);
                        case "centaurian":
                        case "cen":
                        case "11":
                        case "cent":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Centaurian, (short)n);
                                return string.Format("Centaurian skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(11, (short)n);
                            return string.Format("Centaurian skill was set to {0}.", n);
                        case "impish":
                        case "imp":
                        case "12":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Impish, (short)n);
                                return string.Format("Impish skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(12, (short)n);
                            return string.Format("Impish skill was set to {0}.", n);
                        case "lockpicking":
                        case "loc":
                        case "13":
                        case "lockpick":
                        case "lp":
                        case "lock-picking":
                        case "lock-pick":
                        case "pick-lock":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Lockpicking, (short)n);
                                return string.Format("Lockpicking skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(13, (short)n);
                            return string.Format("Lockpicking skill was set to {0}.", n);
                        case "mercantile":
                        case "mer":
                        case "14":
                        case "merchant":
                        case "haggle":
                        case "barter":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Mercantile, (short)n);
                                return string.Format("Mercantile skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(14, (short)n);
                            return string.Format("Mercantile skill was set to {0}.", n);
                        case "pickpocket":
                        case "pic":
                        case "15":
                        case "pick-pocket":
                        case "pp":
                        case "pocket":
                        case "pocket-pick":
                        case "pickpocketing":
                        case "pick-pocketing":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Pickpocket, (short)n);
                                return string.Format("Pickpocket skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(15, (short)n);
                            return string.Format("Pickpocket skill was set to {0}.", n);
                        case "stealth":
                        case "ste":
                        case "16":
                        case "sneak":
                        case "sneaking":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Stealth, (short)n);
                                return string.Format("Stealth skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(16, (short)n);
                            return string.Format("Stealth skill was set to {0}.", n);
                        case "swimming":
                        case "swi":
                        case "17":
                        case "swim":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Swimming, (short)n);
                                return string.Format("Swimming skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(17, (short)n);
                            return string.Format("Swimming skill was set to {0}.", n);
                        case "climbing":
                        case "cli":
                        case "18":
                        case "climb":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Climbing, (short)n);
                                return string.Format("Climbing skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(18, (short)n);
                            return string.Format("Climbing skill was set to {0}.", n);
                        case "backstabbing":
                        case "bac":
                        case "19":
                        case "bs":
                        case "back":
                        case "backstab":
                        case "back-stab":
                        case "back-stabbing":
                        case "ambush":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Backstabbing, (short)n);
                                return string.Format("Backstabbing skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(19, (short)n);
                            return string.Format("Backstabbing skill was set to {0}.", n);
                        case "dodging":
                        case "dod":
                        case "20":
                        case "dodge":
                        case "avoid":
                        case "avoidance":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Dodging, (short)n);
                                return string.Format("Dodging skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(20, (short)n);
                            return string.Format("Dodging skill was set to {0}.", n);
                        case "running":
                        case "run":
                        case "21":
                        case "sprint":
                        case "sprinting":
                        case "runner":
                        case "sonic":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Running, (short)n);
                                return string.Format("Running skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(21, (short)n);
                            return string.Format("Running skill was set to {0}.", n);
                        case "destruction":
                        case "des":
                        case "22":
                        case "destro":
                        case "destruct":
                        case "black":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Destruction, (short)n);
                                return string.Format("Destruction skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(22, (short)n);
                            return string.Format("Destruction skill was set to {0}.", n);
                        case "restoration":
                        case "res":
                        case "23":
                        case "resto":
                        case "restro":
                        case "restore":
                        case "healing":
                        case "white":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Restoration, (short)n);
                                return string.Format("Restoration skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(23, (short)n);
                            return string.Format("Restoration skill was set to {0}.", n);
                        case "illusion":
                        case "ill":
                        case "24":
                        case "trickery":
                        case "mesmer":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Illusion, (short)n);
                                return string.Format("Illusion skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(24, (short)n);
                            return string.Format("Illusion skill was set to {0}.", n);
                        case "alteration":
                        case "alt":
                        case "25":
                        case "alter":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Alteration, (short)n);
                                return string.Format("Alteration skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(25, (short)n);
                            return string.Format("Alteration skill was set to {0}.", n);
                        case "thaumaturgy":
                        case "tha":
                        case "26":
                        case "thaum":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Thaumaturgy, (short)n);
                                return string.Format("Thaumaturgy skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(26, (short)n);
                            return string.Format("Thaumaturgy skill was set to {0}.", n);
                        case "mysticism":
                        case "mys":
                        case "27":
                        case "myst":
                        case "mystic":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Mysticism, (short)n);
                                return string.Format("Mysticism skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(27, (short)n);
                            return string.Format("Mysticism skill was set to {0}.", n);
                        case "shortblade":
                        case "sho":
                        case "28":
                        case "short":
                        case "sb":
                        case "short-blade":
                        case "daggers":
                        case "knives":
                        case "knifefighting":
                        case "knife-fighting":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.ShortBlade, (short)n);
                                return string.Format("ShortBlade skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(28, (short)n);
                            return string.Format("ShortBlade skill was set to {0}.", n);
                        case "longblade":
                        case "lon":
                        case "29":
                        case "long":
                        case "lb":
                        case "long-blade":
                        case "longsword":
                        case "long-sword":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.LongBlade, (short)n);
                                return string.Format("LongBlade skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(29, (short)n);
                            return string.Format("LongBlade skill was set to {0}.", n);
                        case "handtohand":
                        case "han":
                        case "30":
                        case "h2h":
                        case "hand-to-hand":
                        case "htoh":
                        case "fist":
                        case "fistfighting":
                        case "fist-fighting":
                        case "martialarts":
                        case "martial-arts":
                        case "pugilism":
                        case "brawl":
                        case "boxer":
                        case "boxing":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.HandToHand, (short)n);
                                return string.Format("HandToHand skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(30, (short)n);
                            return string.Format("HandToHand skill was set to {0}.", n);
                        case "axe":
                        case "axes":
                        case "31":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Axe, (short)n);
                                return string.Format("Axe skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(31, (short)n);
                            return string.Format("Axe skill was set to {0}.", n);
                        case "bluntweapon":
                        case "blu":
                        case "32":
                        case "blunt":
                        case "mace":
                        case "maces":
                        case "staves":
                        case "maul":
                        case "flail":
                        case "bluntweapons":
                        case "blunt-weapon":
                        case "blunt-weapons":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.BluntWeapon, (short)n);
                                return string.Format("BluntWeapon skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(32, (short)n);
                            return string.Format("BluntWeapon skill was set to {0}.", n);
                        case "archery":
                        case "arc":
                        case "33":
                        case "archer":
                        case "arrow":
                        case "arrows":
                        case "marks":
                        case "marksman":
                        case "marksmanship":
                        case "bow":
                        case "bows":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.Archery, (short)n);
                                return string.Format("Archery skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(33, (short)n);
                            return string.Format("Archery skill was set to {0}.", n);
                        case "criticalstrike":
                        case "cri":
                        case "34":
                        case "crit":
                        case "crits":
                        case "cs":
                        case "critical-strike":
                        case "criticalstrikes":
                        case "critical-strikes":
                        case "critical":
                        case "criticalhit":
                        case "critical-hit":
                        case "criticalhits":
                        case "critical-hits":
                        case "critstrike":
                        case "crithit":
                            if (args.Length > 2)
                            {
                                playerEntity.TallySkill(DFCareer.Skills.CriticalStrike, (short)n);
                                return string.Format("CriticalStrike skill was given {0} 'xp/tallies'.", n);
                            }
                            playerEntity.Skills.SetPermanentSkillValue(34, (short)n);
                            return string.Format("CriticalStrike skill was set to {0}.", n);
                        case "all":
                            if (args.Length > 2)
                            {
                                for (int i = 0; i < 35; i++)
                                    playerEntity.TallySkill(DFCareer.Skills.CriticalStrike, (short)n);
                                return string.Format("All skills were given {0} 'xp/tallies'.", n);
                            }
                            for (int i = 0; i < 35; i++)
                                playerEntity.Skills.SetPermanentSkillValue(i, (short)n);
                            return string.Format("All skills were set to {0}.", n);
                        default:
                            return "Invalid skill, try something like: 'setskill bluntweapon 75' or 'setskill jump 30' or 'setskill 8 95' or even 'setskill all 60'. Also 'setskill swimming 200 xp'";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ChangeGender
        {
            public static readonly string command = "changegender";
            public static readonly string description = "Changes your character's current gender to either male or female.";
            public static readonly string usage = "changegender [male/female]; try something like: 'changegender man' or 'changegender women' even 'changegender m' are valid.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (args.Length <= 0)
                    return "Error - An argument is required, check the usage notes.";
                if (args.Length > 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    switch (args[0])
                    {
                        case "male":
                        case "man":
                        case "m":
                        case "0":
                            if (playerEntity.Gender == Genders.Male)
                                return "You are already a male.";
                            playerEntity.Gender = Genders.Male;
                            return "You are now a male.";
                        case "female":
                        case "woman":
                        case "f":
                        case "w":
                        case "1":
                            if (playerEntity.Gender == Genders.Female)
                                return "You are already a female.";
                            playerEntity.Gender = Genders.Female;
                            return "You are now a female.";
                        default:
                            return "Error - You need to enter a gender, check usage notes.";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ChangeRace
        {
            public static readonly string command = "changerace";
            public static readonly string description = "Changes your character's current race. It highly advised to not use this as a permanent change, and is more intended for testing, some traits might get messed up potentially when changing between races, so just be advised.";
            public static readonly string usage = "changerace [race]; try something like: 'changerace redguard' or 'changerace woodelf' or 'changerace dunmer' or even somewhat related descriptors like 'changerace cat' or 'changerace lizard' many are valid entries.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (args.Length <= 0)
                    return "Error - An argument is required, check the usage notes.";
                if (args.Length > 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    switch (args[0])
                    {
                        case "breton":
                        case "bret":
                        case "manmeri":
                            if (playerEntity.Race == Races.Breton)
                                return "You are already a Breton.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.Breton;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("breton");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2003;
                            playerEntity.BirthRaceTemplate.ClipID = 209;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG00I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY00I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY00I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY10I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY10I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE00I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE10I0.CIF";
                            return "You are now a Breton.";
                        case "redguard":
                        case "red-guard":
                        case "rg":
                        case "yokudans":
                            if (playerEntity.Race == Races.Redguard)
                                return "You are already a Redguard.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.Redguard;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("redguard");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2002;
                            playerEntity.BirthRaceTemplate.ClipID = 210;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG01I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY01I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY01I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY11I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY11I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE01I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE11I0.CIF";
                            return "You are now a Redguard.";
                        case "nord":
                            if (playerEntity.Race == Races.Nord)
                                return "You are already a Nord.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.Nord;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("nord");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2000;
                            playerEntity.BirthRaceTemplate.ClipID = 211;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG02I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY02I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY02I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY12I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY12I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE02I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE12I0.CIF";
                            return "You are now a Nord.";
                        case "darkelf":
                        case "dark-elf":
                        case "de":
                        case "dunmer":
                            if (playerEntity.Race == Races.DarkElf)
                                return "You are already a Dark Elf.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.DarkElf;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("darkElf");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2007;
                            playerEntity.BirthRaceTemplate.ClipID = 212;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG03I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY03I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY03I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY13I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY13I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE03I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE13I0.CIF";
                            return "You are now a Dark Elf.";
                        case "highelf":
                        case "high-elf":
                        case "he":
                        case "altmer":
                            if (playerEntity.Race == Races.HighElf)
                                return "You are already a High Elf.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.HighElf;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("highElf");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2006;
                            playerEntity.BirthRaceTemplate.ClipID = 213;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG04I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY04I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY04I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY14I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY14I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE04I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE14I0.CIF";
                            return "You are now a High Elf.";
                        case "woodelf":
                        case "wood-elf":
                        case "we":
                        case "bosmer":
                            if (playerEntity.Race == Races.WoodElf)
                                return "You are already a Wood Elf.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.WoodElf;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("woodElf");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2005;
                            playerEntity.BirthRaceTemplate.ClipID = 214;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG05I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY05I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY05I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY15I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY15I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE05I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE15I0.CIF";
                            return "You are now a Wood Elf.";
                        case "khajiit":
                        case "kha":
                        case "cat":
                        case "cats":
                            if (playerEntity.Race == Races.Khajiit)
                                return "You are already a Khajiit.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.Khajiit;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("khajiit");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2001;
                            playerEntity.BirthRaceTemplate.ClipID = 215;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG06I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY06I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY06I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY16I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY16I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE06I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE16I0.CIF";
                            return "You are now a Khajiit.";
                        case "argonian":
                        case "saxhleel":
                        case "arg":
                        case "lizard":
                        case "lizards":
                        case "reptilian":
                        case "reptile":
                            if (playerEntity.Race == Races.Argonian)
                                return "You are already a Argonian.";
                            playerEntity.BirthRaceTemplate.ID = (int)Races.Argonian;
                            playerEntity.BirthRaceTemplate.Name = TextManager.Instance.GetLocalizedText("argonian");
                            playerEntity.BirthRaceTemplate.DescriptionID = 2004;
                            playerEntity.BirthRaceTemplate.ClipID = 216;
                            playerEntity.BirthRaceTemplate.PaperDollBackground = "SCBG07I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleUnclothed = "BODY07I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyMaleClothed = "BODY07I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleUnclothed = "BODY17I0.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollBodyFemaleClothed = "BODY17I1.IMG";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsMale = "FACE07I0.CIF";
                            playerEntity.BirthRaceTemplate.PaperDollHeadsFemale = "FACE17I0.CIF";
                            return "You are now a Argonian.";
                        default:
                            return "Error - You need to enter a valid race, check usage notes.";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ChangeFace
        {
            public static readonly string command = "changeface";
            public static readonly string description = "Changes your character's current face index, for whatever race and gender you currently are.";
            public static readonly string usage = "changeface [n]; try something like: 'changeface 5' or 'changeface 9' between 0 and 9 are all valid face index values.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (args.Length <= 0)
                    return "Error - An argument is required, check the usage notes.";
                if (args.Length > 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (!int.TryParse(args[0], out int n))
                    return string.Format("`{0}` is not a number, please use a number for [n].", args[0]);
                if (n < 0 || n > 9)
                    return "Invalid amount, [n] must be a value between 0 and 9.";

                if (player != null)
                {
                    if (playerEntity.FaceIndex == n)
                        return "You are already using that face index.";
                    playerEntity.FaceIndex = n;
                    return "You now have a new face.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class OpenShop
        {
            public static readonly string command = "openshop";
            public static readonly string description = "Opens a shop interface with items you can freely take or try on, items populated depend on the given modifier words.";
            public static readonly string usage = "openshop [modifier] [modifier2] [gender] [race]; try something like: 'openshop' will open a random shop shelf from a random shop type with a random quality. 'openshop alchemist' will open an alchemist shop shelf with a random quality. 'openshop pawnshop 15' will open a pawnshop shelf with a quality of 15. 'openshop artifacts' will open a shelf with all the artifacts in the game. 'openshop ingredients 8' will open a shelf with all ingredients 8 of each. 'openshop armor mithril female highelf' will open a shelf with all armor made of mithril with the body morphology for female elves. 'openshop clothing purple' will open a shelf with all clothing for your character's gender and race in the color purple. Try many different combinations, there are many specifications you can give to filter down your options in different ways.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection playerItems = playerEntity.Items;
                UCCShopWindow tradeWindow = new UCCShopWindow(DaggerfallUI.UIManager, null, UCCShopWindow.WindowModes.Buy, null);

                if (args.Length >= 5)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    tradeWindow.MerchantItems = UCCShopWindow.StockMagicShopShelf(args);

                    if (tradeWindow.MerchantItems == null || tradeWindow.MerchantItems.Count < 1)
                    {
                        return "Error - No items were found, check the usage notes.";
                    }

                    DaggerfallUI.UIManager.PushWindow(tradeWindow);

                    return "Opening Magic Shop Shelf.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class CleanupCorpses
        {
            public static readonly string command = "clearcorpses";
            public static readonly string description = "Destroys all corpse objects from the current scene, add modifier words to specify different types to remove.";
            public static readonly string usage = "clearcorpses [modifier]; try something like: 'clearcorpses' or 'clearcorpses all'. Without any modifier only corpses are removed, if 'all' is used, all loot-piles are removed from the current scene.";

            public static string Execute(params string[] args)
            {
                if (args.Length > 1) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                DaggerfallLoot[] lootContainers = FindObjectsOfType<DaggerfallLoot>();
                int count = 0;

                if (player != null && args.Length == 0)
                {
                    if (lootContainers != null)
                    {
                        for (int i = 0; i < lootContainers.Length; i++)
                        {
                            GameObject gameObject = lootContainers[i].gameObject;

                            if (lootContainers[i].ContainerType == LootContainerTypes.CorpseMarker)
                            {
                                Destroy(gameObject);
                                count++;
                            }
                        }
                    }
                    return string.Format("Removed {0} corpses.", count);
                }
                else if (player != null && args.Length != 0)
                {
                    switch (args[0])
                    {
                        case "all":
                        case "everything":
                            if (lootContainers != null)
                            {
                                for (int i = 0; i < lootContainers.Length; i++)
                                {
                                    GameObject gameObject = lootContainers[i].gameObject;

                                    if (lootContainers[i].ContainerType == LootContainerTypes.CorpseMarker || lootContainers[i].ContainerType == LootContainerTypes.RandomTreasure || lootContainers[i].ContainerType == LootContainerTypes.DroppedLoot)
                                    {
                                        Destroy(gameObject);
                                        count++;
                                    }
                                }
                            }
                            return string.Format("Removed {0} corpses and loot-piles.", count);
                        default:
                            return "Invalid attribute, try something like: 'clearcorpses' or 'clearcorpses all'. Without any modifier only corpses are removed, if 'all' is used, all loot-piles are removed from the current scene.";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class EmptyInventory
        {
            public static readonly string command = "emptyinventory";
            public static readonly string description = "Removes everything from your inventory, add additional modifier for more control of what is removed.";
            public static readonly string usage = "emptyinventory [modifier]; try something like: try something like: 'emptyinventory' or 'emptyinventory all' or 'emptyinventory wagon' or 'emptyinventory gear' to keep and equipped items. Without any modifier word, quest items, light sources, horse, wagon, letters of credit, and the spellbook will be preserved.";

            public static string Execute(params string[] args)
            {
                if (args.Length > 1) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection playerItems = playerEntity.Items;
                ItemCollection wagonItems = playerEntity.WagonItems;
                int invSize = playerItems.Count;
                int h = 0;

                if (player != null && args.Length == 0)
                {
                    for (int i = 0; i < invSize; i++)
                    {
                        DaggerfallUnityItem item = playerEntity.Items.GetItem(h);
                        h++;

                        if (item.IsQuestItem || item.IsLightSource || item.ItemGroup == ItemGroups.Transportation || (item.ItemGroup == ItemGroups.MiscItems && item.TemplateIndex == (int)MiscItems.Spellbook) || (item.ItemGroup == ItemGroups.MiscItems && item.TemplateIndex == (int)MiscItems.Letter_of_credit) || (item.ItemGroup == ItemGroups.UselessItems2 && item.TemplateIndex == (int)UselessItems2.Oil))
                            continue; // By default, ignore quest items, light sources, horse, wagon, letters of credit, and the spellbook item.
                        else
                        {
                            playerItems.RemoveItem(item);
                            h--;
                        }
                    }
                    return "Removed all items from your inventory excluding quest-items, light sources, horse, wagon, letters of credit, and spellbook.";
                }
                else if (player != null && args.Length != 0)
                {
                    switch (args[0])
                    {
                        case "all":
                        case "clear":
                        case "everything":
                        case "completely":
                            playerEntity.GoldPieces = 0;
                            playerEntity.LightSource = null;
                            playerItems.Clear(); // This command clears literally everything from your inventory.
                            return "Removed ALL items from your inventory, including gold.";
                        case "wagon":
                        case "cart":
                            wagonItems.Clear(); // This command clears everything from your wagon inventory.
                            return "Removed all items from your wagon inventory.";
                        case "equip":
                        case "gear":
                        case "keepgear":
                        case "keepequip":
                        case "saveequip":
                        case "savegear":
                        case "keep_equip":
                        case "save_equip":
                        case "keep_gear":
                        case "save_gear":
                            for (int i = 0; i < invSize; i++)
                            {
                                DaggerfallUnityItem item = playerEntity.Items.GetItem(h);
                                h++;

                                if (item.IsEquipped || (item.ItemGroup == ItemGroups.MiscItems && item.TemplateIndex == (int)MiscItems.Spellbook))
                                    continue; // Ignore all equipped items and the spellbook item.
                                else
                                {
                                    playerItems.RemoveItem(item);
                                    h--;
                                }
                            }
                            return "Removed all items from your inventory excluding any items you have equipped, such as armor, clothing, weapons, accessories, spellbook, etc.";
                        default:
                            return "Invalid argument, try something like: try something like: 'emptyinventory' or 'emptyinventory all' or 'emptyinventory wagon' or 'emptyinventory gear' to keep and equipped items. Without any modifier word, quest items, light sources, horse, wagon, letters of credit, and the spellbook will be preserved.";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class LetMeSleep
        {
            public static readonly string command = "letmesleep";
            public static readonly string description = "Kills all enemies within range that may be disallowing you to rest, but leaves others otherside this range alive.";
            public static readonly string usage = "letmesleep; try something like: 'letmesleep'.";

            public static string Execute(params string[] args)
            {
                if (args.Length > 0) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
                int count = 0;

                if (player != null)
                {
                    for (int i = 0; i < entityBehaviours.Length; i++)
                    {
                        DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
                        if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                        {
                            EnemySenses enemySenses = entityBehaviour.GetComponent<EnemySenses>();
                            if (enemySenses)
                            {
                                // Check if enemy can actively target player
                                bool enemyCanSeePlayer = enemySenses.Target == GameManager.Instance.PlayerEntityBehaviour && enemySenses.TargetInSight;

                                // Allow for a shorter test distance if enemy is unaware of player while resting
                                if (!enemyCanSeePlayer && Vector3.Distance(entityBehaviour.transform.position, GameManager.Instance.PlayerController.transform.position) > 12f) // 12f is "restingDistance"
                                    continue;

                                // Can enemy see player or is close enough they would be spawned in classic?
                                if (enemyCanSeePlayer || enemySenses.WouldBeSpawnedInClassic)
                                {
                                    // Is it hostile or pacified?
                                    EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                                    EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
                                    if (enemyMotor.IsHostile && enemyEntity.MobileEnemy.Team != MobileTeams.PlayerAlly)
                                    {
                                        entityBehaviour.Entity.SetHealth(0);
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                    return string.Format("Killed: {0} enemies, that would be disallowing you to rest.", count);
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class FastFlying
        {
            public static readonly string command = "fastflying";
            public static readonly string description = "Toggle noclip by turning off all collisions and activates levitate, but with super sonic speed as well.";
            public static readonly string usage = "fastflying [n]; try something like: 'fastflying' or for a custom speed setting, 'fastflying 100' or even 'fastflying 52.8'";

            public static string Execute(params string[] args)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                LevitateMotor levitateMotor = GameManager.Instance.PlayerMotor.GetComponent<LevitateMotor>();
                float n = 4.0f;

                if (args.Length > 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (args.Length > 0)
                {
                    if (!float.TryParse(args[0], out n))
                        return string.Format("`{0}` is not a number, please use a number for [n].", args[0]);
                    if (n < 4.0f)
                        return "Invalid amount, [n] must be a value greater than or equal to 4.0, since this is the default levitation speed.";
                }

                if (playerEntity != null && levitateMotor != null)
                {
                    playerEntity.NoClipMode = !playerEntity.NoClipMode;
                    levitateMotor.IsLevitating = playerEntity.NoClipMode;
                    GameManager.Instance.PlayerController.gameObject.layer = playerEntity.NoClipMode ? LayerMask.NameToLayer("NoclipLayer") : LayerMask.NameToLayer("Player");

                    if (playerEntity.NoClipMode)
                    {
                        if (args.Length == 1)
                            levitateMotor.LevitateMoveSpeed = n; // Custom Speed Value
                        else
                            levitateMotor.LevitateMoveSpeed = 100.0f; // Default None Custom Speed Value
                    }
                    else
                    {
                        levitateMotor.LevitateMoveSpeed = 4.0f; // Default Levitation Speed, I.E. Very Slow
                    }

                    return string.Format("Fast Flying enabled: {0}", playerEntity.NoClipMode);
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ClearMagic
        {
            public static readonly string command = "clearmagic";
            public static readonly string description = "Removes all magic effect bundles from your character.";
            public static readonly string usage = "clearmagic; try something like: clearmagic";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (args.Length > 0)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    EntityEffectManager manager = GameManager.Instance.PlayerEffectManager;
                    manager.ClearSpellBundles();
                    return "All magic spell bundles removed from you.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ViewNPCReputation
        {
            public static readonly string command = "viewnpcrep";
            public static readonly string description = "Shows the associated faction reptuation you currently have with the last clicked NPC.";
            public static readonly string usage = "viewnpcrep; try something like: viewnpcrep";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                StaticNPC lastClicked = QuestMachine.Instance.LastNPCClicked;

                if (args.Length > 0)
                    return "Error - Too many arguments, check the usage notes.";

                if (lastClicked != null)
                {
                    int reputation = GameManager.Instance.PlayerEntity.FactionData.GetReputation(lastClicked.Data.factionID);
                    return string.Format("Your reputation with {0} is currently '{1}'", lastClicked.DisplayName, reputation);
                }
                else if (lastClicked == null)
                    return "Error - Could not find an NPC last clicked.";
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class CreateInfiniteTorch
        {
            public static readonly string command = "infinitetorch";
            public static readonly string description = "Creates a torch item with practically infinite durability that won't burn out.";
            public static readonly string usage = "infinitetorch; try something like: 'infinitetorch'.";

            public static string Execute(params string[] args)
            {
                if (args.Length > 0) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection playerItems = playerEntity.Items;

                if (player != null)
                {
                    DaggerfallUnityItem item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Torch);
                    item.RenameItem("Infinite Torch");
                    item.maxCondition = 999999;
                    item.currentCondition = 999999;
                    playerItems.AddItem(item);
                    return "Created the 'Infinite Torch', with this you will never be left in the dark.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class QuestTaskToggle // This is still pretty WIP, but hopefully it can still be useful in it's current state to somebody.
        {
            public static readonly string command = "questtasks";
            public static readonly string description = "Allows you to toggle the current set state for various tasks within a quest.";
            public static readonly string usage = "questtasks; try something like: 'questtasks' then pick a quest you wish to alter the task states of, after selecting the quest then choose the task you want to try and toggle between set and unset, true or false, etc. Keep in mind, for some tasks other conditions must be met before they can be set or unset. This command is for those that know what they are doing, don't blame me if you break a quest with this.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ulong[] uids = null;

                if (args.Length >= 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    DaggerfallListPickerWindow validQuestPicker = new DaggerfallListPickerWindow(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    validQuestPicker.OnItemPicked += ShowQuestObjectivesPicker_OnItemPicked; // Tell it what to do next after picking a valid quest from the list.
                    validQuests.Clear(); // Clears the valid quests list before every command use.

                    uids = QuestMachine.Instance.GetAllActiveQuests();

                    for (int i = 0; i < uids.Length; i++)
                    {
                        Quest quest = QuestMachine.Instance.GetQuest(uids[i]);
                        if (quest.GetTaskStates().Length < 1)
                            continue;

                        string validQuestName = quest.UID + "   " + quest.DisplayName;

                        validQuests.Add(quest);
                        validQuestPicker.ListBox.AddItem(validQuestName);
                    }

                    if (validQuestPicker.ListBox.Count > 0)
                    {
                        DaggerfallUI.UIManager.PushWindow(validQuestPicker);
                        return "Here is the list of active and valid quests, pick something.";
                    }
                    else
                        return "You have no valid quests currently active.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        static List<Quest> validQuests = new List<Quest>();

        public static void ShowQuestObjectivesPicker_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();

            chosenQuestUID = 0;
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            Quest questToUse = validQuests[index]; // Gets the quest associated with what was selected in the list window.
            validQuestTasks.Clear();
            chosenQuestUID = questToUse.UID;

            DaggerfallListPickerWindow questTaskPicker = new DaggerfallListPickerWindow(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            questTaskPicker.OnItemPicked += PerformToggleTaskAction_OnItemPicked; // Have it do it's thing after picking a valid quest task from the list.

            Quest.TaskState[] taskStates = questToUse.GetTaskStates();
            if (taskStates == null || taskStates.Length == 0)
            {
                // Do Nothing
            }
            else
            {
                foreach (Quest.TaskState task in taskStates)
                {
                    Symbol symbol = task.symbol;
                    Task.TaskType type = task.type;
                    bool taskSet = task.set;

                    if (type == Task.TaskType.Headless)
                    {
                        validQuestTasks.Add(task);
                        questTaskPicker.ListBox.AddItem("(Headless)" + "   " + symbol.Name + "   " + taskSet.ToString());
                    }
                    else if (type == Task.TaskType.Standard)
                    {
                        validQuestTasks.Add(task);
                        questTaskPicker.ListBox.AddItem("(Standard)" + "   " + symbol.Name + "   " + taskSet.ToString());
                    }
                    else if (type == Task.TaskType.PersistUntil)
                    {
                        validQuestTasks.Add(task);
                        questTaskPicker.ListBox.AddItem("(PersistUntil)" + "   " + string.Format("until_{0}", symbol.Name) + "   " + taskSet.ToString());
                    }
                    else if (type == Task.TaskType.Variable)
                    {
                        validQuestTasks.Add(task);
                        questTaskPicker.ListBox.AddItem("(Variable)" + "   " + symbol.Name + "   " + taskSet.ToString());
                    }
                    else if (type == Task.TaskType.GlobalVarLink)
                    {
                        validQuestTasks.Add(task);
                        questTaskPicker.ListBox.AddItem("(GlobalVarLink)" + "   " + symbol.Name + "   " + taskSet.ToString());
                    }
                }
            }

            if (questTaskPicker.ListBox.Count > 0)
            {
                List<ListBox.ListItem> listItems = questTaskPicker.ListBox.ListItems;
                for (int i = 0; i < listItems.Count; i++)
                {
                    if (validQuestTasks[i].set == false)
                    {
                        listItems[i].textColor = new Color32(188, 60, 60, 255); // Red
                    }
                    else if (validQuestTasks[i].set == true)
                    {
                        listItems[i].textColor = new Color32(6, 108, 0, 255); // Green
                    }
                    else
                    {
                        listItems[i].textColor = DaggerfallUI.DaggerfallDefaultTextColor;
                    }
                    listItems[i].selectedTextColor = new Color32(95, 231, 229, 255); // Light Cyan
                    listItems[i].highlightedTextColor = new Color32(57, 221, 219, 255); // Slightly Darker Cyan
                    listItems[i].highlightedSelectedTextColor = new Color32(27, 197, 196, 255); // Dark Cyan
                    listItems[i].shadowColor = new Color32(0, 0, 0, 0); // Removed Shadow
                }

                DaggerfallUI.UIManager.PushWindow(questTaskPicker);
            }
            else
                DaggerfallUI.MessageBox("That quest has no valid tasks currently available.");
        }

        static List<Quest.TaskState> validQuestTasks = new List<Quest.TaskState>();
        public static ulong chosenQuestUID { get; set; }

        public static void PerformToggleTaskAction_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();

            Quest.TaskState taskPicked = validQuestTasks[index]; // Gets the quest task associated with what was selected in the list window.

            Quest usedQuest = QuestMachine.Instance.GetQuest(chosenQuestUID);
            Quest.TaskState[] tasks = usedQuest.GetTaskStates();
            Task task = null;

            foreach (Quest.TaskState taskState in tasks) // Tries to match the proper Instance of the quest.
            {
                if (taskState.type == taskPicked.type && taskState.symbol == taskPicked.symbol && taskState.set == taskPicked.set)
                {
                    task = usedQuest.GetTask(taskState.symbol);
                    break;
                }
            }

            if (task != null && task.IsTriggered == false)
                task.IsTriggered = true;
            else if (task != null && task.IsTriggered == true) // So the toggle seems to kind of work, but it's weird since it only allows changing if certain conditions are met sometimes.
                task.IsTriggered = false;
        }

        // 5/5/2022, 11:15 PM: Alright, gave up on trying to get this command to work for now, maybe I'll fix it at some point, but not today.
        /*private static class QuestTesting1
        {
            public static readonly string command = "questtesting1";
            public static readonly string description = "Does quest testing stuff.";
            public static readonly string usage = "questtesting1 [n]; try something like: 'changeface 5' or 'changeface 9' between 0 and 9 are all valid face index values.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ulong[] uids = null;

                if (args.Length >= 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    DaggerfallListPickerWindow validQuestPicker = new DaggerfallListPickerWindow(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    validQuestPicker.OnItemPicked += ShowQuestObjectivesPicker_OnItemPicked; // Tell it what to do next after picking a valid quest from the list.
                    validQuests.Clear(); // Clears the valid quests list before every command use.

                    uids = QuestMachine.Instance.GetAllActiveQuests();

                    for (int i = 0; i < uids.Length; i++) // Tomorrow do rest of work for listing active valid quests basically and such.
                    {
                        Quest quest = QuestMachine.Instance.GetQuest(uids[i]);
                        if (quest.GetQuestors().Length < 1)
                            continue;

                        string validQuestName = quest.DisplayName; // Just do this simple name for now, nothing else to keep it simple for testing.

                        validQuests.Add(quest);
                        validQuestPicker.ListBox.AddItem(validQuestName);
                    }

                    if (validQuestPicker.ListBox.Count > 0)
                    {
                        DaggerfallUI.UIManager.PushWindow(validQuestPicker);
                        return "Here is the list of active and valid quests, pick something.";
                    }
                    else
                        return "You have no valid quests currently active.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        static List<Quest> validQuests = new List<Quest>();

        public static void ShowQuestObjectivesPicker_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();

            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            Quest questToUse = validQuests[index]; // Gets the quest associated with what was selected in the list window.
            validQuestResources.Clear();

            DaggerfallListPickerWindow questObjectivePicker = new DaggerfallListPickerWindow(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            questObjectivePicker.OnItemPicked += PerformDebugAction_OnItemPicked; // Have it to it's thing after picking a valid quest objective from the list.

            QuestResource[] placeResources = questToUse.GetAllResources(typeof(Place));
            if (placeResources == null || placeResources.Length == 0)
            {
                // Do Nothing
            }
            else
            {
                foreach (QuestResource resource in placeResources)
                {
                    Place place = (Place)resource;
                    SiteDetails siteDets = place.SiteDetails;
                    string regionName = siteDets.regionName;

                    if (siteDets.siteType == SiteTypes.Dungeon)
                    {
                        validQuestResources.Add(resource);
                        questObjectivePicker.ListBox.AddItem("(" + regionName + ")" + "   " + siteDets.locationName);
                    }
                    else if (siteDets.siteType == SiteTypes.Town)
                    {
                        validQuestResources.Add(resource);
                        questObjectivePicker.ListBox.AddItem("(" + regionName + ")" + "   " + siteDets.locationName);
                    }
                    else if (siteDets.siteType == SiteTypes.Building)
                    {
                        validQuestResources.Add(resource);
                        questObjectivePicker.ListBox.AddItem("(" + regionName + ")" + "   " + siteDets.buildingName);
                    }
                }
            }

            QuestResource[] personResources = questToUse.GetAllResources(typeof(Person));
            if (personResources == null || personResources.Length == 0)
            {
                // Do Nothing
            }
            else
            {
                foreach (QuestResource resource in personResources)
                {
                    Person person = (Person)resource;

                    if (person.IsIndividualNPC && !person.IsDestroyed && !person.IsHidden)
                    {
                        validQuestResources.Add(resource);
                        questObjectivePicker.ListBox.AddItem("(" + person.HomeRegionName + ")" + "   " + person.DisplayName);
                    }
                }
            }

            QuestResource[] itemResources = questToUse.GetAllResources(typeof(Item));
            if (itemResources == null || itemResources.Length == 0)
            {
                // Do Nothing
            }
            else
            {
                foreach (QuestResource resource in itemResources)
                {
                    Item item = (Item)resource;

                    if (!item.IsHidden && !item.PlayerDropped)
                    {
                        validQuestResources.Add(resource);
                        questObjectivePicker.ListBox.AddItem("(Quest Item)" + "   " + item.DaggerfallUnityItem.ItemName);
                    }
                }
            }

            QuestResource[] foeResources = questToUse.GetAllResources(typeof(Foe));
            if (foeResources == null || foeResources.Length == 0)
            {
                // Do Nothing
            }
            else
            {
                foreach (QuestResource resource in foeResources)
                {
                    Foe foe = (Foe)resource;

                    if (!foe.IsHidden)
                    {
                        validQuestResources.Add(resource);
                        questObjectivePicker.ListBox.AddItem("(Quest Mobile)" + "   " + foe.FoeType); // Not sure if will display as string properly.
                    }
                }
            }

            if (questObjectivePicker.ListBox.Count > 0)
                DaggerfallUI.UIManager.PushWindow(questObjectivePicker);
            else
                DaggerfallUI.MessageBox("That quest has no valid resources currently available.");
        }

        static List<QuestResource> validQuestResources = new List<QuestResource>();
        public static bool consoleTeleportCheck { get; set; }
        public static QuestMarker usedQuestMarkerGlobal { get; set; }

        public static void PerformDebugAction_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();

            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            QuestResource resourceUsed = validQuestResources[index]; // Gets the quest resource associated with what was selected in the list window.
            Place usedPlace = null;

            Place place = resourceUsed.ParentQuest.GetPlace(resourceUsed.Symbol);
            Person person = resourceUsed.ParentQuest.GetPerson(resourceUsed.Symbol);

            if (place != null)
                usedPlace = place;
            else if (person != null)
            {
                Symbol personPlace = person.GetAssignedPlaceSymbol();
                place = resourceUsed.ParentQuest.GetPlace(personPlace);
                usedPlace = place;
            }

            usedQuestMarkerGlobal = usedPlace.SiteDetails.selectedMarker;
            DFLocation locationInfo = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionIndex(usedPlace.SiteDetails.regionName), (int)usedPlace.SiteDetails.locationId); // May have to change "mapId" to "locationId" not sure yet.
            DFPosition mapPixel = MapsFile.LongitudeLatitudeToMapPixel(locationInfo.MapTableData.Longitude, locationInfo.MapTableData.Latitude);
            DFPosition worldPos = MapsFile.MapPixelToWorldCoord(mapPixel.X, mapPixel.Y);

            if (usedPlace.SiteDetails.siteType == SiteTypes.Dungeon)
            {
                // Spawn inside dungeon at this world position
                consoleTeleportCheck = true;
                PlayerEnterExit.OnRespawnerComplete += TeleToQuestMarker_OnRespawnerComplete;
                GameManager.Instance.PlayerEnterExit.RespawnPlayer(worldPos.X, worldPos.Y, true, true);
            }
            else if (usedPlace.SiteDetails.siteType == SiteTypes.Building)
            {
                DFBlock[] blocks = RMBLayout.GetLocationBuildingData(locationInfo);
                int width = locationInfo.Exterior.ExteriorData.Width;
                int height = locationInfo.Exterior.ExteriorData.Height;
                List<StaticDoor> doorsOut = new List<StaticDoor>();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Iterate through all buildings in this block
                        int buildIndex = y * width + x;
                        BuildingSummary[] buildingSummary = RMBLayout.GetBuildingData(blocks[buildIndex], x, y);

                        if (buildingSummary[x].buildingKey == place.SiteDetails.buildingKey)
                        {
                            foreach (DFBlock.RmbBlock3dObjectRecord obj in blocks[buildIndex].RmbBlock.Misc3dObjectRecords)
                            {
                                ModelData modelData;
                                DaggerfallUnity.Instance.MeshReader.GetModelData(obj.ModelIdNum, out modelData); // No clue if this will all work, but will just have to test and see for "StaticDoor."

                                doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blocks[buildIndex].Index, 0, buildingSummary[x].Matrix));

                                if (doorsOut.Count > 0 && doorsOut[0].buildingKey == place.SiteDetails.buildingKey)
                                    break;
                            }
                        }
                        if (doorsOut.Count > 0 && doorsOut[0].buildingKey == place.SiteDetails.buildingKey)
                            break;
                    }
                    if (doorsOut.Count > 0 && doorsOut[0].buildingKey == place.SiteDetails.buildingKey)
                        break;
                }

                consoleTeleportCheck = true;
                PlayerEnterExit.OnRespawnerComplete += TeleToQuestMarker_OnRespawnerComplete;
                GameManager.Instance.PlayerEnterExit.RespawnPlayer(worldPos.X, worldPos.Y, false, true, doorsOut.ToArray(), true, true, true);
            }
        }

        public static void TeleToQuestMarker_OnRespawnerComplete()
        {
            if (consoleTeleportCheck && GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
            {
                QuestMarker questMarker = usedQuestMarkerGlobal;

                // Teleport PC to the chosen quest marker within the current dungeon.
                Vector3 dungeonBlockPosition = new Vector3(questMarker.dungeonX * RDBLayout.RDBSide, 0, questMarker.dungeonZ * RDBLayout.RDBSide);
                GameManager.Instance.PlayerObject.transform.localPosition = dungeonBlockPosition + questMarker.flatPosition;
            }
            else if (consoleTeleportCheck && GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
            {
                QuestMarker questMarker = usedQuestMarkerGlobal;

                // Find active marker type - saves on applying building matrix, etc. to derive position again
                Vector3 markerPos;
                if (GameManager.Instance.PlayerEnterExit.Interior.FindClosestMarker(out markerPos, (DaggerfallInterior.InteriorMarkerTypes)questMarker.markerType, GameManager.Instance.PlayerObject.transform.position))
                    GameManager.Instance.PlayerObject.transform.position = markerPos; // start here tomorrow?
            }
            else
                return;

            GameManager.Instance.PlayerMotor.FixStanding();
            consoleTeleportCheck = false;
            PlayerEnterExit.OnRespawnerComplete -= TeleToQuestMarker_OnRespawnerComplete;
        }*/

        // Likely won't have this for the initial release, will see about later if I start to find better methods later on.
        /*private static class TeleportTo
        {
            public static readonly string command = "teleportto";
            public static readonly string description = "Instantly teleports you to a specific location in the world.";
            public static readonly string usage = "teleportto [modifier]; try something like: .";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (args.Length <= 0)
                    return "Error - An argument is required, check the usage notes.";
                if (args.Length > 1)
                    return "Error - Too many arguments, check the usage notes.";

                if (player != null)
                {
                    switch (args[0])
                    {
                        case "male":
                        case "man":
                        case "m":
                        case "0":
                            if (playerEntity.Gender == Genders.Male)
                                return "You are already a male.";
                            playerEntity.Gender = Genders.Male;
                            return "You are now a male.";
                        case "female":
                        case "woman":
                        case "f":
                        case "w":
                        case "1":
                            if (playerEntity.Gender == Genders.Female)
                                return "You are already a female.";
                            playerEntity.Gender = Genders.Female;
                            return "You are now a female.";
                        default:
                            return "Error - You need to enter a gender, check usage notes.";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }*/

        // Just for testing mostly.
        /*private static class ListRegions
        {
            public static readonly string command = "listallregions";
            public static readonly string description = "Lists all regions.)";
            public static readonly string usage = "listallregions";

            public static string Execute(params string[] args)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                for (int i = 0; i < 80; i++)
                {
                    //DFRegion regionInfo = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(i);
                    //Debug.LogFormat("Region Index # {0} named: {1}", i, regionInfo.Name);
                }

                return "All regions listed.";
            }
        }*/
    }
}