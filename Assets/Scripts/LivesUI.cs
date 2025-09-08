using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private Transform livesContainer;
    [SerializeField] private LifeCell lifeCellPrefab;

    [Header("Animation")]
    [SerializeField, Min(1f)] private float beatScale = 1.15f; // <- the single knob

    // Fixed internals (kept private & constant)
    const float AddPopInDuration   = 0.08f;
    const float FullHealStagger    = 0.035f;

    private readonly List<LifeCell> _cells = new();

    void Awake()
    {
        if (playerLives != null)
        {
            playerLives.OnLivesChanged += HandleLivesChanged; // int currentLives
            playerLives.OnFullHeal     += HandleFullHeal;     // always triggers Max animation
        }
    }

    void OnDestroy()
    {
        if (playerLives != null)
        {
            playerLives.OnLivesChanged -= HandleLivesChanged;
            playerLives.OnFullHeal     -= HandleFullHeal;
        }
    }

    void Start()
    {
        ClearAll();
        // Build initial hearts without anim
        for (int i = 0; i < (playerLives ? playerLives.CurrentLives : 0); i++)
            AddOne();
    }

    public void HandleLivesChanged(int currentLives)
    {
        if (!livesContainer || !lifeCellPrefab) return;

        int diff = currentLives - _cells.Count;

        if (diff > 0)
        {
            // Gained lives -> add with pop then beat (uses the same beatScale)
            for (int i = 0; i < diff; i++) AddOneAnimated();
        }
        else if (diff < 0)
        {
            diff = -diff;
            for (int i = 0; i < diff; i++) RemoveOne();
        }
    }

    void HandleFullHeal()
    {
        // Beat all hearts once in a short ripple; same beatScale
        for (int i = 0; i < _cells.Count; i++)
        {
            var cell = _cells[i];
            if (!cell) continue;

            float delay = i * FullHealStagger;
            DOVirtual.DelayedCall(delay, () => cell.Beat(beatScale))
                     .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
    }

    void AddOne()
    {
        var cell = Instantiate(lifeCellPrefab, livesContainer);
        _cells.Add(cell);
    }

    void AddOneAnimated()
    {
        var cell = Instantiate(lifeCellPrefab, livesContainer);
        _cells.Add(cell);

        var rt = cell.GetComponent<RectTransform>();
        if (!rt) return;

        var baseScale = Vector3.one;
        rt.localScale = Vector3.zero;

        // quick pop-in to 1, then the same beat as full-heal
        DOTween.Sequence().SetLink(cell.gameObject, LinkBehaviour.KillOnDisable)
            .Append(rt.DOScale(baseScale, AddPopInDuration).SetEase(Ease.OutCubic))
            .AppendCallback(() => cell.Beat(beatScale));
    }

    void RemoveOne()
    {
        if (_cells.Count == 0) return;
        int last = _cells.Count - 1;
        var go = _cells[last].gameObject;
        _cells.RemoveAt(last);
        Destroy(go);
    }

    void ClearAll()
    {
        if (!livesContainer) return;
        for (int i = livesContainer.childCount - 1; i >= 0; --i)
            Destroy(livesContainer.GetChild(i).gameObject);
        _cells.Clear();
    }
}
