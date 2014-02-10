using Ailon.WP.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using System.Reflection;

namespace SmogonWP.Services
{
  public class RateService
  {
    private const string RateCountKey = "ratecount";
    private const string HasRatedKey = "hasrated";

    private const int RateInterval = 5;

    private readonly ISettingsService _settingsService;

    public RateService(ISettingsService settingsService)
    {
      _settingsService = settingsService;
    }

    /// <summary>
    /// Checks to see if it's time to open the rating prompt
    /// </summary>
    /// <returns>Returns whether or not the rating prompt was opened.</returns>
    public bool CheckForRateReminder()
    {
      if (HasRated()) return false;

      var count = getAndIncrementRatingCount();

      if (isTimeForRating(count))
      {
        openRatingPrompt();
        return true;
      }

      return false;
    }

    public void PopupEmailTask(string priority)
    {
      var ect = new EmailComposeTask
      {
        To = "jason@y2bd.me",
        Subject = "SmogonWP Inquiry",
        Body = "\n\n" + composeDeviceIDBody(priority)
      };

      ect.Show();
    }

    public void StopRatingPrompts()
    {
      _settingsService.Save(HasRatedKey, true);
    }

    public bool HasRated()
    {
      return _settingsService.Load<bool>(HasRatedKey);
    }

    private int getAndIncrementRatingCount()
    {
      var count = _settingsService.Load<int>(RateCountKey);

      count++;

      _settingsService.Save(RateCountKey, count);

      return count;
    }
    
    private bool isTimeForRating(int count)
    {
      return count % RateInterval == 0;
    }

    private void openRatingPrompt()
    {
      var ratingPrompt = makeRatingPrompt();

      ratingPrompt.Dismissed += onRatingPromptDismissed;

      ratingPrompt.Show();
    }

    private void onRatingPromptDismissed(object sender, DismissedEventArgs e)
    {
      if (e.Result == CustomMessageBoxResult.LeftButton)
      {
        popupRatingTask();
      }
      else if (e.Result == CustomMessageBoxResult.RightButton)
      {
        openEmailPrompt();
      }
    }

    private void popupRatingTask()
    {
      StopRatingPrompts();

      var mrt = new MarketplaceReviewTask();

      mrt.Show();
    }

    private void openEmailPrompt()
    {
      var emailPrompt = makeEmailPrompt();

      emailPrompt.Dismissed += onEmailPromptDismissed;

      emailPrompt.Show();
    }

    private void onEmailPromptDismissed(object sender, DismissedEventArgs e)
    {
      if (e.Result == CustomMessageBoxResult.LeftButton)
      {
        PopupEmailTask("MEGANIUM");
      }
    }

    private static CustomMessageBox makeRatingPrompt()
    {
      return new CustomMessageBox
      {
        Caption = "Thanks for using SmogonWP!",
        Message =
          "Would you mind giving it a good rating?\n\nThis app is completely free and always will be, so the best way to help me out and make the app better for everyone is to leave a rating and a review on the store.",
        LeftButtonContent = "rate",
        RightButtonContent = "no thanks"
      };
    }

    private static CustomMessageBox makeEmailPrompt()
    {
      return new CustomMessageBox
      {
        Caption = "Would you prefer to contact me directly?",
        Message =
          "Sorry to hear that you're not up to rating the app right now.\n\nRegardless, do you have suggestions or concerns about this app? Any thoughts on how it could be better? I'm always willing to talk.",
        LeftButtonContent = "email me",
        RightButtonContent = "no thanks"
      };
    }

    private static string composeDeviceIDBody(string priority)
    {
      var resolved = GetDeviceModel();

      var version = GetAppVersion();

      return string.Format(
@"~~~~~~~~~~
Device: {0}
Version: {1}
Status: {2}
~~~~~~~~~~", resolved, version, priority);
    }

    public static string GetAppVersion()
    {
      var asm = Assembly.GetExecutingAssembly();
      var parts = asm.FullName.Split(',');
      return parts[1].Split('=')[1];
    }

    public static string GetDeviceName()
    {
      return DeviceStatus.DeviceName;
    }

    public static string GetDeviceManufacturer()
    {
      return DeviceStatus.DeviceManufacturer;
    }

    public static string GetDeviceModel()
    {
      return PhoneNameResolver.Resolve(GetDeviceManufacturer(), GetDeviceName()).FullCanonicalName;
    }
  }
}
