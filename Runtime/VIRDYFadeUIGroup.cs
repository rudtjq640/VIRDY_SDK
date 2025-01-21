using System.Collections.Generic;

namespace VIRDY.SDK
{
    public class VIRDYFadeUIGroup : VIRDYBehaviour
    {
        private List<VIRDYFadeUI> _fadeUIs = new List<VIRDYFadeUI>();

        protected override void OnInitialize()
        {
            this.GetComponentsInChildren<VIRDYFadeUI>(_fadeUIs);
        }

        public void FadeInAll()
        {
            foreach (var fadeUI in _fadeUIs) fadeUI?.FadeIn();
        }

        public void FadeOutAll()
        {
            foreach (var fadeUI in _fadeUIs) fadeUI?.FadeOut();
        }
    }
}
