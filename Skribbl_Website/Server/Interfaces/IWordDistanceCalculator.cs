namespace Skribbl_Website.Server.Interfaces
{
    public interface IWordDistanceCalculator
    {
        int Calculate(string word, string target);
    }
}