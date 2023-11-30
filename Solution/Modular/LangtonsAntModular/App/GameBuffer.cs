using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangtonsAnt
{
    public class GameBuffer
    {
        private IGame initialState;
        private int nGenerations;
        private int minPrevious;
        private int minNext;

        private LinkedList<IGame> buffered = new LinkedList<IGame>();
        public LinkedListNode<IGame> currentNode;
        private bool gameOver;
        private bool calculating;

        public IGame? Current {
            get
            {
                return currentNode?.Value;
            }
        }


        public GameBuffer(IGame initialState, int nGenerations)
        {
            this.initialState = initialState;
            this.nGenerations = nGenerations;
            this.minPrevious = nGenerations / 4;
            this.minNext = nGenerations / 4;
            currentNode = buffered.AddLast(initialState);
        }

        public bool MovePrevious()
        {
            if (currentNode.Previous != null)
            {
                currentNode = currentNode.Previous;
                return true;
            }
            return false;   
        }

        public bool MoveNext(int n = 1)
        {
            if (Current == null)
                throw new InvalidOperationException("Cannot get the next state when Current is null");

            // try to go n steps forward
            for (; n > 0; n--)
            {
                // current is the last node
                if (currentNode.Next == null)
                {
                    if (gameOver) return false;
                    else
                    {
                        // we replace old game generation states with the new after we calculate nGenerations states,
                        // so we need to calculate fewer states to keep minPrevious old ones available
                        gameOver = !CalculateNGenerations(nGenerations - minPrevious - minNext);
                    }
                }

                // current is the last node
                if (!calculating && !gameOver && IsMinNextReached())
                {
                    calculating = true;
                    _ = Task.Factory.StartNew(() =>
                    {
                        // we replace old game generation states with the new after we calculate nGenerations states,
                        // so we need to calculate fewer states to keep minPrevious old ones available
                        gameOver = !CalculateNGenerations(nGenerations - minPrevious - minNext);
                        calculating = false;
                    }, TaskCreationOptions.LongRunning);
                }

                currentNode = currentNode.Next;
            }
            return true;
        }

        private bool IsMinNextReached()
        {
            return (buffered.Last.Value.GenerationN - Current.GenerationN) <= minNext;
        }

        private bool CalculateNGenerations(int n)
        {
            for(; n > 0; n--)
            {
                try
                {
                    // base the next generation on the last
                    var newState = buffered.Last!.Value.Clone();
                    newState.NextGeneration();
                    // only keep nGenerations game state to avoid going out of memory
                    if (buffered.Count >= nGenerations)
                        buffered.RemoveFirst();
                    buffered.AddLast(newState);
                }
                catch (GameOverException)
                {
                    return false;
                }
            }
            return true;
        }

        public void FlushBuffer()
        {
            if (Current == null)
                throw new InvalidOperationException("Cannot flush the game buffer when current game state is null");

            while (currentNode.Next != null)
                buffered.RemoveLast();
        }
    }
}
