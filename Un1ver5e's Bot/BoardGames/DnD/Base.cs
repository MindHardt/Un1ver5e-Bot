using System;
using System.Collections.Generic;
using System.Text;

namespace Un1ver5e.Bot.BoardGames.DnD
{
    public /*abstract*/ class Health : IDiscordMessageAppliable//IDependingProperty
    {
        public int CurrentAmount { get; private set; }
        public int TotalAmount { get; private set; }
        public int DeathLimit { get; private set; }

        public virtual string GetAsDiscord()
        {
            if (CurrentAmount >= 0)
            {
                int hearts = (CurrentAmount * 10 / TotalAmount) + 1;
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= 10; i++)
                {
                    sb.Append(i >= hearts ? ":red_square:" : ":green_square:");
                }
                return sb.ToString();
            }
            else
            {
                int hearts = (CurrentAmount * 10 / DeathLimit) + 1;
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= 10; i++)
                {
                    sb.Append(i >= hearts ? ":skull:" : ":red_square:");
                }
                return sb.ToString();
            }
        }
        public Health(int total, int now, int limit)
        {
            TotalAmount = total;
            CurrentAmount = now;
            DeathLimit = limit;
        }
    }



    public interface IDependingProperty
    {
        public IDependableProperty[] Roots { get; protected set; }
        public void Recalculate();
    }
    public interface IDependableProperty
    {
        public bool IsChanged { get; protected set; }
        public int GetValue();
    }
    public interface IDiscordMessageAppliable
    {
        public string GetAsDiscord();
    }
}
