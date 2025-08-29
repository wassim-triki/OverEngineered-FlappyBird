namespace DefaultNamespace
{
    public interface IDamageable
    {
        void TakeDamage(int amount,DamageContext ctx = null);
    }
}