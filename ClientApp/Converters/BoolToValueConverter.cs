using System.Windows.Data;
using System.Windows.Markup;

namespace TME1.ClientApp.Converters;
/// <summary>
/// Used to perform ternary operator
/// </summary>
[MarkupExtensionReturnType(typeof(BoolToValueConverter))]
[ValueConversion(typeof(bool),typeof(object))]
class BoolToValueConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// Returned when value is true
  /// </summary>
  public object? IfTrue { get; set; }
  /// <summary>
  /// Returned in all other cases
  /// </summary>
  public object? Else { get; set; }

  /// <summary>
  /// Returns <see cref="IfTrue"/> or <see cref="Else"/> depending if <paramref name="value"/> is <see langword="true"/>
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => value is true ? IfTrue : Else;

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(BoolToValueConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
