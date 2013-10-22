namespace SmogonWP.Utilities
{
  public static class NetUtilities
  {
    public static bool IsNetwork()
    {
      return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
    }
  }
}
