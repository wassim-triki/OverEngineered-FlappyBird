using System;
using DefaultNamespace;
using UnityEngine;

public sealed class Lives : MonoBehaviour
{
    [field: SerializeField, Min(1)] public int CurrentLives { get; private set; } = 3;
    public bool IsAlive => CurrentLives > 0;
    
    [field: SerializeField, Min(1)]
    public int MaxObtainableLives {get; private set;} = 3;
    [field: SerializeField, Min(1)]
    public int InitLives {get; private set;} = 1;
    
    public event Action<int> OnLivesChanged;
    public event Action<DamageContext>OnDeath;
    public event Action<DamageContext> OnLifeLost;

    public event Action OnFullHeal;

    public void ResetLives()
    {
        CurrentLives = InitLives;
        OnLivesChanged?.Invoke(CurrentLives);
    }

    public void GainLife(int amount = 1)
    {
        if(amount <= 0 || !IsAlive) return;
        int oldLives = CurrentLives;
        CurrentLives = Mathf.Clamp(CurrentLives+amount,0,MaxObtainableLives);
        bool lifeGained = oldLives != CurrentLives;
        if (lifeGained)
        {
            OnLivesChanged?.Invoke(CurrentLives);
            Debug.Log($"[Lives] +{amount} → {CurrentLives}/{MaxObtainableLives}", this);
        }
    }
    

    public void Max()
    {
        if(!IsAlive) return;
        int oldLives = CurrentLives;
        CurrentLives = MaxObtainableLives;
        bool lifeGained = oldLives != CurrentLives;
        if (lifeGained)
        {
            OnLivesChanged?.Invoke(CurrentLives);
            Debug.Log($"[Lives] MAX → {CurrentLives}/{MaxObtainableLives}", this);
        }
        OnFullHeal?.Invoke();
    }

    public void LoseLife(int amount = 1, DamageContext ctx = null)
    {
        if(amount <= 0 || !IsAlive) return;
        CurrentLives = Mathf.Clamp(CurrentLives-amount,0,MaxObtainableLives);

        OnLivesChanged?.Invoke(CurrentLives);
        OnLifeLost?.Invoke(ctx);
        Debug.Log($"[Lives] -{amount} → {CurrentLives}/{MaxObtainableLives} [Source] {ctx?.Source}", this);
        
        if (CurrentLives == 0)
        {
            OnDeath?.Invoke(ctx);
            Debug.Log($"[Lives] OUT OF LIVES ☠️", this);
        }
    }
    
    public void Kill(DamageContext ctx = null) => LoseLife(CurrentLives,ctx);
}
