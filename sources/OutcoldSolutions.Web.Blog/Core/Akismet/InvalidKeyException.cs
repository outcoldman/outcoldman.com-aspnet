using System;

namespace PersonalWeb.Core.Akismet
{
  /// <summary>
  /// Akismet didn't accept your API key
  /// </summary>
  public class InvalidKeyException : AkismetException
  {
    internal InvalidKeyException()
      : base("Your API key is not valid.")
    {

    }
    internal InvalidKeyException(string msg)
      : base(msg)
    {

    }

    internal InvalidKeyException(string msg, Exception innerException)
      : base(msg, innerException)
    {

    }
  }
}
