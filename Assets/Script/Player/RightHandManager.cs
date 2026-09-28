using UnityEngine;

public class RightHandManager : MonoBehaviour
{
    public static RightHandManager Instance { get; private set; }
    public Animator animator;

    public void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
