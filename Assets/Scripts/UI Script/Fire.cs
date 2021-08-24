using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {

    [SerializeField] private string fireName; // 불의 이름. 난로, 모닥불, 화톳불

    [SerializeField] private int damage; // 불의 데미지.

    [SerializeField] private float damageTime; // 데미지가 들어갈 딜레이.
    private float currentDamageTime;

    [SerializeField] private float durationTime; // 불의 지속시간.
    private float currentDurationTime;

    [SerializeField]
    private ParticleSystem ps_Flame; // 파티클 시스템.

    // 필요한 컴포넌트
    private StatusController thePlayerStatus;

    //상태변수
    private bool isFire = true;

    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentDurationTime = durationTime;
    }

    // Update is called once per frame
    void Update() {
        if (isFire)
        {
            ElapseTime();
        }
    }

    private void ElapseTime()
    {
        currentDurationTime -= Time.deltaTime;

        if (currentDamageTime > 0)
            currentDamageTime -= Time.deltaTime;

        if(currentDurationTime <= 0)
        {
            Off();
        }
    }
    
    private void Off()
    {
        ps_Flame.Stop();
        isFire = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if(isFire && other.transform.tag == "Player")
        {
            if(currentDamageTime <= 0)
            {
                other.GetComponent<Burn>().StartBurning();
                thePlayerStatus.DecreaseHP(damage);
                currentDamageTime = damageTime;
            }
        }
    }

    public bool GetIsFire()
    {
        return isFire;
    }
}
