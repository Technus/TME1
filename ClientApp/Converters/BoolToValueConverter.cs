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
  public object? IfTrue { get; set; }
  public object? Else { get; set; }

  public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => value is true ? IfTrue : Else;

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(BoolToValueConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
