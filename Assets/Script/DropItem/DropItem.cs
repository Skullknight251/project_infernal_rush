using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum Type
    {
        Soul,
        Stamina,
        Mana,
        Health
    }
    private float speed = 20f;
    private float lifeTimeMax = 4f;
    private float lifeTime;
    private Player player;
    [SerializeField] private int amountToAdd;
    [SerializeField] private Type type;
    void Start()
    {
        lifeTime = 0;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            this.player = player;
            switch (type)
            {
                case Type.Health:
                    Player.Instance.AddHealth(amountToAdd);
                    
                    break;
                case Type.Stamina:
                    Player.Instance.AddStamina(amountToAdd);
                    
                    break;
                case Type.Mana:
                    Player.Instance.AddMana(amountToAdd);
                    
                    break;
                case Type.Soul:
                    player.AddSoul(amountToAdd);
                    break;
                default:
                    break;
            }
            
            Destroy(gameObject);
        }
        
        return;
    }

    

    void Update()
    {
        if (Player.Instance != null) {
            transform.position = Vector2.MoveTowards(transform.position,Player.Instance.transform.position,speed * Time.deltaTime);
        }
        lifeTime += Time.deltaTime;
        if (lifeTime > lifeTimeMax) { 
        
            Destroy(gameObject);
        }
    }
}
