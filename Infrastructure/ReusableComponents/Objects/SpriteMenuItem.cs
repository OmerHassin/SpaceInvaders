using System;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Animators.ConcreteAnimators;
using Infrastructure.ReusableComponents.Screens;
using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents.Objects
{
    public class SpriteMenuItem : TextBlockcs
    {
        private const string k_FontAssetName = @"Fonts\MediumConsolas";
        private const int k_SpaceBeweenSprites = 50;
        private const int k_FirstSpriteYPosition = 150;
        private const int k_XPosition = 85;
        private const int k_XSpaceForText = 17;
        private const int k_YSpaceForText = 8;
        private const float k_TargetScaleAnimation = 1.07f;
        private const float k_PulsePerSecondAnimation = 0.8f;

        private readonly TimeSpan r_AnimationLenght = TimeSpan.Zero;
        private readonly Color r_ActiveColor = Color.AntiqueWhite;
        private readonly Color r_InctiveColor = Color.Gray;
        private readonly string r_ConstText;

        private SpriteMenuItem m_NextInListItem;
        private SpriteMenuItem m_PreviouseInListItem;

        private bool m_ActiveItem;
        private int m_ItemIndex;
        private string m_ReplaceableText;

        public event EventHandler EnterPressedOnItem;

        public event EventHandler PgUpPressedOnItem;

        public event EventHandler PgDownPressedOnItem;

        public SpriteMenuItem(string i_ConstText, Game i_Game, bool i_ActiveItem, SpriteMenuItem i_PreviouseInListItem, int i_ItemIndex, string i_ReplaceableText, MenuItemsScreen i_Screen)
            : this(i_ConstText, i_Game, i_ActiveItem, i_ItemIndex, i_ReplaceableText, i_Screen)
        {
            m_PreviouseInListItem = i_PreviouseInListItem;
            i_PreviouseInListItem.NextItem = this;
        }

        public SpriteMenuItem(string i_ConstText, Game i_Game, SpriteMenuItem i_PreviouseInListItem, int i_ItemIndex, string i_ReplaceableText, MenuItemsScreen i_Screen)
             : this(i_ConstText, i_Game, false, i_ItemIndex, i_ReplaceableText, i_Screen)
        {
            m_PreviouseInListItem = i_PreviouseInListItem;
            i_PreviouseInListItem.NextItem = this;
        }

        public SpriteMenuItem(string i_ConstText, Game i_Game, bool i_ActiveItem, int i_ItemIndex, string i_Text, MenuItemsScreen i_Screen)
            : base(k_FontAssetName, i_Game, i_ConstText + i_Text)
        {
            r_ConstText = i_ConstText;
            m_ItemIndex = i_ItemIndex;
            m_ActiveItem = i_ActiveItem;
            m_ReplaceableText = i_Text;
            setColor();
            selfRegister(i_Screen);
        }

        private void setColor()
        {
            if (m_ActiveItem)
            {
                TintColor = r_ActiveColor;
            }
            else
            {
                TintColor = r_InctiveColor;
            }
        }

        private void selfRegister(MenuItemsScreen i_Screen)
        {
            i_Screen.AddToItemsScreen(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            initAnimations();
        }

        private void initAnimations()
        {
            PulseAnimator faderAnimator = new PulseAnimator(r_AnimationLenght, k_TargetScaleAnimation, k_PulsePerSecondAnimation);
            m_Animations.Add(faderAnimator);
            RotationOrigin = SourceRectangleCenter;

            if (m_ActiveItem)
            {
                m_Animations.Start();
            }
        }

        protected override void InitPositions()
        {
            base.InitPositions();

            float x, y;

            x = k_XPosition;
            y = k_FirstSpriteYPosition + (k_SpaceBeweenSprites * (m_ItemIndex - 1));

            Position = new Vector2(x, y);
        }

        private void updateText()
        {
            Text = r_ConstText + m_ReplaceableText;
        }

        internal void RunPressedOnItem()
        {
            this.OnPressedOnItem();
        }

        protected virtual void OnPressedOnItem()
        {
            EnterPressedOnItem?.Invoke(this, EventArgs.Empty);
        }

        internal void RunPgUpPressedOnItem()
        {
            this.OnPgUpPressedOnItem();
        }

        private void OnPgUpPressedOnItem()
        {
            PgUpPressedOnItem?.Invoke(this, EventArgs.Empty);
        }

        internal void RunPgDownPressedOnItem()
        {
            this.OnPgDownPressedOnItem();
        }

        private void OnPgDownPressedOnItem()
        {
            PgDownPressedOnItem?.Invoke(this, EventArgs.Empty);
        }

        internal SpriteMenuItem MoveUp()
        {
            Active = false;
            m_PreviouseInListItem.Active = true;
            return m_PreviouseInListItem;
        }

        internal SpriteMenuItem MoveDown()
        {
            Active = false;
            m_NextInListItem.Active = true;
            return m_NextInListItem;
        }

        public SpriteMenuItem NextItem
        {
            get { return m_NextInListItem; }
            set { m_NextInListItem = value; }
        }

        public SpriteMenuItem PreviouseItem
        {
            get { return m_PreviouseInListItem; }
            set { m_PreviouseInListItem = value; }
        }

        public string ReplaceableText
        {
            set
            {
                if (m_ReplaceableText != value)
                {
                    m_ReplaceableText = value;
                    updateText();
                }
            }
        }

        public bool Active
        {
            get { return m_ActiveItem; }
            set
            {
                if (m_ActiveItem != value)
                {
                    OnActiveChanged(value);
                }

                m_ActiveItem = value;
            }
        }

        private void OnActiveChanged(bool i_CurrentActivation)
        {
            if (i_CurrentActivation)
            {
                m_Animations.Start();
                updateColor(r_ActiveColor);
            }
            else
            {
                m_Animations.Reset();
                m_Animations.Stop();
                updateColor(r_InctiveColor);
            }
        }

        private void updateColor(Color i_SetColor)
        {
            TintColor = i_SetColor;
        }
    }
}