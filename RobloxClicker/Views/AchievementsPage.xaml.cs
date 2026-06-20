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
        GameManager.Instance.LoadAchievements();
        LoadAchievements();
        UpdateAchievementsUI();
    }

    private void LoadAchievements()
    {
        achievements.Clear();

        // Мемные с анимацией
        achievements.Add(new Achievement
        {
            Id = "crazy_clicker",
            Name = "67 КЛИКОВ!",
            Description = "Сделай ровно 67 кликов",
            Reward = 1000,
            ConditionType = "clicks",
            TargetValue = 67
        });

        achievements.Add(new Achievement
        {
            Id = "navalny",
            Name = "Хотел стать символом, но стал огурцом",
            Description = "Сделай 100 000 кликов",
            Reward = 25000,
            ConditionType = "clicks",
            TargetValue = 100
        });

        achievements.Add(new Achievement
        {
            Id = "prigozhin",
            Name = "Где мои снаряды!",
            Description = "Сделай 1 000 000 кликов",
            Reward = 100000,
            ConditionType = "clicks",
            TargetValue = 130
        });

        // Обычные достижения
        achievements.Add(new Achievement { Id = "first_click", Name = "Первый шаг", Description = "Сделай 1 клик", Reward = 50, ConditionType = "clicks", TargetValue = 1 });
        achievements.Add(new Achievement { Id = "click_100", Name = "Кликер-новичок", Description = "Сделай 100 кликов", Reward = 300, ConditionType = "clicks", TargetValue = 100 });
        achievements.Add(new Achievement { Id = "click_500", Name = "Заработал репутацию", Description = "Сделай 500 кликов", Reward = 800, ConditionType = "clicks", TargetValue = 500 });
        achievements.Add(new Achievement { Id = "robux_10000", Name = "Маленький миллионер", Description = "Накопи 10 000 Robux", Reward = 1500, ConditionType = "robux", TargetValue = 10000 });
        achievements.Add(new Achievement { Id = "robux_100000", Name = "Богатый роблоксер", Description = "Накопи 100 000 Robux", Reward = 5000, ConditionType = "robux", TargetValue = 100000 });
    }

    public void UpdateAchievementsUI()
    {
        AchievementsLayout.Children.Clear();

        foreach (var ach in achievements)
        {
            bool completed = GameManager.Instance.IsAchievementCompleted(ach.Id);
            bool unlocked = CheckIfUnlocked(ach);

            var border = new Border
            {
                BackgroundColor = completed ? Color.FromArgb("#1E3A8A") : Color.FromArgb("#1E2937"),
                Stroke = completed ? Color.FromArgb("#00FFAA") : Color.FromArgb("#00D4FF"),
                StrokeThickness = 2,
                Padding = 14,
                WidthRequest = 380,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 8)
            };
            border.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 }
                }
            };

            var textLayout = new VerticalStackLayout { Spacing = 6 };

            textLayout.Children.Add(new Label { Text = ach.Name, FontSize = 19, FontAttributes = FontAttributes.Bold, TextColor = Colors.White });
            textLayout.Children.Add(new Label { Text = ach.Description, FontSize = 14, TextColor = Color.FromArgb("#94A3B8") });
            textLayout.Children.Add(new Label
            {
                Text = completed ? "✓ Получено" : $"+{ach.Reward} Robux",
                FontSize = 16,
                TextColor = completed ? Color.FromArgb("#00FFAA") : Color.FromArgb("#FFD700"),
                FontAttributes = FontAttributes.Bold
            });

            if (!completed)
            {
                var btn = new Button
                {
                    Text = unlocked ? "Забрать награду" : "Ещё не выполнено",
                    BackgroundColor = unlocked ? Color.FromArgb("#00D4FF") : Color.FromArgb("#475569"),
                    TextColor = unlocked ? Colors.Black : Color.FromArgb("#94A3B8"),
                    CornerRadius = 10,
                    HeightRequest = 52,
                    IsEnabled = unlocked
                };

                if (unlocked)
                    btn.Clicked += (s, e) => ClaimReward(ach);

                textLayout.Children.Add(btn);
            }

            Grid.SetColumn(textLayout, 0);
            grid.Children.Add(textLayout);

            var image = new Image
            {
                Source = "achievement_67.png",
                Aspect = Aspect.AspectFit,
                HeightRequest = 85,
                WidthRequest = 85,
                HorizontalOptions = LayoutOptions.End
            };
            Grid.SetColumn(image, 1);
            grid.Children.Add(image);

            border.Content = grid;
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
        if (GameManager.Instance.IsAchievementCompleted(ach.Id)) return;

        GameManager.Instance.CompleteAchievement(ach.Id);
        GameManager.Instance.AddRobux(ach.Reward);

        DisplayAlert("Поздравляем!", $"Получено +{ach.Reward} Robux!\n\n{ach.Name}", "Круто");

        UpdateAchievementsUI();
    }
}