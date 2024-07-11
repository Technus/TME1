using LanguageExt;

namespace TME1.TestCommon;
/// <summary>
/// Helper to handle <see cref="Fin{A}"/> results
/// </summary>
public static class FinExtensions
{
  /// <summary>
  /// Applies method try pattern to <see cref="Fin{A}"/>
  /// </summary>
  /// <typeparam name="A"></typeparam>
  /// <param name="fin">result struct</param>
  /// <param name="value">underlying value</param>
  /// <returns>true if <see cref="Fin{A}.IsSucc"/></returns>
  public static bool TryGetValue<A>(this Fin<A> fin, out A value)
  {
    if (fin.IsFail)
    {
      value = default!;
      return false;
    }
    value = (A)fin;
    return true;
  }
}
