using T4.Managers;
using UnityEngine;

namespace T4.Units.Characters
{

    [RequireComponent(typeof(CharacterBehaviour))]
    public abstract class ActionManager : MonoBehaviour
    {
        protected CharacterManager character;

        protected CharacterBehaviour behaviour;

        private void Awake()
        {
            character = GetComponent<CharacterManager>();
            behaviour = GetComponent<CharacterBehaviour>();
        }

        private void Update()
        {
            if (!character.IsSelected() || !Input.GetMouseButtonUp(1)) return;

            CheckValidHits();
        }

        protected abstract void CheckValidHits();
    }
}