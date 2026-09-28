using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManagement : MonoBehaviour
{
    public static EquipmentManagement Instance { get; private set; }

    [SerializeField] private RawImage previewPlayerWindow;

    [SerializeField] private Image weapon1Selection;
    [SerializeField] private Image weapon2Selection;
    [SerializeField] private ScrollRect weaponScroller;
    [SerializeField] private Transform weaponSlotContainer;

    [SerializeField] private Image spell1Selection;
    [SerializeField] private Image spell2Selection;
    [SerializeField] private ScrollRect spellScroller;
    [SerializeField] private Transform spellSlotContainer;
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject itemSpellSlotTemplate;
    [SerializeField] private GameObject itemWeaponSlotTemplate;
    [SerializeField] private Button QuitButton;
    public List<ItemSO> equipItemList = new List<ItemSO>();
    private List<WeaponSO> weaponSOList => GameManager.Instance.weaponSOList;
    private List<SpellSO> spellSOList => GameManager.Instance.spellSOList;

    [SerializeField] private Transform previewSpot;
    [SerializeField] private GameObject player;
    private Player spawnedPlayer;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        if (QuitButton != null)
        {
            QuitButton.onClick.AddListener(ReturnToMainMenu);
        }
    }
    private void OnEnable()
    {
        
        if (spawnedPlayer == null && player != null && previewSpot != null)
        {
            spawnedPlayer = Instantiate(
                player,
                previewSpot.position,
                Quaternion.identity,
                previewSpot
            ).GetComponent<Player>();

            spawnedPlayer.transform.localPosition = Vector3.zero;
            spawnedPlayer.transform.localRotation = Quaternion.identity;
            spawnedPlayer.transform.localScale = Vector3.one;

           

        }

        if (spawnedPlayer != null)
        {
            spawnedPlayer.gameObject.SetActive(true);
        }

        RefreshEquipment();
        RefreshInventoryItem();
    }

    private void OnDisable()
    {
        if (spawnedPlayer != null)
        {
            spawnedPlayer.gameObject.SetActive(false);
        }
    }
    private void RefreshPreviewEquipment()
    {
        if (spawnedPlayer == null)
            return;

        PlayerMovement actualPlayer = PlayerMovement.Instance;
        PlayerMovement previewPlayer =
            spawnedPlayer.GetComponent<PlayerMovement>();

        if (actualPlayer == null || previewPlayer == null)
            return;

        previewPlayer.weapon1 = actualPlayer.weapon1;
        previewPlayer.weapon2 = actualPlayer.weapon2;

        previewPlayer.spell1 = actualPlayer.spell1;
        previewPlayer.spell2 = actualPlayer.spell2;

        previewPlayer.currentWeapon =
            previewPlayer.weapon1;

        previewPlayer.currentSpell =
            previewPlayer.spell1;

        previewPlayer.SpawnWeapon();
        previewPlayer.SpawnSpell();

        RefreshPreviewLayers();
    }
    public void RefreshEquipment()
    {
        if (PlayerMovement.Instance == null)
            return;
        GameManager.Instance.RefreshEquippedItem();
        Weapon weapon1 = PlayerMovement.Instance.weapon1;
        Weapon weapon2 = PlayerMovement.Instance.weapon2;
        Spell spell1 = PlayerMovement.Instance.spell1;
        Spell spell2 = PlayerMovement.Instance.spell2;

        equipItemList.Clear();

        SetItemSprite(
            weapon1Selection,
            weapon1 != null ? weapon1.weaponSO.sprite : null
        );

        SetItemSprite(
            weapon2Selection,
            weapon2 != null ? weapon2.weaponSO.sprite : null
        );

        SetItemSprite(
            spell1Selection,
            spell1 != null ? spell1.spellSO.sprite : null
        );

        SetItemSprite(
            spell2Selection,
            spell2 != null ? spell2.spellSO.sprite : null
        );

        if (weapon1 != null)
            equipItemList.Add(weapon1.weaponSO);

        if (weapon2 != null)
            equipItemList.Add(weapon2.weaponSO);

        if (spell1 != null)
            equipItemList.Add(spell1.spellSO);

        if (spell2 != null)
            equipItemList.Add(spell2.spellSO);

        RefreshPreviewEquipment();
    }

    public void RefreshInventoryItem()
    {
        ClearContainer(weaponSlotContainer);
        ClearContainer(spellSlotContainer);
        
        List<string> itemIDList = GameManager.Instance.LoadPurchasedItems();

        if (itemIDList == null)
            return;

        foreach (WeaponSO weaponSO in weaponSOList)
        {
            if (itemIDList.Contains(weaponSO.itemID))
            {
                CreateWeaponItemSlot(
                    weaponSO.sprite,weaponSO.itemID,weaponSO.weaponObject.name,
                    weaponSlotContainer
                );
            }
        }

        foreach (SpellSO spellSO in spellSOList)
        {
            if (itemIDList.Contains(spellSO.itemID))
            {
                CreateSpellItemSlot(
                    spellSO.sprite,spellSO.itemID, spellSO.spellPrefab.name,
                    spellSlotContainer
                );
            }
        }
    }

    private void CreateWeaponItemSlot(Sprite sprite,string id,string name, Transform parent)
    {
        GameObject itemSlot = Instantiate(itemWeaponSlotTemplate, parent);
        EquipmentSlot equipmentSlot = itemSlot.GetComponent<EquipmentSlot>();
        equipmentSlot.Setup(id);
        itemSlot.gameObject.SetActive(true);

        EquipmentSlot IS = itemSlot.GetComponent<EquipmentSlot>();
        Image image = IS.itemSprite;
        IS.itemName.text = name;

        if (image == null)
            return;

        image.sprite = sprite;
        image.enabled = sprite != null;
    }
    private void CreateSpellItemSlot(Sprite sprite, string id, string name, Transform parent)
    {
        GameObject itemSlot = Instantiate(itemSpellSlotTemplate, parent);
        EquipmentSlot equipmentSlot = itemSlot.GetComponent<EquipmentSlot>();
        equipmentSlot.Setup(id);
        
        itemSlot.gameObject.SetActive(true);
        EquipmentSlot IS = itemSlot.GetComponent<EquipmentSlot>();
        Image image = IS.itemSprite;
        IS.itemName.text = name;

        if (image == null)
            return;

        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    private void ClearContainer(Transform container)
    {
        if (container == null)
            return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    public void SetItemSprite(Image targetImage, Sprite sprite)
    {
        if (targetImage == null)
            return;

        if (sprite != null)
        {
            targetImage.sprite = sprite;
            targetImage.enabled = true;
        }
        else
        {
            targetImage.sprite = null;
            targetImage.enabled = false;
        }
    }

    public void SelectItem(string index)
    {
        foreach (WeaponSO weaponSO in weaponSOList)
        {
            if (weaponSO.itemID == index)
            {
                PlayerMovement.Instance.EquipWeapon(
                    weaponSO.weaponObject.GetComponent<Weapon>()
                );

                GameManager.Instance.SaveEquippedItemList();

                RefreshEquipment();
                return;
            }
        }

        foreach (SpellSO spellSO in spellSOList)
        {
            if (spellSO.itemID == index)
            {
                PlayerMovement.Instance.EquipSpell(
                    spellSO.spellPrefab.GetComponent<Spell>()
                );

                GameManager.Instance.SaveEquippedItemList();

                RefreshEquipment();
                return;
            }
        }
    }
    public void UnselectItem(string index)
    {
        foreach (WeaponSO weaponSO in weaponSOList)
        {
            if (weaponSO.itemID == index)
            {
                PlayerMovement.Instance.UnequipWeapon(
                    weaponSO.itemID
                );

                GameManager.Instance.SaveEquippedItemList();

                RefreshEquipment();
                return;
            }
        }

        foreach (SpellSO spellSO in spellSOList)
        {
            if (spellSO.itemID == index)
            {
                PlayerMovement.Instance.UnequipSpell(
                    spellSO.itemID
                );

                GameManager.Instance.SaveEquippedItemList();

                RefreshEquipment();
                return;
            }
        }
    }
    public void SetSelected(bool selected)
    {
        //selectionBorder.SetActive(selected);
    }
    public void RefreshPreviewLayers()
    {
        int previewLayer = LayerMask.NameToLayer("PreviewLayer");

        if (previewLayer == -1 || spawnedPlayer == null)
            return;

        SetLayerRecursively(
            spawnedPlayer.gameObject,
            previewLayer
        );

        PlayerMovement pm = spawnedPlayer.GetComponent<PlayerMovement>();

        if (pm == null)
            return;

        if (pm.currentWeapon != null)
        {
            SetLayerRecursively(
                pm.currentWeapon.gameObject,
                previewLayer
            );
        }

        if (pm.currentSpell != null)
        {
            SetLayerRecursively(
                pm.currentSpell.gameObject,
                previewLayer
            );
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(
                child.gameObject,
                newLayer
            );
        }
    }
    public void ReturnToMainMenu()
    {
        gameObject.SetActive(false);
        mainMenuContainer.GetComponent<MainMenuManager>().ShowMenu();
        GameManager.Instance.StartGameSetUp();
        
    }
   
}