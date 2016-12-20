using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HQF.WPF.Controls.CirclePanel
{
    /// <summary>
    ///     Arranges child elements along the circumference of a circle.
    /// </summary>
    public class RadialPanel2 : Panel
    {
        /// <summary>
        ///     Identifies the RadialPanel.Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(RadialPanel),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Identifies the RadialPanel.ForegroundProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(typeof(RadialPanel),
                new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        ///     The angle of the slice that each child occupies.
        /// </summary>
        private double angleEach;

        /// <summary>
        ///     The distance from the bottom of any child to the center of the circle.
        /// </summary>
        private double innerEdgeFromCenter;

        /// <summary>
        ///     The distance from the top of any child to the center of the circle.
        /// </summary>
        private double outerEdgeFromCenter;

        /// <summary>
        ///     Radius of the circle that surrounds the children.
        /// </summary>
        private double radius;

        /// <summary>
        ///     Backing field for the ShowPieLines property.
        /// </summary>
        private bool showPieLines;

        /// <summary>
        ///     Size of the largest child.
        /// </summary>
        private Size sizeLargest;

        /// <summary>
        ///     Gets or sets a brush that describes the foreground color. This is a dependency property.
        /// </summary>
        [Category("Appearance")]
        [Bindable(true)]
        public Brush Foreground
        {
            get { return (Brush) GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether child elements span the circumference of the panel by their widths or
        ///     by their heights.
        ///     This is a dependency property.
        /// </summary>
        [Category("Layout")]
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to draw lines along the circumference and
        ///     along the spooks that separates the child elements.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowPieLines
        {
            set
            {
                if (showPieLines != value)
                    showPieLines = value;
                InvalidateVisual();
            }

            get { return showPieLines; }
        }

        /// <summary>
        ///     Measures the child elements of a RadialPanel in anticipation
        ///     of arranging them during the RadialPanel.ArrangeOverride(System.Windows.Size)
        ///     pass.
        /// </summary>
        /// <param name="sizeAvailable">
        ///     An upper limit <see cref="T:System.Windows.Size" /> that should not be exceeded.
        /// </param>
        /// <returns>
        ///     The <see cref="T:System.Windows.Size" /> that represents the desired size of the element.
        /// </returns>
        protected override Size MeasureOverride(Size sizeAvailable)
        {
            if (InternalChildren.Count == 0)
                return new Size();

            angleEach = 360.0 / InternalChildren.Count;
            sizeLargest = new Size();

            foreach (UIElement child in InternalChildren)
            {
                // Call Measure for each child ...
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                // Examine DesiredSize property of child.
                sizeLargest.Width = Math.Max(sizeLargest.Width, child.DesiredSize.Width);
                sizeLargest.Height = Math.Max(sizeLargest.Height, child.DesiredSize.Height);
            }

            if (InternalChildren.Count == 1)
            {
                var diagonal = Math.Sqrt(sizeLargest.Width * sizeLargest.Width + sizeLargest.Height * sizeLargest.Height);
                return new Size(diagonal, diagonal);
            }

            double halfLargestSpan;
            double largestHeight;
            if (Orientation == Orientation.Horizontal)
            {
                halfLargestSpan = sizeLargest.Width / 2.0;
                largestHeight = sizeLargest.Height;
            }
            else
            {
                halfLargestSpan = sizeLargest.Height / 2.0;
                largestHeight = sizeLargest.Width;
            }

            // Calculate the distance from the center to element edges.
            innerEdgeFromCenter = InternalChildren.Count == 2
                ? 0.0
                : halfLargestSpan / Math.Tan(angleEach * Math.PI / 360.0);
            outerEdgeFromCenter = innerEdgeFromCenter + largestHeight;

            // Calculate the radius of the circle based on the largest child.
            radius = Math.Sqrt(halfLargestSpan * halfLargestSpan + outerEdgeFromCenter * outerEdgeFromCenter);

            // Return the size of that circle.
            return new Size(2.0 * radius, 2.0 * radius);
        }

        /// <summary>
        ///     Arranges the content of a RadialPanel element.
        /// </summary>
        /// <param name="sizeFinal">
        ///     The <see cref="T:System.Windows.Size" /> that this element should use to arrange its child elements.
        /// </param>
        /// <returns>
        ///     The <see cref="T:System.Windows.Size" /> that represents the arranged size of this RadialPanel
        ///     element and its child elements.
        /// </returns>
        protected override Size ArrangeOverride(Size sizeFinal)
        {
            if (InternalChildren.Count == 0)
                return sizeFinal;
            if (InternalChildren.Count == 1)
            {
                var child = InternalChildren[0];
                child.RenderTransform = Transform.Identity;
                var center = new Point(
                    (sizeFinal.Width - sizeLargest.Width) / 2.0,
                    (sizeFinal.Height - sizeLargest.Height) / 2.0);
                child.Arrange(new Rect(center, new Size(sizeLargest.Width, sizeLargest.Height)));

                if (Orientation == Orientation.Vertical)
                {
                    var rotatePoint = TranslatePoint(center, child);
                    rotatePoint.X += sizeLargest.Width / 2.0;
                    rotatePoint.Y += sizeLargest.Height / 2.0;
                    child.RenderTransform = new RotateTransform(-90.0, rotatePoint.X, rotatePoint.Y);
                }

                return sizeFinal;
            }

            var angleChild = Orientation == Orientation.Horizontal ? 0.0 : -90.0;
            var centerPoint = new Point(sizeFinal.Width / 2.0, sizeFinal.Height / 2.0);
            var multiplier = Math.Min(sizeFinal.Width, sizeFinal.Height) / (2.0 * radius);

            foreach (UIElement child in InternalChildren)
            {
                // Reset RenderTransform.
                child.RenderTransform = Transform.Identity;

                if (Orientation == Orientation.Horizontal)
                    child.Arrange(
                        new Rect(
                            centerPoint.X - multiplier * sizeLargest.Width / 2.0,
                            centerPoint.Y - multiplier * outerEdgeFromCenter,
                            multiplier * sizeLargest.Width,
                            multiplier * sizeLargest.Height));
                else
                    child.Arrange(
                        new Rect(
                            centerPoint.X + multiplier * innerEdgeFromCenter,
                            centerPoint.Y - multiplier * sizeLargest.Height / 2.0,
                            multiplier * sizeLargest.Width,
                            multiplier * sizeLargest.Height));

                // Rotate the child around the center (relative to the child).
                var pt = TranslatePoint(centerPoint, child);
                child.RenderTransform = new RotateTransform(angleChild, pt.X, pt.Y);

                angleChild += angleEach;
            }

            return sizeFinal;
        }

        /// <summary>
        ///     Draws the content of a <see cref="T:System.Windows.Media.DrawingContext" /> object during
        ///     the render pass of a RadialPanel element.
        /// </summary>
        /// <param name="dc">
        ///     The <see cref="T:System.Windows.Media.DrawingContext" /> object to draw.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (ShowPieLines)
            {
                var centerPoint = new Point(RenderSize.Width / 2.0, RenderSize.Height / 2.0);
                var radius = Math.Min(RenderSize.Width, RenderSize.Height) / 2;
                var pen = new Pen(Foreground, 1.0);
                pen.DashStyle = DashStyles.Dash;

                // Display circle.
                dc.DrawEllipse(null, pen, centerPoint, radius, radius);

                if (InternalChildren.Count == 1)
                    return;

                // Initialize angle.
                var angleChild = -(angleEach / 2.0) - 90.0;

                // Loop through each child to draw radial lines from center.
                foreach (UIElement child in InternalChildren)
                {
                    var angleChildInRadian = 2.0 * Math.PI * angleChild / 360;
                    dc.DrawLine(pen, centerPoint,
                        new Point(centerPoint.X + radius * Math.Cos(angleChildInRadian),
                            centerPoint.Y + radius * Math.Sin(angleChildInRadian)));
                    angleChild += angleEach;
                }
            }
        }
    }
}