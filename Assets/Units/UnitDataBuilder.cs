using T4.Player;
using UnityEngine;

namespace T4.Units
{
    public static class UnitDataBuilder
    {
        private static PlayerData _player;
        private static PlayerData _cpu;
        public static PlayerData Player { get { return _player.Clone(); } private set { _player = value; } }
        public static PlayerData CPU { get { return _cpu.Clone(); } private set { _cpu = value; } }

        public static void LoadAvaiablePlayers()
        {
            Player = Resources.Load<PlayerData>("ScriptableObjects/Players/Player");
            CPU = Resources.Load<PlayerData>("ScriptableObjects/Players/CPU");
        }
    }
}
