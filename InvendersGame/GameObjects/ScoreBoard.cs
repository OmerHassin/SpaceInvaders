using System;
using System.Collections.Generic;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;
using Microsoft.Xna.Framework;


namespace InvandersGame.GameObjects
{
    public class ScoreBoard : CompositeDrawableComponent<Sprite2D>
    {
        private const string k_FontAssetName = @"Fonts\Consolas";
        private const string k_FontText = @"P{0} Score: {1}";
        private const float k_SpaceBetweenTextBlocks = 2.5f;
        private const int k_SpaceBetweenUIObject = 8;
        private const int k_SoulSize = 16;

        private readonly Vector2 r_SoulScales = new Vector2(0.5f, 0.5f);
        private readonly List<TextBlockcs> r_Scores;
        private readonly List<Stack<ShipSoul>> r_Souls;
        private readonly int r_NumStartingSouls;

        public ScoreBoard(Game i_Game, List<Player> i_Players, int i_NumStartingSouls)
            : base(i_Game)
        {
            r_NumStartingSouls = i_NumStartingSouls;
            r_Scores = new List<TextBlockcs>();
            r_Souls = new List<Stack<ShipSoul>>();

            foreach (Player player in i_Players)
            {
                createTextBlocks(player);
                creatSoulStack(player);
                player.ScoreChanged += Player_ScoreChanged;
                player.NumSoulesChanged += Player_NumSoulesChanged;
            }
        }

        private void createTextBlocks(Player i_Player)
        {
            Vector2 delta = new Vector2(k_SpaceBetweenUIObject, k_SpaceBetweenUIObject * (1f + (k_SpaceBetweenTextBlocks * ((float)i_Player.Index))));
            string msg = string.Format(k_FontText, i_Player.Index + 1, 0);
            TextBlockcs scoreText = new TextBlockcs(k_FontAssetName, Game, delta, msg, i_Player.Color);

            this.Add(scoreText);
            r_Scores.Add(scoreText);
        }

        private void creatSoulStack(Player i_Player)
        {
            Stack<ShipSoul> soulsSprites = new Stack<ShipSoul>();
            ShipSoul currentSoul;

            for (int i = 0; i < i_Player.Souls; i++)
            {
                currentSoul = createSoul(i_Player, i);
                soulsSprites.Push(currentSoul);
                this.Add(currentSoul);
            }

            r_Souls.Add(soulsSprites);
        }

        private ShipSoul createSoul(Player i_Player, int i_SoulIndex)
        {
            float x, y;
            x = -i_SoulIndex * (k_SpaceBetweenUIObject + k_SoulSize);
            y = (k_SoulSize + k_SpaceBetweenUIObject) * i_Player.Index;

            return new ShipSoul(i_Player.AssetName, Game, new Vector2(x, y), r_SoulScales);
        }

        private void Player_ScoreChanged(object sender, EventArgs e)
        {
            Player player = sender as Player;

            if (player != null)
            {
                r_Scores[player.Index].Text = string.Format(k_FontText, player.Index + 1, player.Score);
            }
        }

        private void Player_NumSoulesChanged(object sender, EventArgs e)
        {
            Player player = sender as Player;
            ShipSoul shipSoul;

            if (r_Souls[player.Index].Count > 0)
            {
                shipSoul = r_Souls[player.Index].Pop();
                shipSoul.Visible = false;
                this.Remove(shipSoul);
            }      
        }
    }
}