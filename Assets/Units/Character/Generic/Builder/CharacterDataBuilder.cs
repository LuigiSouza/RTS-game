using UnityEngine;

namespace T4.Units.Characters
{
    public static class CharacterDataBuilder
    {
        private static CharacterData _soldier;
        private static CharacterData _villager;

        public static CharacterData Soldier { get { return (CharacterData)_soldier.Clone(); } private set { _soldier = value; } }
        public static CharacterData Villager { get { return (CharacterData)_villager.Clone(); } private set { _villager = value; } }

        public static void LoadAvaiableCharacters()
        {
            Soldier = Resources.Load<CharacterData>("ScriptableObjects/Characters/Soldier");
            Villager = Resources.Load<CharacterData>("ScriptableObjects/Characters/Villager");
        }
    }
}
