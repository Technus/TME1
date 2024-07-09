using System.Windows.Data;
using System.Windows.Markup;

namespace TME1.UI.Converters;
/// <summary>
/// Converts objects to string, in the form: `{TypeName}_{ToString()}`.
/// </summary>
/// <remarks>Explicitly added Another <see cref="ValueConversionAttribute"/> attribute with <see cref="Enum"/> as input to signify main use case.</remarks>
[ValueConversion(typeof(object), typeof(string))]
[ValueConversion(typeof(Enum), typeof(string))]
[MarkupExtensionReturnType(typeof(PrependTypeNameConverter))]
public class PrependTypeNameConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// Converts value to string, in the form: `{TypeName}_{ToString()}`.
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns>The string in the form: `{TypeName}_{ToString()}`</returns>
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => $"{value?.GetType()?.Name}_{value?.ToString()}";

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(PrependTypeNameConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
