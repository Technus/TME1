using LanguageExt;

namespace TME1.Abstractions;
public static class FinExtensions
{
  public static bool TryGetValue<A>(this Fin<A> fin, out A value)
  {
    if(fin.IsFail)
    {
      value = default!; 
      return false;
    }
    value = (A)fin;
    return true;
  }
}
