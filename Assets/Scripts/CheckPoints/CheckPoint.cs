using System.Collections;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [HideInInspector] public int index;
    private Animator anim;
    private CheckPointManager manager;
    [SerializeField] private AudioClip activateAudio;
    [HideInInspector] public bool isActive;


    private void Awake() {
        anim = GetComponent<Animator>();
        manager = GetComponentInParent<CheckPointManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log(DataManager.Instance.isLoaded);
        anim.SetTrigger("activate");
        SFXManager.Instance.PlaySound(activateAudio);
        GetComponent<BoxCollider2D>().enabled = false;
        manager.SetActivePoint(transform);
        manager.DeactivateOldFlags(index);
        other.GetComponent<Health>().Heal(3);
        if(!DataManager.Instance.isLoaded) {
            StartCoroutine(Save());
            Debug.Log("SAVED");
        }
        else 
            DataManager.Instance.isLoaded = false;
    }

    private IEnumerator Save() {
        yield return new WaitForSeconds(0.2f);
        DataManager.Instance.SaveData();
        yield return null;
    }
}
