namespace Client
{
    public interface IPathStrategy
    {
        public float Speed { get; set; }
        public void UpdatePosition(Projectile projectile);
    }
}