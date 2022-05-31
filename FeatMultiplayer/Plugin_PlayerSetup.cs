﻿using BepInEx;
using HarmonyLib;
using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FeatMultiplayer
{
    public partial class Plugin : BaseUnityPlugin
    {
        static int shadowInventoryWorldId = 50;
        static int shadowInventoryId;
        static int shadowEquipmentWorldId = 51;
        static int shadowEquipmentId;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GaugesConsumptionHandler), nameof(GaugesConsumptionHandler.GetThirstConsumptionRate))]
        static bool GaugesConsumptionHandler_GetThirstConsumptionRate(ref float __result)
        {
            __result = -0.0001f;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GaugesConsumptionHandler), nameof(GaugesConsumptionHandler.GetOxygenConsumptionRate))]
        static bool GaugesConsumptionHandler_GetOxygenConsumptionRate(ref float __result)
        {
            __result = -0.0001f;
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GaugesConsumptionHandler), nameof(GaugesConsumptionHandler.GetHealthConsumptionRate))]
        static bool GaugesConsumptionHandler_GetHealthConsumptionRate(ref float __result)
        {
            __result = -0.0001f;
            return false;
        }

        static void PrepareHiddenChests()
        {
            // The other player's shadow inventory
            if (TryPrepareHiddenChest(shadowInventoryWorldId, ref shadowInventoryId))
            {
                SetupInitialInventory();
            }
            TryPrepareHiddenChest(shadowEquipmentWorldId, ref shadowEquipmentId);
        }

        static bool TryPrepareHiddenChest(int id, ref int inventoryId)
        {
            WorldObject wo = WorldObjectsHandler.GetWorldObjectViaId(id);
            if (wo == null)
            {
                LogInfo("Creating special inventory " + id);

                wo = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("GROUP_DESC_Container2"), id);
                wo.SetPositionAndRotation(new Vector3(-500, -500, -450), Quaternion.identity);
                WorldObjectsHandler.InstantiateWorldObject(wo, false);
                Inventory inv = InventoriesHandler.CreateNewInventory(1000, 0);
                int invId = inv.GetId();
                inventoryId = invId;
                wo.SetLinkedInventoryId(invId);
                return true;
            }
            return false;
        }

        static void AddToShadowInventory(Dictionary<string, int> itemsToAdd)
        {
            var inv = InventoriesHandler.GetInventoryById(shadowInventoryId);
            foreach (var kv in itemsToAdd)
            {
                var gr = GroupsHandler.GetGroupViaId(kv.Key);
                if (gr != null)
                {
                    for (int i = 0; i < kv.Value; i++)
                    {
                        var wo = WorldObjectsHandler.CreateNewWorldObject(gr);
                        inv.AddItem(wo);
                    }
                }
                else
                {
                    LogWarning("SetupInitialInventory: Unknown groupId " + kv.Key);
                }
            }
        }

        static void SetupInitialInventory()
        {
            LogInfo("SetupInitialInventory");
            AddToShadowInventory(new()
            {
                { "MultiBuild", 1 },
                { "MultiDeconstruct", 1 },
                { "MultiToolLight", 1 },
                { "Iron", 10 },
                { "Magnesium", 10 },
                { "Silicon", 10 },
                { "Titanium", 10},
                { "Cobalt", 10 },
                { "BlueprintT1", 10 },
                { "OxygenCapsule1", 10 },
                { "WaterBottle1", 10 },
                { "astrofood", 10 }
            });
        }
    }
}