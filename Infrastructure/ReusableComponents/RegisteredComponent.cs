using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents
{
	public class RegisteredComponent : GameComponent
	{
		public RegisteredComponent(Game i_Game, int i_UpdateOrder)
			: base(i_Game)
		{
			Game.Components.Add(this); // self-register as a coponent
		}

		public RegisteredComponent(Game i_Game)
			: this(i_Game, int.MaxValue)
		{ }
	}
}
