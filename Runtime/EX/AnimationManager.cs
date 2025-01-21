using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationManager : MonoBehaviour
{
    private Animator _animator;
    private string targetObjectName = "JingAnim";
    private bool objectFound = false;

    void Start()
    {
        // 시작할 때 오브젝트를 찾음
        FindTargetObject();
    }

    void Update()
    {
        // 오브젝트가 아직 발견되지 않은 경우에만 지속적으로 탐색
        if (!objectFound)
        {
            FindTargetObject();
        }
    }

    private void FindTargetObject()
    {
        // 현재 활성화된 씬에서 오브젝트를 찾음
        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var transform in rootGameObject.GetComponentsInChildren<Transform>())
            {
                if (transform.name == targetObjectName)
                {
                    _animator = transform.GetComponent<Animator>();
                    if (_animator != null)
                    {
                        objectFound = true; // 오브젝트를 찾았으므로 다시 탐색하지 않음
                        Debug.Log($"{targetObjectName} 오브젝트와 Animator를 찾았습니다.");
                        return; // 오브젝트를 찾았으면 더 이상 반복하지 않음
                    }
                }
            }
        }
    }

    public void PlayOpenAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Open");
            Debug.Log("Open 애니메이션을 재생합니다.");
        }
    }

    public void PlayCloseAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Close");
            Debug.Log("Close 애니메이션을 재생합니다.");
        }
    }
}
