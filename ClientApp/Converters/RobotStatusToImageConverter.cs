using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TME1.Abstractions.Enumerations;

namespace TME1.ClientApp.Converters;
/// <summary>
/// Converts Robot status values to mathing icon, fallbacks to error
/// </summary>
/// <remarks>The resources are only accesed once during class initialization.</remarks>
[ValueConversion(typeof(RobotStatus), typeof(BitmapImage))]
[MarkupExtensionReturnType(typeof(RobotStatusToImageConverter))]
internal class RobotStatusToImageConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// Mapping for <see cref="RobotStatus"/> to bitmap
  /// </summary>
  private static readonly Dictionary<RobotStatus, BitmapImage> _bitmapDictionary = [];

  /// <summary>
  /// Ideally this mapping would be defined in the database, and maybe automated, but it is good enough
  /// </summary>
  /// <remarks>Takes data from <see cref="Application"/> resources</remarks>
  static RobotStatusToImageConverter()
  {
    AddBitmap(RobotStatus.None, "ErrorImg");
    AddBitmap(RobotStatus.Normal, "SpinImg");
    AddBitmap(RobotStatus.Idle, "CheckmarkImg");
    AddBitmap(RobotStatus.Warning, "WarningImg");
    AddBitmap(RobotStatus.Error, "ErrorImg");
  }

  /// <summary>
  /// Adds a <see cref="BitmapImage"/> to the mapping
  /// </summary>
  /// <param name="status"></param>
  /// <param name="brushName"></param>
  /// <exception cref="InvalidOperationException">When resource couldn't be found</exception>
  private static void AddBitmap(RobotStatus status, string brushName)
  {
    var brush = Application.Current.Resources[brushName] as BitmapImage;
    if (brush is null)
      throw new InvalidOperationException($"{nameof(Brush)} resource with name `{brushName}` does not exist");

    _bitmapDictionary[status] = brush;
  }

  /// <summary>
  /// Converts <paramref name="value"/> if it is a <see cref="RobotStatus"/> to the matching <see cref="BitmapImage"/>.
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns>Matching <see cref="BitmapImage"/></returns>
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if (value is not RobotStatus status)
      throw new NotSupportedException($"Cannot convert from {value}");

    if (_bitmapDictionary.TryGetValue(status, out var brush))
      return brush;

    return _bitmapDictionary[RobotStatus.Error];
  }

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(RobotStatusToBrushConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
