using CommunityToolkit.Mvvm.Input;
using Evergine.Common.Graphics;
using Evergine.Common.Helpers;
using Evergine.DirectX11;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.WPF;
using System.Windows;
using System.Windows.Input;

namespace TME1.ClientApp.Components.Evergine;
public abstract partial class EvergineViewModelBase : BaseViewModel
{
  protected DX11GraphicsContext GraphicsContext { get; init; }
  protected GraphicsPresenter GraphicsPresenter { get; init; }
  protected WPFSurface? Surface { get; private set; }
  protected string? DisplayTag { get; private set; }
  private Display? Display { get; set; }
  protected FrameBuffer? FrameBuffer => Display?.FrameBuffer;
  protected SwapChain? SwapChain => Display?.SwapChain;

  protected bool IsInitialized { get; private set; }
  protected bool IsPresenting { get; set; }

  public FrameworkElement? Control => Surface?.NativeControl;

  protected EvergineViewModelBase(DX11GraphicsContext graphicsContext, GraphicsPresenter graphicsPresenter)
  {
    GraphicsContext = graphicsContext;
    GraphicsPresenter = graphicsPresenter;
  }

  protected abstract void Initialize();

  protected abstract void DeInitialize();

  [RelayCommand]
  public virtual void Load()
  {
    Surface = new WPFSurface(0, 0);
    Display = new Display(Surface, (FrameBuffer)null!);
    DisplayTag = Guid.NewGuid().ToString();

    Surface.SurfaceUpdatedAction += NativeControlSurfaceUpdated;
    Surface.NativeControl.MouseDown += NativeControlMouseDown;
    Surface.NativeControl.MouseUp += NativeControlMouseUp;

    GraphicsPresenter.AddDisplay(DisplayTag, Display);

    Initialize();
    IsInitialized = true;

    OnPropertyChanged(nameof(Control));
  }

  [RelayCommand]
  public virtual void Unload()
  {
    IsInitialized = false;
    DeInitialize();

    GraphicsPresenter.RemoveDisplay(DisplayTag);
    DisplayTag = null;

    Display?.Dispose();
    Display = null;

    if (Surface is not null)
    {
      Surface.SurfaceUpdatedAction -= NativeControlSurfaceUpdated;
      Surface.NativeControl.MouseDown -= NativeControlMouseDown;
      Surface.NativeControl.MouseUp -= NativeControlMouseUp;

      Surface?.Dispose();
      Surface = null;
    }

    OnPropertyChanged(nameof(Control));
  }

  public virtual void Draw()
  {
    if (!IsInitialized)
      return;

    Surface?.KeyboardDispatcher?.DispatchEvents();
    Surface?.MouseDispatcher?.DispatchEvents();
    Surface?.TouchDispatcher?.DispatchEvents();

    if (IsPresenting)
    {
      Display?.SwapChain?.InitFrame();
    }
    
    if(FrameBuffer is not null)
      DrawContent();

    if (IsPresenting)
    {
      Display?.SwapChain?.Present();
    }
  }

  protected abstract void Resized();

  protected abstract void DrawContent();

  ~EvergineViewModelBase() => Dispose(false);

  protected override void Dispose(bool disposing)
  {
    Unload();

    base.Dispose(disposing);
  }

  protected virtual void NativeControlMouseUp(object sender, MouseButtonEventArgs e)
  {
    ((FrameworkElement)sender).ReleaseMouseCapture();
  }

  protected virtual void NativeControlMouseDown(object sender, MouseButtonEventArgs e)
  {
    ((FrameworkElement)sender).Focus();
    ((FrameworkElement)sender).CaptureMouse();
  }

  protected virtual void NativeControlSurfaceUpdated(IntPtr surfaceHandle)
  {
    var sharedObject = new SharpGen.Runtime.ComObject(surfaceHandle);
    var sharedResource = sharedObject.QueryInterface<Vortice.DXGI.IDXGIResource>();
    var nativeRexture = GraphicsContext.DXDevice.OpenSharedResource<Vortice.Direct3D11.ID3D11Texture2D>(sharedResource.SharedHandle);

    var texture = DX11Texture.FromDirectXTexture(GraphicsContext, nativeRexture);
    var rTDepthTargetDescription = new TextureDescription()
    {
      Type = TextureType.Texture2D,
      Format = PixelFormat.D24_UNorm_S8_UInt,
      Width = texture.Description.Width,
      Height = texture.Description.Height,
      Depth = 1,
      ArraySize = 1,
      Faces = 1,
      Flags = TextureFlags.DepthStencil,
      CpuAccess = ResourceCpuAccess.None,
      MipLevels = 1,
      Usage = ResourceUsage.Default,
      SampleCount = TextureSampleCount.None,
    };

    var rTDepthTarget = GraphicsContext.Factory.CreateTexture(ref rTDepthTargetDescription);
    var frameBuffer = GraphicsContext.Factory.CreateFrameBuffer(new FrameBufferAttachment(rTDepthTarget, 0, 1), [new FrameBufferAttachment(texture, 0, 1)]);
    frameBuffer.IntermediateBufferAssociated = true;
    Display?.FrameBuffer?.Dispose();
    Display?.UpdateFrameBuffer(frameBuffer);
    Resized();
  }
}
