using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangtonsAnt
{
    public class GeneralizedAnt : Ant
    {
        public GeneralizedAnt(int i, int j, AntDirection direction) : base(i, j, direction)
        {
        }

        public string Rule { get; set; } = "RL";

        public override byte Act(byte oldValue)
        {
            if (Rule[oldValue] == 'R')
            {
                RotateCW();
            }
            else
            {
                RotateCCW();
            }
            Move();
            byte ret;
            if (oldValue == 0)
                ret = 1;
            else // oldValue = 1
                ret = 0;
            return ret;
        }
    }
}
