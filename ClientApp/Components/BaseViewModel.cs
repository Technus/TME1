using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TME1.ClientApp.Components;
/// <summary>
/// Base 'Observable' View Model, To simplify bindings
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
{
  /// <summary>
  /// True if disposed, otherwise: is disposing or new
  /// </summary>
  private bool _disposedValue;

  /// <summary>
  /// Occurs when a property value was changed.
  /// </summary>
  public event PropertyChangedEventHandler? PropertyChanged;
  /// <summary>
  /// Occurs when a property value is about to change.
  /// </summary>
  public event PropertyChangingEventHandler? PropertyChanging;

  /// <summary>
  /// Notifies listeners that a property value is changing.
  /// </summary>
  /// <param name="propertyName">Name of the property used to notify listeners. This
  /// value is optional and can be provided automatically when invoked from compilers
  /// that support <see cref="CallerMemberNameAttribute"/>.</param>
  protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    => OnPropertyChanging(new PropertyChangingEventArgs(propertyName));

  /// <summary>
  /// Raises the PropertyChanging event.
  /// </summary>
  /// <param name="propertyNames">The property names.</param>
  protected void OnPropertyChanging(params string[] propertyNames)
  {
    ArgumentNullException.ThrowIfNull(propertyNames);

    foreach (var propertyName in propertyNames)
      OnPropertyChanging(propertyName);
  }

  /// <summary>
  /// Raises this object's PropertyChanging event.
  /// </summary>
  /// <param name="args">The PropertyChangingEventArgs</param>
  protected virtual void OnPropertyChanging(PropertyChangingEventArgs args)
    => PropertyChanging?.Invoke(this, args);

  /// <summary>
  /// Notifies listeners that a property value has changed.
  /// </summary>
  /// <param name="propertyName">Name of the property used to notify listeners. This
  /// value is optional and can be provided automatically when invoked from compilers
  /// that support <see cref="CallerMemberNameAttribute"/>.</param>
  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

  /// <summary>
  /// Raises the PropertyChanged event.
  /// </summary>
  /// <param name="propertyNames">The property names.</param>
  protected virtual void OnPropertyChanged(params string[] propertyNames)
  {
    ArgumentNullException.ThrowIfNull(propertyNames);

    foreach (var propertyName in propertyNames)
      OnPropertyChanged(propertyName);
  }

  /// <summary>
  /// Raises this object's PropertyChanged event.
  /// </summary>
  /// <param name="args">The PropertyChangedEventArgs</param>
  protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    => PropertyChanged?.Invoke(this, args);

  /// <summary>
  /// For inheritors to dispose own state
  /// </summary>
  protected virtual void DisposeCore() { }

  /// <summary>
  /// Dispose pattern
  /// </summary>
  /// <param name="disposing"></param>
  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
        DisposeCore();
      PropertyChanged = null;
      PropertyChanging = null;
      _disposedValue = true;
    }
  }

  /// <summary>
  /// Dispose pattern
  /// </summary>
  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
