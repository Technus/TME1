using Evergine.Common.Graphics;
using System.IO;
using System.Text;
using System.Windows.Resources;

namespace TME1.ClientApp.Components.Evergine;
public static class EvergineViewModelHelpers
{
  public static string ShaderSource(this StreamResourceInfo resourceInfo, Encoding encoding = default!)
    => ShaderSource(resourceInfo.Stream, encoding);

  public static string ShaderSource(this Stream shader, Encoding encoding = default!)
  {
    using var stream = shader;
    using var streamReader = new StreamReader(stream, encoding ?? Encoding.ASCII);
    return streamReader.ReadToEnd();
  }

  public static Shader CreateShader(this GraphicsContext context, ShaderStages shaderStage, string entryPoint, string shaderSource)
  {
    var compilationResult = context.ShaderCompile(shaderSource, entryPoint, shaderStage);
    if (compilationResult.HasErrors)
      throw new ArgumentOutOfRangeException(nameof(shaderSource), shaderSource, $"Error on line {compilationResult.ErrorLine}: {compilationResult.Message}");
    var shaderDescription = new ShaderDescription(shaderStage, entryPoint, compilationResult.ByteCode);
    return context.Factory.CreateShader(ref shaderDescription);
  }

  public static SwapChainDescription CreateSwapChainDescription(uint width, uint height, TextureSampleCount sampleCount) => new()
  {
    Width = width,
    Height = height,
    ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
    ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
    DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
    DepthStencilTargetFlags = TextureFlags.DepthStencil,
    SampleCount = sampleCount,
    IsWindowed = true,
    RefreshRate = 60,
  };

  public static InputLayouts Add(this InputLayouts layouts, ReadOnlySpan<ElementDescription> layoutDescriptions)
  {
    var desc = new LayoutDescription();

    for (int i = 0; i < layoutDescriptions.Length; i++)
      desc.Add(layoutDescriptions[i]);

    return layouts.Add(desc);
  }
}
