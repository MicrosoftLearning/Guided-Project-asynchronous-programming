using System;

namespace LangtonsAnt
{
    public class GameOverException : Exception
    {
        public GameOverException(string? message) : base(message) { }
    }
}