using GHPC.State;
using GHPC.Infantry;
using GHPC.AI.Squads;
using UnityEngine;
using MelonLoader;
using SquadLeaders;
using System.Collections;
using System.IO;

[assembly: MelonInfo(typeof(SquadLeadersClass), "Squad Leaders", "1.0.0", "Bluehawk")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace SquadLeaders
{
    public class SergeantPromoted : MonoBehaviour
    {
        void Awake()
        {
            enabled = false;
        }
    }
    public class SquadLeadersClass : MelonMod
    {
        public static GameObject gameManager;
        public static MelonPreferences_Entry<string> soviet_skin;
        public static MelonPreferences_Entry<bool> canadian_skin;

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("Squad Leaders");
            soviet_skin = cfg.CreateEntry<string>("Soviet SL skin", "default");
            soviet_skin.Comment = "'default' for blank red straps, 'CA' red straps with CA letters, 'black' for black straps with CA letters, 'field' for subdued field straps";

            canadian_skin = cfg.CreateEntry<bool>("Canadian SL skin", false);
            canadian_skin.Comment = "switch to true for use with Canadian Leopards mod";
        }

        public static void NewQuad(GameObject go, Material mat)
        {
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            filter.mesh = new Mesh();
            filter.mesh.vertices = new Vector3[] {
                            new Vector3(1f, 0 , 1f), new Vector3(1f, 0, -1f), new Vector3(-1f, 0, 1f), new Vector3(-1f, 0, -1f) };
            filter.mesh.uv = new Vector2[] {
                            new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) };
            filter.mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            filter.mesh.RecalculateNormals();
            render.material = mat;            
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene")
            {
                return;
            }

            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(SquadLeaders), GameStatePriority.Medium);
        }

        public IEnumerator SquadLeaders(GameState _)
        {
            Texture2D SovietSL = new Texture2D(1024, 1024);
            string SovietSLPath;
            switch (soviet_skin.Value) { 
                case "CA":
                case "ca":
                case "Ca":
                    SovietSLPath = "Mods/SquadLeaders/SovietSL_CA.png";
                    break;
                case "black":
                case "Black":
                case "BLACK":
                    SovietSLPath = "Mods/SquadLeaders/SovietSL_Black.png";
                    break;
                case "field":
                case "Field":
                case "FIELD":
                    SovietSLPath = "Mods/SquadLeaders/SovietSL_Field.png";
                    break;
                default:
                    SovietSLPath = "Mods/SquadLeaders/SovietSL.png";
                    break;
            }            
            byte[] SovietSLData = File.ReadAllBytes(SovietSLPath);
            if (SovietSLData != null) { SovietSL.LoadImage(SovietSLData, true); }
            else { MelonLogger.Msg("Wanted texture file at " + SovietSLPath + " missing!"); }

            Texture2D AmericanSL = new Texture2D(353, 444);            
            string AmericanSLPath = "Mods/SquadLeaders/US_SSGT.png";
            byte[] AmericanSLData = File.ReadAllBytes(AmericanSLPath);
            if (AmericanSLData != null) { AmericanSL.LoadImage(AmericanSLData, true); }
            else { MelonLogger.Msg("Wanted texture file at " + AmericanSLPath + " missing!"); }
            Material us_patch = new Material(Shader.Find("Standard (FLIR)"));
            us_patch.mainTexture = AmericanSL;

            Texture2D NVASL = new Texture2D(1024, 1024);
            string NVASLPath = "Mods/SquadLeaders/NVASL.png";
            byte[] NVASLData = File.ReadAllBytes(NVASLPath);
            if (NVASLData != null) { NVASL.LoadImage(NVASLData, true); }
            else { MelonLogger.Msg("Wanted texture file at " + NVASLPath + " missing!"); }

            Texture2D BundSL = new Texture2D(1024, 1024);
            string BundSLPath = (canadian_skin.Value) ? "Mods/SquadLeaders/CanSL.png" : "Mods/SquadLeaders/BundSL.png";
            byte[] BundSLData = File.ReadAllBytes(BundSLPath);
            if (BundSLData != null) { BundSL.LoadImage(BundSLData, true); }
            else { MelonLogger.Msg("Wanted texture file at " + BundSLPath + " missing!"); }            

            SquadData[] squads = GameObject.FindObjectsByType<SquadData>(FindObjectsSortMode.None);
            foreach (SquadData squad in squads)
            {
                if (squad.Leader == null) continue;
                InfantryUnit leader = squad.Leader;
                if (leader.gameObject.GetComponent<SergeantPromoted>() != null) { continue; }
                leader.gameObject.AddComponent<SergeantPromoted>();

                if (leader.name.StartsWith("SA Obr73")) {
                    SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/RED_OBR73_KHAKI/dress").GetComponent<SkinnedMeshRenderer>();
                    uniform.material.SetTexture("_Albedo", SovietSL);                    
                    MelonLogger.Msg(leader.name + " promoted to serzhant");
                }
                else if (leader.name.StartsWith("US PASGT"))
                {                    
                    Transform chest = leader.transform.Find("Troop Base/TRP_SKELETON/soldierHip/soldierSpine1/soldierSpine2/soldierSpine3/soldierChest");
                    Transform helmet = chest.transform.Find("soldierNeck1/soldierNeck2/soldierHead");                    

                    GameObject helmet_patch = new GameObject("helmet patch");
                    helmet_patch.transform.parent = helmet;
                    helmet_patch.transform.position = helmet.transform.position;                    
                    NewQuad(helmet_patch, us_patch);
                    helmet_patch.transform.localScale = new Vector3(0.01f, 0.0125f, 0.0125f);
                    helmet_patch.transform.localPosition = new Vector3(-0.15f, 0.114f, 0f);
                    helmet_patch.transform.localRotation = Quaternion.Euler(new Vector3(25f, 270f, 0f));                    

                    GameObject left_collar_patch = new GameObject("left collar patch");
                    left_collar_patch.transform.parent = chest;
                    left_collar_patch.transform.position = chest.transform.position;
                    NewQuad(left_collar_patch, us_patch);
                    left_collar_patch.transform.localScale = new Vector3(0.011f, 0.015f, 0.015f);
                    left_collar_patch.transform.localPosition = new Vector3(-0.125f, 0.085f, 0.1f); //-0.13, 0.076, 0.1 //-0.0135, 0.065, 0.1
                    left_collar_patch.transform.localRotation = Quaternion.Euler(new Vector3(20f, 215f, 300f));

                    GameObject right_collar_patch = new GameObject("right collar patch");
                    right_collar_patch.transform.parent = chest;
                    right_collar_patch.transform.position = chest.transform.position;
                    NewQuad(right_collar_patch, us_patch);
                    right_collar_patch.transform.localScale = new Vector3(0.011f, 0.015f, 0.015f);
                    right_collar_patch.transform.localPosition = new Vector3(-0.135f, 0.075f, -0.095f);
                    right_collar_patch.transform.localRotation = Quaternion.Euler(new Vector3(20f, 330f, 45f));

                    MelonLogger.Msg(leader.name + " promoted to staff-sergeant");
                }
                else if (leader.name.StartsWith("NVA KAZ64"))
                {
                    SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/RED_KAZ64_STRICH/dress").GetComponent<SkinnedMeshRenderer>();
                    uniform.material.SetTexture("_Albedo", NVASL);
                    MelonLogger.Msg(leader.name + " promoted to Unteroffizier");
                }
                else if (leader.name.StartsWith("BW Feldanzug"))
                {
                    SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/BLU_FAZ63_OLIVE/dress").GetComponent<SkinnedMeshRenderer>();
                    uniform.material.SetTexture("_Albedo", BundSL);
                    if (canadian_skin.Value) MelonLogger.Msg(leader.name + "(CF) promoted to sergeant"); else MelonLogger.Msg(leader.name + " promoted to Stabsunteroffizier");
                }                
            }            
            yield break;
        }
    }
}
