using Fusion;

namespace VIRDY.SDK
{
    public abstract class VIRDYNetworkBehaviour : NetworkBehaviour
    {
        protected bool IsInitalized { get; private set; }

        public void Initialize()
        {
            if (IsInitalized == true) return;

            OnInitialize();

            IsInitalized = true;
        }

        public void DeInitialize()
        {
            if (IsInitalized == false) return;

            OnDeinitialize();

            IsInitalized = false;
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnDeinitialize() { }
    }
}
