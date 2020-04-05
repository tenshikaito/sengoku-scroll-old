using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{

    public class FinancialReporting : BaseObject
    {
        public int id;
        public int stronghold { get; set; }
        public Supplies income;
        public Supplies total;
        /// <summary>上次报告时的人口</summary>
        public int population;
        /// <summary>人口增长</summary>
        public int populationGrowth;

        /// <summary>上次报告时的技术力</summary>
        public int technology;
        /// <summary>技术力增长</summary>
        public int technologyGrowth;

        public void init()
        {
            income.Clear();
            total.Clear();
        }
    }
}
