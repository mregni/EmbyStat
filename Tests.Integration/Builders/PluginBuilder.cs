using MediaBrowser.Model.Plugins;

namespace Tests.Integration.Builders
{
    public class PluginBuilder
    {
        private readonly PluginInfo _model;
        public PluginBuilder()
        {
            _model = new PluginInfo();
        }

        public PluginBuilder AddId(string id)
        {
            _model.Id = id;
            return this;
        }

        public PluginBuilder AddName(string name)
        {
            _model.Name = name;
            return this;
        }

        public PluginBuilder AddConfigurationFileName(string configurationFileName)
        {
            _model.ConfigurationFileName = configurationFileName;
            return this;
        }

        public PluginBuilder AddDescription(string description)
        {
            _model.Description = description;
            return this;
        }

        public PluginBuilder AddImage(string imageUrl)
        {
            _model.ImageTag = imageUrl;
            return this;
        }

        public PluginBuilder AddVersion(string version)
        {
            _model.Version = version;
            return this;
        }

        public PluginInfo Build()
        {
            return _model;
        }

    }
}
