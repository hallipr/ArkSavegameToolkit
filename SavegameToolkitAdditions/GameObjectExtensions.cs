﻿using System.Linq;
using SavegameToolkit;
using SavegameToolkit.Types;

namespace SavegameToolkitAdditions {
    public static class GameObjectExtensions {
        public static bool IsCreature(this GameObject gameObject) {
            return gameObject.HasAnyProperty("bServerInitializedDino");
        }

        public static bool IsDroppedItem(this GameObject gameObject) {
            return gameObject.HasAnyProperty("MyItem");
        }

        public static bool IsInventory(this GameObject gameObject) {
            return gameObject.HasAnyProperty("bInitializedMe");
        }

        public static bool IsPlayer(this GameObject gameObject) {
            return gameObject.HasAnyProperty("LinkedPlayerDataID");
        }

        public static bool IsStatusComponent(this GameObject gameObject) {
            return gameObject.HasAnyProperty("bServerFirstInitialized");
        }

        public static bool IsTamed(this GameObject gameObject) {
            var teamType = TeamTypes.ForTeam(gameObject.GetPropertyValue<int>("TargetingTeam"));
            return teamType.IsTamed();
        }

        public static bool IsUnclaimedBaby(this GameObject gameObject) {
            var teamType = TeamTypes.ForTeam(gameObject.GetPropertyValue<int>("TargetingTeam"));
            return teamType == TeamType.Breeding;
        }

        public static bool IsWeapon(this GameObject gameObject) {
            return gameObject.HasAnyProperty("AssociatedPrimalItem") || gameObject.HasAnyProperty("MyPawn");
        }

        public static bool IsWild(this GameObject gameObject) {
            var teamType = TeamTypes.ForTeam(gameObject.GetPropertyValue<int>("TargetingTeam"));
            return !teamType.IsTamed();
        }

        public static bool IsDeathItemCache(this GameObject gameObject) {
            return gameObject.ClassString == "DeathItemCache_C";
        }

        public static bool IsFemale(this GameObject gameObject) {
            return gameObject.GetPropertyValue<bool>("bIsFemale");
        }

        public static GameObject CharacterStatusComponent(this GameObject gameObject) {
            var myCharacterStatusComponent = gameObject.GetPropertyValue<ObjectReference>("MyCharacterStatusComponent")?.ObjectId;
            return myCharacterStatusComponent.HasValue
                    ? gameObject.Components.FirstOrDefault(component => component.Value.Id == myCharacterStatusComponent).Value
                    : gameObject.Components.FirstOrDefault(component => component.Key.Name.StartsWith("DinoCharacterStatus_")).Value;
        }

        public static int GetBaseLevel(this GameObject gameObject) {
            return gameObject.CharacterStatusComponent()?.GetPropertyValue<int>("BaseCharacterLevel") ?? 1;
        }

        public static int GetBaseLevel(this GameObject gameObject, GameObjectContainer saveFile) {
            var objectReference = gameObject.GetPropertyValue<ObjectReference>("MyCharacterStatusComponent");
            var statusComponent = objectReference != null ? saveFile[objectReference] : null;

            return statusComponent?.GetPropertyValue<int>("BaseCharacterLevel") ?? 1;
        }

        public static int GetFullLevel(this GameObject gameObject) {
            var statusComponent = gameObject.CharacterStatusComponent();
            return getFullLevel(statusComponent);
        }

        public static int GetFullLevel(this GameObject gameObject, GameObjectContainer saveFile) {
            var objectReference = gameObject.GetPropertyValue<ObjectReference>("MyCharacterStatusComponent");

            var statusComponent = objectReference != null ? saveFile[objectReference] : null;

            return getFullLevel(statusComponent);
        }

        private static int getFullLevel(GameObject statusComponent) {
            if (statusComponent == null) {
                return 1;
            }

            var baseLevel = statusComponent.GetPropertyValue<int>("BaseCharacterLevel", defaultValue: 1);
            var extraLevel = statusComponent.GetPropertyValue<short>("ExtraCharacterLevel");
            return baseLevel + extraLevel;
        }

        public static string GetNameForCreature(this GameObject gameObject, ArkData arkData, string valueIfNotFound = null) {
            return arkData.GetCreatureForClass(gameObject.ClassString)?.Name ?? valueIfNotFound;
        }

        public static string GetNameForStructure(this GameObject gameObject, ArkData arkData) {
            return arkData.GetStructureForClass(gameObject.ClassString)?.Name;
        }

        public static string GetNameForItem(this GameObject gameObject, ArkData arkData) {
            return arkData.GetItemForClass(gameObject.ClassString)?.Name;
        }

        public static long GetDinoId(this GameObject gameObject) {
            return CreateDinoId(gameObject.GetPropertyValue<int>("DinoID1"), gameObject.GetPropertyValue<int>("DinoID2"));
        }

        public static long CreateDinoId(int id1, int id2) {
            return ((long)id1 << 32) | (id2 & 0xFFFFFFFFL);
        }
    }
}
