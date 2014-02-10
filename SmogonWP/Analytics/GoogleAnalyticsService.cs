using System.Windows;
using System.ComponentModel.Composition.Hosting;
using Google.WebAnalytics;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Behaviors;
using Microsoft.WebAnalytics.Data;
using SmogonWP.Services;

namespace SmogonWP.Analytics
{
  public class GoogleAnalyticsService : IApplicationService
  {
    private readonly GoogleAnalytics _googleAnalytics;
    private readonly IApplicationService _innerService;

    public GoogleAnalyticsService()
    {
      _googleAnalytics = new GoogleAnalytics();
      _googleAnalytics.CustomVariables.Add(new PropertyValue { PropertyName = "Application Version", Value = RateService.GetAppVersion() });
      _googleAnalytics.CustomVariables.Add(new PropertyValue { PropertyName = "Device", Value = RateService.GetDeviceModel() });

      _innerService = new WebAnalyticsService { IsPageTrackingEnabled = false, Services = { _googleAnalytics, } };
    }

    public string WebPropertyId
    {
      get { return _googleAnalytics.WebPropertyId; }
      set { _googleAnalytics.WebPropertyId = value; }
    }

    #region IApplicationService Members

    public void StartService(ApplicationServiceContext context)
    {
      CompositionHost.Initialize(
          new AssemblyCatalog(
              Application.Current.GetType().Assembly),
          new AssemblyCatalog(typeof(AnalyticsEvent).Assembly),
          new AssemblyCatalog(typeof(TrackAction).Assembly));
      _innerService.StartService(context);
    }

    public void StopService()
    {
      _innerService.StopService();
    }

    #endregion
  }
}