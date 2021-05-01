using Assets.Main.Scenes;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Map;
using Assets.Scripts.Models.Rounds;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Bridge;
using Harmony;
using MelonLoader;
using System.IO;
using UnhollowerBaseLib;
using Newtonsoft.Json;
using UnityEngine;
using Assets.Scripts.Utils;
using Assets.Scripts.Simulation.Bloons;
using Assets.Scripts.Simulation.Towers.Projectiles;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Unity.UI_New.InGame;

namespace slons
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {

            base.OnApplicationStart();
        }


        // roundset logging
        [HarmonyPatch(typeof(TitleScreen), "Start")]
        public class Awake_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                GameModel gm = Game.instance.model;
                if (gm == null) return;
                // normal roundsets
                Il2CppReferenceArray<RoundSetModel> roundSets = Game.instance.model.roundSets;
                int i = 0;
                foreach (RoundSetModel roundset in roundSets)
                {
                    //log(roundset);
                    i++;
                }
                // FBGM - freeplay bloon group model
                // freeplay stuffs
                Il2CppReferenceArray<FreeplayBloonGroupModel> e = Game.instance.model.freeplayGroups;
                int ii = 0;
                string path = "FBGM/";
                Directory.CreateDirectory(path);

                foreach (FreeplayBloonGroupModel fbgm in e)
                {
                    var g = fbgm.name;
                    FileIOUtil.SaveObject("FBGM2/" + g + ".json", fbgm); // I have no idea why when I remove this line emissions is empty, but when I remove everything else it just doesnt work
                    File.Create(path + g + ".json").Close();
                    File.WriteAllText(path + g + ".json", JsonUtility.ToJsonInternal(fbgm, true));
                    ii++;
                }
            }
        }

        public static void log(RoundSetModel roundset)
        {
            int j = 1;
            foreach (RoundModel roundModel in roundset.rounds)
            {
                //log(j + "-----------------------------------");
                string path = roundset.name + "/";
                Directory.CreateDirectory(path);
                log(roundModel.emissions, path, j);
                //log(roundModel.groups, path);
                j++;
            }
        }
        public static void log(Il2CppReferenceArray<BloonEmissionModel> bems, string path, int j)
        {
            string models = "[";
            foreach (var bem in bems)
            {
                //log(JsonUtility.ToJson(bem, false));
                //log(",");
                models += JsonUtility.ToJson(bem, false);
                models += ",";
            }
            models += "]";
            File.Create(path + j + ".txt").Close();
            File.WriteAllText(path + j + ".txt", models);
        }
        public static void log(string s)
        {
            MelonLogger.Msg(s);
        }

        // bloons info logging

        public override void OnUpdate()
        {

            base.OnUpdate();
            bool inAGame = InGame.instance != null && InGame.instance.bridge != null;
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                var bloons = InGame.instance.bridge.GetAllBloons();
                foreach (BloonToSimulation bts in bloons)
                {
                    var e = bts.Def;
                    FileIOUtil.SaveObject("e.txt", e);
                    string btss = JsonUtility.ToJson(e, true);
                    log(btss);
                }
                log(bloons._size.ToString());

            }
        }
    }
}