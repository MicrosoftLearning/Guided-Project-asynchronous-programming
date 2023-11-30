using System.Collections.Generic;

namespace LangtonsAnt
{
    public interface IGame
    {
        public int Size { get; }
        public IList<IAnt> Ants { get; set; }
        public int GenerationN { get; }
        public byte[,] Field { get; }
        public void NextGeneration();
        public IGame Clone();
    }

}
