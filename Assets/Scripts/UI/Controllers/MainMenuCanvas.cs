using UnityEngine;

public class MainMenuCanvas : Submenu
{
    private void Start()
    {
        firstSelectable.Select();
    }

    public void OnPlayClicked()
    {
        //TODO: Reset Timer
        SceneTransitioner.Instance.LoadSceneWithIndex(SceneTransitioner.FIRST_LEVEL_INDEX);
    }

    public void OnSettingsClicked()
    {
        //TODO: Settings Menu
        SettingsCanvas.Instance.SelectFromPast(this);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
