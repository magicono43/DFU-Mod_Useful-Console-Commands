// Project:         UsefulConsoleCommands mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    4/14/2022, 9:00 AM
// Last Edit:		4/17/2020, 5:00 PM
// Version:			1.00
// Special Thanks:  Interkarma, Jefetienne, Hazelnut, Kab the Bird Ranger, Macadaynu, Ralzar, Billyloist
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

        // Options
        public static int MagicRegenType { get; set; }
        public static int FlatOrPercent { get; set; }
        public static int TickRegenFrequency { get; set; }
        public static float RegenAmountModifier { get; set; }
        public static float RestRegenModifier { get; set; }

        static PlayerEntity player = GameManager.Instance.PlayerEntity;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            instance = new GameObject("UsefulConsoleCommands").AddComponent<UsefulConsoleCommandsMain>(); // Add script to the scene.

            mod.LoadSettingsCallback = LoadSettings; // To enable use of the "live settings changes" feature in-game.

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
            MagicRegenType = mod.GetSettings().GetValue<int>("Options", "RegenType");
            FlatOrPercent = mod.GetSettings().GetValue<int>("Options", "FlatOrPercentageBased");
            TickRegenFrequency = mod.GetSettings().GetValue<int>("Options", "TickFrequency");
            RegenAmountModifier = mod.GetSettings().GetValue<float>("Options", "RegenMulti");
            RestRegenModifier = mod.GetSettings().GetValue<float>("Options", "RestMulti");
        }

        #endregion

        public static int GetMagicRegenType()
        {
            return MagicRegenType;
        }

        public static int GetFlatOrPercent()
        {
            return FlatOrPercent;
        }

        public static int GetTickRegenFrequency()
        {
            return TickRegenFrequency;
        }

        public static float GetRegenAmountModifier()
        {
            return RegenAmountModifier;
        }

        public static float GetRestRegenModifier()
        {
            return RestRegenModifier;
        }

        public static void RegisterModConsoleCommands()
        {
            Debug.Log("[UsefulConsoleCommands] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ChangePlayerAttribute.command, ChangePlayerAttribute.description, ChangePlayerAttribute.usage, ChangePlayerAttribute.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(ChangePlayerSkill.command, ChangePlayerSkill.description, ChangePlayerSkill.usage, ChangePlayerSkill.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(EmptyInventory.command, EmptyInventory.description, EmptyInventory.usage, EmptyInventory.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(CreateInfiniteTorch.command, CreateInfiniteTorch.description, CreateInfiniteTorch.usage, CreateInfiniteTorch.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(CleanupCorpses.command, CleanupCorpses.description, CleanupCorpses.usage, CleanupCorpses.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(LetMeSleep.command, LetMeSleep.description, LetMeSleep.usage, LetMeSleep.Execute); // tested
                ConsoleCommandsDatabase.RegisterCommand(OpenShop.command, OpenShop.description, OpenShop.usage, OpenShop.Execute);
                ConsoleCommandsDatabase.RegisterCommand(ListRegions.command, ListRegions.description, ListRegions.usage, ListRegions.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering UsefulConsoleCommands Console commands: {0}", e.Message));
            }
        }

        private static class ChangePlayerAttribute
        {
            public static readonly string command = "setattrib";
            public static readonly string description = "Changes the specified attribute's value, between 1 and 100.";
            public static readonly string usage = "setattrib [attribute] [n]; try something like: 'setattrib strength 75' or 'setattrib per 30' or 'setattrib 4 95' or even 'setattrib all 60'";

            public static string Execute(params string[] args)
            {
                if (args.Length < 2) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (!int.TryParse(args[1], out int n))
                    return string.Format("`{0}` is not a number, please use a number for [n].", args[1]);
                if (n < 1 || n > 100)
                    return "Invalid amount, [n] must be a value between 1 and 100."; // May change this if attribute values above 100 are allowed and don't cause major issues.

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
            public static readonly string description = "Changes the specified skill's value, between 1 and 100.";
            public static readonly string usage = "setskill [skill] [n]; try something like: 'setattrib bluntweapon 75' or 'setskill jump 30' or 'setskill 8 95' or even 'setskill all 60'";

            public static string Execute(params string[] args)
            {
                if (args.Length < 2) return "Invalid entry, see usage notes.";

                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;

                if (!int.TryParse(args[1], out int n))
                    return string.Format("`{0}` is not a number, please use a number for [n].", args[1]);
                if (n < 1 || n > 100)
                    return "Invalid amount, [n] must be a value between 1 and 100."; // May change this if skill values above 100 are allowed and don't cause major issues.

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
                            playerEntity.Skills.SetPermanentSkillValue(0, (short)n); // Possibly try and make this switch statement more compact vertically later, if it still allows it to be easily human readable.
                            return string.Format("Medical skill was set to {0}.", n);
                        case "etiquette":
                        case "eti":
                        case "1":
                        case "posh":
                            playerEntity.Skills.SetPermanentSkillValue(1, (short)n);
                            return string.Format("Etiquette skill was set to {0}.", n);
                        case "streetwise":
                        case "str":
                        case "2":
                        case "sw":
                            playerEntity.Skills.SetPermanentSkillValue(2, (short)n);
                            return string.Format("Streetwise skill was set to {0}.", n);
                        case "jumping":
                        case "jum":
                        case "3":
                        case "jump":
                            playerEntity.Skills.SetPermanentSkillValue(3, (short)n);
                            return string.Format("Jumping skill was set to {0}.", n);
                        case "orcish":
                        case "orc":
                        case "4":
                            playerEntity.Skills.SetPermanentSkillValue(4, (short)n);
                            return string.Format("Orcish skill was set to {0}.", n);
                        case "harpy":
                        case "har":
                        case "5":
                        case "harp":
                            playerEntity.Skills.SetPermanentSkillValue(5, (short)n);
                            return string.Format("Harpy skill was set to {0}.", n);
                        case "giantish":
                        case "gia":
                        case "6":
                        case "giant":
                            playerEntity.Skills.SetPermanentSkillValue(6, (short)n);
                            return string.Format("Giantish skill was set to {0}.", n);
                        case "dragonish":
                        case "dra":
                        case "7":
                        case "dragon":
                            playerEntity.Skills.SetPermanentSkillValue(7, (short)n);
                            return string.Format("Dragonish skill was set to {0}.", n);
                        case "nymph":
                        case "nym":
                        case "8":
                            playerEntity.Skills.SetPermanentSkillValue(8, (short)n);
                            return string.Format("Nymph skill was set to {0}.", n);
                        case "daedric":
                        case "dae":
                        case "9":
                        case "demon":
                            playerEntity.Skills.SetPermanentSkillValue(9, (short)n);
                            return string.Format("Daedric skill was set to {0}.", n);
                        case "spriggan":
                        case "spr":
                        case "10":
                        case "tree":
                            playerEntity.Skills.SetPermanentSkillValue(10, (short)n);
                            return string.Format("Spriggan skill was set to {0}.", n);
                        case "centaurian":
                        case "cen":
                        case "11":
                        case "cent":
                            playerEntity.Skills.SetPermanentSkillValue(11, (short)n);
                            return string.Format("Centaurian skill was set to {0}.", n);
                        case "impish":
                        case "imp":
                        case "12":
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
                            playerEntity.Skills.SetPermanentSkillValue(13, (short)n);
                            return string.Format("Lockpicking skill was set to {0}.", n);
                        case "mercantile":
                        case "mer":
                        case "14":
                        case "merchant":
                        case "haggle":
                        case "barter":
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
                            playerEntity.Skills.SetPermanentSkillValue(15, (short)n);
                            return string.Format("Pickpocket skill was set to {0}.", n);
                        case "stealth":
                        case "ste":
                        case "16":
                        case "sneak":
                        case "sneaking":
                            playerEntity.Skills.SetPermanentSkillValue(16, (short)n);
                            return string.Format("Stealth skill was set to {0}.", n);
                        case "swimming":
                        case "swi":
                        case "17":
                        case "swim":
                            playerEntity.Skills.SetPermanentSkillValue(17, (short)n);
                            return string.Format("Swimming skill was set to {0}.", n);
                        case "climbing":
                        case "cli":
                        case "18":
                        case "climb":
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
                            playerEntity.Skills.SetPermanentSkillValue(19, (short)n);
                            return string.Format("Backstabbing skill was set to {0}.", n);
                        case "dodging":
                        case "dod":
                        case "20":
                        case "dodge":
                        case "avoid":
                        case "avoidance":
                            playerEntity.Skills.SetPermanentSkillValue(20, (short)n);
                            return string.Format("Dodging skill was set to {0}.", n);
                        case "running":
                        case "run":
                        case "21":
                        case "sprint":
                        case "sprinting":
                        case "runner":
                        case "sonic":
                            playerEntity.Skills.SetPermanentSkillValue(21, (short)n);
                            return string.Format("Running skill was set to {0}.", n);
                        case "destruction":
                        case "des":
                        case "22":
                        case "destro":
                        case "destruct":
                        case "black":
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
                            playerEntity.Skills.SetPermanentSkillValue(23, (short)n);
                            return string.Format("Restoration skill was set to {0}.", n);
                        case "illusion":
                        case "ill":
                        case "24":
                        case "trickery":
                        case "mesmer":
                            playerEntity.Skills.SetPermanentSkillValue(24, (short)n);
                            return string.Format("Illusion skill was set to {0}.", n);
                        case "alteration":
                        case "alt":
                        case "25":
                        case "alter":
                            playerEntity.Skills.SetPermanentSkillValue(25, (short)n);
                            return string.Format("Alteration skill was set to {0}.", n);
                        case "thaumaturgy":
                        case "tha":
                        case "26":
                        case "thaum":
                            playerEntity.Skills.SetPermanentSkillValue(26, (short)n);
                            return string.Format("Thaumaturgy skill was set to {0}.", n);
                        case "mysticism":
                        case "mys":
                        case "27":
                        case "myst":
                        case "mystic":
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
                            playerEntity.Skills.SetPermanentSkillValue(30, (short)n);
                            return string.Format("HandToHand skill was set to {0}.", n);
                        case "axe":
                        case "axes":
                        case "31":
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
                            playerEntity.Skills.SetPermanentSkillValue(34, (short)n);
                            return string.Format("CriticalStrike skill was set to {0}.", n);
                        case "all":
                            for (int i = 0; i < 35; i++)
                                playerEntity.Skills.SetPermanentSkillValue(i, (short)n);
                            return string.Format("All skills were set to {0}.", n);
                        default:
                            return "Invalid skill, try something like: 'setattrib bluntweapon 75' or 'setskill jump 30' or 'setskill 8 95' or even 'setskill all 60'";
                    }
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class EmptyInventory // Might need to add more modifiers eventually for more control but that's fine, it works fine for now.
        {
            public static readonly string command = "emptyinventory";
            public static readonly string description = "Removes everything from your inventory, add additional modifier for more control of what is removed.";
            public static readonly string usage = "emptyinventory [modifier]; try something like: 'emptyinventory' or 'emptyinventory all' or 'emptyinventory wagon'. Without any modifier word, quest items, light sources, horse, wagon, and the spellbook will be preserved.";

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
                        default:
                            return "Invalid attribute, try something like: try something like: 'emptyinventory' or 'emptyinventory all' or 'emptyinventory wagon'. Without any modifier word, quest items, light sources, horse, wagon, letters of credit, and the spellbook will be preserved.";
                    }
                }
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

        private static class OpenShop
        {
            public static readonly string command = "openshop";
            public static readonly string description = "Opens a shop interface with items you can freely take or try on, items populated depend on the given modifier words.";
            public static readonly string usage = "openshop [modifier]; try something like: 'emptyinventory' or 'emptyinventory all' or 'emptyinventory wagon'. Without any modifier word, quest items, light sources, horse, wagon, and the spellbook will be preserved.";

            public static string Execute(params string[] args)
            {
                GameObject player = GameManager.Instance.PlayerObject;
                PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
                ItemCollection playerItems = playerEntity.Items;
                UCCShopWindow tradeWindow = new UCCShopWindow(DaggerfallUI.UIManager, null, UCCShopWindow.WindowModes.Buy, null);

                if (player != null)
                {
                    tradeWindow.MerchantItems = UCCShopWindow.StockMagicShopShelf(args);
                    DaggerfallUI.UIManager.PushWindow(tradeWindow);

                    return "Opening Magic Shop Shelf.";
                }
                else
                    return "Error - Something went wrong.";
            }
        }

        private static class ListRegions
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
        }
    }
}