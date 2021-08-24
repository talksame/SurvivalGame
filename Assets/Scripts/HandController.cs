using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class HandController : CloseWeaponController
{
    // 활성화 여부.
    public static bool isActivate = false;
    public static Item currentKit; // 설치하려는 킷 (연금 테이블)

    private bool isPreview = false;

    private GameObject go_preview; // 설치할 키트 프리뷰.
    private Vector3 previewPos; // 설치할 키트 위치.
    [SerializeField] private float rangeAdd; // 건축시 추가 사정거리.

    [SerializeField]
    private QuickSlotController theQuickSlot;

    // Update is called once per frame
    void Update()
    {
        if (isActivate && !Inventory.inventoryActivated)
        {
            if(currentKit == null)
            {
                if (QuickSlotController.go_HandItem == null)
                    TryAttack();
                else
                    TryEating();
            }
            else
            {
                if (!isPreview)
                    InstallPreviewKit();
                PreviewPositionUpdate();
                Build();
            }
        }       
    }
    private void InstallPreviewKit()
    {
        isPreview = true;
        go_preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    private void PreviewPositionUpdate()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_preview.transform.position = previewPos;
        }
    }
    
    private void Build()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (go_preview.GetComponent<PreviewObject>().IsBuildable())
            {
                theQuickSlot.DecreaseSelectedItem(); // 슬롯 아이템 개수 -1;
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    public void Cancel()
    {
        Destroy(go_preview);
        currentKit = null;
        isPreview = false;
    }

    private void TryEating()
    {
        if (Input.GetButtonDown("Fire1") && !theQuickSlot.GetIsCoolTime())
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                isSwing = false;
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
