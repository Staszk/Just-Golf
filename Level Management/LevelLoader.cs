using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public enum LoadType
    {
        OnStart,
        OnTrigger,
        OnDelay
    }

    public SceneUtilities.Scene activeScene;
    public SceneUtilities.Scene[] additiveScenes;

    public LoadType type;

    public bool unload = false;

    private LevelManager levMan;

    //private bool loadProcessing = false;

    private void Start()
    {
        levMan = Toolbox.Instance.GetObject<LevelManager>("Level Manager");

        if (type == LoadType.OnStart)
        {
            StartCoroutine(Process());
        }
    }

    private IEnumerator Process()
    {
        //loadProcessing = true;

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        yield return new WaitForEndOfFrame();

        if (activeScene != null)
        {
            ProcessScene(activeScene.name, LevelManager.OperationType.Load);

            yield return new WaitUntil(() => levMan.IsLoaded(activeScene.name));

            levMan.SetActiveScene(activeScene.name);
        }

        if (additiveScenes != null)
        {
            foreach (SceneUtilities.Scene scene in additiveScenes)
            {
                ProcessScene(scene.name, LevelManager.OperationType.AsyncLoad);
            }
        }

        if (unload)
        {
            ProcessScene(currentSceneName, LevelManager.OperationType.Unload);
        }
    }

    private bool LevelManagerIsDone()
    {
        return levMan.IsCurrentOperationComplete() && levMan.GetQueueSize() == 0;
    }

    private void ProcessScene(string sceneName, LevelManager.OperationType type)
    {
        LevelManager.Operation newOp = new LevelManager.Operation()
        {
            type = type,
            sceneName = sceneName
        };
        levMan.Enqueue(newOp);
    }
}
