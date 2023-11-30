using LangtonsAnt;

namespace LangtonsAnt
{
    public class Ant : IAnt
    {
        public int I { get; set; }
        public int J { get; set; }
        public AntDirection Direction { get; set; }

        public Ant(int i, int j, AntDirection direction)
        {
            I = i;
            J = j;
            Direction = direction;
        }

        protected void Move()
        {
            if (AntDirection.Up == Direction) I--;
            if (AntDirection.Right == Direction) J++;
            if (AntDirection.Down == Direction) I++;
            if (AntDirection.Left == Direction) J--;
        }

        public void RotateCW()
        {
            Direction = (AntDirection)(((int)Direction + 1) % 4);
        }

        public void RotateCCW()
        {
            Direction = (AntDirection)((int)Direction == 0 ? 3 : (int)Direction - 1);
        }

        public virtual byte Act(byte oldValue)
        {
            byte ret;
            if (oldValue == 0)
            {
                ret = 1;
                RotateCW();
            }
            else
            {
                ret = 0;
                RotateCCW();
            }
            Move();
            return ret;
        }

        public virtual IAnt Clone()
        {
            return new Ant(i: this.I, j: this.J, direction: this.Direction);
        }
    }
}