// Autochrome
// a Valheim mod skeleton using Jötunn
// 
// File:    Autochrome.cs
// Project: Autochrome

using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using BlacksmithTools;

namespace Autospace {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]

    internal class Autoheim : BaseUnityPlugin {
        public const string PluginGUID = "com.autochrome.Autoheim";
        public const string PluginName = "Autoheim";
        public const string PluginVersion = "0.0.1";
        
        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        // public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();


        public readonly string modFolder;
        private AssetBundle assetBundle;
        
        private static List<string> raceNames = new List<string>();
        private static Dictionary<Humanoid, Equipment> currentRaceItemsByHumanoid = new Dictionary<Humanoid, Equipment>();
        private static Dictionary<string, GameObject> femalePrefabs = new Dictionary<string, GameObject>();

        private void Awake() { }

        public Autoheim() {
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("Autoheim ping!");
        }

        public void Start() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony("Autoheim_BeastTribesFix").PatchAll(assembly);
            
            assetBundle = AssetBundle.LoadFromStream(
                assembly.GetManifestResourceStream(
                    assembly.GetName().Name + ".Resources.beastraces"));

            string[] tribes = { "Lizard", "Fox" };
            (string, int)[][] CR = new (string, int)[][] {
                new (string, int)[] {("LizardTail", 5), ("LeatherScraps", 2)},
                new (string, int)[] {("Raspberry", 5), ("LeatherScraps", 2)}
            };
            for (int i = 0; i < tribes.Length; i++)
                AddTribe(tribes[i], CR[i]);
            AddLynx();
            AddPanther();
            AddYellowPanther();
            AddWolf();
            AddGreyFox();
            AddSnowleopard();

            Jotunn.Logger.LogInfo("Loaded beast tribes mod");
        }

        private delegate ItemDrop fetch(string name);

        private void AddItem(
          string name,
          string title,
          string description,
          StatusEffect statusEffect,
          List<Piece.Requirement> craftReqs) {
            GameObject gameObject = assetBundle.LoadAsset<GameObject>(name);
            gameObject.layer = 12;

            CustomItem ci = new CustomItem(gameObject, true);
            var sData = ci.ItemDrop.m_itemData.m_shared;
            statusEffect.m_icon = sData.m_icons[0];
            sData.m_equipStatusEffect = statusEffect;
            sData.m_itemType = (ItemDrop.ItemData.ItemType)100;
            sData.m_maxQuality = 1;
            sData.m_armorPerLevel = 0.0f;
            ItemManager.Instance.AddItem(ci);

            Recipe rec = ScriptableObject.CreateInstance<Recipe>();
            craftReqs.Insert(0, MockRequirement.Create("LinenThread", 0, true));
            craftReqs[0].m_amountPerLevel = 1;
            rec.m_resources = craftReqs.ToArray();
            rec.name = name;
            rec.m_craftingStation = Mock<CraftingStation>.Create("piece_workbench");
            rec.m_minStationLevel = 1;
            rec.m_item = ci.ItemDrop;
            ItemManager.Instance.AddRecipe(new CustomRecipe(rec, true, true));
            
            //Language.AddToken("$" + name, title, true);
            //Language.AddToken("$" + name + "_Desc", description, true);
            raceNames.Add(name);
        }

        private void AddTribe(string name, (string, int)[] recShort) {
            SE_Stats seStats = new SE_Stats();
            seStats.m_name = name;
            seStats.m_category = "Tribe";
            seStats.m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftReqs = new List<Piece.Requirement>();
            foreach (var (thing, num) in recShort)
                craftReqs.Add(MockRequirement.Create(thing, num, true));

            AddItem(name, name + " Skin",
                "Turn into a " + name + "to gain carry weight",
                seStats, craftReqs);
            
        }

        private void AddLynx()
        {
            SE_Stats seStats = new SE_Stats();
            ((StatusEffect)seStats).m_name = "Lynx";
            ((StatusEffect)seStats).m_category = "Tribe";
            ((StatusEffect)seStats).m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftRequirements = new List<Piece.Requirement>();
            craftRequirements.Add(MockRequirement.Create("Dandelion", 5, true));
            craftRequirements.Add(MockRequirement.Create("LeatherScraps", 2, true));
            this.AddItem("lynx", "Lynx skin", "Turn into a lynx to gain carry weight", (StatusEffect)seStats, craftRequirements);
        }

        private void AddPanther()
        {
            SE_Stats seStats = new SE_Stats();
            ((StatusEffect)seStats).m_name = "Panther";
            ((StatusEffect)seStats).m_category = "Tribe";
            ((StatusEffect)seStats).m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftRequirements = new List<Piece.Requirement>();
            craftRequirements.Add(MockRequirement.Create("Coal", 5, true));
            craftRequirements.Add(MockRequirement.Create("LeatherScraps", 2, true));
            this.AddItem("panther", "Panther skin", "Turn into a panther to gain carry weight", (StatusEffect)seStats, craftRequirements);
        }

        private void AddYellowPanther()
        {
            SE_Stats seStats = new SE_Stats();
            ((StatusEffect)seStats).m_name = "Leopard";
            ((StatusEffect)seStats).m_category = "Tribe";
            ((StatusEffect)seStats).m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftRequirements = new List<Piece.Requirement>();
            craftRequirements.Add(MockRequirement.Create("Resin", 5, true));
            craftRequirements.Add(MockRequirement.Create("LeatherScraps", 2, true));
            this.AddItem("panther_yellow", "Leopard skin", "Turn into a leopard to gain carry weight", (StatusEffect)seStats, craftRequirements);
        }

        private void AddWolf()
        {
            SE_Stats seStats = new SE_Stats();
            ((StatusEffect)seStats).m_name = "Wolf";
            ((StatusEffect)seStats).m_category = "Tribe";
            ((StatusEffect)seStats).m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftRequirements = new List<Piece.Requirement>();
            craftRequirements.Add(MockRequirement.Create("RawMeat", 5, true));
            craftRequirements.Add(MockRequirement.Create("LeatherScraps", 2, true));
            this.AddItem("wolf", "Wolf skin", "Turn into a Wolf to gain carry weight", (StatusEffect)seStats, craftRequirements);
        }

        private void AddGreyFox()
        {
            SE_Stats seStats = new SE_Stats();
            ((StatusEffect)seStats).m_name = "Greyfox";
            ((StatusEffect)seStats).m_category = "Tribe";
            ((StatusEffect)seStats).m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftRequirements = new List<Piece.Requirement>();
            craftRequirements.Add(MockRequirement.Create("Blueberries", 5, true));
            craftRequirements.Add(MockRequirement.Create("LeatherScraps", 2, true));
            this.AddItem("greyfox", "Greyfox skin", "Turn into a Greyfox to gain carry weight", (StatusEffect)seStats, craftRequirements);
        }

        private void AddSnowleopard()
        {
            SE_Stats seStats = new SE_Stats();
            ((StatusEffect)seStats).m_name = "Snowleopard";
            ((StatusEffect)seStats).m_category = "Tribe";
            ((StatusEffect)seStats).m_tooltip = "Increase max carry weight by 50";
            seStats.m_addMaxCarryWeight = 50f;
            List<Piece.Requirement> craftRequirements = new List<Piece.Requirement>();
            craftRequirements.Add(MockRequirement.Create("Feathers", 5, true));
            craftRequirements.Add(MockRequirement.Create("LeatherScraps", 2, true));
            this.AddItem("snowleopard", "Snowleopard skin", "Turn into a Snowleopard to gain carry weight", (StatusEffect)seStats, craftRequirements);
        }

        // This returns null if the specified equipment
        //   is not a visEquipment component of anything in the
        //   currentRaceItemsByHumanoid map.
        // It also removes any equipment in this list for which
        //   the humanoid component is null.
        public static Equipment GetRaceItem(VisEquipment visEquipment) {
            List<Humanoid> humanoidList = new List<Humanoid>();
            Equipment raceItem = null;
            foreach (Equipment equipment in currentRaceItemsByHumanoid.Values)
            {
                if (equipment.Humanoid == null) //should this be not equals?
                    humanoidList.Add(equipment.Humanoid); //this will add null?
                else if (equipment._visEquipment == visEquipment)
                    raceItem = equipment;
            } // i think this only removes the element at the null key...
            foreach (Humanoid key in humanoidList)
                currentRaceItemsByHumanoid.Remove(key);
            return raceItem;
        }

        public static Equipment GetRaceItemByHumanoid(Humanoid humanoid)
        {
            if (!humanoid.IsPlayer()) return null;
            if (!currentRaceItemsByHumanoid.ContainsKey(humanoid)) {
                currentRaceItemsByHumanoid.Add(humanoid, new Equipment());
                currentRaceItemsByHumanoid[humanoid].Humanoid = humanoid;
            }
            return currentRaceItemsByHumanoid[humanoid];
        }

        // !blacksmith tools dependency!
        private static void HideBodyPart(BodypartSystem.bodyPart part, VisEquipment ve)
        {
            BodypartSystem.bodyPart bodyPart = part;
            if (bodyPart != 14)
            {
                if (bodyPart - 15 <= 1)
                    return;
                ve.m_bodyModel.SetBlendShapeWeight((int)part, 100f);
            }
            else
            {
                foreach (int num in (BodypartSystem.bodyPart[])Enum.GetValues(typeof(BodypartSystem.bodyPart)))
                {
                    BodypartSystem.bodyPart part1 = (BodypartSystem.bodyPart)num;
                    if (part1 != 14)
                        Main.HideBodyPart(part1, ve);
                }
            }
        }

        private static void ShowBodyPart(BodypartSystem.bodyPart part, VisEquipment ve)
        {
            if (ve.m_bodyModel.sharedMesh.blendShapeCount == 0)
                return;
            BodypartSystem.bodyPart bodyPart = part;
            if (bodyPart != 14)
            {
                if (bodyPart - 15 <= 1 || Object.op_Equality((Object)ve.m_bodyModel, (Object)null))
                    return;
                ve.m_bodyModel.SetBlendShapeWeight((int)part, 0.0f);
            }
            else
            {
                foreach (int num in (BodypartSystem.bodyPart[])Enum.GetValues(typeof(BodypartSystem.bodyPart)))
                {
                    BodypartSystem.bodyPart part1 = (BodypartSystem.bodyPart)num;
                    if (part1 != 14)
                        Main.ShowBodyPart(part1, ve);
                }
            }
        }

        private static void UpdateRaceVisibility(Equipment equipment) {
            if (equipment == null || equipment.RaceItem == null) return;
            GameObject raceItem = equipment.RaceItem;
            Transform transform = raceItem.transform.Find(
                equipment._visEquipment.GetModelIndex() == 1 ? "splitbody_female" : "splitbody");
            Transform source = transform != null ? transform : raceItem.transform;

            string[] bits = new string[] { "Chest", "Hands", "Leg", "LowerLeg", "LowerArm", "UpperArm" };
            SkinnedMeshRenderer[] comps = new SkinnedMeshRenderer[6];
            for (int i = 0; i < comps.Length; i++) {
                comps[i] = source.Find(bits[i]).GetComponent<SkinnedMeshRenderer>();
                comps[i].enabled = true;
            }

            // theres a couple new pieces of armor that need to be tried out here
            switch (equipment.currentChestName) {
                case "ArmorPaddedCuirass":
                    comps[4].enabled = false;
                    goto case "ArmorWolfChest";
                case "ArmorRagsChest":
                case "ArmorIronChest":
                case "ArmorWolfChest":
                    comps[5].enabled = false;
                    goto case "ArmorBronzeChest";
                case "ArmorLeatherChest":
                case "ArmorTrollLeatherChest":
                case "ArmorBronzeChest":
                    comps[0].enabled = false;
                    break;
            }
            switch (equipment.currentLegName) {
                case "ArmorBronzeLegs":
                case "ArmorIronLegs":
                case "ArmorLeatherLegs":
                case "ArmorPaddedGreaves":
                case "ArmorTrollLeatherLegs":
                case "ArmorWolfLegs":
                    comps[2].enabled = false;
                    comps[3].enabled = false;
                    break;
                case "ArmorRagsLegs":
                    break;
            }
            UpdatePlayerVisibility(equipment);
            equipment.lastChestName = equipment.currentChestName;
            equipment.lastLegName = equipment.currentLegName;
        }

        //BodyPart indexes!
        private static void UpdatePlayerVisibility(Equipment equipment)
        {
            ShowBodyPart((BodypartSystem.bodyPart)14, equipment._visEquipment);
            HideBodyPart((BodypartSystem.bodyPart)0, equipment._visEquipment);
            switch (equipment.currentChestName)
            {
                case "ArmorBronzeChest":
                    Main.HideBodyPart((BodypartSystem.bodyPart)2, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)5, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                case "ArmorIronChest":
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                case "ArmorLeatherChest":
                    Main.HideBodyPart((BodypartSystem.bodyPart)2, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)5, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                case "ArmorPaddedCuirass":
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                case "ArmorRagsChest":
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                case "ArmorTrollLeatherChest":
                    Main.HideBodyPart((BodypartSystem.bodyPart)2, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)5, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                case "ArmorWolfChest":
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    break;
                default:
                    Main.HideBodyPart((BodypartSystem.bodyPart)1, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)5, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)2, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)6, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)3, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)7, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)4, equipment._visEquipment);
                    break;
            }
            switch (equipment.currentLegName)
            {
                case "ArmorBronzeLegs":
                case "ArmorIronLegs":
                case "ArmorLeatherLegs":
                case "ArmorPaddedGreaves":
                case "ArmorTrollLeatherLegs":
                case "ArmorWolfLegs":
                    break;
                case "ArmorRagsLegs":
                    Main.HideBodyPart((BodypartSystem.bodyPart)11, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)8, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)12, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)9, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)13, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)10, equipment._visEquipment);
                    break;
                default:
                    Main.HideBodyPart((BodypartSystem.bodyPart)11, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)8, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)12, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)9, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)13, equipment._visEquipment);
                    Main.HideBodyPart((BodypartSystem.bodyPart)10, equipment._visEquipment);
                    break;
            }
        }

        [HarmonyPatch(typeof(Humanoid), "UpdateEquipment")]
        private static class Humanoid_UpdateEquipment
        {
            private static void Prefix(Humanoid instance, ref float dt) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                if (eq == null) return;
                ItemDrop.ItemData raceItemData = eq.RaceItemData;
                if (raceItemData != null && raceItemData.m_shared.m_useDurability)
                    instance.DrainEquipedItemDurability(raceItemData, dt);
            }
        }

        [HarmonyPatch(typeof(Humanoid), "EquipItem")]
        private static class Humanoid_EquipItem {
            private static bool Prefix(
              Humanoid instance,
              ref bool result,
              ref ItemDrop.ItemData item,
              ref bool triggerEquipEffects) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                if (eq == null || instance.IsItemEquiped(item)
                    || !instance.GetInventory().ContainsItem(item) || instance.InAttack()
                    || instance.InDodge() || instance.IsPlayer() && !instance.IsDead()
                    && instance.IsSwiming() && !instance.IsOnGround()
                    || item.m_shared.m_useDurability && (double)item.m_durability <= 0.0
                    || item.m_shared.m_itemType != (ItemDrop.ItemData.ItemType) 100)
                    return true;

                instance.UnequipItem(eq.RaceItemData, triggerEquipEffects);
                eq.RaceItemData = item;
                
                if (instance.IsItemEquiped(item))
                    item.m_equiped = true;
                instance.SetupEquipment();
                if (triggerEquipEffects)
                    instance.TriggerEquipEffect(item);

                result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(Humanoid), "UnequipItem")]
        private static class Humanoid_UnequipItem {
            private static bool Prefix(
              Humanoid instance,
              ref ItemDrop.ItemData item,
              ref bool triggerEquipEffects) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                if (item == null || eq == null || eq.RaceItemData != item)
                    return true;
                eq.RaceItemData = null;
                item.m_equiped = false;

                instance.SetupEquipment();
                if (triggerEquipEffects)
                    instance.TriggerEquipEffect(item);
                return false;
            }
        }

        [HarmonyPatch(typeof(Humanoid), "UnequipAllItems")]
        private static class Humanoid_UnequipAllItems {
            private static bool Prefix(Humanoid instance) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                if (eq == null || eq.RaceItemData == null)
                    return true;
                instance.UnequipItem(eq.RaceItemData, true);
                return true;
            }
        }

        [HarmonyPatch(typeof(Humanoid), "SetupVisEquipment")]
        private static class Humanoid_SetupVisEquipment {
            private static void Postfix(
                Humanoid instance, ref VisEquipment visEq, ref bool isRagdoll) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                if (eq == null) return;

                string str = "";
                if (eq.RaceItemData != null) // idon't think this is relevant, 
                    str = eq.RaceItemData.m_dropPrefab.name;

                if (eq.RaceItemName == str) return;
                eq.RaceItemName = str;
                // removed null check on str, shouldn't be a concern by this point
                instance.m_nview.GetZDO()?.Set("RaceItem",
                    str != "" ? StringExtensionMethods.GetStableHashCode(str) : 0);
            }
        }

        [HarmonyPatch(typeof(Humanoid), "UpdateEquipmentStatusEffects")]
        private static class Humanoid_UpdateEquipmentStatusEffects {
            private static void Postfix(Humanoid instance) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                StatusEffect se = eq.RaceItemData.m_shared.m_equipStatusEffect;
                if (eq == null || eq.RaceItemData == null || se)
                    return;
                
                instance.m_eqipmentStatusEffects.Add(se);
                instance.m_seman.AddStatusEffect(se, false);
            }
        }

        [HarmonyPatch(typeof(Humanoid), "IsItemEquiped")]
        private static class Humanoid_IsItemEquiped {
            private static void Postfix(
              Humanoid instance,
              ref bool result,
              ref ItemDrop.ItemData item) {
                Equipment eq = GetRaceItemByHumanoid(instance);
                if (eq == null || eq.RaceItemData != item)
                    return;
                result = true;
            }
        }

        [HarmonyPatch(typeof(Humanoid), "GetEquipmentWeight")]
        private static class Humanoid_GetEquipmentWeight
        {
            private static void Postfix(Humanoid instance, ref float __result)
            {
                if (Main.GetRaceItemByHumanoid(__instance) == null || Main.GetRaceItemByHumanoid(__instance).RaceItemData == null)
                    return;
                __result += Main.GetRaceItemByHumanoid(__instance).RaceItemData.m_shared.m_weight;
            }
        }

        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        private static class Player_FixedUpdate
        {
            private static void Postfix(Player __instance) => Main.GetRaceItemByHumanoid((Humanoid)__instance);
        }

        [HarmonyPatch(typeof(Player), "UpdateMovementModifier")]
        private static class Player_UpdateMovementModifier
        {
            private static void Postfix(Player __instance)
            {
                if (Main.GetRaceItemByHumanoid((Humanoid)__instance) == null || Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData == null)
                    return;
                float num = (float)Traverse.Create((object)__instance).Field("m_equipmentMovementModifier").GetValue() + Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData.m_shared.m_movementModifier;
                Traverse.Create((object)__instance).Field("m_equipmentMovementModifier").SetValue((object)num);
            }
        }

        [HarmonyPatch(typeof(Player), "ApplyArmorDamageMods")]
        private static class Player_ApplyArmorDamageMods
        {
            private static void Postfix(Player __instance, ref HitData.DamageModifiers mods)
            {
                if (Main.GetRaceItemByHumanoid((Humanoid)__instance) == null || Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData == null)
                    return;
                ((HitData.DamageModifiers)ref mods).Apply(Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData.m_shared.m_damageModifiers);
            }
        }

        [HarmonyPatch(typeof(Player), "GetBodyArmor")]
        private static class Player_GetBodyArmor
        {
            private static void Postfix(Player __instance, ref float __result)
            {
                if (Main.GetRaceItemByHumanoid((Humanoid)__instance) == null || Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData == null)
                    return;
                __result += Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData.GetArmor();
            }
        }

        [HarmonyPatch(typeof(Player), "DamageArmorDurability")]
        private static class Player_DamageArmorDurability
        {
            private static void Postfix(Player __instance, ref HitData hit)
            {
                if (Main.GetRaceItemByHumanoid((Humanoid)__instance) == null || Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData == null)
                    return;
                float num = hit.GetTotalPhysicalDamage() + hit.GetTotalElementalDamage();
                Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData.m_durability = Mathf.Max(0.0f, Main.GetRaceItemByHumanoid((Humanoid)__instance).RaceItemData.m_durability - num);
            }
        }

        [HarmonyPatch(typeof(ItemDrop.ItemData), "IsEquipable")]
        private static class ItemData_IsEquipable
        {
            private static void Postfix(ItemDrop.ItemData __instance, ref bool __result)
            {
                if (__instance.m_shared.m_itemType != 100)
                    return;
                __result = true;
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "SetLegEquiped")]
        private static class VisEq_SetLegEquipedPatch
        {
            private static void Postfix(VisEquipment __instance, int hash)
            {
                Equipment raceItem = Main.GetRaceItem(__instance);
                if (raceItem == null)
                    return;
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(hash);
                raceItem.currentLegName = !Object.op_Inequality((Object)itemPrefab, (Object)null) ? "" : ((Object)itemPrefab).name;
                Main.UpdateRaceVisibility(raceItem);
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "SetChestEquiped")]
        private static class VisEq_SetChestEquipedPatch
        {
            private static void Postfix(VisEquipment __instance, int hash)
            {
                Equipment raceItem = Main.GetRaceItem(__instance);
                if (raceItem == null)
                    return;
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(hash);
                raceItem.currentChestName = !Object.op_Inequality((Object)itemPrefab, (Object)null) ? "" : ((Object)itemPrefab).name;
                Main.UpdateRaceVisibility(raceItem);
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "SetHelmetEquiped")]
        private static class VisEq_SetHelmetEquipedPatch
        {
            private static void Prefix(VisEquipment __instance, ref int hash)
            {
                if (Main.GetRaceItem(__instance) == null || !Object.op_Inequality((Object)Main.GetRaceItem(__instance).RaceItem, (Object)null) || hash == 703889544)
                    return;
                hash = 0;
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "SetHairEquiped")]
        private static class VisEq_SetHairEquiped
        {
            private static void Prefix(VisEquipment __instance, ref int hash)
            {
                if (Main.GetRaceItem(__instance) == null || !Object.op_Inequality((Object)Main.GetRaceItem(__instance).RaceItem, (Object)null))
                    return;
                hash = 0;
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "SetBeardEquiped")]
        private static class VisEq_SetBeardEquipedPatch
        {
            private static void Prefix(VisEquipment __instance, ref int hash)
            {
                if (Main.GetRaceItem(__instance) == null || !Object.op_Inequality((Object)Main.GetRaceItem(__instance).RaceItem, (Object)null))
                    return;
                hash = 0;
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "UpdateEquipmentVisuals")]
        private static class VisEq_UpdateEquipmentVisuals
        {
            private static void Postfix(VisEquipment __instance)
            {
                Equipment raceItem = Main.GetRaceItem(__instance);
                if (raceItem == null)
                    return;
                int hash = 0;
                ZDO zdo = ((ZNetView)Traverse.Create((object)__instance).Field("m_nview").GetValue()).GetZDO();
                if (zdo != null)
                    hash = zdo.GetInt("RaceItem", 0);
                else if (!string.IsNullOrEmpty(raceItem.RaceItemName))
                    hash = StringExtensionMethods.GetStableHashCode(raceItem.RaceItemName);
                if (!raceItem.SetRaceItemEquiped(hash))
                    return;
                typeof(VisEquipment).GetMethod("UpdateLodgroup", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object)__instance, new object[0]);
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        private static class VisEq_AttachItemPatch
        {
            private static SkinnedMeshRenderer GetSkinnedMeshRenderer(
              VisEquipment visEquipment)
            {
                return visEquipment.m_bodyModel;
            }

            private static void CleanupInstance(GameObject instance)
            {
                foreach (Collider componentsInChild in instance.GetComponentsInChildren<Collider>())
                    componentsInChild.enabled = false;
            }

            private static void EnableEquipedEffects(GameObject instance)
            {
                Transform transform = instance.transform.Find("equiped");
                if (!Object.op_Implicit((Object)transform))
                    return;
                ((Component)transform).gameObject.SetActive(true);
            }

            private static void Postfix(
              VisEquipment __instance,
              int itemHash,
              int variant,
              Transform joint,
              bool enableEquipEffects = true)
            {
                Main.UpdateRaceVisibility(Main.GetRaceItem(__instance));
            }

            private static bool Prefix(
              VisEquipment __instance,
              ref GameObject __result,
              int itemHash,
              int variant,
              Transform joint,
              bool enableEquipEffects = true)
            {
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
                if (Object.op_Equality((Object)itemPrefab, (Object)null))
                {
                    ZLog.Log((object)("Missing attach item: " + itemHash.ToString() + "  ob:" + ((Object)((Component)__instance).gameObject).name + "  joint:" + (Object.op_Implicit((Object)joint) ? ((Object)joint).name : "none")));
                    __result = (GameObject)null;
                    return false;
                }
                string str = "attach_skin";
                GameObject gameObject = (GameObject)null;
                int childCount = itemPrefab.transform.childCount;
                for (int index = 0; index < childCount; ++index)
                {
                    Transform child = itemPrefab.transform.GetChild(index);
                    if (((Object)((Component)child).gameObject).name == "attach" || ((Object)((Component)child).gameObject).name == str)
                    {
                        gameObject = ((Component)child).gameObject;
                        break;
                    }
                }
                if (Object.op_Equality((Object)gameObject, (Object)null))
                {
                    __result = (GameObject)null;
                    return false;
                }
                GameObject instance = Prefab.InstantiateClone(gameObject, ((Object)gameObject).name, false);
                instance.SetActive(true);
                Main.VisEq_AttachItemPatch.CleanupInstance(instance);
                if (enableEquipEffects)
                    Main.VisEq_AttachItemPatch.EnableEquipedEffects(instance);
                if (((Object)gameObject).name == str)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = Main.VisEq_AttachItemPatch.GetSkinnedMeshRenderer(__instance);
                    if (Main.raceNames.Contains(((Object)itemPrefab).name))
                    {
                        instance.transform.SetParent(((Component)skinnedMeshRenderer).transform.parent);
                        instance.transform.localPosition = Vector3.zero;
                        instance.transform.localRotation = Quaternion.identity;
                        foreach (SkinnedMeshRenderer componentsInChild in instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            for (int index1 = 0; index1 < skinnedMeshRenderer.bones.Length; ++index1)
                            {
                                for (int index2 = 0; index2 < componentsInChild.bones.Length; ++index2)
                                {
                                    if (((Object)skinnedMeshRenderer.bones[index1]).name == ((Object)componentsInChild.bones[index2]).name)
                                    {
                                        componentsInChild.bones[index2].SetParent(skinnedMeshRenderer.bones[index1]);
                                        componentsInChild.bones[index2].SetPositionAndRotation(skinnedMeshRenderer.bones[index1].position, skinnedMeshRenderer.bones[index1].rotation);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        instance.transform.SetParent(((Component)__instance.m_bodyModel).transform.parent);
                        instance.transform.localPosition = Vector3.zero;
                        instance.transform.localRotation = Quaternion.identity;
                        foreach (SkinnedMeshRenderer componentsInChild in instance.GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            componentsInChild.rootBone = __instance.m_bodyModel.rootBone;
                            componentsInChild.bones = __instance.m_bodyModel.bones;
                        }
                    }
                }
                else
                {
                    instance.transform.SetParent(joint);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                }
                instance.GetComponentInChildren<ItemStyle>()?.Setup(variant);
                __result = instance;
                if (Main.raceNames.Contains(((Object)itemPrefab).name))
                {
                    Main.GetRaceItem(__instance).RaceItem = instance;
                    Transform transform1 = instance.transform.Find("splitbody");
                    Transform transform2 = instance.transform.Find("splitbody_female");
                    if (Object.op_Inequality((Object)transform1, (Object)null) && Object.op_Inequality((Object)transform2, (Object)null))
                    {
                        if (__instance.GetModelIndex() == 1)
                        {
                            ((Component)transform1).gameObject.SetActive(false);
                            ((Component)transform2).gameObject.SetActive(true);
                        }
                        else
                        {
                            ((Component)transform1).gameObject.SetActive(true);
                            ((Component)transform2).gameObject.SetActive(false);
                        }
                    }
                }
                return false;
            }
        }
    }
}
