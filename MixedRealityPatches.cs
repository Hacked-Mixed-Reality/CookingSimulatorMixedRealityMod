using DebugOptions;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using Valve.VR;
using static MelonLoader.MelonLogger;

namespace MixedRealityMod
{
    public class MixedRealityPatches : MelonMod
    {

        [HarmonyPatch(typeof(Valve.VR.SteamVR_ExternalCamera), "OnEnable")]
        static class ExternalCameraOnEnablePatch
        {
            public static bool Prefix(ref SteamVR_ExternalCamera __instance)
            {
                MelonLogger.Msg("Fix Picasso");
                AccessTools.Method(null, "AutoEnableActionSet").Invoke(__instance, null);
                return false;
            }

        }

        [HarmonyPatch(typeof(AutoDarkeningController), "UpdateMe")]
        static class AutoDarkeningController_UpdateMePatch
        {
            public static bool Prefix()
            {

                return false;
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            FixLayers();
            base.OnSceneWasInitialized(buildIndex, sceneName);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            FixLayers();
            base.OnSceneWasLoaded(buildIndex, sceneName);
        }

        private static void FixLayers()
        {
            GameObject fade = GameObject.Find("CenterEyeAnchor");
            if (fade != null)
            {
                fade.layer = (int)GameLayer.Player;
            }

            SteamVR_ExternalCamera? instance = SteamVR_Render.instance?.externalCamera;

            if (instance != null)
            {
                var cam = AccessTools.FieldRefAccess<SteamVR_ExternalCamera, Camera>(instance, "cam");
                if (cam != null)
                {
                    cam.cullingMask = ModLayers.GetGameLayerMask();
                }
            }
        }

        public enum GameLayer
        {
            Default = 0,
            TransparentFX = 1,
            IgnoreRaycast = 2,
            Water = 4,
            UI = 5,
            EnviroUI = 8,
            Clickable = 9,
            Ingredient = 10,
            Player = 11,
            IngredientArea = 12,
            Liquids = 13,
            IngredientArea2 = 14,
            ItemInContainer = 15,
            Hintorial = 16,
            InCut = 17,
            BurnableTrigger = 18,
            IngredientAreaTrigger = 19,
            InMove = 20,
            Glass = 21,
            IngredientNotDefault = 22,
            WaterSurface = 23,
            OverUI = 24,
            FinalContainer = 25,
            PredictCollision = 26,
            DoorBreaker = 27,
            Doors = 28,
            GlassChunk = 29,
            Paintable = 30,
            IngredientWall = 31,
        }

        public class ModLayers
        {
            public static LayerMask GetGameLayerMask()
            {
                LayerMask layerMask = ~0;
                
                layerMask = Remove(layerMask, GameLayer.TransparentFX);
                layerMask = Remove(layerMask, GameLayer.IgnoreRaycast);
                layerMask = Remove(layerMask, GameLayer.Player);
                return layerMask;
            }

            private static LayerMask Remove(LayerMask layerMask, GameLayer layer)
            {
                return layerMask &= ~(1 << (int)layer);
            }
        }



        private void ShowDiagnostics()
        {
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam != null)
                {
                    MelonLogger.Msg("Camera {0} has mask {1}", cam.name, cam.cullingMask);
                }
                else
                {
                    MelonLogger.Msg("There's a null camera...");
                }
            }
            for (int i = 0; i < 32; i++)
            {
                MelonLogger.Msg("Layer {0}: {1}", i.ToString(), LayerMask.LayerToName(i));

            }
        }
    }
}