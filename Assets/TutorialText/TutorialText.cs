using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TutorialText : MonoBehaviour {
    public bool disableOnComplete = false;
    public float disableDelay = 2.0f;
    public string[] completeOnKeys;
    public string[] completeOnAxis;
    [Space(20)]
    public CompleteEvent onComplete;
    
    [System.NonSerialized]
    public bool completed = false;

	void Update () {
        if (!completed)
        {
            for (int i = 0; i < completeOnKeys.Length; i++)
            {
                if (Input.GetKey(completeOnKeys[i]))
                {
                    StartCoroutine(Complete());
                    return;
                }
            }

            for (int e = 0; e < completeOnAxis.Length; e += 1)
            {
                float value = Input.GetAxis(completeOnAxis[e]);
                if (value > 0.1f || value < -0.1f)
                {
                    StartCoroutine(Complete());
                    return;
                }
            }
        }
	}

    IEnumerator Complete()
    {
        if (!completed)
            completed = true;
        yield return new WaitForSeconds(disableDelay);

        onComplete.Invoke();
        if (disableOnComplete)
            gameObject.SetActive(false);
    }

    [System.Serializable]
    public class CompleteEvent : UnityEvent { };
}
