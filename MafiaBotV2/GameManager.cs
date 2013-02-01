using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MafiaBotV2.Network;

namespace MafiaBotV2
{
    public class GameManager
    {
        List<IGame> games = new List<IGame>();
        public List<IGame> Games {
            get { return games; }
        }

        Bot bot;

        public GameManager(Bot bot) {
            this.bot = bot;
        }

        public IGame CreateGame(NetUser creator, string name, string type) {
            System.Diagnostics.Debug.Assert(bot.Master != null);
            System.Diagnostics.Debug.Assert(bot.Config != null);

            if(games.Any(g => g.Name.ToLower() == name.ToLower())) {
                throw new DuplicateNameException("A game with the name " + name + " already exists.");
            }

            IGame game = null;
            switch(type) {
                case "mafia":
                    game = new MafiaGame.MafiaGame(bot, creator, name);
                    break;
                case "barebones":
                    game = new Barebones.BarebonesGame(bot, creator, name);
                    break;
                default:
                    throw new InvalidGameTypeException("Gametype " + type + " not found.");

            }
            game.Creator = creator;

            if(game == null) {
                throw new InvalidGameTypeException("Unknown game type " + type);
            }

            games.Add(game);

            creator.Commands.Add(new Commands.DestroyCommand(this, game));

            return game;
        }

        public void Destroy(string name) {
            IGame game = games.Find(g => g.Name.ToLower() == name.ToLower());
            if(game == null) {
                throw new GameNotFoundException("No game with the name " + name);
            }

            game.Creator.Commands.RemoveAll(c => (c is Commands.DestroyCommand) && (c.Parent == game));
            Destroy(game);
        }
        
        public void Destroy(IGame game) {
            game.Destroy();
            games.Remove(game);
        }

        public IGame GetGame(string name) {
            return games.Find(g => g.Name.ToLower() == name.ToLower());
        }

        [global::System.Serializable]
        public class DuplicateNameException : MafiaException
        {
            public DuplicateNameException() { }
            public DuplicateNameException(string message) : base(message) { }
            public DuplicateNameException(string message, Exception inner) : base(message, inner) { }
            protected DuplicateNameException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        [global::System.Serializable]
        public class InvalidGameTypeException : MafiaException
        {
            public InvalidGameTypeException() { }
            public InvalidGameTypeException(string message) : base(message) { }
            public InvalidGameTypeException(string message, Exception inner) : base(message, inner) { }
            protected InvalidGameTypeException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        [global::System.Serializable]
        public class GameNotFoundException : MafiaException
        {
            public GameNotFoundException() { }
            public GameNotFoundException(string message) : base(message) { }
            public GameNotFoundException(string message, Exception inner) : base(message, inner) { }
            protected GameNotFoundException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
