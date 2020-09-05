using UnityEngine;

namespace Koffie.SimpleTasks
{
    public delegate void UpdateAction(float deltaTime);

    public enum UpdateType
    {
        Update = 0,
        LateUpdate,
        FixedUpdate
    }

    public class UpdateHelper : MonoBehaviour
    {

        private static UpdateHelper _instance;

        private static UpdateAction _onUpdate;
        private static UpdateAction _onLateUpdate;
        private static UpdateAction _onFixedUpdate;

        public static void AddSubscriber(UpdateAction action, UpdateType updateType)
        {
            ValidateInstance();

            switch (updateType)
            {
                case UpdateType.Update:
                    _onUpdate += action;
                    break;

                case UpdateType.LateUpdate:
                    _onLateUpdate += action;
                    break;

                case UpdateType.FixedUpdate:
                    _onFixedUpdate += action;
                    break;
            }
        }

        private static void ValidateInstance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("Update Helper").AddComponent<UpdateHelper>();
                DontDestroyOnLoad(_instance);
            }
        }

        private void Update()
        {
            _onUpdate?.Invoke(Time.deltaTime);
        }

        private void LateUpdate()
        {
            _onLateUpdate?.Invoke(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _onFixedUpdate?.Invoke(Time.fixedDeltaTime);
        }
    }
}