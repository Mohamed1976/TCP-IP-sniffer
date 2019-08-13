﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TCP_IP_sniffer.Converter
{
    public class ByteToHexConverter : IValueConverter
    {
        public ByteToHexConverter()
        {
            Separator = ' ';
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = (byte[])value;
            return BitConverter.ToString(bytes).Replace('-', Separator);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public char Separator { get; set; }
    }
}
