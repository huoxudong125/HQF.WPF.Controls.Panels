using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HQF.WPF.Controls.CirclePanel
{
    /// <summary>
    /// This name is too generic and imprecise.
    /// It's made for conrols that can report a certain reference point. This will be used for alignment
    /// purposes by the ReferenceAlignPanel, which also needs a much better name.
    /// </summary>
    interface ICustomAlignedControl
    {
        Point AlignReferencePoint { get; }
    }

}
