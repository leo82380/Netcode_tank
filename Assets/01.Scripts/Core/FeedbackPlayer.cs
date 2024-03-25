using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    private List<Feedback> _feedbackToPlay = null;

    private void Awake()
    {
        _feedbackToPlay = new List<Feedback>();
        GetComponents<Feedback>(_feedbackToPlay);
    }

    public void PlayFeedback()
    {
        CompleteFeedback();
        _feedbackToPlay.ForEach(x => x.CreatFeedback());
    }
    
    public void CompleteFeedback()
    {
        _feedbackToPlay.ForEach(x => x.CompleteFeedback());
    }
}
