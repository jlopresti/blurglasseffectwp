using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage.Pickers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BlurredBackgroundPoc.Resources;

namespace BlurredBackgroundPoc
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            WriteableBitmap writeableBmp = BitmapFactory.New(1, 1).FromContent("bg.jpg");
            //writeableBmp.BoxBlur(19);

            BlurredImage.ImageSource = writeableBmp;
            BlurredImage.ManipulationDelta += LayoutRoot_OnManipulationDelta;

            // Load an image from the calling Assembly's resources only by passing the relative path
            // writeableBmp = BitmapFactory.New(1, 1).FromContent("cat.jpg");

        }

       

        private void LayoutRoot_OnManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var fill = BlurredImage.OverlayFillPercentage + (e.DeltaManipulation.Translation.Y);

            BlurredImage.OverlayFillPercentage = Math.Max(0, Math.Min(fill, 100));
        }



        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            BlurredImage.OverlayDirection = BlurredImage.OverlayDirection ==
                                            System.Windows.Controls.Orientation.Vertical
                ? System.Windows.Controls.Orientation.Horizontal
                : System.Windows.Controls.Orientation.Vertical;
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = (int) ((Slider) sender).Value;
            value = value%2 != 0 ? value : value + 1;
            if (BlurredImage != null)
            {
                BlurredImage.BlurredStrengh = value;
            }

        }
    }
}