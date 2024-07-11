using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace TME1.ClientApp.Converters;
/// <summary>
/// Converts model name to proper image, with a pseudorandom fallback based on <see cref="object.GetHashCode"/>.
/// </summary>
/// <remarks>The resources are only accesed once during class initialization.</remarks>
[ValueConversion(typeof(string), typeof(BitmapSource))]
[MarkupExtensionReturnType(typeof(RobotModelToImageConverter))]
public class RobotModelToImageConverter : MarkupExtension, IValueConverter
{
  /// <summary>
  /// a list for quick access to bitmap by index
  /// </summary>
  private static readonly List<BitmapSource> _images = [];
  /// <summary>
  /// a dictionary for quick access to bitmap by name
  /// </summary>
  private static readonly Dictionary<string, BitmapSource> _imageDictionary = [];

  /// <summary>
  /// Ideally this mapping would be defined in the database, and maybe automated, but it is good enough
  /// </summary>
  /// <remarks>Takes data from <see cref="Application"/> resources</remarks>
  static RobotModelToImageConverter()
  {
    AddBitmap("Robot 1", "Robot1Img");
    AddBitmap("Robot 2", "Robot2Img");
    AddBitmap("Robot 3", "Robot3Img");
    AddBitmap("Robot 4", "Robot4Img");
  }

  /// <summary>
  /// Populates the private fields mappings with resources
  /// </summary>
  /// <param name="modelName">the exact model name to bind to this image</param>
  /// <param name="resourceName">under what application resource key the bitmap resides</param>
  /// <exception cref="InvalidOperationException">When resource couldn't be found</exception>
  /// <remarks>In case of multiple models for one image, it would be beneficial to add a new method with <see langword="params"/> for <paramref name="resourceName"/></remarks>
  private static void AddBitmap(string modelName, string resourceName)
  {
    if (Application.Current.Resources[resourceName] is not BitmapImage bitmap)
      throw new InvalidOperationException($"{nameof(BitmapImage)} resource with name `{resourceName}` does not exist");

    _images.Add(bitmap);
    _imageDictionary[modelName] = bitmap;
  }

  /// <summary>
  /// Converts <paramref name="value"/> if it is a <see cref="string"/> to the matching <see cref="BitmapImage"/>.
  /// Will first lookup for the model name in dictionary of defined bindings
  /// On failure will select a pseudorandom entry based on hash of name
  /// </summary>
  /// <param name="value">string representing: the robot model name</param>
  /// <param name="targetType"></param>
  /// <param name="parameter">when provided a boolean true will output white image</param>
  /// <param name="culture"></param>
  /// <returns>The matching <see cref="BitmapImage"/></returns>
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if (value is not string modelName)
      throw new NotSupportedException($"Cannot convert from {value}");

    if (!_imageDictionary.TryGetValue(modelName, out var imageBitmap))
    {
      var modelNameHash = modelName.GetHashCode();

      ///<see cref="Math.Abs(int)"/> is a solution for % operator with negative lhs parameter returning negative numbers for index
      imageBitmap = _images[Math.Abs(modelNameHash) % _images.Count];
    }

    return imageBitmap;
  }

  object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(RobotModelToImageConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
