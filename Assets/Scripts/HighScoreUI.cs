using System;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreService score;
    [SerializeField] private TextMeshProUGUI highScoreUI; 
    [SerializeField] private TextMeshProUGUI newBadgeLabel;
    private RectTransform _rectTransform;
    

    void Awake()
    {
            score.OnHighScoreChanged += HandleNewHighScore;
            GameStateManager.OnMenu += HandleOnMenu;
    }
    void Start()
    {
        highScoreUI.text = score.High.ToString();
        newBadgeLabel.gameObject.SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        Debug.Log("_rectTransform: " + _rectTransform);
        Debug.Log("_rectTransform.localPosition: " + _rectTransform.localPosition);
        Debug.Log("_rectTransform.anchoredPostion: " + _rectTransform.anchoredPosition);
    }
    
    private void OnDestroy()
    {
        if (score != null)
            score.OnHighScoreChanged -= HandleNewHighScore;
        if (GameStateManager.Instance != null)
            GameStateManager.OnMenu -= HandleOnMenu;
    }
    void HandleOnMenu()
    {
        newBadgeLabel.gameObject.SetActive(false);
    }





    private void HandleNewHighScore(int newHigh)
    {
            highScoreUI.text = newHigh.ToString();
            newBadgeLabel.gameObject.SetActive(true);
    }

    public void Show()
    {
        _rectTransform.DOAnchorPosY( -110f,0.5f).SetEase(Ease.OutBack);
    }
    public void Hide()
    {
        _rectTransform.DOAnchorPosY( -35.2f,0.5f).SetEase(Ease.OutBack);
    }
}
