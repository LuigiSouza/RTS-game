using System.Collections.Generic;
using System.Linq;
using T4.Units;
using T4.Units.Characters;
using UnityEngine;

namespace T4.Managers
{
    [System.Serializable]
    struct StateACtion
    {
        public UnitState state;
        public string key;
    }

    public class AnimatorManager : MonoBehaviour
    {
        [SerializeField]
        private List<StateACtion> animationKeyList;

        [SerializeField]
        private CharacterBehaviour behaviour;

        [SerializeField]
        private Animator animatorController;

        private string previousState = "";

        private void Update()
        {
            if (behaviour != null)
            {
                StateACtion action = animationKeyList.FirstOrDefault(e => e.state == behaviour.CurrentState);
                if (action.key == previousState) return;
                if (previousState != "") animatorController.SetBool(previousState, false);
                previousState = action.key;
                if (action.key == null) return;
                animatorController.SetBool(action.key, true);
            }
        }
    }
}
