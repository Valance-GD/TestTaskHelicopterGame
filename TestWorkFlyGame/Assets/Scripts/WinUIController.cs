using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUIController : MonoBehaviour
{
    [System.Serializable]
    public class TextData
    {
        public TextMeshProUGUI textMeshPro; 
        public string message;
        public bool changeColorPerLetter;
    }

    [SerializeField] private List<TextData> textObjects; 
    [SerializeField] private float letterDelay = 0.1f; 
    [SerializeField] private float textDelay = 1.0f;
    [SerializeField] private float colorChangeSpeed = 1.0f;

    [Header("CountText")]
    [SerializeField] private int countOfCollectPoint = 1000;
    [SerializeField] private int duration = 5;
    [SerializeField] private TextMeshProUGUI countText;

    private void Start()
    {
        StartCoroutine(ShowTextsSequentially());
    }

    private void Update()
    {
        foreach (var textData in textObjects)
        {
            if (textData.changeColorPerLetter)
            {
                ChangeColorPerLetter(textData.textMeshPro);
            }
            else
            {
                ChangeTextColor(textData.textMeshPro);
            }
        }
    }

    private IEnumerator ShowTextsSequentially()
    {
        foreach (var textData in textObjects)
        {
            yield return StartCoroutine(ShowTextByLetter(textData));
            yield return new WaitForSeconds(textDelay); 
        }
    }

    private IEnumerator ShowTextByLetter(TextData textData)
    {
        TextMeshProUGUI textMeshPro = textData.textMeshPro;
        string message = textData.message;

        textMeshPro.text = ""; 
        for (int i = 0; i < message.Length; i++)
        {
            textMeshPro.text += message[i];
            yield return new WaitForSeconds(letterDelay);
        }
    }

    private void ChangeTextColor(TextMeshProUGUI text)
    {
        float red = Mathf.PingPong(Time.time * colorChangeSpeed, 1.0f);
        float green = Mathf.PingPong(Time.time * colorChangeSpeed * 0.8f, 1.0f);
        float blue = Mathf.PingPong(Time.time * colorChangeSpeed * 0.5f, 1.0f);

        text.color = new Color(red, green, blue);
    }

    private void ChangeColorPerLetter(TextMeshProUGUI text)
    {
        string originalText = text.text;
        text.ForceMeshUpdate();

        var textInfo = text.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            float red = Mathf.PingPong(Time.time * colorChangeSpeed + i * 0.1f, 1.0f);
            float green = Mathf.PingPong(Time.time * colorChangeSpeed * 0.8f + i * 0.1f, 1.0f);
            float blue = Mathf.PingPong(Time.time * colorChangeSpeed * 0.5f + i * 0.1f, 1.0f);

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = text.textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].colors32;

            Color32 newColor = new Color(red, green, blue);
            vertexColors[vertexIndex + 0] = newColor;
            vertexColors[vertexIndex + 1] = newColor;
            vertexColors[vertexIndex + 2] = newColor;
            vertexColors[vertexIndex + 3] = newColor;
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }


    public void StartCollectPoint()
    {
        GetComponent<AudioSource>().Play();
        StartCoroutine(IncrementCountText());
    }
    private IEnumerator IncrementCountText()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(0, countOfCollectPoint, elapsedTime / duration));
            countText.text = currentValue.ToString();
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        countText.text = countOfCollectPoint.ToString();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
