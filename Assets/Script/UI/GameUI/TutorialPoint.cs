using UnityEngine;

public class TutorialPoint : MonoBehaviour
{
    public static TutorialPoint Instance { get; private set; }
    public Transform parryTutorialPoint;
    public Transform basicSkillTutorialPoint;
    public Transform dropItemTutorialPoint;
    public Transform stunTutorialPoint;
    public Transform blockTutorialPoint;
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        
    }
}

