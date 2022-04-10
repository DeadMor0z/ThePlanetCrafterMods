﻿using BepInEx;
using UnityEngine;
using BepInEx.Bootstrap;
using System.Reflection;
using System;
using HarmonyLib;
using SpaceCraft;
using System.Collections.Generic;
using BepInEx.Logging;
using MijuTools;
using System.Text;

namespace ExampleModLoadSaveSupportSoft
{
    [BepInPlugin(guid, "(Example) Soft Dependency on ModLoadSaveSupport", "1.0.0.1")]
    [BepInDependency(libModLoadSaveSupportGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        const string libModLoadSaveSupportGuid = "akarnokd.theplanetcraftermods.libmodloadsavesupport";
        const string guid = "akarnokd.theplanetcraftermods.examplemodloadsavesupportsoft";

        private IDisposable handle;

        static ManualLogSource logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin is loaded!");

            // Locate the libModLoadSaveSupport plugin
            if (Chainloader.PluginInfos.TryGetValue(libModLoadSaveSupportGuid, out BepInEx.PluginInfo pi))
            {
                // locate its RegisterLoadSave method
                MethodInfo mi = pi.Instance.GetType().GetMethod("RegisterLoadSave",
                    new Type[] { typeof(string), typeof(Action<string>), typeof(Func<string>) });

                // call it with our guid and the delegates to our load and save methods
                handle = (IDisposable)mi.Invoke(pi.Instance, new object[] { guid, new Action<string>(OnLoad), new Func<string>(OnSave) });

                Logger.LogInfo("Successfully registered with " + libModLoadSaveSupportGuid);
            } else
            {
                Logger.LogInfo("Could not find " + libModLoadSaveSupportGuid);
            }

            logger = Logger;

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        void OnDestroy()
        {
            handle?.Dispose();
            handle = null;
        }

        void OnLoad(string content)
        {
            Logger.LogInfo("Executing OnLoad");
            Logger.LogInfo(content);
        }

        string OnSave()
        {
            Logger.LogInfo("Executing OnSave");
            return "ExampleModLoadSaveSupportSoft example content";
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TerraformStagesHandler), "Start")]
        static void TerraformStagesHandler_Start(List<TerraformStage> ___allGlobalTerraStage)
        {
            foreach (TerraformStage stage in ___allGlobalTerraStage)
            {
                logger.LogInfo(stage.GetTerraId() + " \"" + Readable.GetTerraformStageName(stage) + "\" @ " 
                    + string.Format("{0:##,###}", stage.GetStageStartValue()) + " " + stage.GetWorldUnitType());                
            }

            UnlockingHandler unlock = Managers.GetManager<UnlockingHandler>();

            List<List<GroupData>> tiers = new List<List<GroupData>>
            {
                unlock.tier1GroupToUnlock,
                unlock.tier2GroupToUnlock,
                unlock.tier3GroupToUnlock,
                unlock.tier4GroupToUnlock,
                unlock.tier5GroupToUnlock,
                unlock.tier6GroupToUnlock,
                unlock.tier7GroupToUnlock,
                unlock.tier8GroupToUnlock,
                unlock.tier9GroupToUnlock,
                unlock.tier10GroupToUnlock,
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n");

            for (int i = 0; i < tiers.Count; i++)
            {
                List<GroupData> gd = tiers[i];
                sb.Append("Tier #").Append(i + 1).Append("\r\n");

                foreach (GroupData g in gd)
                {
                    sb.Append("- ").Append(g.id).Append("\r\n");
                }
            }

            logger.LogInfo(sb.ToString());
        }
    }
}
