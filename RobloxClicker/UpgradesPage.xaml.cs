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
                Padding = 12,
                WidthRequest = 370,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 6)
            };
            border.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = 90 }   // шире под картинку
                }
            };

            var textLayout = new VerticalStackLayout { Spacing = 6, Margin = new Thickness(0, 0, 12, 0) };

            textLayout.Children.Add(new Label
            {
                Text = upgrade.Name,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            });

            textLayout.Children.Add(new Label
            {
                Text = upgrade.Description,
                FontSize = 14,
                TextColor = Color.FromArgb("#94A3B8")
            });

            textLayout.Children.Add(new Label
            {
                Text = $"Цена: {upgrade.CalculateCost():N0} Robux | Ур. {upgrade.Level}",
                FontSize = 15,
                TextColor = Color.FromArgb("#FFD700"),
                FontAttributes = FontAttributes.Bold
            });

            var buyButton = new Button
            {
                Text = "КУПИТЬ",
                BackgroundColor = Color.FromArgb("#00D4FF"),
                TextColor = Colors.Black,
                CornerRadius = 10,
                HeightRequest = 50,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 8, 0, 0)
            };
            buyButton.Clicked += (s, e) => BuyUpgrade(upgrade, buyButton);

            textLayout.Children.Add(buyButton);

            Grid.SetColumn(textLayout, 0);
            grid.Children.Add(textLayout);

            // Картинка справа (прижата к краю, побольше)
            var image = new Image
            {
                Source = "achievement_67.png",
                Aspect = Aspect.AspectFit,
                HeightRequest = 85,
                WidthRequest = 85,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(image, 1);
            grid.Children.Add(image);

            border.Content = grid;
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