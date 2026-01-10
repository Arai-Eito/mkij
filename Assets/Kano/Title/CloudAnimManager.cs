using UnityEngine;
using System.Collections;

public class CloudAnimManager : MonoBehaviour
{
    [SerializeField] CloudAnim[] clouds;
    [SerializeField] float interval = 0.08f;
    public void Play()
    {
        StartCoroutine(Sequence());
    }
    
    IEnumerator Sequence()
    {
        for (int i = 0; i < clouds.Length; i++)
        {
            bool moveRight = (i % 2 == 1);
            StartCoroutine(clouds[i].Play(moveRight));
            yield return new WaitForSecondsRealtime(interval);
        }
    }
}
