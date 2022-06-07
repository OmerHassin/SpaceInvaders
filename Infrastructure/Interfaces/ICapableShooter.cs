using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents
{
    public interface ICapableShooter
    {
        public Vector2 GetPositionForShootiong();
        Color BulletTintColor { get; }
        int BulletDirection { get; }
    }
}
