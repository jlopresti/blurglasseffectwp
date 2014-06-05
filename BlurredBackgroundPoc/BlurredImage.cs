using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlurredBackgroundPoc
{
    public class BlurredImage : Control
    {
        private Image _blurredImage;
        private Panel _rootGrid;
        private Panel _verticalOverlay;
        private Panel _horizontalOverlay;
        private Panel _verticalPanel;
        private Panel _horizontalPanel;

        public BlurredImage()
        {
            DefaultStyleKey = typeof(BlurredImage);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootGrid = this.GetTemplateChild("LayoutRoot") as Panel;
            _verticalPanel = this.GetTemplateChild("VerticalContentPanel") as Panel;
            _horizontalPanel = this.GetTemplateChild("HorizontalContentPanel") as Panel;
            _blurredImage = this.GetTemplateChild("BlurredImage") as Image;
            _verticalOverlay = this.GetTemplateChild("VerticalOverlayBackground") as Panel;
            _horizontalOverlay = this.GetTemplateChild("HorizontalOverlayBackground") as Panel;

            _rootGrid.SizeChanged += _rootGrid_SizeChanged;

            OnOverlayDirectionChanged();
            OnImageSourceChanged();
        }

        void _rootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetOverlaySize();
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(BlurredImage), new PropertyMetadata(OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bi = (BlurredImage)d;
            bi.OnImageSourceChanged();
        }
     

        public SolidColorBrush OverlayColorBrush
        {
            get { return (SolidColorBrush)GetValue(OverlayColorBrushProperty); }
            set { SetValue(OverlayColorBrushProperty, value); } 
        }

        // Using a DependencyProperty as the backing store for OverlayColorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayColorBrushProperty =
            DependencyProperty.Register("OverlayColorBrush", typeof(SolidColorBrush), typeof(BlurredImage), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        public double OverlayFillPercentage
        {
            get { return (double)GetValue(OverlayFillPercentageProperty); }
            set { SetValue(OverlayFillPercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverlayFillPercentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayFillPercentageProperty =
            DependencyProperty.Register("OverlayFillPercentage", typeof(double), typeof(BlurredImage), new PropertyMetadata(OnPercentageChanged));

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bi = (BlurredImage) d;
            bi.OnOverlayFillPercentageChanged();
        }


        public int BlurredStrengh
        {
            get { return (int)GetValue(BlurredStrenghProperty); }
            set { SetValue(BlurredStrenghProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlurredStrengh.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlurredStrenghProperty =
            DependencyProperty.Register("BlurredStrengh", typeof(int), typeof(BlurredImage), new PropertyMetadata(BlurredStrenghChanged));

        private static void BlurredStrenghChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bi = (BlurredImage) d;
            bi.OnBlurredStrenghChanged();
        }


        public Orientation OverlayDirection
        {
            get { return (Orientation)GetValue(OverlayDirectionProperty); }
            set { SetValue(OverlayDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverlayDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayDirectionProperty =
            DependencyProperty.Register("OverlayDirection", typeof(Orientation), typeof(BlurredImage), new PropertyMetadata(OnDirectionChanged));

        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bi = (BlurredImage)d;
            bi.OnOverlayDirectionChanged();
        }

        private void OnOverlayDirectionChanged()
        {
            if (_horizontalPanel == null || _verticalPanel == null) return;

            if (OverlayDirection == Orientation.Vertical)
            {
                _horizontalPanel.Visibility = Visibility.Collapsed;
                _verticalPanel.Visibility = Visibility.Visible;
            }
            else
            {
                _verticalPanel.Visibility = Visibility.Collapsed;
                _horizontalPanel.Visibility = Visibility.Visible;
            }

            SetOverlaySize();
        }


        void BlurredImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (ImageSource is BitmapImage)
                ((BitmapImage)ImageSource).ImageOpened -= BlurredImage_ImageOpened;

            CreateBlurredImage(ImageSource);
            OnOverlayDirectionChanged();
        }
        private void OnImageSourceChanged()
        {
            if (_rootGrid == null) return;

            if (ImageSource != null && ImageSource is BitmapImage)
            {
                _rootGrid.Opacity = 0;
                ((BitmapImage)ImageSource).ImageOpened += BlurredImage_ImageOpened;
            }
            else if (ImageSource != null && ImageSource is BitmapSource)
            {
                _rootGrid.Opacity = 0;
                CreateBlurredImage(ImageSource);
            }
            else if(_blurredImage != null)
            {
                _blurredImage.Source = null;
            }
        }

        private void CreateBlurredImage(ImageSource imageSource)
        {
            if (BlurredStrengh % 2 == 0 && BlurredStrengh != 0)
                throw new ArgumentException();

            if (ImageSource == null || _blurredImage == null) 
                return;

            var bitmapImage = ((BitmapSource)imageSource);
            if (bitmapImage.PixelHeight == 0)
                return;

            WriteableBitmap writeableBmp = new WriteableBitmap(bitmapImage);
            if (BlurredStrengh > 0)
                writeableBmp.BoxBlur(BlurredStrengh);

            _blurredImage.Source = writeableBmp;

            SetOverlaySize();
            _rootGrid.Opacity = 1;
        }

        private void OnOverlayFillPercentageChanged()
        {
            SetOverlaySize();
        }

        private void SetOverlaySize()
        {
            if (OverlayFillPercentage < 0 || OverlayFillPercentage > 100)
                throw new ArgumentOutOfRangeException();

            if (_rootGrid == null)
                return;

            var bitmap = (BitmapSource) ImageSource;

            if (OverlayDirection == Orientation.Horizontal)
            {
                var currentWidth = _rootGrid.ActualWidth;

                if (currentWidth == 0.0)
                    return;

                _horizontalOverlay.Width = currentWidth * OverlayFillPercentage / 100;
            }
            else
            {
                var currentHeight = _rootGrid.ActualHeight;

                if (currentHeight == 0.0)
                    return;

                _verticalOverlay.Height = currentHeight*OverlayFillPercentage/100;
            }
        }

        private void OnBlurredStrenghChanged()
        {
            CreateBlurredImage(ImageSource);
        }
    }
}
