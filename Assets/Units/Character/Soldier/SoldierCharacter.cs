using T4.Managers;

namespace T4.Units.Characters
{
    public class SoldierCharacter : Character
    {
        public SoldierCharacter(CharacterData data, int owner) : base(data, owner)
        {
            if (owner == GameManager.Instance.PlayerId)
            {
                Transform.gameObject.AddComponent<SoldierActionManager>();
            }
        }
    }
}
