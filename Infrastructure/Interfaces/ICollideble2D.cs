using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infrastructure.ReusableComponents
{
    public interface ICollideble2D : ICollidable
    {
        Rectangle Bounds { get; }
        Vector2 Velocity { get; }
        Vector2 Size2D { get; }
        Vector2 Position { get; }
    }
}
