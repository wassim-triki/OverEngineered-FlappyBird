namespace DefaultNamespace
{
    public interface IFreezable
    {
        void Freeze();
        void Unfreeze();
        bool IsFrozen { get; }
    }
}