
using T4.Managers;

namespace T4.Units.Characters
{
    public abstract class Character : Unit
    {
        private readonly CharacterData data;
        public new UnitData Data { get { return data; } }

        public Character(CharacterData data, int owner) : base(data, owner)
        {
            this.data = data;
        }

        public override void Place()
        {
            base.Place();

            GameManager.Instance.USER_CHARACTERS[Owner].Add(this);
        }

        public override void Dispose()
        {
            GameManager.Instance.USER_CHARACTERS[Owner].Remove(this);
        }
    }
}