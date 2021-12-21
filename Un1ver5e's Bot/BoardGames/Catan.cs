using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using Un1ver5e.Service;

namespace Un1ver5e.Bot.BoardGames.Catan
{
    public class Game
    {
        public class Field
        {


            public abstract class Hex
            {
                public readonly TownPosition[] Towns;
                public readonly RoadPosition[] Roads;
                public abstract ResourceStack.Resource ResourceType { get; }

            }
            public class RoadPosition
            {
                public readonly int Id;
                public Player Owner { get; private set; }
                public RoadStatus Status { get; private set; }
                public readonly TownPosition AdjacentTowns;

                public enum RoadStatus
                {
                    Empty = 0,
                    Build = 1
                }

            }

            public class TownPosition
            {
                public readonly int Id;
                public Player Owner { get; private set; }
                public TownStatus Status { get; private set; }
                public readonly TownPosition[] AdjacentTowns;
                public readonly RoadPosition[] AdjacentRoads;

                public enum TownStatus
                {
                    Blocked = -1,
                    Empty = 0,
                    Village = 1,
                    Town = 2
                }
            }
        }

        public class Player
        {
            public readonly Game Game;
            public readonly string NickName;
            public readonly Color Color;

            public readonly ResourceStack Clay;
            public readonly ResourceStack Lumber;
            public readonly ResourceStack Ore;
            public readonly ResourceStack Sheep;
            public readonly ResourceStack Wheat;


            public ResourceStack GetResourceStack(ResourceStack.Resource type) => type switch
            {
                ResourceStack.Resource.Clay => Clay,
                ResourceStack.Resource.Lumber => Lumber,
                ResourceStack.Resource.Ore => Ore,
                ResourceStack.Resource.Sheep => Sheep,
                ResourceStack.Resource.Wheat => Wheat,
                _ => throw new NotImplementedException()
            };

            public Player(DiscordMember member, Game game)
            {
                NickName = member.Nickname;
                Game = game;

                Clay     = new ResourceStack(this, ResourceStack.Resource.Clay);
                Lumber   = new ResourceStack(this, ResourceStack.Resource.Lumber);
                Ore      = new ResourceStack(this, ResourceStack.Resource.Ore);
                Sheep    = new ResourceStack(this, ResourceStack.Resource.Sheep);
                Wheat    = new ResourceStack(this, ResourceStack.Resource.Wheat);

                Color = game._colors.Pop();
            }

        }

        public class ResourceStack
        {
            public readonly Player Owner;
            public int Amount { get; private set; } = 0;
            public readonly Resource Type;

            /// <summary>
            /// Removes a number of resources from the game.
            /// </summary>
            /// <param name="amount">A number of resources to remove.</param>
            public void Discard(int amount) => Amount -= amount;

            /// <summary>
            /// Creates a number of resources for this stack.
            /// </summary>
            /// <param name="amount">A number of resources to give.</param>
            public void Gain(int amount) => Amount += amount;

            private void SendTo(ResourceStack recipient, int amount)
            {
                Amount -= amount;
                recipient.Amount += amount;
            }

            /// <summary>
            /// Tries to send a designated amount of resources from this stack to a target player's stack.
            /// </summary>
            /// <param name="amount">The number of resources to send.</param>
            /// <param name="recipient">The player which recieves the </param>
            /// <exception cref="ArgumentException"></exception>
            public void TrySend(int amount, Player recipient)
            {
                if (Amount < amount) throw new ArgumentException($"Недостаточно ресурсов! [{Type}]");
                SendTo(recipient.GetResourceStack(Type), amount);
            }

            public ResourceStack(Player owner, Resource resource)
            {
                Type = resource;
            }

            public enum Resource
            {
                Clay,
                Lumber,
                Ore,
                Sheep,
                Wheat
            }
        }

        public record Recipe
        {
            public Dictionary<ResourceStack.Resource, int> Resources { get; init; }

            public readonly static Recipe TownRecipe = new Recipe()
            {
                Resources = new Dictionary<ResourceStack.Resource, int>()
                {
                    { ResourceStack.Resource.Ore, 3},
                    { ResourceStack.Resource.Wheat, 2}
                }
            };

            public readonly static Recipe VillageRecipe = new Recipe()
            {
                Resources = new Dictionary<ResourceStack.Resource, int>()
                {
                    { ResourceStack.Resource.Clay, 1},
                    { ResourceStack.Resource.Lumber, 1},
                    { ResourceStack.Resource.Sheep, 1},
                    { ResourceStack.Resource.Wheat, 1}
                }
            };

            public readonly static Recipe RoadRecipe = new Recipe()
            {
                Resources = new Dictionary<ResourceStack.Resource, int>()
                {
                    { ResourceStack.Resource.Clay, 1},
                    { ResourceStack.Resource.Lumber, 1}
                }
            };

            public readonly static Recipe DevCardRecipe = new Recipe()
            {
                Resources = new Dictionary<ResourceStack.Resource, int>()
                {
                    { ResourceStack.Resource.Ore, 1},
                    { ResourceStack.Resource.Sheep, 1},
                    { ResourceStack.Resource.Wheat, 1}
                }
            };
        }

        public abstract class Building
        {
            public abstract Recipe Price { get; }

            public abstract class Settlement : Building
            {
                public abstract int Quantifier { get; }
                public readonly Field.TownPosition Position;

                public class Town : Settlement
                {
                    public override Recipe Price => Recipe.TownRecipe;

                    public override int Quantifier => 2;
                }

                public class Village : Settlement
                {
                    public override Recipe Price => Recipe.VillageRecipe;
                    public override int Quantifier => 1;

                }
            }

            public class Road : Building
            {
                public readonly Field.RoadPosition Position;
                public override Recipe Price => Recipe.RoadRecipe;
            }

            public class DevelopmentCard : Building
            {
                public override Recipe Price => Recipe.DevCardRecipe;
            }
        }

        private Stack<Color> _colors = new Stack<Color>(new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.White, Color.Black }.Shuffle());
    }
}
