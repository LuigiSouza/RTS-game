using T4.Managers;
using UnityEngine;

namespace T4.Units.Characters
{
    public class VillagerCharacter : Character
    {
        public VillagerCharacter(CharacterData data, int owner) : base(data, owner)
        {
            if (owner == GameManager.Instance.PlayerId)
            {
                Transform.gameObject.AddComponent<VillagerActionManager>();
            }
        }
    }
}
