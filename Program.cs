using System.Diagnostics.Metrics;
using System.Text;

Console.OutputEncoding = System.Text.Encoding.UTF8; // выводим символ юникода в консоли
List<SnakePart> list = new List<SnakePart>();
int[,] display = new int[20, 20];
int counter = 0;
SnakePart snakePart = new SnakePart();
snakePart.i = 0;
snakePart.j = 0;
snakePart.history.Add(DirectionHistory.Right);
Meal meal = new Meal();
Random rnd = new Random();
meal.i = rnd.Next(1, display.GetLength(0));
meal.j = rnd.Next(1, display.GetLength(1));
ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
Task.Run(() =>
{
    while (true)
    {
        keyInfo = Console.ReadKey(true);
    }
});

for (; ; )
{
    Thread.Sleep(100);
    foreach (var item in list)
    {
        ChangeIndexesByHistory(item);
    }
    HeadMotionHistory(keyInfo, snakePart);
    BodyMotionHistory(list, snakePart);
    if (TryEat(snakePart, display, meal, rnd, list))
    {
        counter++;
    }
    bool gameOver = GameOver(display, snakePart, list);
    DrawDisplay(list, display, snakePart, gameOver, meal, counter);
    if (gameOver)
    {
        break;
    }
}

void BodyMotionHistory(List<SnakePart> list, SnakePart head)
{
    for (int i = 0; i < list.Count; i++)
    {
        if (i == 0)
        {
            list[i].history.Add(head.history.Last());
        }
        else
        {
            list[i].history.Add(list[i - 1].history.Last());
        }
    }
}

static bool TryEat(SnakePart snakePart, int[,] display, Meal meal, Random rnd,
    List<SnakePart> list)
{
    if (snakePart.i == meal.i && snakePart.j == meal.j)
    {
        meal.i = rnd.Next(1, display.GetLength(0));
        meal.j = rnd.Next(1, display.GetLength(1));
        SnakePart tail = new SnakePart();
        tail.i = snakePart.i;
        tail.j = snakePart.j;
        list.Add(tail);
        for (int i = 0; i < list.Count; i++)
        {
            tail.history.Add(DirectionHistory.None);
        }
        return true;
    }
    return false;
}

static void ChangeIndexesByHistory(SnakePart snakePart)
{
    var historyAction = snakePart.history.First();
    if (historyAction == DirectionHistory.Right)
    {
        snakePart.j++;
    }
    else if (historyAction == DirectionHistory.Left)
    {
        snakePart.j--;
    }
    else if (historyAction == DirectionHistory.Up)
    {
        snakePart.i--;
    }
    else if (historyAction == DirectionHistory.Down)
    {
        snakePart.i++;
    }
    snakePart.history.RemoveAt(0);
}

static void HeadMotionHistory(ConsoleKeyInfo keyInfo, SnakePart head)
{
    if (keyInfo.Key == ConsoleKey.D)
    {
        head.history.Add(DirectionHistory.Right);
        head.j++;
    }
    else if (keyInfo.Key == ConsoleKey.A)
    {
        head.history.Add(DirectionHistory.Left);
        head.j--;
    }
    else if (keyInfo.Key == ConsoleKey.W)
    {
        head.history.Add(DirectionHistory.Up);
        head.i--;
    }
    else if (keyInfo.Key == ConsoleKey.S)
    {
        head.history.Add(DirectionHistory.Down);
        head.i++;
    }
}

static bool GameOver(int[,] matrix, SnakePart snakePart, List<SnakePart> list)
{
    //голова попала в границу
    if (matrix.GetLength(0) == snakePart.i || matrix.GetLength(1) == snakePart.j ||
        snakePart.i < 0 || snakePart.j < 0)
    {
        return true;
    }
    //голова попала в свое тело
    if (list.FirstOrDefault(z => z.i == snakePart.i && z.j == snakePart.j 
    && z.history.FirstOrDefault() != DirectionHistory.None) != null)
    {
        return true;
    }
    return false;
}


static void DrawDisplay(List<SnakePart> list, int[,] matrix, SnakePart snakePart, bool gameOver, Meal meal,
    int counter)
{
    Console.Clear();
    if (gameOver)
    {
        Console.WriteLine("Конец игры. Ваш счёт: " + counter);
        return;
    }
    for (int j = 0; j <= matrix.GetLength(1) + 1; j++)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write("  ");
        Console.BackgroundColor = ConsoleColor.Black;
    }
    Console.WriteLine();
    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write("  ");
        Console.BackgroundColor = ConsoleColor.Black;
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (i == snakePart.i && j == snakePart.j)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("🐲");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (list.FirstOrDefault(z => z.i == i && z.j == j) != null)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("  ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else if (i == meal.i && j == meal.j)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write("  ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.Write("  ");
            }
        }
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write("  ");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine();
    }
    for (int j = 0; j <= matrix.GetLength(1) + 1; j++)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write("  ");
        Console.BackgroundColor = ConsoleColor.Black;
    }
    Console.WriteLine("\nВаш счёт: " + counter);
}
class SnakePart
{
    public int i;
    public int j;
    public List<DirectionHistory> history = new List<DirectionHistory>();
}
enum DirectionHistory
{
    None,
    Up,
    Down,
    Left,
    Right
}
class Meal
{
    public int i;
    public int j;
}