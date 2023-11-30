namespace LangtonsAnt
{
    public interface IAnt
    {
        public int I { get; set; }
        public int J { get; set; }
        public AntDirection Direction { get; set; }
        public byte Act(byte oldValue);

        public void RotateCW();
        public void RotateCCW();
        public IAnt Clone();
    }
}
