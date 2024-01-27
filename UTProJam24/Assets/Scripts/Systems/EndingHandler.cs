using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingHandler : MonoBehaviour
{
    [SerializeField] private EndingData debugEndingData;
    private Image endingImage;
    private TextMeshProUGUI endingText;
    // Start is called before the first frame update
    private void Awake()
    {
        endingImage = transform.GetChild(0).GetComponent<Image>();
        endingText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        EndingData endingData = SaveManager.Instance.runtimeData.currentEnding;
        if (endingData == null)
        {
            Debug.LogWarning("no ending found, loading debug ending...");
            endingData = debugEndingData;
        }
        endingImage.sprite = endingData.endingSprite;
        endingText.text = endingData.endingText;
        AudioManager.Instance.InitializeMusic(endingData.endingMusic);
        AudioManager.Instance.StartMusic();
        LevelChanger.Instance.FadeIn();
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        yield return new WaitForSecondsRealtime(5f);
        LevelChanger.Instance.FadeToLevel("Credits");
    }
}
