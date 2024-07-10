using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TME1.Abstractions.Enumerations;

namespace TME1.ClientApp.Converters;
/// <summary>
/// Converts Robot status values to mathing color, fallbacks to error
/// </summary>
/// <remarks>The resources are only accesed once during class initialization.</remarks>
[ValueConversion(typeof(RobotStatus), typeof(Brush))]
[MarkupExtensionReturnType(typeof(RobotStatusToBrushConverter))]
internal class RobotStatusToBrushConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// Mapping for <see cref="RobotStatus"/> to brush
  /// </summary>
  private static readonly Dictionary<RobotStatus, Brush> _brushDictionary = [];

  /// <summary>
  /// Builds mapping from <see cref="RobotStatus"/> to it's corresponding <see cref="Brush"/>
  /// </summary>
  /// <remarks>Takes data from <see cref="Application"/> resources</remarks>
  static RobotStatusToBrushConverter()
  {
    AddBrush(RobotStatus.None, "GrayBg1Brush");
    AddBrush(RobotStatus.Normal, "StatusMgBBrush");
    AddBrush(RobotStatus.Idle, "StatusMgGBrush");
    AddBrush(RobotStatus.Warning, "StatusMgYBrush");
    AddBrush(RobotStatus.Error, "StatusMgRBrush");
  }

  /// <summary>
  /// Adds a <see cref="Brush"/> to the mapping
  /// </summary>
  /// <param name="status">Status to map</param>
  /// <param name="brushName">Brush to map</param>
  /// <exception cref="InvalidOperationException">When resource couldn't be found</exception>
  private static void AddBrush(RobotStatus status, string brushName)
  {
    if (Application.Current.Resources[brushName] is not Brush brush)
      throw new InvalidOperationException($"{nameof(Brush)} resource with name `{brushName}` does not exist");

    _brushDictionary[status] = brush;
  }

  /// <summary>
  /// Converts <paramref name="value"/> if it is a <see cref="RobotStatus"/> to the matching <see cref="Brush"/>.
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns>Matching <see cref="Brush"/></returns>
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if (value is not RobotStatus status)
      throw new NotSupportedException($"Cannot convert from {value}");

    if (_brushDictionary.TryGetValue(status, out var brush))
      return brush;

    return _brushDictionary[RobotStatus.Error];
  }

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(RobotStatusToBrushConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
