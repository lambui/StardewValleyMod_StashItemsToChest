using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace StashItemsToChest
{
    public class StashItemsToChest : Mod
    {
        public static StashItemsToChestConfig ModConfig { get; protected set; }
        public override void Entry(params object[] objects)
        {
            ModConfig = new StashItemsToChestConfig().InitializeConfig(BaseConfigPath);
            StardewModdingAPI.Events.GameEvents.UpdateTick += UpdateTickEvent;
        }

        //PhthaloBlue: these blocks of codes below are from Chest Pooling mod by mralbobo
        //repo link here: https://github.com/mralbobo/stardew-chest-pooling, they are useful so I use them
        static StardewValley.Objects.Chest getOpenChest()
        {
            if (StardewValley.Game1.activeClickableMenu == null) { return null; }

            if (StardewValley.Game1.activeClickableMenu is StardewValley.Menus.ItemGrabMenu)
            {
                //myLog("it's an item grab");
                StardewValley.Menus.ItemGrabMenu menu = StardewValley.Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu;
                if (menu.behaviorOnItemGrab != null && menu.behaviorOnItemGrab.Target is StardewValley.Objects.Chest)
                {
                    return menu.behaviorOnItemGrab.Target as StardewValley.Objects.Chest;
                }
            }
            else
            {
                
                if (StardewValley.Game1.activeClickableMenu.GetType().Name == "ACAMenu")
                {
                    dynamic thing = (dynamic)StardewValley.Game1.activeClickableMenu;
                    if (thing != null && thing.chestItems != null)
                    {
                      
                        StardewValley.Objects.Chest aChest = new StardewValley.Objects.Chest(true);
                        aChest.items = thing.chestItems;
                        return aChest;
                    }
                }

                //debugThing(StardewValley.Game1.activeClickableMenu);
            }
            return null;
        }


        //PhthaloBlue: these are my codes
        static void UpdateTickEvent(object sender, EventArgs e)
        {
            if (ModConfig == null)
                return;

            if (StardewValley.Game1.currentLocation == null)
                return;

            KeyboardState currentKeyboardState = Keyboard.GetState();
            StashUp(currentKeyboardState);
        }

        static bool isChestFull(StardewValley.Objects.Chest inputChest)
        {
            return inputChest.items.Count >= StardewValley.Objects.Chest.capacity;
        }

        static void StashUp(KeyboardState currentKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(ModConfig.stashKey))
            {
                List<Item> PlayerInventory = Game1.player.items;
                StardewValley.Objects.Chest OpenChest = getOpenChest();
                if (OpenChest == null)
                    return;

                if (OpenChest.isEmpty())
                    return;

                List<Item> OpenChestItemList = OpenChest.items;
                foreach (Item chestItem in OpenChestItemList)
                {
                    foreach (Item playerItem in PlayerInventory)
                    {
                        if (playerItem == null)
                            continue;

                        if (playerItem.canStackWith(chestItem))
                        {
                            OpenChest.grabItemFromInventory(playerItem, Game1.player);
                        }
                    }
                }

                
            }
        }
    }


    //PhthaloBlue: these simple config code I learned from looking thru SprintAndDash mod by speedy.
    //repo link here: https://gitlab.com/speeder1/SPDSprintAndDashMod
    public class StashItemsToChestConfig : Config
    {
        public Keys stashKey;

        public override T GenerateDefaultConfig<T>()
        {
            stashKey = Keys.Tab;
            return this as T;
        }
    }
}
