// Decompiled with JetBrains decompiler
// Type: BeastRacesMod.Equipment
// Assembly: BeastRacesMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19927FD4-CBDA-432C-8380-AAD98EC51272
// Assembly location: D:\BeastTribes-1206-1-1-0-1642108216\plugins\BeastTribes\BeastRacesMod.dll

using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace Autospace
{
    public class Equipment{
        private Traverse visEquipmentTraverse;

        private Humanoid humanoid;
        private VisEquipment _visEquipment;
        public Humanoid Humanoid {
            set {
                humanoid = value;
                _visEquipment = humanoid.m_visEquipment;
            }
            get => humanoid;
        }
        public VisEquipment visEquipment => _visEquipment;

        public GameObject RaceItem;
        public ItemDrop.ItemData RaceItemData;
        public string RaceItemName;
        public int raceItemHash = 0;

        public string lastChestName;
        public string currentChestName;
        public string lastLegName;
        public string currentLegName;

        public bool SetRaceItemEquiped(int hash) {
            if (raceItemHash == hash) return false;
            if (RaceItem) {
                Object.Destroy(RaceItem);
                RaceItem = null;
            }
      
            raceItemHash = hash;
                Transform transform = _visEquipment.m_helmet;
                _visEquipment.AttachItem(hash, 0, transform, true);
            return true;
        }
    }
}
