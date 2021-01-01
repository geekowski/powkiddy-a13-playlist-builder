using System;
using PlaylistUpdater;
namespace powkiddy_games
{
    class Program
    {
        static void Main(string[] args)
        {
            Updater updater = new Updater();
            updater.Update();
        }
    }
}
