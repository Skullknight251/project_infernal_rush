using UnityEngine;

public class Skill : MonoBehaviour
{

    public enum ControlType
    {
        Touch,
        CanCharging
    }

    [SerializeField] private ControlType controlType;
    [SerializeField] private SkillSO skillSO;
    void Start()
    {
        
    }

    public ControlType GetControlType()
    {
        return controlType;
    }
    void Update()
    {
        
    }
}
