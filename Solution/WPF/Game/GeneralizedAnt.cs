using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LangtonsAnt
{
    public class GeneralizedAnt : Ant
    {
        protected int maxColor = 1;
        protected string rule = "LR";

        public string Rule
        {
            get { return rule; }
            set
            {
                if (!IsRuleValid(value))
                    throw new ArgumentException("The rule can only consist from L and R characters and be longer than 2 characters. Example: LLRR");
                rule = value;
                maxColor = value.Length - 1;
            }
        }

        private bool IsRuleValid(string proposedRule)
        {
            return proposedRule != null && Regex.IsMatch(proposedRule, "^[L|R]{2,14}$");
        }

        public GeneralizedAnt(int i, int j, AntDirection direction) : base(i, j, direction)
        {
        }

        public override byte Act(byte oldValue)
        {
            var oldValueNormalized = (byte)(oldValue % (maxColor + 1));
            if (rule[oldValueNormalized] == 'R')
            {
                RotateCW();
            }
            else  // == 'L'
            {
                RotateCCW();
            }
            Move();
            return (byte)((oldValueNormalized + 1) % (maxColor + 1));
        }

        public override IAnt Clone()
        {
            return new GeneralizedAnt(i: this.I, j: this.J, direction: this.Direction)
            {
                Rule = this.Rule
            };
        }

    }
}
