using System;

namespace Infrastructure.ReusableComponents
{
    public interface ICollidable
    {
        event EventHandler<EventArgs> PositionChanged;
        event EventHandler<EventArgs> SizeChanged;
        event EventHandler<EventArgs> VisibleChanged;
        event EventHandler<EventArgs> Disposed;
        bool Visible { get; }
        int Score { get; }
        bool CheckCollision(ICollidable i_Source);
        void Collided(ICollidable i_Collidable);
    }
}
