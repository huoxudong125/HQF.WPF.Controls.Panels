using System;
using System.Windows;
using System.Windows.Controls;

namespace HQF.WPF.Controls.CirclePanel
{
    public class RadialPanel : Panel

    {
        // Measure each children and give as much room as they want

        protected override Size MeasureOverride(Size availableSize)

        {
            foreach (UIElement elem in Children)

                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return base.MeasureOverride(availableSize);
        }

        //Arrange all children based on the geometric equations for the circle.

        protected override Size ArrangeOverride(Size finalSize)

        {
            if (Children.Count == 0)
                return finalSize;

            double _angle = 0;

            //Degrees converted to Radian by multiplying with PI/180

            var _incrementalAngularSpace = 360.0 / Children.Count * (Math.PI / 180);

            //An approximate radii based on the avialable size , obviusly a better approach is needed here.

            var radiusX = finalSize.Width / 2.4;

            var radiusY = finalSize.Height / 2.4;

            foreach (UIElement elem in Children)

            {
                //Calculate the point on the circle for the element

                var childPoint = new Point(Math.Cos(_angle) * radiusX, -Math.Sin(_angle) * radiusY);

                //Offsetting the point to the Avalable rectangular area which is FinalSize.

                var actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2,
                    finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

                //Call Arrange method on the child element by giving the calculated point as the placementPoint.

                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width,
                    elem.DesiredSize.Height));

                //Calculate the new _angle for the next element

                _angle += _incrementalAngularSpace;
            }

            return finalSize;
        }
    }
}