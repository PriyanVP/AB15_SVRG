using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using System.Reflection;
using System.Text;

namespace AB15_GUI.WPF.NLog
{
    [LayoutRenderer("build-configuration")]
    [ThreadAgnostic]
    public class BuildConfigurationLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Current GUI build configuration
        /// </summary>
        public string BuildConfiguration
        {
            get 
            {
                AssemblyConfigurationAttribute? assemblyConfigurationAttribute = this.GetType().Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
                string buildConfiguration = assemblyConfigurationAttribute?.Configuration ?? "Debug"; // Default value in case of null provided
                return buildConfiguration; 
            }
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(BuildConfiguration);
        }
    }
}
