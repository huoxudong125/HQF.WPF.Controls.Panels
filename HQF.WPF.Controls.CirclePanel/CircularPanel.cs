using System;
using System.Windows;
using System.Windows.Controls;

namespace HQF.WPF.Controls.CirclePanel
{
    public class CircularPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return base.MeasureOverride(availableSize);
        }

        // Arrange stuff in a circle
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count > 0)
            {
                // Center & radius of panel
                var center = new Point(finalSize.Width / 2, finalSize.Height / 2);
                var radius = Math.Min(finalSize.Width, finalSize.Height) / 2.0;
                radius *= 0.8; // To avoid hitting edges

                // # radians between children
                var angleIncrRadians = 2.0 * Math.PI / Children.Count;

                var angleInRadians = 0.0;

                foreach (UIElement child in Children)
                {
                    var childPosition = new Point(
                        radius * Math.Cos(angleInRadians) + center.X,
                        radius * Math.Sin(angleInRadians) + center.Y);

                    child.Arrange(new Rect(childPosition, child.DesiredSize));

                    angleInRadians += angleIncrRadians;
                }
            }

            return finalSize;
        }
    }
}