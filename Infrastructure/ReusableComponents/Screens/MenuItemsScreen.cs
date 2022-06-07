using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents.Objects;

namespace Infrastructure.ReusableComponents.Screens
{
    public class MenuItemsScreen : GameScreen
    {
        private const string k_MenuMoveSoundAsset = @"Sounds\MenuMove";
        protected const string k_OnMsg = @"On";
        protected const string k_OffMsg = @"Off";
        protected const string k_OneMsg = @"One";
        protected const string k_TwoMsg = @"Two";
        protected const string k_VissibleMsg = @"Visible";
        protected const string k_InvisibleMsg = @"Invisible";
        private const int k_HeadLineSpaceFromBorder = 25;

        protected List<SpriteMenuItem> m_ListItems;
        protected SpriteMenuItem m_ActiveItem;
        protected Sprite2D m_HeadLine;
        protected IGameManager m_GameManager;
        protected bool m_DelegatesSets = false;

        public MenuItemsScreen(string i_HeadLineAssetName, Game i_Game)
            : base(i_Game)
        {
            m_GameManager = Game.Services.GetService(typeof(IGameManager)) as IGameManager;
            this.Add(new Background(i_Game, Color.DarkGray));
            this.Add(m_HeadLine = new Sprite2D(i_HeadLineAssetName, i_Game));
            m_ListItems = new List<SpriteMenuItem>();
        }

        public override void Initialize()
        {
            base.Initialize();

            openMenu();
            m_HeadLine.Position = new Vector2(CenterOfViewPort.X - (m_HeadLine.Width / 2), k_HeadLineSpaceFromBorder);

            if (!m_DelegatesSets)
            {
                setDelegates();
            }
        }

        private MenuItemsScreen openMenu()
        {
            if (m_ActiveItem != m_ListItems[0])
            {
                m_ActiveItem.Active = false;
                m_ActiveItem = m_ListItems[0];
                m_ActiveItem.Active = true;
            }

            return this;
        }

        public virtual void setDelegates()
        {
            m_DelegatesSets = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            soundManager.AddSoundEffect(k_MenuMoveSoundAsset);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            checkKeyboardInput();
            checkMouseInput();
            checkMouseColoition();
        }

        private void checkKeyboardInput()
        {
            if (InputManager.KeyPressed(Keys.Up))
            {
                ActiveItem = ActiveItem.MoveUp();
            }
            else if (InputManager.KeyPressed(Keys.Down))
            {
                ActiveItem = ActiveItem.MoveDown();
            }
            else if (InputManager.KeyPressed(Keys.Enter))
            {
                ActiveItem.RunPressedOnItem();
            }
            else if (InputManager.KeyPressed(Keys.PageUp))
            {
                ActiveItem.RunPgUpPressedOnItem();
            }
            else if (InputManager.KeyPressed(Keys.PageDown))
            {
                ActiveItem.RunPgDownPressedOnItem();
            }
            else if (InputManager.KeyPressed(Keys.M))
            {
                (Game.Services.GetService(typeof(ISoundManager)) as ISoundManager).ToggelMute();
            }
        }

        private void checkMouseInput()
        {
            if (m_ActiveItem != null && m_ActiveItem.Bounds.Contains(InputManager.MouseState.X, InputManager.MouseState.Y))
            {
                if (InputManager.ButtonPressed(eInputButtons.Left))
                {
                    ActiveItem.RunPressedOnItem();
                }
                else if (InputManager.ButtonPressed(eInputButtons.Right) || InputManager.ScrollWheelDelta > 0)
                {
                    ActiveItem.RunPgUpPressedOnItem();
                }
                else if (InputManager.ScrollWheelDelta < 0)
                {
                    ActiveItem.RunPgDownPressedOnItem();
                }
            }
        }

        private void checkMouseColoition()
        {
            foreach (SpriteMenuItem item in m_ListItems)
            {
                if (ActiveItem != item && item.Bounds.Contains(InputManager.MouseState.X, InputManager.MouseState.Y))
                {
                    ActiveItem.Active = false;
                    item.Active = true;
                    ActiveItem = item;
                }
            }
        }

        public void AddToItemsScreen(IGameComponent i_Component)
        {
            this.Add(i_Component);
            m_ListItems.Add(i_Component as SpriteMenuItem);
        }

        private SpriteMenuItem ActiveItem
        {
            get { return m_ActiveItem; }
            set
            {
                if (m_ActiveItem != value)
                {
                    m_ActiveItem = value;
                    onActivaChanged();
                }
            }
        }

        private void onActivaChanged()
        {
            ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            soundManager.PlayInstance(k_MenuMoveSoundAsset);
        }
    }
}