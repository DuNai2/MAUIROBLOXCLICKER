namespace RobloxClicker.Models;

public class Upgrade
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long BaseCost { get; set; }
    public long CurrentCost { get; set; }
    public int Level { get; set; } = 0;
    public double CostMultiplier { get; set; } = 1.18; // рост цены
    public long RobuxPerClick { get; set; } = 0;
    public double RobuxPerSecond { get; set; } = 0;
    public string Icon { get; set; } = "dotnet_bot.png"; // можно менять потом
}