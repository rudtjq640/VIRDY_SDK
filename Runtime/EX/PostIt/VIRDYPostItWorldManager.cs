using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VIRDY.SDK
{
    public class VIRDYPostItSceneManager : MonoBehaviour
    {
        /// <summary>
        /// 지정한 substring(대소문자 무시)을 포함하는 루트 오브젝트를 찾아
        /// 그 하위의 VIRDYPostItController를 모두 활성화
        /// </summary>
        public void ActivatePostIts(string actorNameSubstring)
        {
            ApplyToActor(actorNameSubstring, false);
            ApplyToActor(actorNameSubstring, true);
        }

        /// <summary>
        /// 지정한 substring(대소문자 무시)을 포함하는 루트 오브젝트를 찾아
        /// 그 하위의 VIRDYPostItController를 모두 비활성화(리셋)
        /// </summary>
        public void DeactivatePostIts(string actorNameSubstring)
        {
            ApplyToActor(actorNameSubstring, false);
        }

        private void ApplyToActor(string actorNameSubstring, bool active)
        {
            if (string.IsNullOrEmpty(actorNameSubstring))
            {
                return;
            }

            var roots = SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(r => r &&
                            r.name.IndexOf(actorNameSubstring,
                                           System.StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            foreach (var root in roots)
            {
                var controllers = root.GetComponentsInChildren<VIRDYPostItController>();
                foreach (var c in controllers)
                {
                    if (!c) continue;
                    c.SetAllActive(active);
                }
            }
        }
    }
}