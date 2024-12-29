using System;
using UnityEngine;

public class WinGameTrigger : MonoBehaviour
{
    public static Action OnGameEnd;
    [SerializeField] private GameObject winUI;
    [SerializeField] private Transform winEffect;
    [SerializeField] private MovePlatform movePlatform;
    [SerializeField] private AudioSource mainSourse;
    [SerializeField] private AudioClip winClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gift"))
        {
            Win();
        }
    }
    private void Win()
    {
        movePlatform.CantMove();
        winEffect.gameObject.SetActive(true);
        winUI.SetActive(true);
        GetComponent<AudioSource>().Play();
        mainSourse.clip = winClip;
        mainSourse.Play();
        OnGameEnd.Invoke();
        OnGameEnd = null;   
    }

}
