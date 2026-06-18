using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace RobloxClicker;

public partial class MainPage : ContentPage
{
    private long robux = 0;
    private int clickPower = 1;
    private int totalClicks = 0;

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
        UpdateUI();
        UpdateAvatar();
    }

    private void OnClickButtonClicked(object sender, EventArgs e)
    {
        if (!ClickButton.IsEnabled) return;

        robux += clickPower;
        totalClicks++;

        UpdateUI();
        AnimateClick();
        CheckAvatarUpgrade();
        CheckSpecialAchievement();
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

    }
    private async Task ClosePopupInternal()
    {
        await PopupFrame.FadeTo(0, 250);
        await Task.Delay(250);

        PopupFrame.IsVisible = false;
        PopupFrame.Opacity = 1;
        PopupFrame.Scale = 1;

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

    private async void ClosePopup(object sender, EventArgs e)
    {
        await ClosePopupInternal();
    }

    private async void AnimateClick()
    {
        await ClickButton.ScaleTo(0.82, 70, Easing.CubicOut);
        await ClickButton.ScaleTo(1.0, 130, Easing.CubicIn);
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

        if (StatsLabel != null)
            StatsLabel.Text = $"Клик: {clickPower} Robux";
    }

    private void BuyUpgrade1(object sender, EventArgs e)
    {
        if (robux >= 100)
        {
            robux -= 100;
            clickPower = 10;
            UpgradeButton1.IsVisible = false;
            UpgradeButton2.IsVisible = true;
            UpdateUI();
        }
        else
        {
            DisplayAlert("Мало Robux", "Недостаточно средств!", "Ок");
        }
    }

    private void BuyUpgrade2(object sender, EventArgs e)
    {
        if (robux >= 800)
        {
            robux -= 800;
            clickPower = 50;
            UpgradeButton2.IsVisible = false;
            UpdateUI();
            DisplayAlert("Мощь!", "Теперь 1 клик = 50 Robux!", "Огонь");
        }
    }
}