using System;

namespace InvendersGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new InvadersGame())
                game.Run();
        }
    }
}
