using BeauRoutine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPopupController : Singleton<GameplayPopupController>
{
    [SerializeField] private GameObject winOverlay;
    [SerializeField] private GameObject deadOverlay;
    [SerializeField] private Button respawnButton;

    [SerializeField] private TextMeshProUGUI popupLabel;
    private Transform popupTransform;

    private Routine popupRoutine;

    private void Start()
    {
        //Time.timeScale = 1;
        respawnButton.onClick.AddListener(OnRespawnClicked);

        popupTransform = popupLabel.transform;
    }

    public void OnPlayerDeath()
    {
        //Time.timeScale = 0;

        deadOverlay.SetActive(true);
        respawnButton.Select();
    }

    private void OnRespawnClicked()
    {
        SceneTransitioner.Instance.ReloadCurrScene();
    }

    public void OnGameWin()
    {
        winOverlay.SetActive(true);
    }

    public void ShowPopupText(string _text, float duration = 0.5f)
    {
        popupLabel.text = _text;
        popupRoutine.Stop();
        popupRoutine = Routine.Start(this, PopupRoutine(duration));
    }

    private IEnumerator PopupRoutine(float duration)
    {
        popupTransform.localScale = Vector3.zero;
        popupLabel.gameObject.SetActive(true);

        yield return Tween.Float(0, 1, (t) =>
        {
            popupTransform.localScale = Vector3.one * t;
        }, 0.125f);

        yield return duration;

        yield return Tween.Float(0, 1, (t) =>
        {
            popupTransform.localScale = Vector3.one * (1 - t);
        }, 0.125f);
    }
}
