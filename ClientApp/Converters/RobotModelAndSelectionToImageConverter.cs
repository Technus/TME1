using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TME1.ClientApp.Converters;
/// <summary>
/// Converts model name to proper image, with a pseudorandom fallback based on <see cref="object.GetHashCode"/>.
/// </summary>
/// <remarks>The resources are only accesed once during class initialization.</remarks>
[ValueConversion(typeof(string), typeof(BitmapSource))]
[MarkupExtensionReturnType(typeof(RobotModelAndSelectionToImageConverter))]
public class RobotModelAndSelectionToImageConverter : MarkupExtension, IMultiValueConverter
{
  private static readonly Color _whiteColor = Colors.White;

  /// <summary>
  /// a list for quick access to bitmap by index
  /// </summary>
  private static readonly List<(BitmapSource black, BitmapSource white)> _images = [];
  /// <summary>
  /// a dictionary for quick access to bitmap by name
  /// </summary>
  private static readonly Dictionary<string, (BitmapSource black, BitmapSource white)> _imageDictionary = [];

  /// <summary>
  /// Ideally this mapping would be defined in the database, and maybe automated, but it is good enough
  /// </summary>
  /// <remarks>Takes data from <see cref="Application"/> resources</remarks>
  static RobotModelAndSelectionToImageConverter()
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

    var blackAndWhiteImages = (bitmap, GetWhite(bitmap));

    _images.Add(blackAndWhiteImages);
    _imageDictionary[modelName] = blackAndWhiteImages;
  }

  /// <summary>
  /// Creates new bitmap based on the <paramref name="bitmap"/>
  /// </summary>
  /// <param name="bitmap"><see cref="BitmapImage"/> to process</param>
  /// <returns>Image recolorized to white</returns>
  private static BitmapSource GetWhite(BitmapImage bitmap)
  {
    var writeableBitmap = new WriteableBitmap(new BitmapImage(bitmap.UriSource));

    // Define the new color to use for recoloring

    // Get the pixel data from the original image
    var width = writeableBitmap.PixelWidth;
    var height = writeableBitmap.PixelHeight;
    var stride = width * (writeableBitmap.Format.BitsPerPixel / 8);
    var pixelData = new byte[height * stride];
    bitmap.CopyPixels(pixelData, stride, 0);

    // Iterate through each pixel and recolor based on opacity
    for (var y = 0; y < height; y++)
    {
      for (var x = 0; x < width; x++)
      {
        var index = y * stride + x * 4;

        // Get the alpha value
        var alpha = pixelData[index + 3];

        // Set the new color based on the alpha value
        pixelData[index] = (byte)(_whiteColor.B * alpha / 255);
        pixelData[index + 1] = (byte)(_whiteColor.G * alpha / 255);
        pixelData[index + 2] = (byte)(_whiteColor.R * alpha / 255);
        pixelData[index + 3] = alpha;
      }
    }

    // Write the modified pixel data back to the WriteableBitmap
    writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

    return writeableBitmap;
  }

  /// <summary>
  /// Converts <paramref name="value"/> if it is a <see cref="string"/> to the matching <see cref="BitmapImage"/>.
  /// Will first lookup for the model name in dictionary of defined bindings
  /// On failure will select a pseudorandom entry based on hash of name
  /// </summary>
  /// <param name="values">string representing: the robot model name, boolean representing selection: switch to white image</param>
  /// <param name="targetType"></param>
  /// <param name="parameter">when provided a boolean true will output white image</param>
  /// <param name="culture"></param>
  /// <returns>The matching <see cref="BitmapImage"/></returns>
  public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    if (values is null || values.Length < 2)
      throw new NotSupportedException($"Cannot convert from {values}");

    var modelName = values[0] as string ?? string.Empty;

    if (!_imageDictionary.TryGetValue(modelName, out (BitmapSource black, BitmapSource white) imageEntry))
    {
      var modelNameHash = modelName.GetHashCode();

      ///<see cref="Math.Abs(int)"/> is a solution for % operator with negative lhs parameter returning negative numbers for index
      imageEntry = _images[Math.Abs(modelNameHash) % _images.Count];
    }

    var useWhite = values[1] is true;

    return useWhite ? imageEntry.white : imageEntry.black;
  }

  object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    => throw new NotSupportedException($"{nameof(RobotModelAndSelectionToImageConverter)} can only be used in OneWay bindings");

  public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
