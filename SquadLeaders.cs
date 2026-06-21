using GHPC.State;
using GHPC.Infantry;
using GHPC.AI.Squads;
using GHPC.Vehicle;
using GHPC.AI.Platoons;
using UnityEngine;
using MelonLoader;
using SquadLeaders;
using System.Collections;
using System.IO;

[assembly: MelonInfo(typeof(SquadLeadersClass), "Squad Leaders", "1.1.1", "Bluehawk")]
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
        public static MelonPreferences_Entry<bool> mute_console;

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("Squad Leaders");
            soviet_skin = cfg.CreateEntry<string>("Soviet SL skin", "default");
            soviet_skin.Comment = "'default' for blank red straps, 'CA' - red straps with CA letters, 'black' - black straps with CA letters, 'field' - khaki straps";

            canadian_skin = cfg.CreateEntry<bool>("Canadian SL skin", false);
            canadian_skin.Comment = "switch to true for use with Canadian Leopards mod";

            mute_console = cfg.CreateEntry<bool>("Mute console", false);
            mute_console.Comment = "Mutes MelonLoader console messages except errors";
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

        public static Texture2D FetchTex(int x, int y, string path)
        {                     
            try
            {
                Texture2D temp = new Texture2D(x, y);
                byte[] data = File.ReadAllBytes(path);
                temp.LoadImage(data, true);
                return temp;
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); return null; }
            catch (System.Exception e) { MelonLogger.Error(e); return null; }            
        }

        public static void PromotePL(InfantryUnit leader, Texture2D SovietPL, Texture2D SovietPL_nm, Texture2D SovietPL_sm, 
            Material us_off_patch, Texture2D NVAPL, Texture2D BundPL)
        {
            if (leader.name.StartsWith("SA Obr73"))
            {
                SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/RED_OBR73_KHAKI/dress").GetComponent<SkinnedMeshRenderer>();                
                uniform.material.SetTexture("_Albedo", SovietPL);
                uniform.material.SetTexture("_Normal", SovietPL_nm);
                uniform.material.SetTexture("_Smoothness", SovietPL_sm);               

                leader.transform.Find("Troop Base/RED_OBR73_KHAKI/accoutrements").gameObject.SetActive(false);
                leader.transform.Find("Troop Base/RED_OBR73_KHAKI/webbing").gameObject.SetActive(false);
                if (!mute_console.Value) MelonLogger.Msg(leader.name + " promoted to leytenant");
            }
            else if (leader.name.StartsWith("US PASGT"))
            {
                Transform chest = leader.transform.Find("Troop Base/TRP_SKELETON/soldierHip/soldierSpine1/soldierSpine2/soldierSpine3/soldierChest");
                Transform helmet = chest.transform.Find("soldierNeck1/soldierNeck2/soldierHead");

                GameObject helmet_patch = new GameObject("helmet patch");
                helmet_patch.transform.parent = helmet;
                helmet_patch.transform.position = helmet.transform.position;
                NewQuad(helmet_patch, us_off_patch);
                helmet_patch.transform.localScale = new Vector3(0.007f, 0.0125f, 0.0125f);
                helmet_patch.transform.localPosition = new Vector3(-0.15f, 0.114f, 0f);
                helmet_patch.transform.localRotation = Quaternion.Euler(new Vector3(25f, 270f, 0f));

                GameObject left_collar_patch = new GameObject("left collar patch");
                left_collar_patch.transform.parent = chest;
                left_collar_patch.transform.position = chest.transform.position;
                NewQuad(left_collar_patch, us_off_patch);
                left_collar_patch.transform.localScale = new Vector3(0.007f, 0.015f, 0.015f);
                left_collar_patch.transform.localPosition = new Vector3(-0.125f, 0.085f, 0.1f); //-0.13, 0.076, 0.1 //-0.0135, 0.065, 0.1
                left_collar_patch.transform.localRotation = Quaternion.Euler(new Vector3(20f, 215f, 300f));

                GameObject right_collar_patch = new GameObject("right collar patch");
                right_collar_patch.transform.parent = chest;
                right_collar_patch.transform.position = chest.transform.position;
                NewQuad(right_collar_patch, us_off_patch);
                right_collar_patch.transform.localScale = new Vector3(0.007f, 0.015f, 0.015f);
                right_collar_patch.transform.localPosition = new Vector3(-0.135f, 0.075f, -0.095f);
                right_collar_patch.transform.localRotation = Quaternion.Euler(new Vector3(20f, 330f, 45f));

                if (!mute_console.Value) MelonLogger.Msg(leader.name + " promoted to 1st lieutenant");
            }
            else if (leader.name.StartsWith("NVA KAZ64"))
            {
                SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/RED_KAZ64_STRICH/dress").GetComponent<SkinnedMeshRenderer>();
                uniform.material.SetTexture("_Albedo", NVAPL);
                if (!mute_console.Value) MelonLogger.Msg(leader.name + " promoted to Leutnant");
            }
            else if (leader.name.StartsWith("BW Feldanzug"))
            {
                SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/BLU_FAZ63_OLIVE/dress").GetComponent<SkinnedMeshRenderer>();
                uniform.material.SetTexture("_Albedo", BundPL);
                if (!mute_console.Value) { 
                    if (canadian_skin.Value) MelonLogger.Msg(leader.name + "(CF) promoted to lieutenant"); else MelonLogger.Msg(leader.name + " promoted to Oberleutnant");
                }
            }
        }

        public static void PromoteSL(InfantryUnit leader, Material us_patch, Texture2D SovietSL, Texture2D BundSL, Texture2D NVASL, bool PlatoonSgt)
        {
            if (leader.name.StartsWith("SA Obr73"))
            {
                SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/RED_OBR73_KHAKI/dress").GetComponent<SkinnedMeshRenderer>();
                uniform.material.SetTexture("_Albedo", SovietSL);
                if (!mute_console.Value) { 
                        if (PlatoonSgt) { MelonLogger.Msg(leader.name + " promoted to starshiy serzhant"); } else MelonLogger.Msg(leader.name + " promoted to serzhant");
                }
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
                left_collar_patch.transform.localScale = new Vector3(0.008f, 0.015f, 0.015f);
                left_collar_patch.transform.localPosition = new Vector3(-0.125f, 0.085f, 0.1f); //-0.13, 0.076, 0.1 //-0.0135, 0.065, 0.1
                left_collar_patch.transform.localRotation = Quaternion.Euler(new Vector3(20f, 215f, 300f));

                GameObject right_collar_patch = new GameObject("right collar patch");
                right_collar_patch.transform.parent = chest;
                right_collar_patch.transform.position = chest.transform.position;
                NewQuad(right_collar_patch, us_patch);
                right_collar_patch.transform.localScale = new Vector3(0.008f, 0.015f, 0.015f);
                right_collar_patch.transform.localPosition = new Vector3(-0.135f, 0.075f, -0.095f);
                right_collar_patch.transform.localRotation = Quaternion.Euler(new Vector3(20f, 330f, 45f));

                if (!mute_console.Value)
                {
                    if (PlatoonSgt) { MelonLogger.Msg(leader.name + " promoted to sergeant first class"); } else MelonLogger.Msg(leader.name + " promoted to staff-sergeant");
                }
            }
            else if (leader.name.StartsWith("NVA KAZ64"))
            {
                SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/RED_KAZ64_STRICH/dress").GetComponent<SkinnedMeshRenderer>();
                uniform.material.SetTexture("_Albedo", NVASL);
                if (!mute_console.Value)
                {
                    if (PlatoonSgt) { MelonLogger.Msg(leader.name + " promoted to Feldwebel"); } else MelonLogger.Msg(leader.name + " promoted to Unteroffizier");
                }
            }
            else if (leader.name.StartsWith("BW Feldanzug"))
            {
                SkinnedMeshRenderer uniform = leader.transform.Find("Troop Base/BLU_FAZ63_OLIVE/dress").GetComponent<SkinnedMeshRenderer>();
                uniform.material.SetTexture("_Albedo", BundSL);
                if (!mute_console.Value)
                {
                    if (PlatoonSgt)
                    {
                        if (canadian_skin.Value) MelonLogger.Msg(leader.name + "(CF) promoted to warrant officer"); else MelonLogger.Msg(leader.name + " promoted to Oberfeldwebel");
                    }
                    else
                    {
                        if (canadian_skin.Value) MelonLogger.Msg(leader.name + "(CF) promoted to sergeant"); else MelonLogger.Msg(leader.name + " promoted to Stabsunteroffizier");
                    }
                }
            }
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
            string SovietSLPath;
            string SovietSrSgtPath;
            switch (soviet_skin.Value) { 
                case "CA":
                case "ca":
                case "Ca":
                    SovietSLPath = "Mods/SquadLeaders/SovietSL_CA.png";
                    SovietSrSgtPath = "Mods/SquadLeaders/SovietSrSgt_CA.png";
                    break;
                case "black":
                case "Black":
                case "BLACK":
                    SovietSLPath = "Mods/SquadLeaders/SovietSL_Black.png";
                    SovietSrSgtPath = "Mods/SquadLeaders/SovietSrSgt_Black.png";
                    break;
                case "field":
                case "Field":
                case "FIELD":
                    SovietSLPath = "Mods/SquadLeaders/SovietSL_Field.png";
                    SovietSrSgtPath = "Mods/SquadLeaders/SovietSrSgt_Field.png";
                    break;
                default:
                    SovietSLPath = "Mods/SquadLeaders/SovietSL.png";
                    SovietSrSgtPath = "Mods/SquadLeaders/SovietSrSgt.png";
                    break;
            }
            Texture2D SovietSL = FetchTex(1024, 1024, SovietSLPath);             
            Texture2D SovietSrSgt = FetchTex(1024, 1024, SovietSrSgtPath);

            string SovietPLPath;
            if (soviet_skin.Value == "black" || soviet_skin.Value == "Black" || soviet_skin.Value == "BLACK") 
            { SovietPLPath = "Mods/SquadLeaders/SovietPL_Black.png"; } else { SovietPLPath = "Mods/SquadLeaders/SovietPL.png"; }
            Texture2D SovietPL = FetchTex(1024, 1024, SovietPLPath);

            string SovietPL_nm_Path = "Mods/SquadLeaders/SovietPL_nm.png";
            Texture2D SovietPL_nm = new Texture2D(1024, 1024, TextureFormat.DXT5, true, true);
            try
            {
                byte[] data = File.ReadAllBytes(SovietPL_nm_Path);
                SovietPL_nm.LoadImage(data, true);                
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e);}

            string SovietPL_sm_Path = "Mods/SquadLeaders/SovietPL_sm.png";
            Texture2D SovietPL_sm = new Texture2D(1024, 1024, TextureFormat.DXT5, true, true);
            try
            {
                byte[] data = File.ReadAllBytes(SovietPL_sm_Path);
                SovietPL_sm.LoadImage(data, true);                
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); }

            string AmericanSLPath = "Mods/SquadLeaders/US_SSGT.png";
            Texture2D AmericanSL = FetchTex(353, 444, AmericanSLPath);
            Material us_patch = new Material(Shader.Find("Standard (FLIR)"));
            us_patch.mainTexture = AmericanSL;

            string AmericanSFCPath = "Mods/SquadLeaders/US_SFC.png";
            Texture2D AmericanSFC = FetchTex(353, 444, AmericanSFCPath);
            Material us_patch_sfc = new Material(Shader.Find("Standard (FLIR)"));
            us_patch_sfc.mainTexture = AmericanSFC;

            string AmericanPLPath = "Mods/SquadLeaders/US_LT.png";
            Texture2D AmericanPL = FetchTex(178, 416, AmericanPLPath);
            Material us_off_patch = new Material(Shader.Find("Standard (FLIR)"));
            us_off_patch.mainTexture = AmericanPL;            

            string NVASLPath = "Mods/SquadLeaders/NVASL.png";
            Texture2D NVASL = FetchTex(1024, 1024, NVASLPath);

            string NVAFeldPath = "Mods/SquadLeaders/NVAFeld.png";
            Texture2D NVAFeld = FetchTex(1024, 1024, NVAFeldPath);

            string NVAPLPath = "Mods/SquadLeaders/NVAPL.png";
            Texture2D NVAPL = FetchTex(1024, 1024, NVAPLPath);

            string BundSLPath = (canadian_skin.Value) ? "Mods/SquadLeaders/CanSL.png" : "Mods/SquadLeaders/BundSL.png";
            Texture2D BundSL = FetchTex(1024, 1024, BundSLPath);

            string BundFeldPath = (canadian_skin.Value) ? "Mods/SquadLeaders/CanWarrant.png" : "Mods/SquadLeaders/BundFeld.png";
            Texture2D BundFeld = FetchTex(1024, 1024, BundFeldPath);

            string BundPLPath = (canadian_skin.Value) ? "Mods/SquadLeaders/CanPL.png" : "Mods/SquadLeaders/BundPL.png";
            Texture2D BundPL = FetchTex(1024, 1024, BundPLPath);
            
            InfantryEmplacementHolder[] emplacements = GameObject.FindObjectsByType<InfantryEmplacementHolder>(FindObjectsSortMode.None);
            foreach (var emplacement in emplacements)
            {
                if (emplacement.transform.parent == null) { continue; }
                GameObject trench_holder = emplacement.transform.parent.gameObject;
                InfantryUnit[] troopsInTrench = trench_holder.GetComponentsInChildren<InfantryUnit>();
                int squadCount = troopsInTrench.Length / 6;
                if (squadCount > 0) {
                    for (int i = 0; i < squadCount; i++) { 

                        if (troopsInTrench[i + (i * 5)].gameObject.GetComponent<SergeantPromoted>() == null)
                        {
                            if (i == 0 && squadCount >= 3) {
                                PromotePL(troopsInTrench[0], SovietPL, SovietPL_nm, SovietPL_sm, us_off_patch, NVAPL, BundPL);
                                PromoteSL(troopsInTrench[1], us_patch_sfc, SovietSrSgt, BundFeld, NVAFeld, true);
                            } 
                            else { PromoteSL(troopsInTrench[i + (i * 5)], us_patch, SovietSL, BundSL, NVASL, false); }
                            troopsInTrench[i + (i * 5)].gameObject.AddComponent<SergeantPromoted>();
                        }
                    }
                }
                else if (troopsInTrench[0].gameObject.GetComponent<SergeantPromoted>() == null)
                {
                    PromoteSL(troopsInTrench[0], us_patch, SovietSL, BundSL, NVASL, false);
                    troopsInTrench[0].gameObject.AddComponent<SergeantPromoted>();
                }
            }

            SquadData[] squads = GameObject.FindObjectsByType<SquadData>(FindObjectsSortMode.None);
            foreach (SquadData squad in squads)
            {
                if (squad.Leader == null) { continue; }
                if (squad.Leader.Emplacement != null && squad.InfantryManager == null) { continue; }
                InfantryUnit leader = squad.Leader;                
                if (squad.InfantryManager != null)
                {                    
                    Vehicle carrier = squad.InfantryManager.gameObject.GetComponent<Vehicle>();
                    PlatoonData platoon = carrier.Platoon;
                    if (platoon != null && carrier == platoon.Units[0])
                    {                        
                        if (leader.gameObject.GetComponent<SergeantPromoted>() != null) { continue; }
                        leader.gameObject.AddComponent<SergeantPromoted>();
                        PromotePL(leader, SovietPL, SovietPL_nm, SovietPL_sm, us_off_patch, NVAPL, BundPL);
                        PromoteSL(squad.Units[1], us_patch_sfc, SovietSrSgt, BundFeld, NVAFeld, true);
                    }
                }
                
                if (leader.gameObject.GetComponent<SergeantPromoted>() != null) { continue; }
                leader.gameObject.AddComponent<SergeantPromoted>();

                PromoteSL(leader, us_patch, SovietSL, BundSL, NVASL, false);
            }            
            yield break;
        }
    }
}
