using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public class ForbiddenFruitCanMatchInfo(BooleanReference canMatch, BooleanReference isFirstUnit, StringReference otherForbiddenFruitID, IUnit otherUnit, StringReference passiveName, Sprite passiveSprite) : BasicBoolAndStringReferenceHolder(canMatch, otherForbiddenFruitID)
    {
        public IUnit OtherUnit = otherUnit;
        public BooleanReference IsFirstUnit = isFirstUnit;
        public StringReference PassiveName = passiveName;
        public Sprite PassiveSprite = passiveSprite;
    }

    public class ForbiddenFruitSuccessfulMatchInfo(StringReference otherForbiddenFruitID, IUnit otherUnit, string otherUnitPassiveName, Sprite otherUnitPassiveSprite) : BasicStringReferenceHolder(otherForbiddenFruitID)
    {
        public IUnit OtherUnit = otherUnit;
        public string OtherUnitPassiveName = otherUnitPassiveName;
        public Sprite OtherUnitPassiveSprite = otherUnitPassiveSprite;
    }
}
