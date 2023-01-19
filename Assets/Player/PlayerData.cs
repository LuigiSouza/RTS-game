
using UnityEngine;

namespace T4.Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Players", order = 5)]
    public class PlayerData : ScriptableObject
    {
        public string playerName;
        public Color color;
        public int id;

        public PlayerData(string playerName, Color color, int id)
        {
            this.playerName = playerName;
            this.color = color;
            this.id = id;
        }
        public PlayerData Clone()
        {
            return (PlayerData)MemberwiseClone();
        }
    }
}
