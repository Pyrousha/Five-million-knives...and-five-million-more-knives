using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent onPickup;
    [SerializeField] private UnityEvent onFocus;
    [SerializeField] private UnityEvent onUnfocus;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.parent.TryGetComponent(out PlayerController player)) {
            player.AddInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.transform.parent.TryGetComponent(out PlayerController player)) {
            player.RemoveInteractable(this);
        }
    }

    public void Focus() {
        onFocus.Invoke();
    }

    public void Unfocus() {
        onUnfocus.Invoke();
    }

    public void Interact() {
        onPickup.Invoke();
    }
}
