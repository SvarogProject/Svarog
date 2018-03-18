using UnityEngine;
using System.Collections;

public class DamageColliderHandler : MonoBehaviour {
    //public DCtype dcType;

    public GameObject[] DamageCollidersLeft;
    public GameObject[] DamageColldersRight;

    public enum DamageType {
        Light,
        Heavy
    }

    public enum DCtype {
        Bottom,
        Up
    }

    private PlayerStateManager _states;

    public void Start() {
        _states = GetComponent<PlayerStateManager>();
        CloseColliders();
    }

    public void OpenCollider(DCtype type, float delay, DamageType damageType) {
        if (!_states.LookRight) {
            switch (type) {
                case DCtype.Bottom:
                    StartCoroutine(OpenCollider(DamageCollidersLeft, 0, delay, damageType));
                    break;
                case DCtype.Up:
                    StartCoroutine(OpenCollider(DamageCollidersLeft, 1, delay, damageType));
                    break;
            }
        }
        else {
            switch (type) {
                case DCtype.Bottom:
                    StartCoroutine(OpenCollider(DamageColldersRight, 0, delay, damageType));
                    break;
                case DCtype.Up:
                    StartCoroutine(OpenCollider(DamageColldersRight, 1, delay, damageType));
                    break;
            }
        }
    }

    private IEnumerator OpenCollider(GameObject[] array, int index, float delay, DamageType damageType) {
        yield return new WaitForSeconds(delay);
        array[index].SetActive(true);
        array[index].GetComponent<DoDamage>().DamageType = damageType;
    }

    public void CloseColliders() {
        for (var i = 0; i < DamageCollidersLeft.Length; i++) {
            DamageCollidersLeft[i].SetActive(false);
            DamageColldersRight[i].SetActive(false);
        }
    }
}