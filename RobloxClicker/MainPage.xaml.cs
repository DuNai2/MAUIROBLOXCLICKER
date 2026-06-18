using RobloxClicker.Models;
using RobloxClicker.Services;
using System.Timers;

namespace RobloxClicker;

public partial class MainPage : ContentPage
{
    private long totalClicks = 0;
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
        Loaded += OnPageLoaded;
        passiveTimer.Elapsed += PassiveIncomeTick;
        passiveTimer.Start();
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        GameManager.Instance.InitializeUpgrades();
        UpdateUI();
    }

    private void OnClickButtonClicked(object sender, EventArgs e)
    {
        GameManager.Instance.AddRobux(GameManager.Instance.ClickPower);
        totalClicks++;
        UpdateUI();
        AnimateClick();
        CheckAvatarUpgrade();
        CheckSpecialAchievement();
    }

    private void PassiveIncomeTick(object? sender, ElapsedEventArgs e)
    {
        double passive = GameManager.Instance.GetPassivePerSecond();
        if (passive > 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                GameManager.Instance.AddRobux((long)Math.Floor(passive));
                UpdateUI();
            });
        }
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
            if (GameManager.Instance.Robux >= avatarMilestones[i].threshold)
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
            RobuxLabel.Text = $"Robux: {GameManager.Instance.Robux:N0}";

        if (StatsLabel != null)
            StatsLabel.Text = $"{GameManager.Instance.ClickPower} Robux за клик";

        if (currentAvatarIndex + 1 < avatarMilestones.Count)
        {
            long current = avatarMilestones[currentAvatarIndex].threshold;
            long next = avatarMilestones[currentAvatarIndex + 1].threshold;
            double progress = (double)(GameManager.Instance.Robux - current) / (next - current);
            if (ProgressBar != null) ProgressBar.Progress = Math.Clamp(progress, 0, 1);
            if (NextMilestoneLabel != null) NextMilestoneLabel.Text = $"До {next:N0} Robux";
        }
    }

    private async void ResetProgress(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Сброс прогресса",
            "Весь прогресс будет удалён.\n\nПродолжить?", "Да", "Отмена");

        if (confirm)
        {
            GameManager.Instance.ResetAllProgress();
            currentAvatarIndex = 0;
            UpdateAvatar();
            UpdateUI();
            await DisplayAlert("Готово", "Прогресс сброшен!", "ОК");
        }
    }
}