using System.Collections.Generic;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private Transform livesContainer; // use Transform directly
    [SerializeField] private LifeCell lifeCellPrefab;

    private readonly List<LifeCell> _cells = new();

    void Awake()
    {
        if (playerLives != null)
            playerLives.OnLivesChanged += HandleLivesChanged;
    }

    void OnDestroy()
    {
        if (playerLives != null)
            playerLives.OnLivesChanged -= HandleLivesChanged;
    }

    void Start()
    {
        ClearAll();
        // Seed exactly CurrentLives hearts at startup
        for (int i = 0; i < (playerLives ? playerLives.CurrentLives : 0); i++)
            AddOne();
    }

    // Keep container count == currentLives (only filled hearts are shown)
    public void HandleLivesChanged(int currentLives)
    {
        if (!livesContainer || !lifeCellPrefab) return;

        int diff = currentLives - _cells.Count;

        if (diff > 0)
        {
            // gained lives -> add that many hearts
            for (int i = 0; i < diff; i++) AddOne();
        }
        else if (diff < 0)
        {
            // lost lives -> remove that many hearts from the end
            diff = -diff;
            for (int i = 0; i < diff; i++) RemoveOne();
        }
        // If diff == 0, nothing to do.
    }

    private void AddOne()
    {
        var cell = Instantiate(lifeCellPrefab, livesContainer);
        _cells.Add(cell);
    }

    private void RemoveOne()
    {
        if (_cells.Count == 0) return;
        int last = _cells.Count - 1;
        var go = _cells[last].gameObject;
        _cells.RemoveAt(last);
        Destroy(go);
    }

    private void ClearAll()
    {
        if (!livesContainer) return;

        for (int i = livesContainer.childCount - 1; i >= 0; --i)
            Destroy(livesContainer.GetChild(i).gameObject);

        _cells.Clear();
    }
}
