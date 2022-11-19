using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Loader
{
    public const float k_ActivateThreshold = 0.9f;

    public static event UnityAction OnLoadingStart;
    public static event UnityAction OnLoadingEnd;

    private static AsyncOperation s_LoadingOperation;

    public static float CurrentLoadProgress { get; private set; }

    public static void Load(string sceneName, bool activateAutomatically = false) =>
        Load(SceneManager.GetSceneByName(sceneName).buildIndex, activateAutomatically);

    public static async void Load(int scene, bool activateAutomatically = false)
    {
        CurrentLoadProgress = 0;

        OnLoadingStart?.Invoke();

        s_LoadingOperation = SceneManager.LoadSceneAsync(scene);
        s_LoadingOperation.allowSceneActivation = false;
        do
        {
            await Task.Delay(500);
            CurrentLoadProgress = s_LoadingOperation.progress;
        } while (s_LoadingOperation.progress < k_ActivateThreshold);
        if (activateAutomatically) ActivateLoadedScene();

        OnLoadingEnd?.Invoke();
    }

    public static void ActivateLoadedScene()
    {
        if(s_LoadingOperation != null)
        {
            s_LoadingOperation.allowSceneActivation = true;
        }
    }
}
