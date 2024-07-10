using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TME1.Abstractions.Enumerations;

namespace TME1.UI.Converters;
/// <summary>
/// Used to hide the Shield icon on robot card when it is not safe.
/// </summary>
[MarkupExtensionReturnType(typeof(RobotPositionToVisibilityConverter))]
[ValueConversion(typeof(RobotPosition), typeof(Visibility))]
public class RobotPositionToVisibilityConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// Converts <see cref="RobotPosition"/> to <see cref="Visibility"/>
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns>Proper icon visibility</returns>
  /// <remarks>Currently only allows to show the Shield icon when value is <see cref="RobotPosition.Safe"/></remarks>
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if(value is not RobotPosition position)
      throw new NotSupportedException($"Cannot convert from {value}");

    return position switch
    {
      RobotPosition.Safe => Visibility.Visible,
      _ => Visibility.Collapsed,
    };
  }

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(RobotPositionToVisibilityConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
