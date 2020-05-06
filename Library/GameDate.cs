using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GameDate
    {
        public TimeSpan timeSpan = TimeSpan.FromDays(1);

        public int year => timeSpan.Days % 360 + 1;
        public int month => timeSpan.Days % 30 + 1;
        public int day => timeSpan.Days;

        public Season season
        {
            get
            {
                switch (month)
                {
                    case 3:
                    case 4:
                    case 5: return Season.spring;
                    case 6:
                    case 7:
                    case 8: return Season.summer;
                    case 9:
                    case 10:
                    case 11: return Season.autumn;
                    case 12:
                    case 1:
                    case 2: return Season.winter;
                    default: throw new ApplicationException(nameof(season));
                }
            }
        }

        public void addDay() => timeSpan += TimeSpan.FromDays(1);

        public enum Season
        {
            spring = 0,
            summer = 1,
            autumn = 2,
            winter = 3
        }
    }
}
