using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimeButtonAnimationController : MonoBehaviour
{
    [SerializeField] private Animator stopAnim, playAnim;
    [SerializeField] private RotateObjects rotate;

    private bool isPlay, isStop, isRewind;
    private void Start()
    {
        TimeController.OnTimeStoped += StopAnim;
        TimeController.OnTimeStart += PlayAnim;
        TimeController.OnStartRewind += RewindAnim;
        playAnim.SetTrigger("Play");
        isPlay = true;
    }
    private void RewindAnim()
    {
        if (!isRewind)
        {
            DisableAllAnimations();
            StopAllCoroutines();
            StartCoroutine(StartRewind());
            isRewind = true;
        }
    }

    private void StopAnim()
    {
        if (!isStop)
        {
            DisableAllAnimations();
            stopAnim.SetTrigger("StopTime");
            isStop = true;
        }
    }

    private void PlayAnim()
    {
        if (!isPlay)
        {
            DisableAllAnimations();
            playAnim.SetTrigger("Play");
            isPlay = true;
        }
    }

    private void DisableAllAnimations()
    {
        if (isRewind)
        {
            StopAllCoroutines();
            StartCoroutine(StopRewind());
            isRewind = false;
        }

        if (isStop)
        {
            stopAnim.SetTrigger("Stop");
            isStop = false;
        }

        if (isPlay)
        {
            playAnim.SetTrigger("StopPlay");
            isPlay = false;
        }
    }

    private IEnumerator StartRewind()
    {
        int speed = 0;
        while (speed != 1000)
        {
            speed += 100;
            rotate.SetRotationSpeed("z", speed);
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator StopRewind()
    {
        const float alignmentThreshold = 50;
        float currentZRotation;
        rotate.SetRotationSpeed("z", 2000, true);
        yield return new WaitForSecondsRealtime(0.2f);
        while (true)
        {
            currentZRotation = rotate.transform.eulerAngles.z;
           

            if (Mathf.Abs(currentZRotation) <= alignmentThreshold)
            {
                rotate.transform.eulerAngles = new Vector3(0, 0, 0);
                rotate.SetRotationSpeed("z", 0, true);
                break;
            }

            yield return null;
        }
    }
}
