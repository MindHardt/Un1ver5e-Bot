using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using static Catan.Drawer;

namespace CatanGame
{
    public class Game : IDrawable
    {
        public Player[] Players;
        public VictoryPoint[] VictoryPoints
        {
            get
            {
                UpdateVictoryPoints();
                return victoryPoints;
            }
        }
        private VictoryPoint[] victoryPoints;
        private void UpdateVictoryPoints()
        {

        }




        public enum FieldSize
        {
            Default,
            Big
        }
    }

    /// <summary>
    /// Represents game field's hex.
    /// </summary>
    public class Hex
    {
        public readonly Resources.ResourceType Type;
        public readonly Crossroad[] NearbyCrossroads;
        public readonly RoadPosition[] NearbyRoadPositions;
    }

    /// <summary>
    /// Represents game field's crossroad on which towns and villages are built.
    /// </summary>
    public class Crossroad
    {
        public readonly Hex[] NearbyHexes;
        public readonly RoadPosition[] NearbyRoadPositions;
    }

    /// <summary>
    /// Represents game field's road position
    /// </summary>
    public class RoadPosition
    {
        public readonly Crossroad[] ConnectedCrossroads;
    }

    /// <summary>
    /// Represents a player with all their resources, nickname and avatar.
    /// </summary>
    public class Player : IDrawable
    {
        public readonly string Nickname;
        public readonly string AvatarUrl;
        public List<DevelopmentCard> DevelopmentCards = new List<DevelopmentCard>();
        public Resources.Stack Clay   { get; private set; }
        public Resources.Stack Lumber { get; private set; }
        public Resources.Stack Ore    { get; private set; }
        public Resources.Stack Sheep  { get; private set; }
        public Resources.Stack Wheat  { get; private set; }

        /// <summary>
        /// Checks if the Player has the resources, and if so, sends them to the other player.
        /// </summary>
        /// <param name="recipient">A player to whom the resources are given</param>
        /// <param name="resources">A pack of all the resources sent</param>
        public void TrySendResources(Player recipient, Resources.Pack resources)
        {
            if (!Has(resources)) throw new Exception("У вас нет таких ресурсов!");
            Resources.Package clay   = new Resources.Package(resources.Clay,   Resources.ResourceType.Clay,   this, recipient);
            Resources.Package lumber = new Resources.Package(resources.Lumber, Resources.ResourceType.Lumber, this, recipient);
            Resources.Package ore    = new Resources.Package(resources.Ore,    Resources.ResourceType.Ore,    this, recipient);
            Resources.Package sheep  = new Resources.Package(resources.Sheep,  Resources.ResourceType.Sheep,  this, recipient);
            Resources.Package wheat  = new Resources.Package(resources.Wheat,  Resources.ResourceType.Wheat,  this, recipient);
        }

        /// <summary>
        /// Returns a resource stack by the type of the resource
        /// </summary>
        /// <param name="type">A resource type to search</param>
        /// <returns></returns>
        public Resources.Stack GetStack(Resources.ResourceType type) =>
        type switch
        {
            Resources.ResourceType.Clay => Clay,
            Resources.ResourceType.Lumber => Lumber,
            Resources.ResourceType.Ore => Ore,
            Resources.ResourceType.Sheep => Sheep,
            Resources.ResourceType.Wheat => Wheat,
            _ => throw new Exception("How did you do that? 0_o"),
        };

        /// <summary>
        /// Checks if Player has all of resources in Pack
        /// </summary>
        /// <param name="resources">A pack to check</param>
        /// <returns></returns>
        public bool Has(Resources.Pack resources)
        {
            return (
                Clay   .Has(resources.Clay) &&
                Lumber .Has(resources.Lumber) &&
                Ore    .Has(resources.Ore) &&
                Sheep  .Has(resources.Sheep) &&
                Wheat  .Has(resources.Wheat));
        }

        /// <summary>
        /// Checks if Player has enough resources to cut in halves when 7 is thrown
        /// </summary>
        /// <returns></returns>
        public bool HasTooMuchResources() => (Clay.Amount + Lumber.Amount + Ore.Amount + Sheep.Amount + Wheat.Amount > 7) ? true : false;
    }

    /// <summary>
    /// A class for all the actions with resources imaginable.
    /// </summary>
    public static class Resources
    {
        public class Stack
        {
            public int Amount { get; private set; }
            public void Give(int amount)
            {
                if (amount <= 0) throw new Exception("Так нельзя!");
                Amount += amount;
            }
            public bool Has(int amount)
            {
                return Amount >= amount;
            }
            public void Take(int amount)
            {
                if (amount <= 0) throw new Exception("Так нельзя!");
                Amount -= amount;
            }

        }
        /// <summary>
        /// Represents a pack of all 5 resources
        /// </summary>
        public struct Pack
        {
            public readonly int Clay;
            public readonly int Lumber;
            public readonly int Ore;
            public readonly int Sheep;
            public readonly int Wheat;

            /// <summary>
            /// A constructor for a Pack from text string.
            /// </summary>
            /// <param name="stringQuery">A string with 5 numbers of resources - "c,l,o,s,w" </param>
            public Pack(string stringQuery)
            {
                Clay = 0;
                Lumber = 0;
                Ore = 0;
                Sheep = 0;
                Wheat = 0;
                string[] resources = stringQuery.Split(',');
                if (
                    resources.Length == 5 &&
                    int.TryParse(resources[0], out Clay) &&
                    int.TryParse(resources[1], out Lumber) &&
                    int.TryParse(resources[2], out Ore) &&
                    int.TryParse(resources[3], out Sheep) &&
                    int.TryParse(resources[4], out Wheat))
                    throw new ArgumentException("Некорректный запрос!");
            }
            /// <summary>
            /// A constructor for a Pack from 5 integer numbers.
            /// </summary>
            /// <param name="resources">5 non-negative integers representing all 5 resources</param>
            public Pack(params int[] resources)
            {
                if (resources.Length < 5) throw new ArgumentException("Некорректный запрос!");
                Clay = resources[0];
                Lumber = resources[1];
                Ore = resources[2];
                Sheep = resources[3];
                Wheat = resources[4];
            }
        }
        /// <summary>
        /// Represents delivery package. Once it's created it is delivered
        /// </summary>
        public struct Package
        {
            public readonly Player Sender;
            public readonly Player Recipient;
            public readonly int Amount;
            public readonly ResourceType Type;

            public Package(int amount, ResourceType type, Player from, Player to)
            {
                Amount = amount;
                Type = type;
                Sender = from;
                Recipient = to;
                Retrieve();
                Deliver();
            }
            private void Deliver()
            {
                Recipient.GetStack(Type).Give(Amount);
            }
            private void Retrieve()
            {
                Sender.GetStack(Type).Take(Amount);
            }
        }

        public enum ResourceType
        {
            Clay,
            Lumber,
            Ore,
            Sheep,
            Wheat
        }

    }

    /// <summary>
    /// Represents an abstract class for all development cards.
    /// </summary>
    public abstract class DevelopmentCard : IDrawable
    {
        public abstract void Use();
        public Player Owner;
        public Game IncludingGame;


        /// <summary>
        /// A knight development card
        /// </summary>
        public class Knight : DevelopmentCard, IDrawable
        {
            public override void Use()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// A road building development card
        /// </summary>
        public class RoadBuilding : DevelopmentCard, IDrawable
        {
            public override void Use()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// A monopoly development card
        /// </summary>
        public class Monopoly : DevelopmentCard, IDrawable
        {
            public Resources.ResourceType Type;
            public override void Use()
            {
                foreach (Player player in IncludingGame.Players)
                {
                    if (player != Owner) player.TrySendResources(Owner, new Resources.Pack
                        (
                        player.Clay.Amount,
                        player.Lumber.Amount,
                        player.Ore.Amount,
                        player.Sheep.Amount,
                        player.Wheat.Amount
                        ));
                }
            }
        }
        /// <summary>
        /// A victory point development card
        /// </summary>
        public class VictoryPoint : DevelopmentCard, IDrawable
        {
            public override void Use()
            {
                throw new NotImplementedException();
            }

        }
        /// <summary>
        /// An invention development card
        /// </summary>
        public class Invention : DevelopmentCard, IDrawable
        {
            public override void Use()
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Builds a default random order development card deck, according to field size
        /// </summary>
        /// <param name="size">A size of your field</param>
        /// <returns></returns>
        public static Stack<DevelopmentCard> BuildDevelopmentCardsDeck(Game.FieldSize size)
        {
            List<DevelopmentCard> rawDeck = new List<DevelopmentCard>();
            for (int i = 0; i < 14 + (6 * (int)size); i++)
            {
                rawDeck.Add(new Knight());
            }
            for (int i = 0; i < 2 + (int)size; i++)
            {
                rawDeck.Add(new RoadBuilding());
                rawDeck.Add(new Monopoly    ());
                rawDeck.Add(new Invention   ());
            }
            for (int i = 0; i < 5; i++)
            {
                rawDeck.Add(new VictoryPoint());
            }
            Random rnd = new Random();
            return new Stack<DevelopmentCard>(rawDeck.OrderBy(c => rnd.Next()));
        }

    }

    /// <summary>
    /// Represents a game victory point, with its origin.
    /// </summary>
    public struct VictoryPoint
    {
        public readonly Source OriginType;
        public readonly object Origin;
        public enum Source
        {
            Village,
            Town,
            DevelopmentCard,
            LongestRoad,
            BiggestArmy
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Type of victory point.</param>
        /// <param name="sender">The object which granted this victory point.</param>
        public VictoryPoint(Source source, object sender)
        {
            OriginType = source;
            Origin = sender;
        }
    }
}
