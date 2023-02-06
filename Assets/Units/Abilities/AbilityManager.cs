using System.Collections;
using UnityEngine;

namespace T4.Units.Abilities
{
    public class AbilityManager : MonoBehaviour
    {
        public AbilityData ability;
        public bool Succeed { get; private set; } = false;

        private GameObject source;
        private bool isReady = true;

        public void Initialize(AbilityData ability, GameObject source)
        {
            this.ability = ability.Clone();
            this.source = source;
        }

        public IEnumerator Trigger(GameObject target = null)
        {
            if (isReady) yield return StartCoroutine(WrappedTrigger(target));
        }

        private IEnumerator WrappedTrigger(GameObject target)
        {
            SetReady(false);
            yield return new WaitForSeconds(ability.castTime);
            SetSuccess(ability.Trigger(source, target));
            SetReady(true);
            yield return null;
        }

        private void SetReady(bool ready)
        {
            isReady = ready;
        }

        private void SetSuccess(bool succeed)
        {
            this.Succeed = succeed;
        }
    }
}