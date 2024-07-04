using VirusesGame.Classes;
using VirusesGame.Enums;
namespace VirusesGame;

public partial class GamePage : ContentPage
{
    private Board board;
    private ImageButton[,] boardButtons;
    private Player leadingPlayer;
    private Player secondPlayer;
    
    public GamePage()
    {
        InitializeComponent();
        BuildBoardButtons();
        InitializePlayers();
    }
    private async void BuildBoardButtons()
    {
        boardButtons = new ImageButton[10, 10];
        InitializeBoardButtons();
        board = new Board();
        board.Initialize();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {               
                await Task.Run(() =>
                {
                    boardButtons[i, j].Clicked += OnImageButtonClicked!;
                    Dispatcher.DispatchAsync(() =>
                        boardButtons[i, j].Source = LoadImages(i,j));
                });
            }
        }
    }

    private async void InitializeBoardButtons()
    {
        for (int i = 0; i < 10; i++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
        }

        for (int j = 0; j < 10; j++)
        {
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                boardButtons[i, j] = new ImageButton();
                boardButtons[i, j].BorderColor = Colors.Black;
                boardButtons[i, j].BorderWidth = 1;
                BoardGrid.Add(boardButtons[i, j], i, j);
            }
        }

        await Task.Delay(100);
    }

    private void InitializePlayers()
    {
        var player1 = new Player(State.Cross, State.FilledZero, "�������");
        var player2 = new Player(State.Zero, State.�ircled�ross,"�������");
        leadingPlayer = player1;
        secondPlayer = player2;
    }

    private async void OnGiveUpButtonClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("����������� ��������", "�� ������� ��� ������ �������?", "��", "���");
        if (result)
        {
            await Navigation.PushAsync(new CongratulationPage(secondPlayer.Name));
        }
    }
    private async void OnImageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button != null && leadingPlayer.CountMoves < 3)
        {
            var location = FindLocationImageButton(button);
            var visited = new bool[10, 10];
            if (board[location.x, location.y].State == State.Empty 
                && leadingPlayer.CheckIsCellAvailable(board,location.x,location.y,visited))
            {
                leadingPlayer.Multiply(board, location.x, location.y);
                leadingPlayer.AllLivingCells.Add((location.x,location.y));
                button.Source = leadingPlayer.Symbols.nativeSymbol == State.Zero ? ImageSource.FromFile("circle.png")
                    : ImageSource.FromFile("cross.png");
            }
            if (board[location.x, location.y].State == secondPlayer.Symbols.nativeSymbol
                && leadingPlayer.CheckIsCellAvailable(board, location.x, location.y, visited))
            {
                leadingPlayer.Kill(board, location.x, location.y);
                secondPlayer.AllLivingCells.Remove((location.x, location.y));
                button.Source = leadingPlayer.Symbols.nativeSymbol == State.Zero ? ImageSource.FromFile("cross_dead.png")
                    : ImageSource.FromFile("circle_dead.png");
            }
            if (secondPlayer.AllLivingCells.Count == 0)
            {
                await Task.Delay(500);
                await Navigation.PushAsync(new CongratulationPage(leadingPlayer.Name));
            }
        }
    }
    private (int x, int y) FindLocationImageButton(ImageButton btn)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (boardButtons[i, j].Equals(btn))
                {
                    return (i, j);
                }
            }
        }
        return (-1, -1);
    }

    private async void OnConfirmButtonClicked(object sender, EventArgs e)
    {
        if (leadingPlayer.CountMoves == 3)
        {
            ReplacePlayer();
        }
        else
        {
            await DisplayAlert("����������", "�� ������� ������ 3 �����. �������� �������� ��� ���������� ���", "��");
        }
    }

    private async void OnCancelButtonClicked(object sender, EventArgs e)
    {
        if (leadingPlayer.CountMoves == 0)
        {
            await DisplayAlert("����������", "�� �� ������� �� ������ ����, ������� �� ������ �������� ���", "��");
        }
        else
        {
            var loc = leadingPlayer.CancelMove(board);
            boardButtons[loc.x, loc.y].Source = LoadImages(loc.x, loc.y);
        }
       
    }

    private async void OnSkipButtonClicked(object sender, EventArgs e)
    {
        if (leadingPlayer.CountMoves == 0)
        {
            ReplacePlayer();
        }
        else if (leadingPlayer.CountMoves == 3)
        {
            await DisplayAlert("����������", "�� ������� 3 ����, ������� ������ �����������", "��");
        }
        else
        {
            await DisplayAlert("����������", "�� ������� ������ 3 �����. �������� �������� ��� �������� 3 ����", "��");
        }
    }
    private void ReplacePlayer()
    {
        leadingPlayer.ResetMoves();
        var tempPlayer = leadingPlayer;
        leadingPlayer = secondPlayer;
        secondPlayer = tempPlayer;
        LeadingPlayer.Text = leadingPlayer.Name.ToUpper();
        
    }
    private ImageSource LoadImages(int x, int y)
    {
        return board[x, y].State == State.Empty ? ImageSource.FromFile("cell.png")
            : board[x, y].State == State.Zero ? ImageSource.FromFile("circle.png")
            : ImageSource.FromFile("cross.png");
    }
}