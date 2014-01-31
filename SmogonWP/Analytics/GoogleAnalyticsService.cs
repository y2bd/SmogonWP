using System.Windows;
using System.ComponentModel.Composition.Hosting;
using Google.WebAnalytics;
using Microsoft.Phone.Info;
using Microsoft.WebAnalytics;
using Microsoft.WebAnalytics.Behaviors;
using Microsoft.WebAnalytics.Data;
using MC.Phone.Analytics;

namespace SmogonWP.Analytics
{
    public class GoogleAnalyticsService : IApplicationService
    {
        private readonly GoogleAnalytics _googleAnalytics;
        private readonly IApplicationService _innerService;

        public GoogleAnalyticsService()
        {
            _googleAnalytics = new GoogleAnalytics();
            _googleAnalytics.CustomVariables.Add(new PropertyValue
                                                     {
                                                         PropertyName = "Application Version",
                                                         Value = AnalyticsProperties.ApplicationVersion
                                                     });
            _googleAnalytics.CustomVariables.Add(new PropertyValue
                                                     {PropertyName = "Device OS", Value = AnalyticsProperties.OsVersion});
            _googleAnalytics.CustomVariables.Add(new PropertyValue
                                                     {PropertyName = "Device", Value = AnalyticsProperties.Device});
            _innerService = new WebAnalyticsService
                                {
                                    IsPageTrackingEnabled = false,
                                    Services = {_googleAnalytics,}
                                };
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
                new AssemblyCatalog(typeof (AnalyticsEvent).Assembly),
                new AssemblyCatalog(typeof (TrackAction).Assembly));
            _innerService.StartService(context);
        }

        public void StopService()
        {
            _innerService.StopService();
        }

        #endregion
    }
}