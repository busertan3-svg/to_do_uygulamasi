using System;
using System.Collections.Generic; // For List<T>
using System.Linq;                // For LINQ methods like Where, OrderBy, FirstOrDefault, ToList

// Enum for card sizes
public enum CardSize
{
    XS = 1,
    S,
    M,
    L,
    XL
}

// Enum for board lines (columns)
public enum BoardLine
{
    TODO,
    IN_PROGRESS,
    DONE
}

// Represents a team member
public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; }

    public TeamMember(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Name} (ID: {Id})";
    }
}

// Represents a single ToDo card
public class Card
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int AssignedPersonId { get; set; }
    public CardSize Size { get; set; }
    public BoardLine Line { get; set; }

    public Card(string title, string content, int assignedPersonId, CardSize size, BoardLine line)
    {
        Title = title;
        Content = content;
        AssignedPersonId = assignedPersonId;
        Size = size;
        Line = line;
    }
}

// Manages all ToDo board operations
public class BoardManager
{
    private List<Card> _cards;
    private List<TeamMember> _teamMembers;

    public BoardManager()
    {
        // Define default team members
        _teamMembers = new List<TeamMember>
        {
            new TeamMember(1, "John Doe"),
            new TeamMember(2, "Jane Smith"),
            new TeamMember(3, "Peter Jones"),
            new TeamMember(4, "Alice Brown")
        };

        // Define default cards and distribute them across lines
        _cards = new List<Card>
        {
            new Card("Frontend Dev", "Develop user interface", 1, CardSize.M, BoardLine.TODO),
            new Card("Backend API", "Create RESTful endpoints", 2, CardSize.L, BoardLine.IN_PROGRESS),
            new Card("Database Setup", "Configure SQL database", 1, CardSize.S, BoardLine.DONE),
            new Card("User Auth", "Implement user authentication", 3, CardSize.XL, BoardLine.TODO)
        };
    }

    // Displays the main menu options
    public void DisplayMenu()
    {
        Console.WriteLine("\nPlease select an operation you want to perform :)");
        Console.WriteLine("*******************************************");
        Console.WriteLine("(1) List Board");
        Console.WriteLine("(2) Add Card to Board");
        Console.WriteLine("(3) Delete Card from Board");
        Console.WriteLine("(4) Move Card");
        Console.WriteLine("(0) Exit Application");
        Console.Write("Your choice: ");
    }

    // (1) Lists all cards on the board, grouped by line
    public void ListBoard()
    {
        Console.WriteLine("\n--- TODO Line ---");
        Console.WriteLine("************************");
        DisplayCardsByLine(BoardLine.TODO);

        Console.WriteLine("\n--- IN PROGRESS Line ---");
        Console.WriteLine("************************");
        DisplayCardsByLine(BoardLine.IN_PROGRESS);

        Console.WriteLine("\n--- DONE Line ---");
        Console.WriteLine("************************");
        DisplayCardsByLine(BoardLine.DONE);
    }

    // Helper method to display cards for a specific line
    private void DisplayCardsByLine(BoardLine line)
    {
        var cardsInLine = _cards.Where(c => c.Line == line).ToList();

        if (cardsInLine.Count == 0)
        {
            Console.WriteLine("~ EMPTY ~");
        }
        else
        {
            foreach (var card in cardsInLine)
            {
                DisplayCardDetails(card);
            }
        }
    }

    // Helper method to display card details in a formatted way
    private void DisplayCardDetails(Card card)
    {
        Console.WriteLine($"Title       : {card.Title}");
        Console.WriteLine($"Content     : {card.Content}");
        Console.WriteLine($"Assigned To : {GetTeamMemberName(card.AssignedPersonId)}");
        Console.WriteLine($"Size        : {card.Size}");
        Console.WriteLine("-");
    }

    // Helper method to get team member name by ID
    private string GetTeamMemberName(int id)
    {
        return _teamMembers.FirstOrDefault(m => m.Id == id)?.Name ?? "Unknown";
    }

    // (2) Adds a new card to the board
    public void AddCard()
    {
        Console.Write("Enter Title                                : ");
        string title = Console.ReadLine();
        Console.Write("Enter Content                              : ");
        string content = Console.ReadLine();

        // Get and validate Card Size
        CardSize size = GetCardSizeInput();
        if (size == 0) // 0 is an invalid size, indicating failed input
        {
            Console.WriteLine("Invalid size selected. Card addition cancelled.");
            return;
        }

        // Get and validate Assigned Person ID
        int assignedPersonId = GetAssignedPersonIdInput();
        if (assignedPersonId == 0) // 0 is an invalid ID, indicating failed input
        {
            Console.WriteLine("Invalid person ID. Card addition cancelled.");
            return;
        }

        // New cards are added to the TODO line by default
        _cards.Add(new Card(title, content, assignedPersonId, size, BoardLine.TODO));
        Console.WriteLine("Card added successfully to TODO line!");
    }

    // Helper for getting and validating CardSize input
    private CardSize GetCardSizeInput()
    {
        while (true)
        {
            Console.Write("Select Size -> XS(1),S(2),M(3),L(4),XL(5)  : ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int sizeChoice) && Enum.IsDefined(typeof(CardSize), sizeChoice))
            {
                return (CardSize)sizeChoice;
            }
            else
            {
                Console.WriteLine("Invalid size input. Please enter a number between 1 and 5.");
            }
        }
    }

    // Helper for getting and validating AssignedPersonId input
    private int GetAssignedPersonIdInput()
    {
        while (true)
        {
            Console.WriteLine("Available Team Members:");
            foreach (var member in _teamMembers)
            {
                Console.WriteLine($"- {member.Id}: {member.Name}");
            }
            Console.Write("Select Person ID                           : ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int personId))
            {
                if (_teamMembers.Any(m => m.Id == personId))
                {
                    return personId;
                }
                else
                {
                    Console.WriteLine("Invalid ID. No team member found with that ID.");
                    Console.WriteLine("Invalid entries made! Operation cancelled.");
                    return 0; // Indicate failure
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a numeric ID.");
                Console.WriteLine("Invalid entries made! Operation cancelled.");
                return 0; // Indicate failure
            }
        }
    }

    // (3) Deletes a card from the board
    public void DeleteCard()
    {
        Console.Write("\nFirst, you need to select the card you want to delete. Please enter the card title: ");
        string titleToDelete = Console.ReadLine();

        List<Card> matchingCards = _cards
            .Where(c => c.Title.Equals(titleToDelete, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingCards.Count == 0)
        {
            Console.WriteLine("No card matching your criteria was found on the board. Please make a selection.");
            HandleNotFoundOption(DeleteCard, "delete");
            return;
        }

        // As per requirement, if multiple cards found, all are deleted.
        _cards.RemoveAll(c => c.Title.Equals(titleToDelete, StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"All cards with title '{titleToDelete}' deleted successfully!");
    }

    // (4) Moves a card to a different line
    public void MoveCard()
    {
        Console.Write("\nFirst, you need to select the card you want to move. Please enter the card title: ");
        string titleToMove = Console.ReadLine();

        Card cardToMove = _cards
            .FirstOrDefault(c => c.Title.Equals(titleToMove, StringComparison.OrdinalIgnoreCase));

        if (cardToMove == null)
        {
            Console.WriteLine("No card matching your criteria was found on the board. Please make a selection.");
            HandleNotFoundOption(MoveCard, "move");
            return;
        }

        Console.WriteLine("\nFound Card Information:");
        Console.WriteLine("**************************************");
        DisplayCardDetails(cardToMove);
        Console.WriteLine($"Line        : {cardToMove.Line}"); // Display current line

        Console.WriteLine("\nPlease select the Line you want to move to:");
        Console.WriteLine("(1) TODO");
        Console.WriteLine("(2) IN PROGRESS");
        Console.WriteLine("(3) DONE");
        Console.Write("Your choice: ");
        string lineChoice = Console.ReadLine();

        BoardLine newBoardLine;
        switch (lineChoice)
        {
            case "1":
                newBoardLine = BoardLine.TODO;
                break;
            case "2":
                newBoardLine = BoardLine.IN_PROGRESS;
                break;
            case "3":
                newBoardLine = BoardLine.DONE;
                break;
            default:
                Console.WriteLine("You made an invalid selection! Operation cancelled.");
                return;
        }

        cardToMove.Line = newBoardLine;
        Console.WriteLine($"Card '{cardToMove.Title}' moved to {newBoardLine} line successfully!");
        ListBoard(); // Display the board after moving the card
    }

    // Helper method for "card not found" scenarios for delete/move
    private void HandleNotFoundOption(Action retryAction, string operationType)
    {
        Console.WriteLine($"* To end the {operationType}: (1)");
        Console.WriteLine("* To try again        : (2)");
        Console.Write("Your choice: ");
        string choice = Console.ReadLine();

        if (choice == "2")
        {
            retryAction.Invoke(); // Call the original method again
        }
        else if (choice != "1")
        {
            Console.WriteLine("Invalid choice. Returning to main menu.");
        }
    }
}

// Main program class
public class Program
{
    public static void Main(string[] args)
    {
        BoardManager boardManager = new BoardManager();
        bool continueApp = true;

        while (continueApp)
        {
            boardManager.DisplayMenu();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    boardManager.ListBoard();
                    break;
                case "2":
                    boardManager.AddCard();
                    break;
                case "3":
                    boardManager.DeleteCard();
                    break;
                case "4":
                    boardManager.MoveCard();
                    break;
                case "0": // Exit the application
                    continueApp = false;
                    Console.WriteLine("Exiting ToDo Application. Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            
            // Only prompt to return to menu if the app is not exiting
            if (continueApp) {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
    }
}