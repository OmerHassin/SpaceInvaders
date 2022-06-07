using System;
using Infrastructure.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infrastructure.ReusableComponents
{
    public abstract class LoadableDrawableComponent : DrawableGameComponent
    {
        public event EventHandler<EventArgs> Disposed;

        public event EventHandler<EventArgs> PositionChanged;

        public event EventHandler<EventArgs> SizeChanged;

        protected string m_AssetName;

        public LoadableDrawableComponent(
    string i_AssetName, Game i_Game, int i_UpdateOrder, int i_DrawOrder)
    : base(i_Game)
        {
            this.AssetName = i_AssetName;
            this.UpdateOrder = i_UpdateOrder;
            this.DrawOrder = i_DrawOrder;
        }

        public LoadableDrawableComponent(
            string i_AssetName,
            Game i_Game,
            int i_CallsOrder)
            : this(i_AssetName, i_Game, i_CallsOrder, i_CallsOrder)
        { 
        }

        public override void Initialize()
        {
            base.Initialize();

            if (this is ICollidable)
            {
                ICollisionsManager collisionMgr =
                    this.Game.Services.GetService(typeof(ICollisionsManager))
                        as ICollisionsManager;

                if (collisionMgr != null)
                {
                    collisionMgr.AddCollidable(this as ICollidable);
                }
            }

            InitBounds();
        }

        protected abstract void InitBounds();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            OnDisposed();
        }

        protected virtual void OnPositionChanged()
        {
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public string AssetName
        {
            get { return m_AssetName; }
            set { m_AssetName = value; }
        }

        protected ContentManager ContentManager
        {
            get { return this.Game.Content; }
        }
    }
}