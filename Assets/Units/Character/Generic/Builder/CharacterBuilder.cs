using System;

namespace T4.Units.Characters
{
    public static class CharacterBuilder
    {
        public static Character Build(UnitType type, int owner)
        {
            switch (type)
            {
                case CharacterType.SOLDIER:
                    return new SoldierCharacter(CharacterDataBuilder.Soldier, owner);
                case CharacterType.VILLAGER:
                    return new VillagerCharacter(CharacterDataBuilder.Villager, owner);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
