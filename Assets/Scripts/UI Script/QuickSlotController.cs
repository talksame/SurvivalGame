using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour {

    [SerializeField] private Slot[] quickSlots; // 퀵슬롯들.
    [SerializeField] private Image[] img_CoolTime; // 퀵슬롯 쿨타임 이미지.
    [SerializeField] private Transform tf_parent; // 퀵슬롯의 부모 객체.

    [SerializeField] private Transform tf_ItemPos; // 아이템이 위치할 손 끝.
    public static GameObject go_HandItem; // 손에 든 아이템.

    // 쿨타임 내용.
    [SerializeField] private float coolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    // 등장 내용.
    [SerializeField] private float appearTime;
    private float currentAppearTime;
    private bool isAppear;

    private int selectedSlot; // 선택된 퀵슬롯. (0~7) = 8개.

    // 필요한 컴포넌트.
    [SerializeField]
    private GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지.
    [SerializeField]
    private WeaponManager theWeaponManager;
    private Animator anim;

	// Use this for initialization
	void Start () {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        anim = GetComponent<Animator>();
        selectedSlot = 0;
	}
	
	// Update is called once per frame
	void Update () {
        TryInputNumber();
        CoolTimeCalc();
        AppearCalc();
	}

    private void AppearReset()
    {
        currentAppearTime = appearTime;
        isAppear = true;
        anim.SetBool("Appear", isAppear);
    }

    private void AppearCalc()
    {
        if (Inventory.inventoryActivated)
            AppearReset();
        else
        {
            if (isAppear)
            {
                currentAppearTime -= Time.deltaTime;
                if (currentAppearTime <= 0)
                {
                    isAppear = false;
                    anim.SetBool("Appear", isAppear);
                }
            }
        }
        
    }

    private void CoolTimeReset()
    {
        currentCoolTime = coolTime;
        isCoolTime = true;
    }

    private void CoolTimeCalc()
    {
        if (isCoolTime)
        {
            currentCoolTime -= Time.deltaTime;
            for (int i = 0; i < img_CoolTime.Length; i++)
                img_CoolTime[i].fillAmount = currentCoolTime / coolTime;
            
            if (currentCoolTime <= 0)
                isCoolTime = false;
        }
    }

    private void TryInputNumber()
    {
        if (!isCoolTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                ChangeSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                ChangeSlot(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                ChangeSlot(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                ChangeSlot(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                ChangeSlot(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                ChangeSlot(7);
        }
    }

    public void IsActivatedQuickSlot(int _num)
    {
        if (selectedSlot == _num)
        {
            Execute();
            return;
        }
        if(DragSlot.instance != null)
            if(DragSlot.instance.dragSlot != null)
                if (DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
                    Execute();

    }

    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);
        Execute();
    }

    private void SelectedSlot(int _num)
    {
        // 선택된 슬롯;
        selectedSlot = _num;

        // 선택된 슬롯으로 이미지 이동.
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }



    private void Execute()
    {
        CoolTimeReset();
        AppearReset();

        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Equipment)
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            else if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Used || quickSlots[selectedSlot].item.itemType == Item.ItemType.Kit)
                ChangeHand(quickSlots[selectedSlot].item);
            else
                ChangeHand();
        }
        else
        {
            ChangeHand();
        }
    }

    private void ChangeHand(Item _item = null)
    {
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));

        if (_item != null)
        {
            StartCoroutine(HandItemCoroutine(_item));
        }

    }

    IEnumerator HandItemCoroutine(Item _item)
    {
        HandController.isActivate = false;

        yield return new WaitUntil(() => HandController.isActivate);

        if (_item.itemType == Item.ItemType.Kit)
            HandController.currentKit = _item;

        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_ItemPos.position, tf_ItemPos.rotation);
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;
        go_HandItem.GetComponent<BoxCollider>().enabled = false;
        go_HandItem.tag = "Untagged";
        go_HandItem.layer = 9; // Weapon
        go_HandItem.transform.SetParent(tf_ItemPos);
    }

    public void DecreaseSelectedItem()
    {
        CoolTimeReset();
        AppearReset();

        quickSlots[selectedSlot].SetSlotCount(-1);

        if (quickSlots[selectedSlot].itemCount <= 0)
            Destroy(go_HandItem);
    }

    public bool GetIsCoolTime()
    {
        return isCoolTime;
    }

    public Slot GetSelectedSlot()
    {
        return quickSlots[selectedSlot];
    }
}
