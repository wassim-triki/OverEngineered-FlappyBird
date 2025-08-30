using System;
using DefaultNamespace;
using UnityEngine;

public sealed class Lives : MonoBehaviour
{
    [field: SerializeField, Min(1)]
    public int MaxLives {get; private set;} = 3;
    public int CurrentLives {get; private set;}
    public bool IsAlive => CurrentLives > 0;
    
    public event Action<int,int> OnLivesChanged;
    public event Action<DamageContext>OnDeath;
    public event Action<DamageContext> OnLifeLost;


    public void ResetRun()
    {
        CurrentLives = MaxLives;
        OnLivesChanged?.Invoke(CurrentLives,MaxLives);
        Debug.Log($"[Lives] ResetRun → {CurrentLives}/{MaxLives}", this);
    }

    public void GainLife(int amount = 1)
    {
        if(amount <= 0 || !IsAlive) return;
        int oldLives = CurrentLives;
        CurrentLives = Mathf.Clamp(CurrentLives+amount,0,MaxLives);
        bool lifeGained = oldLives != CurrentLives;
        if (lifeGained)
        {
            OnLivesChanged?.Invoke(CurrentLives,MaxLives);
            Debug.Log($"[Lives] +{amount} → {CurrentLives}/{MaxLives}", this);
        }
    }

    public void LoseLife(int amount = 1, DamageContext ctx = null)
    {
        if(amount <= 0 || !IsAlive) return;
        CurrentLives = Mathf.Clamp(CurrentLives-amount,0,MaxLives);

        OnLivesChanged?.Invoke(CurrentLives,MaxLives);
        OnLifeLost?.Invoke(ctx);
        Debug.Log($"[Lives] -{amount} → {CurrentLives}/{MaxLives} [Source] {ctx?.Source}", this);
        
        if (CurrentLives == 0)
        {
            OnDeath?.Invoke(ctx);
            Debug.Log($"[Lives] OUT OF LIVES ☠️", this);
        }
    }
    
    [ContextMenu("Debug/Reset Run")]
    private void Debug_Reset() => ResetRun();
    
    [ContextMenu("Debug/Lose Life")]
    private void Debug_LoseLife() => LoseLife(1,new DamageContext{Source = gameObject});
    
    [ContextMenu("Debug/Gain Life")]
    private void Debug_GainLife() => GainLife(1);

}
