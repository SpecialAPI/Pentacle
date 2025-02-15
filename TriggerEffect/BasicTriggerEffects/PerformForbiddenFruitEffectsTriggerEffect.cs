using Pentacle.CustomEvent.Args;
using Pentacle.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.TriggerEffect.BasicTriggerEffects
{
    public class PerformForbiddenFruitEffectsTriggerEffect : TriggerEffect
    {
        public List<EffectInfo> selfDefaultEffects;
        public List<EffectInfo> otherDefaultEffects;

        public Dictionary<string, List<EffectInfo>> specialForbiddenFruitIdSelfEffects;
        public Dictionary<string, List<EffectInfo>> specialForbiddenFruitIdOtherEffects;

        public override void DoEffect(IUnit sender, object args, TriggeredEffect triggerInfo, object activator = null)
        {
            if (args is not ForbiddenFruitSuccessfulMatchInfo ffInfo)
                return;

            var unitIds = new List<int>();
            var unitsCharacter = new List<bool>();
            var passiveNames = new List<string>();
            var passiveSprites = new List<Sprite>();

            if (activator.TryGetActivatorNameAndSprite(out var selfPassiveName, out var selfPassiveSprite))
            {
                unitIds.Add(sender.ID);
                unitsCharacter.Add(sender.IsUnitCharacter);
                passiveNames.Add(selfPassiveName);
                passiveSprites.Add(selfPassiveSprite);
            }

            if (!string.IsNullOrEmpty(ffInfo.OtherUnitPassiveName) || ffInfo.OtherUnitPassiveSprite != null)
            {
                unitIds.Add(ffInfo.OtherUnit.ID);
                unitsCharacter.Add(ffInfo.OtherUnit.IsUnitCharacter);
                passiveNames.Add(ffInfo.OtherUnitPassiveName);
                passiveSprites.Add(ffInfo.OtherUnitPassiveSprite);
            }

            if (unitIds.Count > 0)
                CombatManager.Instance.AddUIAction(new ShowMultiplePassiveInformationUIAction([.. unitIds], [.. unitsCharacter], [.. passiveNames], [.. passiveSprites]));

            var selfEffects = selfDefaultEffects;
            if (specialForbiddenFruitIdSelfEffects != null && specialForbiddenFruitIdSelfEffects.TryGetValue(ffInfo.StringReference.value, out var specialSelfEffects))
                selfEffects = specialSelfEffects;

            var otherEffects = otherDefaultEffects;
            if (specialForbiddenFruitIdOtherEffects != null && specialForbiddenFruitIdOtherEffects.TryGetValue(ffInfo.StringReference.value, out var specialOtherEffects))
                otherEffects = specialOtherEffects;

            if (selfEffects != null && selfEffects.Count > 0)
            {
                if (triggerInfo.immediate)
                    CombatManager.Instance.ProcessImmediateAction(new ImmediateEffectAction([.. selfEffects], sender));

                else
                    CombatManager.Instance.AddSubAction(new EffectAction([.. selfEffects], sender));
            }

            if (otherEffects != null && otherEffects.Count > 0)
            {
                if (triggerInfo.immediate)
                    CombatManager.Instance.ProcessImmediateAction(new ImmediateEffectAction([.. otherEffects], ffInfo.OtherUnit));

                else
                    CombatManager.Instance.AddSubAction(new EffectAction([.. otherEffects], ffInfo.OtherUnit));
            }
        }
    }
}
