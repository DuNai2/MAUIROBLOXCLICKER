using RobloxClicker.Models;
using RobloxClicker.Services;

namespace RobloxClicker;

public partial class UpgradesPage : ContentPage
{
    public UpgradesPage()
    {
        InitializeComponent();
        GameManager.Instance.InitializeUpgrades();
        LoadUpgradesUI();
    }

    private void LoadUpgradesUI()
    {
        UpgradesLayout.Children.Clear();

        foreach (var upgrade in GameManager.Instance.Upgrades)
        {
            var btn = new Button
            {
                Text = $"{upgrade.Name}\n{upgrade.Description}\nЦена: {upgrade.CalculateCost():N0} Robux",
                HeightRequest = 92,
                WidthRequest = 320,
                BackgroundColor = Color.FromArgb("#1E40AF"),
                TextColor = Colors.White,
                CornerRadius = 14,
                FontSize = 15.5,
                Padding = new Thickness(12),
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalOptions = LayoutOptions.Center
            };

            btn.Clicked += (s, e) => BuyUpgrade(upgrade, btn);
            UpgradesLayout.Children.Add(btn);
        }
    }

    private void BuyUpgrade(Upgrade upgrade, Button button)
    {
        if (GameManager.Instance.TryBuy(upgrade))
        {
            LoadUpgradesUI(); // обновляем цены и уровни
        }
        else
        {
            DisplayAlert("Мало Robux", "Недостаточно средств!", "Ок");
        }
    }

    private async void ResetProgressClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Сброс прогресса",
            "Весь прогресс будет удалён навсегда.\n\nТы уверен?",
            "Да, сбросить", "Отмена");

        if (confirm)
        {
            GameManager.Instance.ResetAllProgress();
            LoadUpgradesUI();
            await DisplayAlert("Готово", "Прогресс сброшен!", "ОК");
        }
    }
}