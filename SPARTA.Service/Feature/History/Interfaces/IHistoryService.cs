namespace SPARTA.Service.Feature.History.Interfaces;

public interface IHistoryService
{
    Task<string> GenerateAsync(string inputText, bool hasTest, bool hasUpgrade);
}

