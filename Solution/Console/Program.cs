using LangtonsAnt;

Game game = new Game(16);

void Print(Game game)
{
  // NOTE Turn off Debug › Console: Collapse Identical Lines
  for (int i = 0; i < game.Field.GetLength(0); i++)
  {
    for (int j = 0; j < game.Field.GetLength(1); j++)
    {
      char fieldChar = '░';
      // If ant is at the cell, display ant direction instead of color value
      IAnt? ant = game.Ants.FirstOrDefault(a => (i == a.I) && (j == a.J));
      if (ant != null)
      {
        // Draw one of the ants
        switch (ant.Direction)
        {
          case AntDirection.Up:
            fieldChar = '▲';
            break;
          case AntDirection.Right:
            fieldChar = '►';
            break;
          case AntDirection.Down:
            fieldChar = '▼';
            break;
          case AntDirection.Left:
            fieldChar = '◄';
            break;
        }
      }
      else
      {
        fieldChar = game.Field[i, j] == 0 ? '░' : '▓';
      }

      Console.Write($"{fieldChar}");
    }
    Console.WriteLine();
  }
}

string? input;
do
{
  game.NextGeneration();
  Console.WriteLine($"Generation #: {game.GenerationN}");
  Console.WriteLine("Field:");
  Print(game);
  Console.WriteLine("Press Enter for a new turn or any other key for exit");
  input = Console.ReadLine();
}
while (String.IsNullOrEmpty(input));


