using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string PURCHASED_ITEM_LIST_PLAYERPREF = "purchasedItemListPlayerPref";
    public string EQUIPPED_ITEM_LIST_PLAYERPREF = "equippedItemListPlayerPref";
    public string SOULS = "souls";
    public List<WeaponSO> weaponSOList;
    public List<SpellSO> spellSOList;
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject statContainer;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private GameObject pauseContainer;
    [SerializeField] private Transform playerStartPoint;
    [SerializeField] private Transform bossStartPoint;
    [SerializeField] private GameObject bossPrefab;
    public GameObject playerPrefab;
    [SerializeField] private GameObject gameOverWindow;
    
     private PurchasedItemData purchasedItemData = new PurchasedItemData();
    private bool isGameOver;
    public bool isPlayingGame;
    private BossEnemy boss;
    private Player player;
    public void Awake()
    {
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        Instance = this;
        //DeleteItem("celestial");
        //DeleteItem("wind");
        SetupDefaultPurchasedItems();
    }
    void Start()
    {
        if (EquipmentManagement.Instance != null)
        {
            EquipmentManagement.Instance.RefreshInventoryItem();
        }
        //if (LoadCurrentSouls() <= 0)
        //{
            //UpdateSouls(100000);
        //}
        
        GameInput.Instance.onEscape += GameInput_onEscape;
        StartGameSetUp();
    }

    private void GameInput_onEscape(object sender, System.EventArgs e)
    {
        PauseGame();
    }

    public void PauseGame()
    {
        if (isPlayingGame)
        {
            if (PlayerMovement.Instance.CanRun == true)
            {
                PlayerMovement.Instance.SetCanRun(false);
                pauseContainer.SetActive(true);
            }
            else
            {
                PlayerMovement.Instance.SetCanRun(true);
                pauseContainer.SetActive(false);
            }
        }
        
    }

    private void Update()
    {
        if (player != null)
        {
            if (player.state == Player.PlayerState.Died && !isGameOver)
            {
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        isPlayingGame = false;
        statContainer.SetActive(false);
        buttonContainer.SetActive(false);
        pauseContainer.SetActive(false);
        gameOverWindow.SetActive(true);
        int totalSouls = player.GetSoulsEarned();
        UpdateSouls(totalSouls);
        gameOverWindow.GetComponent<GameOverManager>().SetSoulsAmount(totalSouls);
    }

    public void StartIntro()
    {
        BossEnemy.Instance.StartIntro();
        CameraManager.Instance.StartFollowingBoss();
        PlayerMovement.Instance.SetCanRun(true);
        LoadPurchasedItems();
        RefreshEquippedItem();
    }
    public void StartGameplay()
    {
        CameraManager.Instance.StartFollowingPlayer();
        statContainer.SetActive(true);
        buttonContainer.SetActive(true);
        StartCoroutine(BasicSkillTutorial());
    }

    public IEnumerator BasicSkillTutorial()
    {
        yield return new WaitForSeconds(3);
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.basicSkill))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.basicSkill, TutorialPoint.Instance.basicSkillTutorialPoint);
        }
    }

    public void itemListUpdate()
    {
        PlayerPrefs.Save();
    }

    public void AddPurchasedItem(ItemSO itemSO)
    {
        if (itemSO == null) return;

        if (purchasedItemData.itemIDs.Contains(itemSO.itemID))
            return;

        purchasedItemData.itemIDs.Add(itemSO.itemID);
        SavePurchasedItems();
    }

    public void DeleteItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return;

        LoadPurchasedItems();

        if (purchasedItemData.itemIDs.Contains(itemID))
        {
            purchasedItemData.itemIDs.Remove(itemID);
            SavePurchasedItems();
            Debug.Log("Deleted: " + itemID);
        }
    }

    public void UpdateSouls(int soulAmount)
    {
        int currentSouls;
        if (!PlayerPrefs.HasKey(SOULS))
        {
            currentSouls = 0;
        }
        currentSouls = LoadCurrentSouls();
        currentSouls += soulAmount; 
        PlayerPrefs.SetInt(SOULS,currentSouls);
    }



    public int LoadCurrentSouls()
    {
        return PlayerPrefs.GetInt(SOULS);
    }

    public void SaveEquippedItemList()
    {
        EquippedItemData data = new EquippedItemData();

        if (PlayerMovement.Instance.weapon1 != null)
            data.weapon1ID = PlayerMovement.Instance.weapon1.weaponSO.itemID;

        if (PlayerMovement.Instance.weapon2 != null)
            data.weapon2ID = PlayerMovement.Instance.weapon2.weaponSO.itemID;

        if (PlayerMovement.Instance.spell1 != null)
            data.spell1ID = PlayerMovement.Instance.spell1.spellSO.itemID;

        if (PlayerMovement.Instance.spell2 != null)
            data.spell2ID = PlayerMovement.Instance.spell2.spellSO.itemID;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(EQUIPPED_ITEM_LIST_PLAYERPREF, json);
        PlayerPrefs.Save();
    }
    public void RefreshEquippedItem()
    {
        if (!PlayerPrefs.HasKey(EQUIPPED_ITEM_LIST_PLAYERPREF)) return;

        string json = PlayerPrefs.GetString(EQUIPPED_ITEM_LIST_PLAYERPREF);
        EquippedItemData data = JsonUtility.FromJson<EquippedItemData>(json);

        if (data == null) return;
        
        PlayerMovement.Instance.weapon1 = null;
        PlayerMovement.Instance.weapon2 = null;
        PlayerMovement.Instance.spell1 = null;
        PlayerMovement.Instance.spell2 = null;

        foreach (WeaponSO weaponSO in weaponSOList)
        {
            if (weaponSO == null || weaponSO.weaponObject == null) continue;

            if (!string.IsNullOrEmpty(data.weapon1ID) && weaponSO.itemID == data.weapon1ID)
                PlayerMovement.Instance.weapon1 = weaponSO.weaponObject.GetComponent<Weapon>();

            if (!string.IsNullOrEmpty(data.weapon2ID) && weaponSO.itemID == data.weapon2ID)
                PlayerMovement.Instance.weapon2 = weaponSO.weaponObject.GetComponent<Weapon>();
        }
        foreach (SpellSO spellSO in spellSOList)
        {
            if (spellSO == null || spellSO.spellPrefab == null) continue;

            if (!string.IsNullOrEmpty(data.spell1ID) && spellSO.itemID == data.spell1ID)
                PlayerMovement.Instance.spell1 = spellSO.spellPrefab.GetComponent<Spell>();

            if (!string.IsNullOrEmpty(data.spell2ID) && spellSO.itemID == data.spell2ID)
                PlayerMovement.Instance.spell2 = spellSO.spellPrefab.GetComponent<Spell>();
        }
        PlayerMovement.Instance.RefreshCurrentItem();
    }

    public bool HasPurchasedItem(ItemSO itemSO)
    {
        if (itemSO == null) return false;
        return purchasedItemData.itemIDs.Contains(itemSO.itemID);
    }
    public void SavePurchasedItems()
    {
        string json = JsonUtility.ToJson(purchasedItemData);
        PlayerPrefs.SetString(PURCHASED_ITEM_LIST_PLAYERPREF, json);
        PlayerPrefs.Save();
    }

    public void SetPlayer(Player player)
    {
        if (player == null) return;
        this.player = player;
    }
    public void StartGameSetUp()
    {
        isGameOver = false;
        statContainer.SetActive(false);
        buttonContainer.SetActive(false);
        pauseContainer.SetActive(false);
        isPlayingGame = false;
        ResetBoss();
        ResetPlayer();
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.ResetCamera();
        }
        
        boss = Instantiate(bossPrefab,bossStartPoint.position,bossStartPoint.rotation).GetComponent<BossEnemy>();

        player = Instantiate(playerPrefab,playerStartPoint.position,playerStartPoint.rotation).GetComponent<Player>();
        
        RefreshEquippedItem();
        PlayerMovement.Instance.RefreshCurrentItem();
    }
    public List<string> LoadPurchasedItems()
    {
        if (!PlayerPrefs.HasKey(PURCHASED_ITEM_LIST_PLAYERPREF))
            return new List<string>();

        string json = PlayerPrefs.GetString(PURCHASED_ITEM_LIST_PLAYERPREF);
        purchasedItemData = JsonUtility.FromJson<PurchasedItemData>(json);
        
        if (purchasedItemData == null)
            purchasedItemData = new PurchasedItemData();
        return purchasedItemData.itemIDs;
    }
    private void ResetBoss()
    {
        BossEnemy oldBoss = boss;

        if (oldBoss == null)
        {
            oldBoss = BossEnemy.Instance;
        }

        if (oldBoss == null)
            return;

        oldBoss.ResetBoss();
        oldBoss.ClearInstance();

        Destroy(oldBoss.gameObject);

        boss = null;
    }
    public void ResetPlayer()
    {
        Player oldPlayer = player;

        if (oldPlayer == null)
        {
            oldPlayer = Player.Instance;
        }

        if (oldPlayer == null)
            return;

        oldPlayer.ClearInstance();

        Destroy(oldPlayer.gameObject);

        player = null;
    }

    private void SetupDefaultPurchasedItems()
    {
        LoadPurchasedItems();

        if (purchasedItemData.itemIDs.Count > 0)
            return;

        purchasedItemData.itemIDs.Add("big_sword");
        purchasedItemData.itemIDs.Add("fire");
        SavePurchasedItems();
    }
}

