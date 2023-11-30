using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LangtonsAnt
{
    public class Game : IGame
    {
        protected const int defaultSize = 128;
        public int GenerationN { get; set; } = 0;
        public byte[,] Field { get; set; }
        public IList<IAnt> Ants { get; set; }
        public int Size
        {
            get => Field?.GetLength(0) ?? defaultSize;
        }

        public Game(int size, IAnt[]? initialAnts)
        {
          Field = new byte[size, size];
          Ants = new List<IAnt>(initialAnts ?? new IAnt[] { });
        }

        public Game() : this(defaultSize, null) { }

        private byte[,] CalcNextGeneration()
        {
            var newField = (byte[,])Field.Clone();

            if (Ants.Count == 0)
                throw new GameOverException("We no longer have any ants");

            for (int index = Ants.Count - 1; index >= 0; index--)
            {
                var ant = Ants[index];

                // Check if the ant is still within the field
                if (ant.I < 0 || ant.J < 0 || ant.J >= Size || ant.I >= Size)
                {
                    Ants.RemoveAt(index);
                    continue;
                }

                byte v = newField[ant.I, ant.J];
                int i = ant.I;
                int j = ant.J;
                byte newVal = ant.Act(v);
                newField[i, j] = newVal;
            }

            return newField;
        }

        public void NextGeneration()
        {
            Field = CalcNextGeneration();
            GenerationN++;
        }
        
        public IGame Clone()
        {
            var game = new Game(this.Size, this.Ants.Select(a => (IAnt)a.Clone()).ToArray())
            {
                Field = (byte[,])Field.Clone(),
                GenerationN = this.GenerationN
            };
            return game;
        }
    }
}