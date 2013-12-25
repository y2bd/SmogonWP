using System;
using System.Windows;
using Coding4Fun.Toolkit.Controls;

namespace SmogonWP.Services
{
  public static class TipService
  {
    private static readonly string[] Tips = {
      "This app has an awesome forum (via Reddit) that you can visit by swiping up on the appbar below!",
      "This app has a live tile! Pin it to your start page to see!",
      "Although this app doesn't have X and Y data, you can still see how the Fairy type fares on the Type page!",
      "After choosing any of a Pokemon's movesets, you can click the stats button to jump straight to the stats calculator with values filled in!",
      "Many things are tappable! Try tapping on something and see what happens!",
      "Studies have shown that rating applications makes you at least twenty percent more awesome!",
      "On the Pokemon search page, you can filter your search by both type and tier at the same time!",
      "Many pages let you open up Bulbapedia! Just look for the minimized appbar at the bottom.",
      "Web scraping is hard! If you find an entry that looks wrong, email the developer and help him fix it!",
      "Blissey has an HP base stat of 255, the highest possible base stat!",
      "Shuckle takes the record for highest defense and special defense base stats, both being 230!",
      "Pokemon with multiple forms are all listed seperately for ease of searching!",
      "This app has a beta! Email the dev for information on how to join!",
      "The app is open-source! Swipe up on the appbar and open the 'about + credits' page if you want to learn more.",
      "You can swipe these annoying toast prompts away!",
      "The developer's favorite Pokemon is Haunter!",
      "On the Move page, tapping on the move's type will bring you to the Type page with fields filled in!",
      "Some colors in the app depend on your accent color, while others depends on Type colors!",
      "The app also looks fantastic with your phone's Light theme!",
      "If you have any suggestions for the app, you should email the developer!",
      "This app has voice commands! Try holding down your home button and saying 'Smogon, search for Gardevoir'!",
      "You can pin any of the menu items below to your start screen. Just press and hold!",
      "There's a master ball hidden underneath this message! No, seriously!"
    };

    public static void ShowTipOfTheDay()
    {
      var rnd = new Random();

      if (rnd.Next(5) > 1) return;

      var tip = Tips[rnd.Next(Tips.Length)];

      var toast = new ToastPrompt
      {
        Title = "Did you know?",
        Message = tip,
        TextWrapping = TextWrapping.Wrap,
      };

      toast.Show();
    }
  }
}
