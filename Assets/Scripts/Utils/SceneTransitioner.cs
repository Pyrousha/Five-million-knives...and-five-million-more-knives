using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : Singleton<SceneTransitioner>
{
    public bool LevelFinished { get; private set; } = false;

    [SerializeField] private Animator anim;

    public const int MAIN_MENU_INDEX = 1;
    public const int FIRST_LEVEL_INDEX = 2;
    public const int CREDITS_SCENE_INDEX = 14;

    public const float FADE_ANIM_DURATION = 0.25f;

    public static int CurrBuildIndex = 0;

    public static bool GotNewBestTime { get; private set; }

    private void Start()
    {
        ToMainMenu();
    }

    //public void OnLevelFinished()
    //{
    //    LevelFinished = true;
    //    Timer.Instance.PauseTimer();
    //    bool isNewBestTime = SaveData.Instance.OnLevelCompleted(CurrBuildIndex - FIRST_LEVEL_INDEX, Timer.Instance.CurrTime);

    //    if (IsFullGame)
    //    {
    //        int nextIndex = CurrBuildIndex + 1;
    //        if (nextIndex == CREDITS_SCENE_INDEX)
    //        {
    //            //Full game just finished! 
    //            GotNewBestTime = SaveData.Instance.OnFullGameCompleted(Timer.Instance.TotalTime);
    //        }

    //        LoadSceneWithIndex(nextIndex);
    //    }
    //    else
    //    {
    //        StageClearCanvas.Instance.OpenPopup(SaveData.CurrSaveData.LevelsList[CurrBuildIndex - FIRST_LEVEL_INDEX], isNewBestTime);
    //        PlayerController.Instance.OnLevelEnd();
    //    }
    //}

    public void ToMainMenu()
    {
        LoadSceneWithIndex(MAIN_MENU_INDEX);
    }

    public void ReloadCurrScene()
    {
        LoadSceneWithIndex(CurrBuildIndex);
    }

    public void LoadSceneWithIndex(int _index)
    {
        Routine.Start(this, LoadSceneRoutine(_index));
    }
    private IEnumerator LoadSceneRoutine(int _index)
    {
        anim.ResetTrigger("ToClear");
        anim.SetTrigger("ToBlack");

        yield return FADE_ANIM_DURATION;

        LevelFinished = false;
        SceneManager.LoadScene(_index);
    }

    //public void OnDeath()
    //{
    //    if (LevelFinished)
    //        return;

    //    Timer.Instance.PauseTimer();
    //    LoadSceneWithIndex(CurrBuildIndex);
    //}

    public void OnSceneFinishedLoading()
    {
        CurrBuildIndex = SceneManager.GetActiveScene().buildIndex;

        anim.ResetTrigger("ToBlack");
        anim.SetTrigger("ToClear");

        //switch (CurrBuildIndex)
        //{
        //    case 0:
        //        {
        //            Debug.LogError("OnFinishedLoading should not be called in boot");
        //            break;
        //        }
        //    case MAIN_MENU_INDEX:
        //        {
        //            Timer.Instance.SetTimerVisualsStatus(false, false);
        //            break;
        //        }
        //    case FIRST_LEVEL_INDEX:
        //        {
        //            StartCoroutine(Timer.Instance.RestartTimer());
        //            break;
        //        }
        //    case CREDITS_SCENE_INDEX:
        //        {
        //            Timer.Instance.SetTimerVisualsStatus(false, false);
        //            break;
        //        }
        //    default:
        //        {
        //            StartCoroutine(Timer.Instance.ResumeTimer());
        //            break;
        //        }
        //}
    }
}
