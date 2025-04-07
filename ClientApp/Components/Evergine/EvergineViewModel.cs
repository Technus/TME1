using Evergine.Common.Graphics;
using Evergine.DirectX11;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System.Runtime.CompilerServices;
using System.Windows;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace TME1.ClientApp.Components.Evergine;
public partial class EvergineViewModel : EvergineViewModelBase
{
  private CancellationTokenSource cts;
  private CommandQueue commandQueue;
  private GraphicsPipelineDescription pipelineDescription;
  private GraphicsPipelineState pipelineState;
  private Buffer[] vertexBuffers;
  private Viewport[] viewports;
  private Rectangle[] scissors;
  private Vector4[] vertexData =
  [
            // TriangleList
            new Vector4(0f, 0.5f, 0.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(-0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
  ];

  public EvergineViewModel(DX11GraphicsContext graphicsContext, GraphicsPresenter graphicsPresenter) : base(graphicsContext, graphicsPresenter)
  {
  }

  protected override void Initialize()
  {
    var ss = Application.GetResourceStream(new Uri(@"\Components\Evergine\Shaders\HLSL.fx", UriKind.Relative)).ShaderSource();
    var vs = GraphicsContext.CreateShader(ShaderStages.Vertex, "VS", ss);
    var ps = GraphicsContext.CreateShader(ShaderStages.Pixel, "PS", ss);

    var vertexBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Vector4>() * (uint)vertexData.Length, BufferFlags.VertexBuffer, ResourceUsage.Default);
    var vertexBuffer = GraphicsContext.Factory.CreateBuffer(vertexData, ref vertexBufferDescription);

    var vertexLayouts = new InputLayouts()
      .Add([
        new ElementDescription(ElementFormat.Float4, ElementSemanticType.Position), 
        new ElementDescription(ElementFormat.Float4, ElementSemanticType.Color)]);

    pipelineDescription = new GraphicsPipelineDescription
    {
      PrimitiveTopology = PrimitiveTopology.TriangleList,
      InputLayouts = vertexLayouts,
      Shaders = new GraphicsShaderStateDescription()
      {
        VertexShader = vs,
        PixelShader = ps,
      },
      RenderStates = new RenderStateDescription()
      {
        RasterizerState = RasterizerStates.CullBack,
        BlendState = BlendStates.Opaque,
        DepthStencilState = DepthStencilStates.ReadWrite,
      },
    };

    commandQueue = GraphicsContext.Factory.CreateCommandQueue();

    vertexBuffers = new Buffer[1];
    vertexBuffers[0] = vertexBuffer;

    cts = new CancellationTokenSource();
    Task.Factory.StartNew(() => DrawAsync(cts.Token));
  }

  private async Task DrawAsync(CancellationToken token = default)
  {
    while (!token.IsCancellationRequested)
    {
      await Application.Current.Dispatcher.InvokeAsync(Draw);
      await Task.Delay(100, token);
    }
  }

  protected override void DeInitialize()
  {
    cts.Dispose();
    commandQueue.Dispose();
    pipelineState.Dispose();
  }

  protected override void Resized()
  {
    pipelineDescription.Outputs = FrameBuffer?.OutputDescription ?? throw new InvalidOperationException("Missing Frame buffer");
    pipelineState = GraphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);

    var swapChainDescription = SwapChain?.SwapChainDescription;
    var width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : Surface?.Width ?? 0;
    var height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : Surface?.Height ?? 0;

    viewports = new Viewport[1];
    viewports[0] = new Viewport(0, 0, width, height);
    scissors = new Rectangle[1];
    scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
  }

  protected override void DrawContent()
  {
    var commandBuffer = commandQueue.CommandBuffer();

    commandBuffer.Begin();

    RenderPassDescription renderPassDescription = new RenderPassDescription(FrameBuffer, new ClearValue(ClearFlags.All, 1, 0, Color.CornflowerBlue));
    commandBuffer.BeginRenderPass(ref renderPassDescription);

    commandBuffer.SetViewports(viewports);
    commandBuffer.SetScissorRectangles(scissors);
    commandBuffer.SetGraphicsPipelineState(pipelineState);
    commandBuffer.SetVertexBuffers(vertexBuffers);

    commandBuffer.Draw((uint)vertexData.Length / 2);

    commandBuffer.EndRenderPass();
    commandBuffer.End();

    commandBuffer.Commit();

    commandQueue.Submit();
    commandQueue.WaitIdle();
  }
}
