using JasperFx.Core;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace TME1.ClientApp.Converters;
/// <summary>
/// Wraps the string in parenthesis if it is not null or empty
/// </summary>
public class WrapInParenthesisConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// Converts <paramref name="value"/> to <see cref="string"/> in the form: `( Value )`
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns>string wrapped in parenthesis</returns>
  /// <remarks>In case of value being empty or null return string empty</remarks>
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if (value is null)
      return string.Empty;

    if (value is not string text)
      throw new NotSupportedException($"Cannot convert from {value}");

    if (text.IsEmpty())
      return string.Empty;

    return $"({value})";
  }

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(WrapInParenthesisConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
