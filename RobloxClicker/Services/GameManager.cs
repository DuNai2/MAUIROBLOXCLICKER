using Microsoft.Maui.Storage;
using RobloxClicker.Models;

namespace RobloxClicker.Services;

public class GameManager
{
    private static GameManager? _instance;
    public static GameManager Instance => _instance ??= new GameManager();

    public long Robux { get; private set; } = 0;
    public int ClickPower { get; private set; } = 1;
    public long TotalClicks { get; private set; } = 0;

    public List<Upgrade> Upgrades { get; private set; } = new();

    private GameManager()
    {
        LoadProgress();
    }

    public void InitializeUpgrades()
    {
        if (Upgrades.Count > 0) return;

        // Клик-улучшения
        Upgrades.Add(new Upgrade { Id = "click1", Name = "Сильная рука", Description = "+1 Robux за клик", BaseCost = 50, RobuxPerClick = 1 });
        Upgrades.Add(new Upgrade { Id = "click2", Name = "Золотой клик", Description = "+8 Robux за клик", BaseCost = 350, RobuxPerClick = 8 });
        Upgrades.Add(new Upgrade { Id = "click3", Name = "Клик-машина", Description = "+35 Robux за клик", BaseCost = 2500, RobuxPerClick = 35 });
        Upgrades.Add(new Upgrade { Id = "click4", Name = "Божественный клик", Description = "+150 Robux за клик", BaseCost = 18000, RobuxPerClick = 150 });

        // Пассивные улучшения
        Upgrades.Add(new Upgrade { Id = "passive1", Name = "Бабушка-кликерша", Description = "+1 Robux/сек", BaseCost = 120, RobuxPerSecond = 1 });
        Upgrades.Add(new Upgrade { Id = "passive2", Name = "Робот-кликер", Description = "+6 Robux/сек", BaseCost = 800, RobuxPerSecond = 6 });
        Upgrades.Add(new Upgrade { Id = "passive3", Name = "Фабрика Robux", Description = "+28 Robux/сек", BaseCost = 4500, RobuxPerSecond = 28 });
        Upgrades.Add(new Upgrade { Id = "passive4", Name = "Roblox Корпорация", Description = "+120 Robux/сек", BaseCost = 25000, RobuxPerSecond = 120 });
    }

    public bool TryBuy(Upgrade upgrade)
    {
        long cost = upgrade.CalculateCost();
        if (Robux >= cost)
        {
            Robux -= cost;
            upgrade.Level++;

            if (upgrade.RobuxPerClick > 0)
                ClickPower += (int)upgrade.RobuxPerClick;

            SaveProgress();
            return true;
        }
        return false;
    }

    public double GetPassivePerSecond() => Upgrades.Sum(u => u.RobuxPerSecond * u.Level);

    public void AddRobux(long amount)
    {
        Robux += amount;
        SaveProgress();
    }

    private void LoadProgress()
    {
        Robux = Preferences.Get("Robux", 0L);
        ClickPower = Preferences.Get("ClickPower", 1);
        TotalClicks = Preferences.Get("TotalClicks", 0L);
    }

    private void SaveProgress()
    {
        Preferences.Set("Robux", Robux);
        Preferences.Set("ClickPower", ClickPower);
        Preferences.Set("TotalClicks", TotalClicks);
    }

    public void ResetAllProgress()
    {
        Preferences.Clear();
        Robux = 0;
        ClickPower = 1;
        TotalClicks = 0;
        Upgrades.ForEach(u => u.Level = 0);
    }
}