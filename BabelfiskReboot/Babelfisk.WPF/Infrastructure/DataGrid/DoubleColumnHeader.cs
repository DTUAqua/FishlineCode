using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Babelfisk.WPF.Infrastructure.DataGrid
{
    internal class DoubleColumnHeader : DependencyObject
    {
        private string _strUpperHeader;

        private Brush _bUpperHeaderBackground;

        private Brush _bUpperHeaderBorderBrush = Brushes.Transparent;

        private Thickness _bUppHeaderBorderThickness = new Thickness(0,0,0,2);

        private Brush _bUpperForeground = Brushes.Black;

        private Brush _bLowerForeground = Brushes.Black;

        private Thickness _tHeaderBorder = new Thickness(0,0,1,1);

        private Thickness _tLowerHeaderBorder;

        private HorizontalAlignment _hAlignment = HorizontalAlignment.Left;


        public string LowerHeader
        {
            get { return GetValue(LowerHeaderProperty) as string; }
            set { SetValue(LowerHeaderProperty, value); }
        }

        public static readonly DependencyProperty LowerHeaderProperty =
            DependencyProperty.Register("LowerHeader", typeof(string), typeof(DoubleColumnHeader), new UIPropertyMetadata(null));


        public string LowerHeaderToolTip
        {
            get { return GetValue(LowerHeaderToolTipProperty) as string; }
            set { SetValue(LowerHeaderToolTipProperty, value); }
        }

        public static readonly DependencyProperty LowerHeaderToolTipProperty = DependencyProperty.Register("LowerHeaderToolTip", typeof(string), typeof(DoubleColumnHeader), new UIPropertyMetadata(null));


        public string UpperHeader
        {
            get { return _strUpperHeader; }
            set { _strUpperHeader = value; }
        }


        public Brush UpperHeaderBackground
        {
            get { return _bUpperHeaderBackground; }
            set { _bUpperHeaderBackground = value; }
        }

        public Thickness HeaderBorder
        {
            get { return _tHeaderBorder; }
            set { _tHeaderBorder = value; }
        }


        public Thickness LowerHeaderBorder
        {
            get { return _tLowerHeaderBorder; }
            set { _tLowerHeaderBorder = value; }
        }


        public Brush UpperHeaderForeground
        {
            get { return _bUpperForeground; }
            set { _bUpperForeground = value; }
        }

        public Brush UpperHeaderBorderBrush
        {
            get { return _bUpperHeaderBorderBrush; }
            set { _bUpperHeaderBorderBrush = value; }
        }

        public Thickness UpperHeaderBorderThickness
        {
            get { return _bUppHeaderBorderThickness; }
            set { _bUppHeaderBorderThickness = value; }
        }

        public Brush LowerHeaderForeground
        {
            get { return _bLowerForeground; }
            set { _bLowerForeground = value; }
        }


        public HorizontalAlignment LowerHeaderHorizontalAlignment
        {
            get { return _hAlignment; }
            set { _hAlignment = value; }
        }





        public DoubleColumnHeader()
        {
          
        }
    }
}
