using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LivesUI : MonoBehaviour
{
    [SerializeField] private Lives playerLives;
    [SerializeField] private LifeCell lifeCellPrefab;
    private readonly List<LifeCell> _cells = new();

    private void Start()
    {
        BuildCells(playerLives.MaxLives);
        // ClearLives();
        // BuildLives(LifeCell.LifeCellStatus.Full);
    }

    public void BuildCells(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var cell = Instantiate<LifeCell>(lifeCellPrefab, transform);
            _cells.Add(cell);
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

    public void BuildLives(LifeCell.LifeCellStatus lifeCellStatus)
    {
        for (int i = 0; i < _cells.Count; i++)
        {
            var lifeCell = Instantiate(lifeCellPrefab,this.transform);
            lifeCell.SetImage(lifeCellStatus);
            _cells.Add(lifeCell);
        }
    }


    public void ClearLives()
    {
        // foreach (Transform child in this.transform)
        // {
        //     Destroy((child.gameObject));
        // }
        _cells.Clear();
    }

    public void HandleLivesChanged(int currentLives, int maxLives)
    {
        for (int i = 0; i < currentLives; i++)
        {
            BuildLives(LifeCell.LifeCellStatus.Full);
        }

        for (int i = currentLives; i < maxLives; i++)
        {
            BuildLives(LifeCell.LifeCellStatus.Empty);
        }
    }
}
