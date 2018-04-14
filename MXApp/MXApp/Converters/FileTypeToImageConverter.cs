using MXApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MXApp.Converters
{
    public class FileTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString() == PODFileTypes.PDF.ToString())
                {
                    if (Device.RuntimePlatform == Device.UWP)
                        return "Assets/pdf.png";
                    else
                        return "pdf.png";
                }
                else if (value.ToString() == PODFileTypes.Excel.ToString())
                {
                    if (Device.RuntimePlatform == Device.UWP)
                        return "Assets/excel.png";
                    else
                        return "excel.png";
                }
                else if (value.ToString() == PODFileTypes.Docx.ToString())
                {
                    if (Device.RuntimePlatform == Device.UWP)
                        return "Assets/word.png";
                    else
                        return "word.png";
                }
                else if (value.ToString() == PODFileTypes.Image.ToString())
                {
                    if (Device.RuntimePlatform == Device.UWP)
                        return "Assets/landscape.png";
                    else
                        return "landscape.png";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}