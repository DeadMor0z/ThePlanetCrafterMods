﻿using BepInEx;
using SpaceCraft;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MijuTools;

namespace UIPhotomodeHideWater
{
    [BepInPlugin("akarnokd.theplanetcraftermods.uiphotomodehidewater", "(Cheat) Hide Water in Photomode", "1.0.0.3")]
    public class Plugin : BaseUnityPlugin
    {

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin is loaded!");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(VisualsToggler), nameof(VisualsToggler.ToggleUi))]
        static void VisualsToggler_ToggleUi(List<GameObject> ___uisToHide)
        {
            bool active = ___uisToHide[0].activeSelf;
            if (Keyboard.current[Key.LeftShift].isPressed)
            {
                foreach (GameObject gameObject2 in Managers.GetManager<WaterHandler>().waterVolumes)
                {
                    gameObject2.SetActive(active);
                }

                foreach (GameObject gameObject2 in FindObjectsOfType<GameObject>(true))
                {
                    if (gameObject2.name == "Water")
                    {
                        gameObject2.SetActive(active);
                    }
                }
            }
        }
    }
}
