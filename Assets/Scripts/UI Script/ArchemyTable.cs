using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ArchemyItem
{
    public string itemName;
    public string itemDesc;
    public Sprite itemImage;

    public string[] needItemName;
    public int[] needItemNumber;

    public float itemCraftingTime; // 포션 제조에 걸리는 시간 (5초, 10초, 100초)

    public GameObject go_ItemPrefab;
}

public class ArchemyTable : MonoBehaviour {

    public bool GetIsOpen()
    {
        return isOpen;
    }

    private bool isOpen = false;
    private bool isCrafting = false; // 아이템의 제작 시작 여부.

    [SerializeField] private ArchemyItem[] archemyItems; // 제작할 수 있는 연금 아이템 리스트.
    private Queue<ArchemyItem> archemyItemQueue = new Queue<ArchemyItem>(); // 연금 아이템 제작 대기열 (큐)
    private ArchemyItem currentCraftingItem; // 현재 제작중인 연금 아이템.

    private float craftingTime; // 포션 제작 시간.
    private float currentCraftingTime; // 실제 계산.
    private int page = 1; // 연금 제작 테이블의 페이지.
    [SerializeField] private int theNumberOfSlot; // 한 페이지당 슬롯의 최대 개수(4개)

    [SerializeField] private Image[] image_ArchemyItems; // 페이지에 따른 포션 이미지들
    [SerializeField] private Text[] text_ArchemyItems; // 페이지에 따른 포션 텍스트들.
    [SerializeField] private Button[] btn_ArchemyItems; // 페이지에 따른 포션 버튼.
    [SerializeField] private Slider slider_gauge; // 슬라이더 게이지.
    [SerializeField] private Transform tf_BaseUi; // 베이스 UI
    [SerializeField] private Transform tf_PotionAppearPos; // 포션 나올 위치.
    [SerializeField] private GameObject go_Liquid; // 동작시키면 액체 등장.
    [SerializeField] private Image[] image_CraftingItems; // 대기열 슬롯에 있는 아이템 이미지들.


    //필요한 컴포넌트
    [SerializeField] private ArchemyToolTip theToolTip;
    private AudioSource theAudio;
    private Inventory theInven;
    [SerializeField] private AudioClip sound_ButtonClick;
    [SerializeField] private AudioClip sound_Beep;
    [SerializeField] private AudioClip sound_Activate;
    [SerializeField] private AudioClip sound_ExitItem;


    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
        ClearSlot();
        PageSetting();
    }

    void Update()
    {
        if (!IsFinish())
        {
            Crafting();
        }

        if(isOpen)
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseWindow();
    }

    private bool IsFinish()
    {
        if(archemyItemQueue.Count == 0 && !isCrafting)
        {
            go_Liquid.SetActive(false);
            slider_gauge.gameObject.SetActive(false);
            return true;
        }
        else
        {
            go_Liquid.SetActive(true);
            slider_gauge.gameObject.SetActive(true);
            return false;
        }
    }

    private void Crafting()
    {
        if (!isCrafting && archemyItemQueue.Count != 0)
            DequeueItem();

        if (isCrafting)
        {
            currentCraftingTime += Time.deltaTime;
            slider_gauge.value = currentCraftingTime;

            if (currentCraftingTime >= craftingTime)
                ProductionComplete();
        }
    }

    private void DequeueItem()
    {
        PlaySE(sound_Activate);
        isCrafting = true;
        currentCraftingItem = archemyItemQueue.Dequeue();

        craftingTime = currentCraftingItem.itemCraftingTime;
        currentCraftingTime = 0;
        slider_gauge.maxValue = craftingTime;

        CraftingImageChange();
    }

    private void CraftingImageChange()
    {
        image_CraftingItems[0].gameObject.SetActive(true);

        // 위에서 Dequeue를 했으므로 Count에 1을 더함.
        for (int i = 0; i < archemyItemQueue.Count + 1; i++)
        {
            image_CraftingItems[i].sprite = image_CraftingItems[i + 1].sprite;
            if (i + 1 == archemyItemQueue.Count + 1)
                image_CraftingItems[i + 1].gameObject.SetActive(false);
        }
    }

    public void Window()
    {
        isOpen = !isOpen;

        if (isOpen)
            OpenWindow();
        else
            CloseWindow();
    }

    private void OpenWindow()
    {
        GameManager.isOpenArchemyTable = true;
        tf_BaseUi.localScale = new Vector3(1f, 1f, 1f);
    }

    private void CloseWindow()
    {
        isOpen = false;
        GameManager.isOpenArchemyTable = false;
        tf_BaseUi.localScale = new Vector3(0f, 0f, 0f);
    }


    public void ButtonClick(int _buttonNum)
    {

        PlaySE(sound_ButtonClick);

        if (archemyItemQueue.Count < 3)
        {
            int archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);

            // 인벤토리에서 재료 검색
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                if(theInven.GetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i]) < archemyItems[archemyItemArrayNumber].needItemNumber[i])
                {
                    PlaySE(sound_Beep);
                    return;
                }
            }

            // 인벤토리 재료 감산.
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                theInven.SetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i], archemyItems[archemyItemArrayNumber].needItemNumber[i]);
            }

            // 제작 시작
            archemyItemQueue.Enqueue(archemyItems[archemyItemArrayNumber]);

            image_CraftingItems[archemyItemQueue.Count].gameObject.SetActive(true);
            image_CraftingItems[archemyItemQueue.Count].sprite = archemyItems[archemyItemArrayNumber].itemImage;
        }
        else
        {
            PlaySE(sound_Beep);
        }
    }

    private void ProductionComplete()
    {
        isCrafting = false;
        image_CraftingItems[0].gameObject.SetActive(false);

        PlaySE(sound_ExitItem);

        Instantiate(currentCraftingItem.go_ItemPrefab, tf_PotionAppearPos.position, Quaternion.identity);
    }

    
    public void UpButton()
    {
        PlaySE(sound_ButtonClick);

        if (page != 1)
            page--;
        else
            page = 1 + (archemyItems.Length / theNumberOfSlot);

        ClearSlot();
        PageSetting();
    }
    public void DownButton()
    {
        PlaySE(sound_ButtonClick);

        if (page < 1 + (archemyItems.Length / theNumberOfSlot))
            page++;
        else
            page = 1;

        ClearSlot();
        PageSetting();
    }

    private void ClearSlot()
    {
        for (int i = 0; i < theNumberOfSlot; i++)
        {
            image_ArchemyItems[i].sprite = null;
            image_ArchemyItems[i].gameObject.SetActive(false);
            btn_ArchemyItems[i].gameObject.SetActive(false);
            text_ArchemyItems[i].text = "";
        }
    }

    private void PageSetting()
    {
        int pageArrayStartNumber = (page - 1) * theNumberOfSlot; // 0, 4, 8, 12

        for (int i = pageArrayStartNumber; i < archemyItems.Length; i++)
        {
            if (i == page * theNumberOfSlot)
                break;

            image_ArchemyItems[i - pageArrayStartNumber].sprite = archemyItems[i].itemImage;
            image_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            btn_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            text_ArchemyItems[i - pageArrayStartNumber].text = archemyItems[i].itemName + "\n" + archemyItems[i].itemDesc;
        }
    }

    public void ShowToolTip(int _buttonNum)
    {
        int _archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);
        theToolTip.ShowTooltip(archemyItems[_archemyItemArrayNumber].needItemName, archemyItems[_archemyItemArrayNumber].needItemNumber);
    }

    public void HideToolTip()
    {
        theToolTip.HideToolTip();
    }
}
