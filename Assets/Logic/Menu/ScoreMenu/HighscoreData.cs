/**
 * Data container for highscores.
 **/
 // TODO low priority: Add player and track names.
public class HighscoreData
{
    private int score;
    private int rank;

    public int Score { get => score; set => score = value; }
    public int Rank { get => rank; set => rank = value; }
}