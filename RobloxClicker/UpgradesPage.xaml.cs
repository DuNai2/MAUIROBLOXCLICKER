using Microsoft.Maui.Controls.Shapes;
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
            var border = new Border
            {
                BackgroundColor = Color.FromArgb("#1E2937"),
                Stroke = Color.FromArgb("#00D4FF"),
                StrokeThickness = 2,
                Padding = 14,
                WidthRequest = 340,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 6)
            };
            border.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) };

            var layout = new VerticalStackLayout { Spacing = 8 };

            var name = new Label
            {
                Text = upgrade.Name,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            };

            var desc = new Label
            {
                Text = upgrade.Description,
                FontSize = 14,
                TextColor = Color.FromArgb("#94A3B8")
            };

            var cost = new Label
            {
                Text = $"Цена: {upgrade.CalculateCost():N0} Robux | Ур. {upgrade.Level}",
                FontSize = 16,
                TextColor = Color.FromArgb("#FFD700"),
                FontAttributes = FontAttributes.Bold
            };

            var buyButton = new Button
            {
                Text = "КУПИТЬ",
                BackgroundColor = Color.FromArgb("#00D4FF"),
                TextColor = Colors.Black,
                CornerRadius = 10,
                HeightRequest = 52,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };

            buyButton.Clicked += (s, e) => BuyUpgrade(upgrade, buyButton);

            layout.Children.Add(name);
            layout.Children.Add(desc);
            layout.Children.Add(cost);
            layout.Children.Add(buyButton);

            border.Content = layout;
            UpgradesLayout.Children.Add(border);
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