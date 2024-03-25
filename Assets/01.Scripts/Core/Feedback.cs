using UnityEngine;

public abstract class Feedback : MonoBehaviour
{
    public abstract void CreatFeedback();
    public abstract void CompleteFeedback();

    protected void OnDestroy()
    {
        CompleteFeedback();
    }
}
