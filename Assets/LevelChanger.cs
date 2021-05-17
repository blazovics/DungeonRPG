using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            FadeToLevel(1);
        }
    }
    
    public void FadeToLevel(int level)
    {
        animator.SetTrigger("FadeIntoBlack");
    }
}
