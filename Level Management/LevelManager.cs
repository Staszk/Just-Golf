using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public bool displayOperationMessages = false;

    public enum OperationType
    {
        None, Load, AsyncLoad, Unload, Activate
    }

    public struct Operation
    {
        public string sceneName;
        public OperationType type;
        public bool activateOnEmptyQueue;
    };

    private Queue<Operation> operationQueue;
    private Operation currentOperation;

    private AsyncOperation asyncOp;
    private Coroutine processRoutine;

    private void Awake()
    {
        operationQueue = new Queue<Operation>();
    }

    private void Update()
    {
        // If there is a current operation in progress, then continue to next update
        if (!IsAsyncOpComplete(currentOperation) || !IsOperationComplete(currentOperation))
        {
            return;
        }

        // If there is no operation to process, then continue to next update
        if (operationQueue.Count == 0)
        {
            return;
        }

        Operation nextOp = operationQueue.Dequeue();

        // If the next operation is already complete, then continue to next update 
        if (IsOperationComplete(nextOp))
        {
            return;
        }

        currentOperation = nextOp;

        switch (currentOperation.type)
        {
            case OperationType.Load:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (displayOperationMessages) Debug.Log("Loading: " + currentOperation.sceneName);
#endif
                LoadAdditive(currentOperation.sceneName);
                break;
            case OperationType.AsyncLoad:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (displayOperationMessages) Debug.Log("Loading async: " + currentOperation.sceneName);
#endif
                asyncOp = LoadAdditiveAsync(currentOperation.sceneName);
                asyncOp.allowSceneActivation = true;
                break;
            case OperationType.Unload:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (displayOperationMessages) Debug.Log("Unloading: " + currentOperation.sceneName);
#endif
                asyncOp = UnloadAsync(currentOperation.sceneName);
                break;
            case OperationType.Activate:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (displayOperationMessages) Debug.Log("Activating: " + currentOperation.sceneName);
#endif
                SetActiveScene(currentOperation.sceneName);
                break;
            default:
                break;
        }
    }

    public void Clear()
    {
        currentOperation = new Operation();
        operationQueue.Clear();
    }

    private bool IsAsyncOpComplete(Operation op)
    {
        bool complete = true;

        if (asyncOp != null)
        {
            complete = asyncOp.isDone;
        }

        return complete;
    }

    public bool IsCurrentOperationComplete()
    {
        return IsOperationComplete(currentOperation);
    }

    private bool IsOperationComplete(Operation op)
    {
        switch (op.type)
        {
            case OperationType.None:
                return true;
            case OperationType.Load:
                return IsLoaded(op.sceneName);
            case OperationType.AsyncLoad:
                return IsLoaded(op.sceneName);
            case OperationType.Unload:
                return IsLoaded(op.sceneName) == false; // return true if it is not loaded
            case OperationType.Activate:
                return IsActive(op.sceneName);
        }
        return true;
    }

    // Add operation to the queue
    public void Enqueue(Operation op)
    {
        if (operationQueue.Contains(op) == false)
        {
            operationQueue.Enqueue(op);
        }
    }

    public AsyncOperation UnloadResources()
    {
        return Resources.UnloadUnusedAssets();
    }

    public int GetQueueSize()
    {
        return operationQueue.Count;
    }

    public float GetAsyncProgress()
    {
        if (asyncOp != null)
        {
            return asyncOp.progress;
        }
        return 0f;
    }

    // load scene
    public void Load(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // load scene additive
    public void LoadAdditive(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }

    public void LoadAdditive(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    // load scene async
    public AsyncOperation LoadAsync(int sceneIndex)
    {
        return SceneManager.LoadSceneAsync(sceneIndex);
    }

    public AsyncOperation LoadAsync(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName);
    }

    // load scene async additive
    public AsyncOperation LoadAdditiveAsync(int sceneIndex)
    {
        return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    }

    public AsyncOperation LoadAdditiveAsync(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    // unload scene
    public AsyncOperation UnloadAsync(int sceneIndex)
    {
        return SceneManager.UnloadSceneAsync(sceneIndex);
    }

    public AsyncOperation UnloadAsync(string sceneName)
    {
        return SceneManager.UnloadSceneAsync(sceneName);
    }

    public bool IsLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }

    public bool IsActive(string sceneName)
    {
        return SceneManager.GetActiveScene().name == sceneName;
    }

    public void SetActiveScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    public void SetActiveScene(int sceneIndex)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(sceneIndex));
    }
}
