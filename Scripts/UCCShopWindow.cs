// Project:         UsefulConsoleCommands mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    4/14/2022, 9:00 AM
// Last Edit:		5/11/2022, 11:30 PM
// Version:			1.00
// Special Thanks:  Interkarma, Jefetienne, Hazelnut, Kab the Bird Ranger, Macadaynu, Ralzar, Billyloist, Extract
// Modifier:

using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements Useful Console Commands' Shop Window.
    /// </summary>
    public class UCCShopWindow : DaggerfallInventoryWindow, IMacroContextProvider
    {
        #region UI Rects

        Rect actionButtonsPanelRect = new Rect(222, 10, 39, 190);
        new Rect infoButtonRect = new Rect(4, 26, 31, 14);
        Rect selectButtonRect = new Rect(4, 48, 31, 14);
        Rect modeActionButtonRect = new Rect(4, 124, 31, 14);
        Rect clearButtonRect = new Rect(4, 146, 31, 14);

        new Rect itemInfoPanelRect = new Rect(223, 87, 37, 32);
        Rect itemBuyInfoPanelRect = new Rect(223, 76, 37, 32);

        #endregion

        #region UI Controls

        Panel actionButtonsPanel;
        Button selectButton;
        Button modeActionButton;
        Button clearButton;

        #endregion

        #region UI Textures

        Texture2D costPanelTexture;
        Texture2D actionButtonsTexture;
        Texture2D actionButtonsGoldTexture;
        Texture2D selectSelected;
        Texture2D selectNotSelected;

        #endregion

        #region Fields

        const string buyButtonsTextureName = "INVE08I0.IMG";
        const string sellButtonsTextureName = "INVE10I0.IMG";
        const string sellButtonsGoldTextureName = "INVE11I0.IMG";
        const string repairButtonsTextureName = "INVE12I0.IMG";
        const string identifyButtonsTextureName = "INVE14I0.IMG";
        const string costPanelTextureName = "SHOP00I0.IMG";

        PlayerGPS.DiscoveredBuilding buildingDiscoveryData;
        List<ItemGroups> itemTypesAccepted = storeBuysItemType[DFLocation.BuildingTypes.GeneralStore];

        protected ItemCollection merchantItems = new ItemCollection();
        protected ItemCollection basketItems = new ItemCollection();

        bool suppressInventory = false;
        string suppressInventoryMessage = string.Empty;

        bool isModeActionDeferred = false;

        static Dictionary<DFLocation.BuildingTypes, List<ItemGroups>> storeBuysItemType = new Dictionary<DFLocation.BuildingTypes, List<ItemGroups>>()
        {
            { DFLocation.BuildingTypes.Alchemist, new List<ItemGroups>()
                { ItemGroups.Gems, ItemGroups.CreatureIngredients1, ItemGroups.CreatureIngredients2, ItemGroups.CreatureIngredients3, ItemGroups.PlantIngredients1, ItemGroups.PlantIngredients2, ItemGroups.MiscellaneousIngredients1, ItemGroups.MiscellaneousIngredients2, ItemGroups.MetalIngredients } },
            { DFLocation.BuildingTypes.Armorer, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Weapons } },
            { DFLocation.BuildingTypes.Bookseller, new List<ItemGroups>()
                { ItemGroups.Books } },
            { DFLocation.BuildingTypes.ClothingStore, new List<ItemGroups>()
                { ItemGroups.MensClothing, ItemGroups.WomensClothing } },
            { DFLocation.BuildingTypes.FurnitureStore, new List<ItemGroups>()
                { ItemGroups.Furniture } },
            { DFLocation.BuildingTypes.GemStore, new List<ItemGroups>()
                { ItemGroups.Gems, ItemGroups.Jewellery } },
            { DFLocation.BuildingTypes.GeneralStore, new List<ItemGroups>()
                { ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Transportation, ItemGroups.Jewellery, ItemGroups.Weapons, ItemGroups.UselessItems2 } },
            { DFLocation.BuildingTypes.PawnShop, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Books, ItemGroups.MensClothing, ItemGroups.WomensClothing, ItemGroups.Gems, ItemGroups.Jewellery, ItemGroups.ReligiousItems, ItemGroups.Weapons, ItemGroups.UselessItems2, ItemGroups.Paintings } },
            { DFLocation.BuildingTypes.WeaponSmith, new List<ItemGroups>()
                { ItemGroups.Armor, ItemGroups.Weapons } },
        };

        #endregion

        #region Enums

        public enum WindowModes
        {
            Inventory,      // Should never get used, treat as 'none'
            Buy
        }

        #endregion

        #region Properties

        protected WindowModes WindowMode { get; private set; }
        protected IGuild Guild { get; private set; }

        protected List<ItemGroups> ItemTypesAccepted
        {
            get { return itemTypesAccepted; }
        }

        protected ItemCollection BasketItems
        {
            get { return basketItems; }
        }

        public ItemCollection MerchantItems
        {
            get { return merchantItems; }
            set { merchantItems = value; }
        }

        #endregion

        #region Constructors

        public UCCShopWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, WindowModes windowMode = WindowModes.Buy, IGuild guild = null)
            : base(uiManager, previous)
        {
            this.WindowMode = windowMode;
            this.Guild = guild;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by inventory system
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;

            // Character portrait
            SetupPaperdoll();

            // Setup action button panel.
            actionButtonsPanel = DaggerfallUI.AddPanel(actionButtonsPanelRect, NativePanel);
            // If not inventory mode, overlay mode button texture.
            if (actionButtonsTexture != null)
                actionButtonsPanel.BackgroundTexture = actionButtonsTexture;

            // Setup item info panel if configured
            if (DaggerfallUnity.Settings.EnableInventoryInfoPanel)
            {
                if (WindowMode == WindowModes.Buy)
                    itemInfoPanel = DaggerfallUI.AddPanel(itemBuyInfoPanelRect, NativePanel);
                else
                    itemInfoPanel = DaggerfallUI.AddPanel(itemInfoPanelRect, NativePanel);
                SetupItemInfoPanel();
            }

            // Setup UI
            SetupTargetIconPanels();
            SetupTabPageButtons();
            SetupActionButtons();
            SetupAccessoryElements();
            SetupItemListScrollers();

            // Highlight purchasable items
            if (WindowMode == WindowModes.Buy)
            {
                localItemListScroller.BackgroundAnimationHandler = BuyItemBackgroundAnimationHandler;
                remoteItemListScroller.BackgroundAnimationHandler = BuyItemBackgroundAnimationHandler;
                localItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
                remoteItemListScroller.BackgroundAnimationDelay = coinsAnimationDelay;
            }
            // Exit buttons
            Button exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeExit);
            //exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

            // Setup initial state
            SelectTabPage(TabPages.WeaponsAndArmor);
            SelectActionMode(ActionModes.Select);

            // Setup initial display
            FilterLocalItems();
            localItemListScroller.Items = localItemsFiltered;
            FilterRemoteItems();
            remoteItemListScroller.Items = remoteItemsFiltered;
            UpdateAccessoryItemsDisplay();
            UpdateLocalTargetIcon();
            UpdateRemoteTargetIcon();
        }

        Texture2D[] BuyItemBackgroundAnimationHandler(DaggerfallUnityItem item)
        {
            return (basketItems.Contains(item) || remoteItems.Contains(item)) ? coinsAnimation.animatedTextures : null;
        }

        protected override void SetupActionButtons()
        {
            infoButton = DaggerfallUI.AddButton(infoButtonRect, actionButtonsPanel);
            infoButton.OnMouseClick += InfoButton_OnMouseClick;
            infoButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeInfo);

            selectButton = DaggerfallUI.AddButton(selectButtonRect, actionButtonsPanel);
            selectButton.OnMouseClick += SelectButton_OnMouseClick;
            selectButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeSelect);

            modeActionButton = DaggerfallUI.AddButton(modeActionButtonRect, actionButtonsPanel);
            modeActionButton.OnMouseClick += ModeActionButton_OnMouseClick;
            switch (WindowMode)
            {
                case WindowModes.Buy:
                    modeActionButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeBuy);
                    break;
                case WindowModes.Inventory:
                    // Shouldn't happen
                    break;
            }
            modeActionButton.OnKeyboardEvent += ModeActionButton_OnKeyboardEvent;

            clearButton = DaggerfallUI.AddButton(clearButtonRect, actionButtonsPanel);
            clearButton.OnMouseClick += ClearButton_OnMouseClick;
            clearButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.TradeClear);
        }

        #endregion

        #region Public Methods

        public override void Update()
        {
            base.Update();

            // Close window immediately if trade suppressed
            if (suppressInventory)
            {
                CloseWindow();
                if (!string.IsNullOrEmpty(suppressInventoryMessage))
                    DaggerfallUI.MessageBox(suppressInventoryMessage);
                return;
            }
        }

        public override void OnPush()
        {
            // Local items starts pointing to player inventory
            localItems = PlayerEntity.Items;

            // Initialise remote items
            remoteItems = merchantItems;
            remoteTargetType = RemoteTargetTypes.Merchant;

            // Refresh window
            Refresh();
        }

        public override void OnPop()
        {
            ClearSelectedItems();
        }

        public override void Refresh(bool refreshPaperDoll = true)
        {
            if (!IsSetup)
                return;

            base.Refresh(refreshPaperDoll);
        }

        #endregion

        #region Helper Methods

        public static ItemCollection StockMagicShopShelf(string[] args)
        {
            GameObject player = GameManager.Instance.PlayerObject;
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            byte[] itemGroups = { 0 };

            ItemCollection items = new ItemCollection();
            items.Clear();

            DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.AnyShop;
            int shopQuality = 1;

            if (args.Length == 0)
            {
                shopQuality = UnityEngine.Random.Range(1, 21);
                buildingType = (DFLocation.BuildingTypes)PickOneOf(0, 2, 5, 6, 7, 8, 9, 12, 13);
            }
            else if (args.Length == 1)
            {
                shopQuality = UnityEngine.Random.Range(1, 21);

                switch (args[0])
                {
                    case "alchemist":
                    case "alch":
                        buildingType = DFLocation.BuildingTypes.Alchemist; break;
                    case "armorer":
                    case "armorsmith":
                        buildingType = DFLocation.BuildingTypes.Armorer; break;
                    case "bookseller":
                    case "bookshop":
                        buildingType = DFLocation.BuildingTypes.Bookseller; break;
                    case "clothingstore":
                    case "clothier":
                    case "tailor":
                        buildingType = DFLocation.BuildingTypes.ClothingStore; break;
                    case "furniturestore":
                        buildingType = DFLocation.BuildingTypes.FurnitureStore; break;
                    case "gemstore":
                    case "jeweler":
                        buildingType = DFLocation.BuildingTypes.GemStore; break;
                    case "generalstore":
                    case "general":
                        buildingType = DFLocation.BuildingTypes.GeneralStore; break;
                    case "pawnshop":
                    case "pawn":
                        buildingType = DFLocation.BuildingTypes.PawnShop; break;
                    case "weaponsmith":
                        buildingType = DFLocation.BuildingTypes.WeaponSmith; break;
                    case "armor":
                        items = AddAllArmorHelper(items); return items;
                    case "weapons":
                        items = AddAllWeaponsHelper(items); return items;
                    case "artifacts":
                        items = AddAllArtifactsHelper(items); return items;
                    case "clothing":
                        items = AddAllClothingHelper(items); return items;
                    //case "books":
                        //items = AddAllBooksHelper(items); return items; Just removed this for now, because when trying to build the mod apparently bookreplacement is internal and won't work here.
                    case "religious":
                        items = AddReligiousItemsHelper(items); return items;
                    case "gems":
                        items = AddGemItemsHelper(items); return items;
                    case "ingredients":
                        items = AddAllIngredientsHelper(items); return items;
                    case "jewelry":
                        items = AddJewelryItemsHelper(items); return items;
                    case "consume":
                    case "consumes":
                    case "consumable":
                    case "consumables":
                        items = AddConsumableItemsHelper(items); return items;
                    case "mod":
                    case "mods":
                    case "modded":
                    case "moditems":
                    case "moddeditems":
                        items = AddModdedItemsHelper(items); return items;
                    default:
                        return null;
                }
            }
            else if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "alchemist":
                    case "alch":
                        buildingType = DFLocation.BuildingTypes.Alchemist; break;
                    case "armorer":
                    case "armorsmith":
                        buildingType = DFLocation.BuildingTypes.Armorer; break;
                    case "bookseller":
                    case "bookshop":
                        buildingType = DFLocation.BuildingTypes.Bookseller; break;
                    case "clothingstore":
                    case "clothier":
                    case "tailor":
                        buildingType = DFLocation.BuildingTypes.ClothingStore; break;
                    case "furniturestore":
                        buildingType = DFLocation.BuildingTypes.FurnitureStore; break;
                    case "gemstore":
                    case "jeweler":
                        buildingType = DFLocation.BuildingTypes.GemStore; break;
                    case "generalstore":
                    case "general":
                        buildingType = DFLocation.BuildingTypes.GeneralStore; break;
                    case "pawnshop":
                    case "pawn":
                        buildingType = DFLocation.BuildingTypes.PawnShop; break;
                    case "weaponsmith":
                        buildingType = DFLocation.BuildingTypes.WeaponSmith; break;
                    case "armor":
                        items = AddAllArmorHelper(items, true, args[1]); return items;
                    case "weapons":
                        items = AddAllWeaponsHelper(items, true, args[1]); return items;
                    case "clothing":
                        items = AddAllClothingHelper(items, true, args[1]); return items;
                    case "ingredients":
                        items = AddAllIngredientsHelper(items, args[1]); return items;
                    default:
                        return null;
                }

                if (!int.TryParse(args[1], out int n))
                    return null;
                if (n < 1 || n > 20) // Quality range limits for vanilla buildings.
                    return null;

                shopQuality = n;
            }
            else if (args.Length == 3)
            {
                switch (args[0])
                {
                    case "armor":
                        items = AddAllArmorHelper(items, true, args[1], args[2]); return items;
                    case "clothing":
                        items = AddAllClothingHelper(items, true, args[1], args[2]); return items;
                    default:
                        return null;
                }
            }
            else if (args.Length == 4)
            {
                switch (args[0])
                {
                    case "armor":
                        items = AddAllArmorHelper(items, true, args[1], args[2], args[3]); return items;
                    case "clothing":
                        items = AddAllClothingHelper(items, true, args[1], args[2], args[3]); return items;
                    default:
                        return null;
                }
            }

            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                    itemGroups = DaggerfallLootDataTables.itemGroupsAlchemist;
                    DaggerfallLoot.RandomlyAddPotionRecipe(25, items);
                    break;
                case DFLocation.BuildingTypes.Armorer:
                    itemGroups = DaggerfallLootDataTables.itemGroupsArmorer;
                    break;
                case DFLocation.BuildingTypes.Bookseller:
                    itemGroups = DaggerfallLootDataTables.itemGroupsBookseller;
                    break;
                case DFLocation.BuildingTypes.ClothingStore:
                    itemGroups = DaggerfallLootDataTables.itemGroupsClothingStore;
                    break;
                case DFLocation.BuildingTypes.GemStore:
                    itemGroups = DaggerfallLootDataTables.itemGroupsGemStore;
                    break;
                case DFLocation.BuildingTypes.GeneralStore:
                    itemGroups = DaggerfallLootDataTables.itemGroupsGeneralStore;
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Horse));
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Small_cart));
                    break;
                case DFLocation.BuildingTypes.PawnShop:
                    itemGroups = DaggerfallLootDataTables.itemGroupsPawnShop;
                    break;
                case DFLocation.BuildingTypes.WeaponSmith:
                    itemGroups = DaggerfallLootDataTables.itemGroupsWeaponSmith;
                    break;
            }

            for (int i = 0; i < itemGroups.Length; i += 2)
            {
                ItemGroups itemGroup = (ItemGroups)itemGroups[i];
                int chanceMod = itemGroups[i + 1];
                if (itemGroup == ItemGroups.MensClothing && playerEntity.Gender == Genders.Female)
                    itemGroup = ItemGroups.WomensClothing;
                if (itemGroup == ItemGroups.WomensClothing && playerEntity.Gender == Genders.Male)
                    itemGroup = ItemGroups.MensClothing;

                if (itemGroup != ItemGroups.Furniture && itemGroup != ItemGroups.UselessItems1)
                {
                    if (itemGroup == ItemGroups.Books)
                    {
                        int qualityMod = (shopQuality + 3) / 5;
                        if (qualityMod >= 4)
                            --qualityMod;
                        qualityMod++;
                        for (int j = 0; j <= qualityMod; ++j)
                        {
                            items.AddItem(ItemBuilder.CreateRandomBook());
                        }
                    }
                    else
                    {
                        System.Array enumArray = itemHelper.GetEnumArray(itemGroup);
                        for (int j = 0; j < enumArray.Length; ++j)
                        {
                            ItemTemplate itemTemplate = itemHelper.GetItemTemplate(itemGroup, j);
                            if (itemTemplate.rarity <= shopQuality)
                            {
                                int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                if (Dice100.SuccessRoll(stockChance))
                                {
                                    DaggerfallUnityItem item = null;
                                    if (itemGroup == ItemGroups.Weapons)
                                        item = ItemBuilder.CreateWeapon(j + Weapons.Dagger, FormulaHelper.RandomMaterial(playerEntity.Level));
                                    else if (itemGroup == ItemGroups.Armor)
                                        item = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, j + Armor.Cuirass, FormulaHelper.RandomArmorMaterial(playerEntity.Level));
                                    else if (itemGroup == ItemGroups.MensClothing)
                                    {
                                        item = ItemBuilder.CreateMensClothing(j + MensClothing.Straps, playerEntity.Race);
                                        item.dyeColor = ItemBuilder.RandomClothingDye();
                                    }
                                    else if (itemGroup == ItemGroups.WomensClothing)
                                    {
                                        item = ItemBuilder.CreateWomensClothing(j + WomensClothing.Brassier, playerEntity.Race);
                                        item.dyeColor = ItemBuilder.RandomClothingDye();
                                    }
                                    else if (itemGroup == ItemGroups.MagicItems)
                                    {
                                        item = ItemBuilder.CreateRandomMagicItem(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                                    }
                                    else
                                    {
                                        item = new DaggerfallUnityItem(itemGroup, j);
                                        if (DaggerfallUnity.Settings.PlayerTorchFromItems && item.IsOfTemplate(ItemGroups.UselessItems2, (int)UselessItems2.Oil))
                                            item.stackCount = UnityEngine.Random.Range(5, 20 + 1);  // Shops stock 5-20 bottles
                                    }
                                    items.AddItem(item);
                                }
                            }
                        }
                        // Add any modded items registered in applicable groups
                        int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(itemGroup);
                        for (int j = 0; j < customItemTemplates.Length; j++)
                        {
                            ItemTemplate itemTemplate = itemHelper.GetItemTemplate(itemGroup, customItemTemplates[j]);
                            if (itemTemplate.rarity <= shopQuality)
                            {
                                int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                if (Dice100.SuccessRoll(stockChance))
                                {
                                    DaggerfallUnityItem item = ItemBuilder.CreateItem(itemGroup, customItemTemplates[j]);

                                    // Setup specific group stats
                                    if (itemGroup == ItemGroups.Weapons)
                                    {
                                        WeaponMaterialTypes material = FormulaHelper.RandomMaterial(playerEntity.Level);
                                        ItemBuilder.ApplyWeaponMaterial(item, material);
                                    }
                                    else if (itemGroup == ItemGroups.Armor)
                                    {
                                        ArmorMaterialTypes material = FormulaHelper.RandomArmorMaterial(playerEntity.Level);
                                        ItemBuilder.ApplyArmorSettings(item, playerEntity.Gender, playerEntity.Race, material);
                                    }

                                    items.AddItem(item);
                                }
                            }
                        }
                    }
                }
            }
            return items;
        }

        public static ArmorMaterialTypes[] armorMaterials = {
                ArmorMaterialTypes.Leather, ArmorMaterialTypes.Chain, ArmorMaterialTypes.Iron, ArmorMaterialTypes.Steel,
                ArmorMaterialTypes.Silver, ArmorMaterialTypes.Elven, ArmorMaterialTypes.Dwarven, ArmorMaterialTypes.Mithril,
                ArmorMaterialTypes.Adamantium, ArmorMaterialTypes.Ebony, ArmorMaterialTypes.Orcish, ArmorMaterialTypes.Daedric
            };

        public static WeaponMaterialTypes[] weaponMaterials = {
                WeaponMaterialTypes.Iron, WeaponMaterialTypes.Steel, WeaponMaterialTypes.Silver, WeaponMaterialTypes.Elven,
                WeaponMaterialTypes.Dwarven, WeaponMaterialTypes.Mithril, WeaponMaterialTypes.Adamantium, WeaponMaterialTypes.Ebony,
                WeaponMaterialTypes.Orcish, WeaponMaterialTypes.Daedric
            };

        public static ArtifactsSubTypes[] vanillaArtifacts = {
                ArtifactsSubTypes.Masque_of_Clavicus, ArtifactsSubTypes.Mehrunes_Razor, ArtifactsSubTypes.Mace_of_Molag_Bal, ArtifactsSubTypes.Hircine_Ring,
                ArtifactsSubTypes.Sanguine_Rose, ArtifactsSubTypes.Oghma_Infinium, ArtifactsSubTypes.Wabbajack, ArtifactsSubTypes.Ring_of_Namira,
                ArtifactsSubTypes.Skull_of_Corruption, ArtifactsSubTypes.Azuras_Star, ArtifactsSubTypes.Volendrung, ArtifactsSubTypes.Warlocks_Ring,
                ArtifactsSubTypes.Auriels_Bow, ArtifactsSubTypes.Necromancers_Amulet, ArtifactsSubTypes.Chrysamere, ArtifactsSubTypes.Lords_Mail,
                ArtifactsSubTypes.Staff_of_Magnus, ArtifactsSubTypes.Ring_of_Khajiit, ArtifactsSubTypes.Ebony_Mail, ArtifactsSubTypes.Auriels_Shield,
                ArtifactsSubTypes.Spell_Breaker, ArtifactsSubTypes.Skeletons_Key, ArtifactsSubTypes.Ebony_Blade
            };

        public static List<MensClothing> mensUsableClothing = new List<MensClothing>() {
                MensClothing.Casual_cloak, MensClothing.Formal_cloak, MensClothing.Reversible_tunic, MensClothing.Plain_robes,
                MensClothing.Short_shirt, MensClothing.Short_shirt_with_belt, MensClothing.Long_shirt, MensClothing.Long_shirt_with_belt,
                MensClothing.Short_shirt_closed_top, MensClothing.Short_shirt_closed_top2, MensClothing.Long_shirt_closed_top, MensClothing.Long_shirt_closed_top2
            };

        public static List<WomensClothing> womensUsableClothing = new List<WomensClothing>() {
                WomensClothing.Casual_cloak, WomensClothing.Formal_cloak, WomensClothing.Strapless_dress, WomensClothing.Plain_robes,
                WomensClothing.Short_shirt, WomensClothing.Short_shirt_belt, WomensClothing.Long_shirt, WomensClothing.Long_shirt_belt,
                WomensClothing.Short_shirt_closed, WomensClothing.Short_shirt_closed_belt, WomensClothing.Long_shirt_closed, WomensClothing.Long_shirt_closed_belt
            };

        public static List<ItemGroups> ingredientItemGroups = new List<ItemGroups>() {
                ItemGroups.Gems, ItemGroups.PlantIngredients1, ItemGroups.PlantIngredients2, ItemGroups.CreatureIngredients1,
                ItemGroups.CreatureIngredients2, ItemGroups.CreatureIngredients3, ItemGroups.MiscellaneousIngredients1, ItemGroups.MetalIngredients,
                ItemGroups.MiscellaneousIngredients2
            };

        public static ItemCollection AddAllArmorHelper(ItemCollection items, bool modifier = false, string argMat = "", string argGend = "", string argRace = "")
        {
            GameObject player = GameManager.Instance.PlayerObject;
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            DaggerfallUnityItem newItem = null;

            foreach (ArmorMaterialTypes material in armorMaterials)
            {
                if (modifier && material != GetArmorMatTypeByString(argMat))
                    continue;

                Genders gend = GetGenderByString(argGend);
                Races race = GetRaceByString(argRace);

                Array enumArray = itemHelper.GetEnumArray(ItemGroups.Armor);
                for (int i = 0; i < enumArray.Length; i++)
                {
                    Armor armorType = (Armor)enumArray.GetValue(i);
                    int vs = 0;
                    int vf = 0;
                    if (armorType == Armor.Cuirass || armorType == Armor.Left_Pauldron || armorType == Armor.Right_Pauldron)
                    {
                        if (material == ArmorMaterialTypes.Chain)
                        {
                            vs = 4;
                        }
                        else if (material >= ArmorMaterialTypes.Iron)
                        {
                            vs = 1;
                            vf = 4;
                        }
                    }
                    else if (armorType == Armor.Greaves)
                    {
                        if (material == ArmorMaterialTypes.Leather)
                        {
                            vs = 0;
                            vf = 2;
                        }
                        else if (material == ArmorMaterialTypes.Chain)
                        {
                            vs = 6;
                        }
                        else if (material >= ArmorMaterialTypes.Iron)
                        {
                            vs = 2;
                            vf = 6;
                        }
                    }
                    else if (armorType == Armor.Gauntlets || armorType == Armor.Boots)
                    {
                        if (material == ArmorMaterialTypes.Leather)
                        {
                            vf = 1;
                        }
                        else
                        {
                            vs = 1;
                            vf = itemHelper.GetItemTemplate(ItemGroups.Armor, i).variants;
                        }
                    }
                    else
                    {
                        vf = itemHelper.GetItemTemplate(ItemGroups.Armor, i).variants;
                    }
                    if (vf == 0)
                        vf = vs + 1;

                    for (int v = vs; v < vf; v++)
                    {
                        newItem = ItemBuilder.CreateArmor(gend, race, armorType, material, v);
                        items.AddItem(newItem);
                    }
                }
                int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(ItemGroups.Armor);
                for (int i = 0; i < customItemTemplates.Length; i++)
                {
                    newItem = ItemBuilder.CreateItem(ItemGroups.Armor, customItemTemplates[i]);
                    ItemBuilder.ApplyArmorSettings(newItem, gend, race, material);
                    items.AddItem(newItem);
                }
            }
            return items;
        }

        public static ArmorMaterialTypes GetArmorMatTypeByString(string arg)
        {
            switch (arg)
            {
                case "leather":
                    return ArmorMaterialTypes.Leather;
                case "chain":
                case "chainmail":
                    return ArmorMaterialTypes.Chain; // Completely forget what "Chain2" is for, but screw it.
                case "iron":
                    return ArmorMaterialTypes.Iron;
                case "steel":
                    return ArmorMaterialTypes.Steel;
                case "silver":
                    return ArmorMaterialTypes.Silver;
                case "elven":
                    return ArmorMaterialTypes.Elven;
                case "dwarven":
                case "dwarf":
                    return ArmorMaterialTypes.Dwarven;
                case "mithril":
                case "mith":
                    return ArmorMaterialTypes.Mithril;
                case "adamantium":
                case "adam":
                    return ArmorMaterialTypes.Adamantium;
                case "ebony":
                    return ArmorMaterialTypes.Ebony;
                case "orcish":
                case "orc":
                    return ArmorMaterialTypes.Orcish;
                case "daedric":
                case "dae":
                    return ArmorMaterialTypes.Daedric;
                default:
                    return ArmorMaterialTypes.None;
            }
        }

        public static ItemCollection AddAllWeaponsHelper(ItemCollection items, bool modifier = false, string arg = "")
        {
            GameObject player = GameManager.Instance.PlayerObject;
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            DaggerfallUnityItem newItem = null;

            foreach (WeaponMaterialTypes material in weaponMaterials)
            {
                if (modifier && material != GetWeaponMatTypeByString(arg))
                    continue;

                Array enumArray = itemHelper.GetEnumArray(ItemGroups.Weapons);
                for (int i = 0; i < enumArray.Length - 1; i++)
                {
                    newItem = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(i), material);
                    items.AddItem(newItem);
                }
                int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(ItemGroups.Weapons);
                for (int i = 0; i < customItemTemplates.Length; i++)
                {
                    newItem = ItemBuilder.CreateItem(ItemGroups.Weapons, customItemTemplates[i]);
                    ItemBuilder.ApplyWeaponMaterial(newItem, material);
                    items.AddItem(newItem);
                }
            }
            return items;
        }

        public static WeaponMaterialTypes GetWeaponMatTypeByString(string arg)
        {
            switch (arg)
            {
                case "iron":
                    return WeaponMaterialTypes.Iron;
                case "steel":
                    return WeaponMaterialTypes.Steel;
                case "silver":
                    return WeaponMaterialTypes.Silver;
                case "elven":
                    return WeaponMaterialTypes.Elven;
                case "dwarven":
                case "dwarf":
                    return WeaponMaterialTypes.Dwarven;
                case "mithril":
                case "mith":
                    return WeaponMaterialTypes.Mithril;
                case "adamantium":
                case "adam":
                    return WeaponMaterialTypes.Adamantium;
                case "ebony":
                    return WeaponMaterialTypes.Ebony;
                case "orcish":
                case "orc":
                    return WeaponMaterialTypes.Orcish;
                case "daedric":
                case "dae":
                    return WeaponMaterialTypes.Daedric;
                default:
                    return WeaponMaterialTypes.None;
            }
        }

        public static ItemCollection AddAllArtifactsHelper(ItemCollection items)
        {
            GameObject player = GameManager.Instance.PlayerObject;
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            DaggerfallUnityItem newItem = null;

            foreach (ArtifactsSubTypes artifact in vanillaArtifacts)
            {
                newItem = ItemBuilder.CreateItem(ItemGroups.Artifacts, (int)artifact);
                items.AddItem(newItem);
            }
            return items;
        }

        public static ItemCollection AddAllClothingHelper(ItemCollection items, bool modifier = false, string argDye = "", string argGend = "", string argRace = "")
        {
            GameObject player = GameManager.Instance.PlayerObject;
            PlayerEntity playerEntity = player.GetComponent<DaggerfallEntityBehaviour>().Entity as PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            DaggerfallUnityItem newItem = null;

            Genders gend = GetGenderByString(argGend);
            Races race = GetRaceByString(argRace);

            DyeColors[] clothingDyes =  ItemBuilder.clothingDyes;
            ItemGroups clothing = (gend == Genders.Male) ? ItemGroups.MensClothing : ItemGroups.WomensClothing;
            foreach (DyeColors dye in clothingDyes)
            {
                if (modifier && dye != GetClothingDyeTypeByString(argDye))
                    continue;

                Array enumArray = itemHelper.GetEnumArray(clothing);
                for (int i = 0; i < enumArray.Length; i++)
                {
                    ItemTemplate itemTemplate = itemHelper.GetItemTemplate(clothing, i);
                    if ((gend == Genders.Male && mensUsableClothing.Contains((MensClothing)enumArray.GetValue(i))) ||
                        womensUsableClothing.Contains((WomensClothing)enumArray.GetValue(i)) || itemTemplate.variants == 0)
                        itemTemplate.variants = 1;

                    for (int v = 0; v < itemTemplate.variants; v++)
                    {
                        newItem = new DaggerfallUnityItem(clothing, i);
                        ItemBuilder.SetRace(newItem, race);
                        newItem.dyeColor = dye;
                        newItem.CurrentVariant = v;
                        items.AddItem(newItem);
                    }
                }
            }
            return items;
        }

        public static DyeColors GetClothingDyeTypeByString(string arg)
        {
            switch (arg)
            {
                case "blue":
                case "b":
                    return DyeColors.Blue;
                case "grey":
                case "gray":
                    return DyeColors.Grey;
                case "red":
                case "r":
                    return DyeColors.Red;
                case "darkbrown":
                case "db":
                    return DyeColors.DarkBrown;
                case "purple":
                case "purp":
                case "p":
                    return DyeColors.Purple;
                case "lightbrown":
                case "brown":
                case "lb":
                    return DyeColors.LightBrown;
                case "white":
                case "w":
                    return DyeColors.White;
                case "aquamarine":
                case "teal":
                case "aqua":
                case "a":
                    return DyeColors.Aquamarine;
                case "yellow":
                case "yell":
                case "y":
                    return DyeColors.Yellow;
                case "green":
                case "gre":
                case "g":
                    return DyeColors.Green;
                default:
                    return DyeColors.Unchanged;
            }
        }

        public static Genders GetGenderByString(string arg)
        {
            switch (arg)
            {
                case "male":
                case "man":
                case "m":
                case "0":
                    return Genders.Male;
                case "female":
                case "woman":
                case "f":
                case "w":
                case "1":
                    return Genders.Female;
                default:
                    return GameManager.Instance.PlayerEntity.Gender;
            }
        }

        public static Races GetRaceByString(string arg)
        {
            switch (arg)
            {
                case "breton":
                case "bret":
                case "manmeri":
                    return Races.Breton;
                case "redguard":
                case "red-guard":
                case "rg":
                case "yokudans":
                    return Races.Redguard;
                case "nord":
                    return Races.Nord;
                case "darkelf":
                case "dark-elf":
                case "de":
                case "dunmer":
                    return Races.DarkElf;
                case "highelf":
                case "high-elf":
                case "he":
                case "altmer":
                    return Races.HighElf;
                case "woodelf":
                case "wood-elf":
                case "we":
                case "bosmer":
                    return Races.WoodElf;
                case "khajiit":
                case "kha":
                case "cat":
                case "cats":
                    return Races.Khajiit;
                case "argonian":
                case "saxhleel":
                case "arg":
                case "lizard":
                case "lizards":
                case "reptilian":
                case "reptile":
                    return Races.Argonian;
                default:
                    return GameManager.Instance.PlayerEntity.Race;
            }
        }

        /*public static ItemCollection AddAllBooksHelper(ItemCollection items) // ItemHelper.bookIDNameMapping is readonly private currently, so just do lazy approach for now.
        {
            DaggerfallUnityItem newItem = null;
            LoadBookIDNameMapping(); // So this hacky-lazy approach seems to have worked fine so far, not optimal but if it works good enough for now I suppose.

            for (int i = 0; i < 200; i++)
            {
                newItem = CreateBook(i);

                if (newItem == null)
                    continue;
                else
                    items.AddItem(newItem);
            }
            return items;
        }

        public static DaggerfallUnityItem CreateBook(int id)
        {
            var bookFile = new BookFile();

            string name = GetBookFileName(id);

            if (name == null)
                return null;

            if (!DaggerfallWorkshop.Utility.AssetInjection.BookReplacement.TryImportBook(name, bookFile) &&
                !bookFile.OpenBook(DaggerfallUnity.Instance.Arena2Path, name))
                return null;

            return new DaggerfallUnityItem(ItemGroups.Books, 0)
            {
                message = id,
                value = bookFile.Price
            };
        }

        readonly static Dictionary<int, String> bookIDNameMapping = new Dictionary<int, String>();

        public static string GetBookFileName(int id)
        {
            // Get name for custom book
            DaggerfallWorkshop.Utility.AssetInjection.BookMappingEntry entry;
            if (DaggerfallWorkshop.Utility.AssetInjection.BookReplacement.BookMappingEntries.TryGetValue(id, out entry))
                return entry.Name;

            // Check if classic book and make name from id
            if (bookIDNameMapping.ContainsKey(id))
                return BookFile.messageToBookFilename(id);

            return null;
        }

        static void LoadBookIDNameMapping()
        {
            try
            {
                TextAsset bookNames = Resources.Load<TextAsset>("books");
                List<BookMappingTemplate> mappings = Serialization.SaveLoadManager.Deserialize(typeof(List<BookMappingTemplate>), bookNames.text) as List<BookMappingTemplate>;
                foreach (BookMappingTemplate entry in mappings)
                {
                    bookIDNameMapping.Add(entry.id, entry.title);
                }
            }
            catch
            {
                Debug.Log("Could not load the BookIDName mapping from Resources. Check file exists and is in correct format.");
            }

            if (DaggerfallUnity.Settings.AssetInjection)
                DaggerfallWorkshop.Utility.AssetInjection.BookReplacement.FindAdditionalBooks(bookIDNameMapping);
        }*/

        public static ItemCollection AddReligiousItemsHelper(ItemCollection items)
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.ReligiousItems);
            DaggerfallUnityItem newItem = null;

            for (int i = 0; i < enumArray.Length; i++)
            {
                newItem = new DaggerfallUnityItem(ItemGroups.ReligiousItems, i);
                items.AddItem(newItem);
            }
            return items;
        }

        public static ItemCollection AddGemItemsHelper(ItemCollection items)
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Gems);
            DaggerfallUnityItem newItem = null;

            for (int i = 0; i < enumArray.Length; i++)
            {
                newItem = new DaggerfallUnityItem(ItemGroups.Gems, i);
                items.AddItem(newItem);
            }
            return items;
        }

        public static ItemCollection AddAllIngredientsHelper(ItemCollection items, string arg = "")
        {
            DaggerfallUnityItem newItem = null;

            if (!int.TryParse(arg, out int n))
                n = 1;
            if (n < 1)
                n = 1;

            foreach (ItemGroups group in ingredientItemGroups)
            {
                Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(group);

                for (int i = 0; i < enumArray.Length; i++)
                {
                    for (int v = 0; v < n; v++)
                    {
                        newItem = new DaggerfallUnityItem(group, i);
                        items.AddItem(newItem);
                    }
                }
            }
            return items;
        }

        public static ItemCollection AddJewelryItemsHelper(ItemCollection items)
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Jewellery);
            DaggerfallUnityItem newItem = null;

            for (int i = 0; i < enumArray.Length; i++)
            {
                newItem = new DaggerfallUnityItem(ItemGroups.Jewellery, i);
                items.AddItem(newItem);
            }
            return items;
        }

        public static ItemCollection AddConsumableItemsHelper(ItemCollection items)
        {
            DaggerfallUnityItem newItem = null;
            int groupIndex = 0;

            groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.Weapons, (int)Weapons.Arrow);
            newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);
            newItem.stackCount = 500;
            items.AddItem(newItem);

            groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);
            newItem = new DaggerfallUnityItem(ItemGroups.MiscItems, groupIndex);
            newItem.TrappedSoulType = MobileTypes.None; // Empty
            items.AddItem(newItem);

            if (DaggerfallUnity.Settings.PlayerTorchFromItems)
            {
                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, (int)UselessItems2.Lantern);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex);
                items.AddItem(newItem);

                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, (int)UselessItems2.Oil);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex);
                newItem.stackCount = 500;
                items.AddItem(newItem);
            }

            if (UsefulConsoleCommands.UsefulConsoleCommandsMain.RolePlayRealismBandagingModule)
            {
                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, (int)UselessItems2.Bandage);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex);
                newItem.stackCount = 500;
                items.AddItem(newItem);
            }

            if (UsefulConsoleCommands.UsefulConsoleCommandsMain.RepairToolsCheck)
            {
                for (int i = 800; i < 806; i++) // Test later to see if it gives all tools or not, otherwise will have to change these loop values a bit.
                {
                    groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, i);
                    newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex);
                    items.AddItem(newItem);
                }
            }

            if (UsefulConsoleCommands.UsefulConsoleCommandsMain.RealisticWagonCheck)
            {
                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, 541);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex); // Sugar Lumps
                items.AddItem(newItem);

                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, 542);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex); // Wagon Parts
                items.AddItem(newItem);
            }

            if (UsefulConsoleCommands.UsefulConsoleCommandsMain.ClimatesAndCaloriesCheck)
            {
                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, 530);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex); // Camping Equipment
                items.AddItem(newItem);

                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, 531);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex); // Rations
                items.AddItem(newItem);

                groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.UselessItems2, 539);
                newItem = new DaggerfallUnityItem(ItemGroups.UselessItems2, groupIndex); // Waterskin
                items.AddItem(newItem);
            }

            List<int> recipeKeys = GameManager.Instance.EntityEffectBroker.GetPotionRecipeKeys();
            for (int i = 0; i < recipeKeys.Count; i++)
            {
                newItem = ItemBuilder.CreatePotion(recipeKeys[i], 500);
                items.AddItem(newItem);
            }

            return items;
        }

        public static ItemCollection AddModdedItemsHelper(ItemCollection items) // Any modded items that use variants to build them will likely not work properly.
        {
            // Add any modded items registered in applicable groups
            for (int i = 0; i < Enum.GetNames(typeof(ItemGroups)).Length; i++)
            {
                int[] customItemTemplates = DaggerfallUnity.Instance.ItemHelper.GetCustomItemsForGroup((ItemGroups)i);
                if (customItemTemplates.Length > 0)
                {
                    for (int n = 0; n < customItemTemplates.Length; n++)
                    {
                        ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate((ItemGroups)i, customItemTemplates[n]);

                        DaggerfallUnityItem item = ItemBuilder.CreateItem((ItemGroups)i, itemTemplate.index);
                        //DaggerfallUnityItem item = ItemBuilder.CreateItem((ItemGroups)i, customItemTemplates[n]);

                        // Setup specific group stats
                        if ((ItemGroups)i == ItemGroups.Weapons)
                        {
                            WeaponMaterialTypes material = FormulaHelper.RandomMaterial(GameManager.Instance.PlayerEntity.Level);
                            ItemBuilder.ApplyWeaponMaterial(item, material);
                        }
                        else if ((ItemGroups)i == ItemGroups.Armor)
                        {
                            ArmorMaterialTypes material = FormulaHelper.RandomArmorMaterial(GameManager.Instance.PlayerEntity.Level);
                            ItemBuilder.ApplyArmorSettings(item, GameManager.Instance.PlayerEntity.Gender, GameManager.Instance.PlayerEntity.Race, material);
                        }

                        items.AddItem(item);
                    }
                }
            }

            return items;
        }

        public static int PickOneOf(params int[] values) // Pango provided assistance in making this much cleaner way of doing the random value choice part, awesome.
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        protected override void SelectActionMode(ActionModes mode)
        {
            selectedActionMode = mode;
            if (mode == ActionModes.Info)
            {
                infoButton.BackgroundTexture = infoSelected;
                selectButton.BackgroundTexture = selectNotSelected;
            }
            else if (mode == ActionModes.Select)
            {
                infoButton.BackgroundTexture = infoNotSelected;
                selectButton.BackgroundTexture = selectSelected;
            }
        }

        protected void ClearSelectedItems()
        {
            if (WindowMode == WindowModes.Buy)
            {   // Return all basket items to merchant, unequipping if necessary.
                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    if (item.IsEquipped)
                        UnequipItem(item);
                }
                remoteItems.TransferAll(basketItems);
            }
            else
            {   // Return items to player inventory. 
                // Note: ignoring weight here, like classic. Priority is to not lose any items.
                localItems.TransferAll(remoteItems);
            }
        }

        protected override float GetCarriedWeight()
        {
            return PlayerEntity.CarriedWeight + basketItems.GetWeight();
        }

        protected override void UpdateLocalTargetIcon()
        {
            base.UpdateLocalTargetIcon();
        }

        protected override void UpdateRemoteTargetIcon()
        {
            ImageData containerImage;
            switch (WindowMode)
            {
                default:
                case WindowModes.Buy:
                    containerImage = DaggerfallUnity.ItemHelper.GetContainerImage(InventoryContainerImages.Shelves);
                    break;
            }
            remoteTargetIconPanel.BackgroundTexture = containerImage.texture;
        }

        protected override void FilterLocalItems()
        {
            localItemsFiltered.Clear();

            // Add any basket items to filtered list first, if not using wagon
            if (WindowMode == WindowModes.Buy && basketItems != null)
            {
                for (int i = 0; i < basketItems.Count; i++)
                {
                    DaggerfallUnityItem item = basketItems.GetItem(i);
                    // Add if not equipped
                    if (!item.IsEquipped)
                        AddLocalItem(item);
                }
            }
            // Add local items to filtered list
            if (localItems != null)
            {
                for (int i = 0; i < localItems.Count; i++)
                {
                    // Add if not equipped & accepted for selling
                    DaggerfallUnityItem item = localItems.GetItem(i);
                    if (!item.IsEquipped)
                    {
                        AddLocalItem(item);
                    }
                }
            }
        }

        protected override void FilterRemoteItems()
        {
            base.FilterRemoteItems();
        }

        protected override void LoadTextures()
        {
            base.LoadTextures();

            // Load special button texture.
            actionButtonsTexture = ImageReader.GetTexture(buyButtonsTextureName);
            actionButtonsGoldTexture = ImageReader.GetTexture(sellButtonsGoldTextureName);
            DFSize actionButtonsFullSize = new DFSize(39, 190);
            selectNotSelected = ImageReader.GetSubTexture(actionButtonsTexture, selectButtonRect, actionButtonsFullSize);
            selectSelected = ImageReader.GetSubTexture(actionButtonsGoldTexture, selectButtonRect, actionButtonsFullSize);

            costPanelTexture = ImageReader.GetTexture(costPanelTextureName);
        }

        #endregion

        #region Item Click Event Handlers

        protected override void LocalItemListScroller_OnItemClick(DaggerfallUnityItem item, ActionModes actionMode)
        {
            // Handle click based on action & mode
            if (actionMode == ActionModes.Select || actionMode == ActionModes.Remove)
            {
                switch (WindowMode)
                {
                    case WindowModes.Buy:
                        if (actionMode == ActionModes.Remove && basketItems.Contains(item))    // Allows clearing individual items
                            TransferItem(item, basketItems, remoteItems);
                        else if (actionMode == ActionModes.Select)  // Allows player to equip and unequip while purchasing.
                            EquipItem(item);
                        break;
                }
            }
            else if (actionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        protected override void RemoteItemListScroller_OnItemClick(DaggerfallUnityItem item, ActionModes actionMode)
        {
            // Handle click based on action
            if (actionMode == ActionModes.Select || actionMode == ActionModes.Remove)
            {
                TransferItem(item, remoteItems, basketItems, 100000, equip: !item.IsAStack() && actionMode == ActionModes.Select);
            }
            else if (actionMode == ActionModes.Info)
            {
                ShowInfoPopup(item);
            }
        }

        #endregion

        #region Action Button Event Handlers

        private void InfoButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            SelectActionMode(ActionModes.Info);
        }

        private void SelectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            SelectActionMode(ActionModes.Select);
        }

        private void DoModeAction()
        {
            ShowTradePopup();
        }

        private void ModeActionButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DoModeAction();
        }

        void ModeActionButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isModeActionDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isModeActionDeferred)
            {
                isModeActionDeferred = false;
                DoModeAction();
            }
        }

        private void ClearButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ClearSelectedItems();
            Refresh();
        }

        protected virtual void ConfirmTrade_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Proceed with trade.
                switch (WindowMode)
                {
                    case WindowModes.Buy:
                        RaiseOnTradeHandler(basketItems.GetNumItems(), 0);
                        PlayerEntity.Items.TransferAll(basketItems);
                        break;
                }
                DaggerfallUI.Instance.PlayOneShot(SoundClips.GoldPieces);
                Refresh();
            }
            CloseWindow();
        }

        #endregion

        #region Misc Events & Helpers

        protected virtual void ShowTradePopup()
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter, "You take the items from the magical shelf.");
            messageBox.SetTextTokens(tokens, this);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
            messageBox.OnButtonClick += ConfirmTrade_OnButtonClick;
            uiManager.PushWindow(messageBox);
        }

        // OnTrade event. (value=0:steal, numItems=0:caught)
        public delegate void OnTradeHandler(WindowModes mode, int numItems, int value);
        public event OnTradeHandler OnTrade;
        protected virtual void RaiseOnTradeHandler(int numItems, int value)
        {
            if (OnTrade != null)
                OnTrade(WindowMode, numItems, value);
        }

        protected override void StartGameBehaviour_OnNewGame()
        {
            // Do nothing when game starts, as this window class is not used in a persisted manner like its parent.
        }

        #endregion

        #region Macro handling

        public MacroDataSource GetMacroDataSource()
        {
            return new BRETradeMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for trade window.
        /// </summary>
        private class BRETradeMacroDataSource : MacroDataSource
        {
            private UCCShopWindow parent;
            public BRETradeMacroDataSource(UCCShopWindow tradeWindow)
            {
                this.parent = tradeWindow;
            }

            public override string Amount()
            {
                return 0.ToString();
            }

            public override string ShopName()
            {
                return parent.buildingDiscoveryData.displayName;
            }

            public override string GuildTitle()
            {
                if (parent.Guild != null)
                    return parent.Guild.GetTitle();
                else
                    return MacroHelper.GetFirstname(GameManager.Instance.PlayerEntity.Name);
            }
        }

        #endregion
    }
}
