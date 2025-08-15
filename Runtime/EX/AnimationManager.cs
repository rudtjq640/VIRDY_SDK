using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimationTarget
    {
        public string ObjectName;
        public string OpenTrigger = "Open";
        public string CloseTrigger = "Close";

        [HideInInspector] public Animator animator;
        [HideInInspector] public bool isFound = false;
        [HideInInspector] public bool isOpen = true;
    }

    public List<AnimationTarget> targets = new List<AnimationTarget>();

    void Start()
    {
        foreach (var t in targets)
        {
            t.isFound = false;
            t.animator = null;
        }
    }

    void Update()
    {
        foreach (var t in targets)
        {
            if (!t.isFound)
                FindAndCacheAnimator(t);
        }
    }

    private void FindAndCacheAnimator(AnimationTarget target)
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var tr in root.GetComponentsInChildren<Transform>(true))
            {
                if (tr.name == target.ObjectName)
                {
                    var anim = tr.GetComponent<Animator>();
                    if (anim != null)
                    {
                        target.animator = anim;
                        target.isFound = true;
                        return;
                    }
                }
            }
        }
    }

    public void PlayOpenAnimation(string ObjectName)
    {
        var t = targets.Find(x => x.ObjectName == ObjectName);
        if (t != null && t.isFound)
        {
            if (t.isOpen) return;
            else
            {
                t.animator.SetTrigger(t.OpenTrigger);
                t.isOpen = true;
            }
        }
        else
            Debug.LogWarning($"[AnimationManager] '{ObjectName}' or its Animator not found.");
    }

    public void PlayCloseAnimation(string ObjectName)
    {
        var t = targets.Find(x => x.ObjectName == ObjectName);
        if (t != null && t.isFound)
        {
            if (!t.isOpen) return;
            else
            {
                t.animator.SetTrigger(t.CloseTrigger);
                t.isOpen = false;
            }
        }
        else
            Debug.LogWarning($"[AnimationManager] '{ObjectName}' or its Animator not found.");
    }

    public void PlayTrigger(string ObjectName, string triggerName)
    {
        var t = targets.Find(x => x.ObjectName == ObjectName);
        if (t != null && t.isFound)
            t.animator.SetTrigger(triggerName);
        else
            Debug.LogWarning($"[AnimationManager] '{ObjectName}' or its Animator not found.");
    }
}
