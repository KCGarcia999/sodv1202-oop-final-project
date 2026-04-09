namespace ConnectFourProject
{
    public abstract class Player
    {
        public string Name { get; }
        public char Symbol { get; }
        public int Wins { get; set; }

        protected Player(string name, char symbol, int wins)
        {
            Name = name;
            Symbol = symbol;
            Wins = wins;
        }

        public abstract int GetMove(Board board, ConsoleView view);
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, char symbol, int wins) : base(name, symbol, wins)
        {
        }

        public override int GetMove(Board board, ConsoleView view)
        {
            while (true)
            {
                view.ShowMessage($"{Name} ({Symbol}), choose a column (1-7): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int column))
                {
                    column--; // convert 1-7 to 0-6

                    if (column >= 0 && column < Board.Columns)
                    {
                        if (!board.IsColumnFull(column))
                        {
                            return column;
                        }
                        else
                        {
                            view.ShowMessage("That column is full. Try again.");
                        }
                    }
                    else
                    {
                        view.ShowMessage("Please enter a number from 1 to 7.");
                    }
                }
                else
                {
                    view.ShowMessage("Invalid input. Please enter a number.");
                }
            }
        }
    }

    public class Board
    {
        public const int Rows = 6;
        public const int Columns = 7;

        private char[,] grid;

        public Board()
        {
            grid = new char[Rows, Columns];
            Initialize();
        }

        public void Initialize()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    grid[row, col] = '.';
                }
            }
        }

        public char GetCell(int row, int col)
        {
            return grid[row, col];
        }

        public bool IsColumnFull(int column)
        {
            return grid[0, column] != '.';
        }

        public bool DropDisc(int column, char symbol)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (grid[row, column] == '.')
                {
                    grid[row, column] = symbol;
                    return true;
                }
            }

            return false;
        }

        public bool IsFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (!IsColumnFull(col))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckWin(char symbol)
        {
            return CheckHorizontal(symbol) ||
                   CheckVertical(symbol) ||
                   CheckDiagonalDownRight(symbol) ||
                   CheckDiagonalUpRight(symbol);
        }

        private bool CheckHorizontal(char symbol)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row, col + 1] == symbol &&
                        grid[row, col + 2] == symbol &&
                        grid[row, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckVertical(char symbol)
        {
            for (int row = 0; row <= Rows - 4; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row + 1, col] == symbol &&
                        grid[row + 2, col] == symbol &&
                        grid[row + 3, col] == symbol)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckDiagonalDownRight(char symbol)
        {
            for (int row = 0; row <= Rows - 4; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row + 1, col + 1] == symbol &&
                        grid[row + 2, col + 2] == symbol &&
                        grid[row + 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckDiagonalUpRight(char symbol)
        {
            for (int row = 3; row < Rows; row++)
            {
                for (int col = 0; col <= Columns - 4; col++)
                {
                    if (grid[row, col] == symbol &&
                        grid[row - 1, col + 1] == symbol &&
                        grid[row - 2, col + 2] == symbol &&
                        grid[row - 3, col + 3] == symbol)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class ConsoleView
    {
        public void DisplayWelcome()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("        CONNECT FOUR");
            Console.WriteLine("=================================");
        }

        public void DisplayBoard(Board board)
        {
            Console.WriteLine();

            for (int row = 0; row < Board.Rows; row++)
            {
                for (int col = 0; col < Board.Columns; col++)
                {
                    Console.Write(board.GetCell(row, col) + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("1 2 3 4 5 6 7");
            Console.WriteLine();
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void DisplayWinner(Player player)
        {
            Console.WriteLine($"Congratulations! {player.Name} ({player.Symbol}) wins!");
            player.Wins += 1;
        }

        public void DisplayDraw()
        {
            Console.WriteLine("It's a draw!");
        }

        public bool AskPlayAgain()
        {
            Console.Write("Play again? (y/n): ");
            string input = Console.ReadLine()?.Trim().ToLower();
            Console.Clear();
            return input == "y";
        }
    }

    public class GameController
    {
        private Board board;
        private ConsoleView view;
        private Player player1;
        private Player player2;
        private Player currentPlayer;

        public GameController()
        {
            board = new Board();
            view = new ConsoleView();
        }

        public void Start()
        {
            view.DisplayWelcome();
            SetupPlayers();

            bool playAgain;
            do
            {
                PlayGame();
                playAgain = view.AskPlayAgain();
                board.Initialize();
            }
            while (playAgain);
        }

        private void SetupPlayers()
        {
            Console.Write("Enter name for Player 1 (X): ");
            string name1 = Console.ReadLine();

            Console.Write("Enter name for Player 2 (O): ");
            string name2 = Console.ReadLine();

            player1 = new HumanPlayer(name1, 'X', 0);
            player2 = new HumanPlayer(name2, 'O', 0);
            currentPlayer = player1;
        }

        private void PlayGame()
        {
            currentPlayer = player1;

            while (true)
            {
                view.DisplayBoard(board);

                int column = currentPlayer.GetMove(board, view);
                board.DropDisc(column, currentPlayer.Symbol);

                if (board.CheckWin(currentPlayer.Symbol))
                {
                    Console.Clear();
                    view.DisplayBoard(board);
                    view.DisplayWinner(currentPlayer);
                    Console.WriteLine("Current score:");
                    Console.WriteLine($"{player1.Name}: {player1.Wins}, {player2.Name}: {player2.Wins}");
                    break;
                }

                if (board.IsFull())
                {
                    Console.Clear();
                    view.DisplayBoard(board);
                    view.DisplayDraw();
                    Console.WriteLine("Current score:");
                    Console.WriteLine($"{player1.Name}: {player1.Wins}, {player2.Name}: {player2.Wins}");
                    break;
                }

                SwitchPlayer();
                Console.Clear();
            }
        }

        private void SwitchPlayer()
        {
            currentPlayer = currentPlayer == player1 ? player2 : player1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameController game = new GameController();
            game.Start();
        }
    }
}