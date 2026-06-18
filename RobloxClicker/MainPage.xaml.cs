using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Storage;
using RobloxClicker.Models;
using System.Collections.ObjectModel;
using System.Timers;
using Microsoft.Maui.Controls;

namespace RobloxClicker;

public partial class MainPage : ContentPage
{
    private long robux = 0;
    private long totalClicks = 0;
    private int clickPower = 1;

    private readonly ObservableCollection<Upgrade> upgrades = new();
    private readonly System.Timers.Timer passiveTimer = new(1000);

    private readonly List<(long threshold, string imageName)> avatarMilestones = new()
    {
        (0, "dotnet_bot.png"),
        (10000, "avatar_rich1.png"),
        (100000, "avatar_rich2.png"),
        (1000000, "avatar_millionaire.png"),
    };
    private int currentAvatarIndex = 0;

    public MainPage()
    {
        InitializeComponent();

        Loaded += OnPageLoaded;   // ← Это решает проблему с инициализацией
        passiveTimer.Elapsed += PassiveIncomeTick;
        passiveTimer.Start();
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        LoadProgress();
        InitializeUpgrades();
        UpdateUI();
    }

    private void InitializeUpgrades()
    {
        if (UpgradesLayout == null)
        {
            DisplayAlert("Ошибка", "UpgradesLayout не найден", "OK");
            return;
        }

        UpgradesLayout.Children.Clear();

        // Заполняем список улучшений
        upgrades.Clear();

        // Клик улучшения
        upgrades.Add(new Upgrade { Id = "click1", Name = "Сильная рука", Description = "+1 Robux за клик", BaseCost = 50, RobuxPerClick = 1, CostMultiplier = 1.18 });
        upgrades.Add(new Upgrade { Id = "click2", Name = "Золотой клик", Description = "+8 Robux за клик", BaseCost = 350, RobuxPerClick = 8, CostMultiplier = 1.18 });
        upgrades.Add(new Upgrade { Id = "click3", Name = "Клик-машина", Description = "+35 Robux за клик", BaseCost = 2500, RobuxPerClick = 35, CostMultiplier = 1.18 });
        upgrades.Add(new Upgrade { Id = "click4", Name = "Божественный клик", Description = "+150 Robux за клик", BaseCost = 18000, RobuxPerClick = 150, CostMultiplier = 1.18 });

        // Пассивные
        upgrades.Add(new Upgrade { Id = "passive1", Name = "Бабушка-кликерша", Description = "+1 Robux/сек", BaseCost = 120, RobuxPerSecond = 1, CostMultiplier = 1.18 });
        upgrades.Add(new Upgrade { Id = "passive2", Name = "Робот-кликер", Description = "+6 Robux/сек", BaseCost = 800, RobuxPerSecond = 6, CostMultiplier = 1.18 });
        upgrades.Add(new Upgrade { Id = "passive3", Name = "Фабрика Robux", Description = "+28 Robux/сек", BaseCost = 4500, RobuxPerSecond = 28, CostMultiplier = 1.18 });
        upgrades.Add(new Upgrade { Id = "passive4", Name = "Roblox Корпорация", Description = "+120 Robux/сек", BaseCost = 25000, RobuxPerSecond = 120, CostMultiplier = 1.18 });

        // Создаём кнопки
        foreach (var upgrade in upgrades)
        {
            var btn = new Button
            {
                Text = $"{upgrade.Name}\n{upgrade.Description}\nЦена: {upgrade.BaseCost:N0} Robux",
                HeightRequest = 92,
                BackgroundColor = Color.FromArgb("#FF4500"),
                TextColor = Colors.White,
                CornerRadius = 12,
                FontSize = 15,
                Margin = new Thickness(0, 6),
                Padding = new Thickness(10, 8),
                LineBreakMode = LineBreakMode.WordWrap
            };

            btn.Clicked += (s, e) => BuyUpgrade(upgrade);
            UpgradesLayout.Children.Add(btn);
        }
    }

    private void BuyUpgrade(Upgrade upgrade)
    {
        long cost = CalculateCost(upgrade);
        if (robux >= cost)
        {
            robux -= cost;
            upgrade.Level++;

            if (upgrade.RobuxPerClick > 0)
                clickPower += (int)upgrade.RobuxPerClick;

            UpdateUI();
            RefreshUpgradeButtons();
            SaveProgress();
        }
        else
        {
            DisplayAlert("Мало Robux", "Недостаточно средств!", "Ок");
        }
    }

    private long CalculateCost(Upgrade upgrade) =>
        (long)(upgrade.BaseCost * Math.Pow(upgrade.CostMultiplier, upgrade.Level));

    private void RefreshUpgradeButtons()
    {
        int index = 0;
        foreach (var upgrade in upgrades)
        {
            if (UpgradesLayout.Children[index] is Button btn)
            {
                long cost = CalculateCost(upgrade);
                string level = upgrade.Level > 0 ? $" (Ур. {upgrade.Level})" : "";
                btn.Text = $"{upgrade.Name}{level}\n{upgrade.Description}\nЦена: {cost:N0} Robux";
            }
            index++;
        }
    }

    private void PassiveIncomeTick(object? sender, ElapsedEventArgs e)
    {
        double passive = upgrades.Sum(u => u.RobuxPerSecond * u.Level);
        if (passive > 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                robux += (long)Math.Floor(passive);
                UpdateUI();
                SaveProgress();
            });
        }
    }

    private void OnClickButtonClicked(object sender, EventArgs e)
    {
        robux += clickPower;
        totalClicks++;
        UpdateUI();
        AnimateClick();
        CheckAvatarUpgrade();
        CheckSpecialAchievement();
    }

    private async void AnimateClick()
    {
        await ClickButton.ScaleTo(0.82, 70, Easing.CubicOut);
        await ClickButton.ScaleTo(1.0, 130, Easing.CubicIn);
    }

    private async void CheckSpecialAchievement()
    {
        if (totalClicks == 67)
        {
            await TriggerCrazyEffect();
        }
    }

    private async Task TriggerCrazyEffect()
    {
        ClickButton.IsEnabled = false;
        await ShakeScreen();
        await ShowPopupImage();
        await Task.Delay(3000);
        if (PopupFrame.IsVisible)
        {
            await ClosePopupInternal();
        }
        ClickButton.IsEnabled = true;
    }

    private async Task ShakeScreen()
    {
        const int shakes = 14;
        const int delay = 45;
        for (int i = 0; i < shakes; i++)
        {
            double x = (i % 2 == 0) ? 15 : -15;
            double y = (i % 3 == 0) ? 10 : -10;
            await MainPageRoot.TranslateTo(x, y, delay);
            await MainPageRoot.TranslateTo(0, 0, delay);
        }
    }

    private async Task ShowPopupImage()
    {
        if (PopupFrame == null) return;
        PopupFrame.IsVisible = true;
        PopupFrame.Opacity = 0;
        PopupFrame.Scale = 0.7;
        await PopupFrame.FadeTo(1, 300);
        await PopupFrame.ScaleTo(1.05, 250, Easing.CubicOut);
        await PopupFrame.ScaleTo(1.0, 180, Easing.CubicIn);
    }

    private async Task ClosePopupInternal()
    {
        await PopupFrame.FadeTo(0, 250);
        await Task.Delay(250);
        PopupFrame.IsVisible = false;
        PopupFrame.Opacity = 1;
        PopupFrame.Scale = 1;
    }

    private async void ClosePopup(object sender, EventArgs e)
    {
        await ClosePopupInternal();
    }

    private void CheckAvatarUpgrade()
    {
        for (int i = currentAvatarIndex + 1; i < avatarMilestones.Count; i++)
        {
            if (robux >= avatarMilestones[i].threshold)
            {
                currentAvatarIndex = i;
                UpdateAvatar();
                DisplayAlert("Новый аватар!",
                    $"Разблокирован новый образ на {avatarMilestones[i].threshold:N0} Robux!", "Круто");
                break;
            }
        }
    }

    private void UpdateAvatar()
    {
        if (ClickButton != null)
            ClickButton.Source = avatarMilestones[currentAvatarIndex].imageName;
    }

    private void UpdateUI()
    {
        if (RobuxLabel != null)
            RobuxLabel.Text = $"Robux: {robux:N0}";

        if (StatsLabel != null)
            StatsLabel.Text = $"{clickPower} Robux за клик";

        // Прогресс бар
        if (currentAvatarIndex + 1 < avatarMilestones.Count)
        {
            long current = avatarMilestones[currentAvatarIndex].threshold;
            long next = avatarMilestones[currentAvatarIndex + 1].threshold;
            double progress = (double)(robux - current) / (next - current);
            if (ProgressBar != null) ProgressBar.Progress = Math.Clamp(progress, 0, 1);
            if (NextMilestoneLabel != null) NextMilestoneLabel.Text = $"До {next:N0} Robux";
        }
        else
        {
            if (ProgressBar != null) ProgressBar.Progress = 1;
            if (NextMilestoneLabel != null) NextMilestoneLabel.Text = "Максимум достигнут!";
        }
    }

    private void SaveProgress()
    {
        Preferences.Set("Robux", robux);
        Preferences.Set("TotalClicks", totalClicks);
        Preferences.Set("ClickPower", clickPower);
        // Уровни улучшений можно сохранить позже
    }

    private void LoadProgress()
    {
        robux = Preferences.Get("Robux", 0L);
        totalClicks = Preferences.Get("TotalClicks", 0L);
        clickPower = Preferences.Get("ClickPower", 1);
    }
    private async void ResetProgress(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Сброс прогресса",
            "Внимание!\n\nВесь прогресс, Robux и улучшения будут полностью удалены.\n\nТы точно хочешь сбросить?",
            "Да, сбросить всё", "Отмена");

        if (confirm)
        {
            Preferences.Clear();

            robux = 0;
            totalClicks = 0;
            clickPower = 1;

            foreach (var upgrade in upgrades)
                upgrade.Level = 0;

            InitializeUpgrades();
            UpdateUI();

            await DisplayAlert("Готово", "Прогресс успешно сброшен!", "ОК");
        }
    }

}