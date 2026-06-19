using RobloxClicker.Models;
using RobloxClicker.Services;
using Microsoft.Maui.Controls.Shapes;

namespace RobloxClicker;

public partial class AchievementsPage : ContentPage
{
    private readonly List<Achievement> achievements = new();

    public AchievementsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadAchievements();
        UpdateAchievementsUI();
    }

    private void LoadAchievements()
    {
        achievements.Clear();

        achievements.Add(new Achievement
        {
            Id = "crazy_clicker",
            Name = "67 КЛИКОВ!",
            Description = "Сделай ровно 67 кликов",
            Reward = 1000,
            ConditionType = "clicks",
            TargetValue = 67
        });

        achievements.Add(new Achievement { Id = "first_click", Name = "Первый шаг", Description = "Сделай 1 клик", Reward = 50, ConditionType = "clicks", TargetValue = 1 });
        achievements.Add(new Achievement { Id = "click_100", Name = "Кликер-новичок", Description = "Сделай 100 кликов", Reward = 300, ConditionType = "clicks", TargetValue = 100 });
        achievements.Add(new Achievement { Id = "click_500", Name = "Заработал репутацию", Description = "Сделай 500 кликов", Reward = 800, ConditionType = "clicks", TargetValue = 500 });
        achievements.Add(new Achievement { Id = "robux_10000", Name = "Маленький миллионер", Description = "Накопи 10 000 Robux", Reward = 1500, ConditionType = "robux", TargetValue = 10000 });
    }

    private void UpdateAchievementsUI()
    {
        AchievementsLayout.Children.Clear();

        foreach (var ach in achievements)
        {
            bool isUnlocked = CheckIfUnlocked(ach);
            bool isCompleted = ach.IsCompleted;

            var border = new Border
            {
                BackgroundColor = isCompleted ? Color.FromArgb("#1E3A8A") : Color.FromArgb("#1E2937"),
                Stroke = isCompleted ? Color.FromArgb("#00FFAA") : Color.FromArgb("#00D4FF"),
                StrokeThickness = 2,
                Padding = 14,
                WidthRequest = 340,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 8)
            };
            border.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) };

            var layout = new VerticalStackLayout { Spacing = 8 };

            layout.Children.Add(new Label { Text = ach.Name, FontSize = 19, FontAttributes = FontAttributes.Bold, TextColor = Colors.White });
            layout.Children.Add(new Label { Text = ach.Description, FontSize = 14, TextColor = Color.FromArgb("#94A3B8") });
            layout.Children.Add(new Label
            {
                Text = isCompleted ? "✓ Получено" : $"+{ach.Reward} Robux",
                FontSize = 16,
                TextColor = isCompleted ? Color.FromArgb("#00FFAA") : Color.FromArgb("#FFD700"),
                FontAttributes = FontAttributes.Bold
            });

            if (!isCompleted)
            {
                var btn = new Button
                {
                    Text = isUnlocked ? "Забрать награду" : "Ещё не выполнено",
                    BackgroundColor = isUnlocked ? Color.FromArgb("#00D4FF") : Color.FromArgb("#475569"),
                    TextColor = isUnlocked ? Colors.Black : Color.FromArgb("#94A3B8"),
                    CornerRadius = 10,
                    HeightRequest = 52,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    IsEnabled = isUnlocked,
                    Margin = new Thickness(0, 8, 0, 0)
                };

                if (isUnlocked)
                    btn.Clicked += (s, e) => ClaimReward(ach);

                layout.Children.Add(btn);
            }

            border.Content = layout;
            AchievementsLayout.Children.Add(border);
        }
    }

    private bool CheckIfUnlocked(Achievement ach)
    {
        if (ach.ConditionType == "clicks")
            return GameManager.Instance.TotalClicks >= ach.TargetValue;
        if (ach.ConditionType == "robux")
            return GameManager.Instance.Robux >= ach.TargetValue;
        return false;
    }

    private void ClaimReward(Achievement ach)
    {
        if (ach.IsCompleted) return;

        ach.IsCompleted = true;
        GameManager.Instance.AddRobux(ach.Reward);

        DisplayAlert("Поздравляем!", $"Получено +{ach.Reward} Robux!\n\n{ach.Name}", "Круто");

        // Специальный эффект для 67 кликов
        if (ach.Id == "crazy_clicker")
        {
            // Можно вызвать метод из MainPage, но пока просто алерт
        }

        UpdateAchievementsUI();
    }
}