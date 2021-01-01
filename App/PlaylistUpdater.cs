using System;
using System.IO;
using System.Security;

namespace PlaylistUpdater {
    class GameSystem {
        public string[] Extensions;
        public string Name;

        public GameSystem(string name, string[] extensions) {
            Extensions = extensions;
            Name = name;
        }
    }
    class Game {
        public string Title;
        public string File;
        public Game(string title, string file) {
            Title = title;
            File = file;
        }
    }

    class Updater {
        string gamesPath = "game";
        GameSystem[] gameSystems = { 
            new GameSystem("CPS", new string[] { "zip" }),
            new GameSystem("FBA", new string[] { "zip" }),
            new GameSystem("FC", new string[] { "nes" }),
            new GameSystem("GB", new string[] { "gb" }),
            new GameSystem("GBA", new string[] { "gba" }),
            new GameSystem("GBC", new string[] { "gb", "zip" }),
            new GameSystem("GG", new string[] { "zip" }),
            new GameSystem("MD", new string[] { "zip" }),
            new GameSystem("NEOGEO", new string[] { "zip" }),
            new GameSystem("PS", new string[] { "cue", "pbp" }), 
            new GameSystem("SFC", new string[] { "sfc", "smc" })
        };

        public void Update() {
            Array.ForEach(gameSystems, system => {
                Game[] games = scanDirectory(gamesPath, system.Name, system.Extensions);
                Playlist playlist = new Playlist(system.Name, games);
                playlist.Create();
            });
            
        }
        private Game[] scanDirectory(string path, string systemName, string[] extensions) {
            string directoryPath = Path.Join(path, systemName);
            if (!Directory.Exists(directoryPath)) { return null; }
            string[] fileNames = Directory.GetFiles(directoryPath);
            fileNames = Array.FindAll(fileNames, fileName =>  Array.Exists(extensions, extension => fileName.ToUpper().EndsWith(extension.ToUpper())));
            Array.Sort(fileNames, StringComparer.InvariantCulture);
            Game[] games = Array.ConvertAll(fileNames, fileName => fileToGame(fileName));
            return games;
        }

        private Game fileToGame(string path) {
            string title = Path.GetFileNameWithoutExtension(path);
            string file = Path.GetFileName(path);
            Game game = new Game(title, file);
            Console.WriteLine(title + " " + file);
            return game;
        }
    }

    class Playlist {
        string System;
        Game[] Games;
        string resPath = "settings/res";
        string[] playLists = new string[] { "game_strings_ch.xml", "game_strings_en.xml" };
        public Playlist(string system, Game[] games) {
         System = system;
         Games = games;
        }
        public void Create() {
            backupPlaylists();
            buildPlaylist();
        }
        private void backupPlaylists() {
            Array.ForEach(playLists, path => {
                string filePath = Path.Join(resPath, System, "string", path);
                string backupPath = Path.Join(resPath, System, "string", "_" + path);
                File.Copy(filePath, backupPath, true);
            }); 
        }
        private void buildPlaylist() {
            Array.ForEach(playLists, path => {
                string filePath = Path.Join(resPath, System, "string", path);
                StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine($"<?xml version=\"1.0\"?><strings_resources>");
                writer.WriteLine($"<icon_para game_list_total=\"{Games.Length}\"></icon_para>");
                int pageNumber = 1;
                int iconNumber = 0;
                int index = 0;
                Array.ForEach(Games, game => {
                    if (iconNumber == 0) {
                        writer.WriteLine($"  <icon_page{pageNumber}>");        
                    }
                    writer.WriteLine($"    <icon{iconNumber}_para name=\"{SecurityElement.Escape(game.Title)}\" game_path=\"{SecurityElement.Escape(game.File)}\"></icon{iconNumber}_para>");
                    iconNumber += 1;
                    index = index + 1;
                    if (iconNumber == 8 || index == Games.Length) {
                        writer.WriteLine($"  </icon_page{pageNumber}>"); 
                        pageNumber += 1;
                        iconNumber = 0; 
                    }
                });
                writer.WriteLine($"</strings_resources>");
                writer.Close();
            }); 
        }
    }
}
