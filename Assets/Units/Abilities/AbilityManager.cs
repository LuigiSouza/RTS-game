using System.Collections;
using UnityEngine;

namespace T4.Units.Abilities
{
    public class AbilityManager : MonoBehaviour
    {
        public AbilityData ability;
        GameObject source;
        bool isReady = true;

        public void Initialize(AbilityData ability, GameObject source)
        {
            this.ability = ability;
            this.source = source;
        }

        public void Trigger(GameObject target = null)
        {
            if (!isReady) return;
            StartCoroutine(WrappedTrigger(target));
        }

        private IEnumerator WrappedTrigger(GameObject target)
        {
            yield return new WaitForSeconds(ability.castTime);
            ability.Trigger(source, target);
            SetReady(false);
            yield return new WaitForSeconds(ability.castTime);
            SetReady(true);
        }

        private void SetReady(bool ready)
        {
            isReady = ready;
        }
    }
}