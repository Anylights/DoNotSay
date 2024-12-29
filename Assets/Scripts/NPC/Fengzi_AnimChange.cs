using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fengzi_AnimChange : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    public AutoNPCManager autonpcManager;

    public Collider2D Fengzi_collider2D;
    public Collider2D Fengzi_Dialogue_collider;
    void Start()
    {
        anim = GetComponent<Animator>();
        autonpcManager.enabled = false;
        Fengzi_collider2D = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        EventCenter.Instance.Subscribe("FengziWordTriggered", ChangeAnim);
    }

    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("FengziWordTriggered", ChangeAnim);
    }

    void ChangeAnim()
    {
        StartCoroutine(ChangeAnimCoroutine());
    }

    IEnumerator ChangeAnimCoroutine()
    {
        autonpcManager.enabled = true;
        Fengzi_collider2D.isTrigger = true;
        Fengzi_Dialogue_collider.enabled = true;
        yield return new WaitForSeconds(1.5f);
        anim.SetBool("IsAwaken", true);
    }
}
