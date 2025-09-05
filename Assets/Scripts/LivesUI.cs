using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private GameObject livesContainer;
    [SerializeField] private LifeCell lifeCellPrefab;
    private readonly List<LifeCell> _cells = new();
    

    private void Start()
    {
        ClearCells();
        BuildCells();
    }

    public void BuildCells()
    {
        for (int i = 0; i < playerLives.MaxLives; i++)
        {
            var cell = Instantiate<LifeCell>(lifeCellPrefab, livesContainer.transform);
            _cells.Add(cell);
            cell.SetFull(i<playerLives.CurrentLives);
        }
    }
    
    public void HandleLivesChanged(int currentLives, int maxLives)
    {
        if (_cells.Count > 0 && _cells.Count != maxLives)
        {
            ClearCells();
            BuildCells();
        }

        for (int i = 0; i < _cells.Count; i++)
        {
            _cells[i].SetFull(i < currentLives);
        }
            
    }

    private void OnEnable()
    {
        playerLives.OnLivesChanged += HandleLivesChanged;
    }

    private void OnDisable()
    {
        playerLives.OnLivesChanged -= HandleLivesChanged;
    }

    public void ClearCells()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
            Destroy(transform.GetChild(i).gameObject);
        _cells.Clear();
    }

}
