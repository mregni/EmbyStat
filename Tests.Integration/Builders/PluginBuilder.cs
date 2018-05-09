using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Plugins;

namespace Tests.Integration.Builders
{
    public class PluginBuilder
    {
        private PluginInfo model;
        public PluginBuilder()
        {
            model = new PluginInfo();
        }

        public PluginBuilder AddId(string id)
        {
            model.Id = id;
            return this;
        }

        public PluginBuilder AddName(string name)
        {
            model.Name = name;
            return this;
        }

        public PluginBuilder AddAssemblyFileName(string sssemblyFileName)
        {
            model.AssemblyFileName = sssemblyFileName;
            return this;
        }

        public PluginBuilder AddConfigurationDateLastModified(DateTime date)
        {
            model.ConfigurationDateLastModified = date;
            return this;
        }

        public PluginBuilder AddConfigurationFileName(string configurationFileName)
        {
            model.ConfigurationFileName = configurationFileName;
            return this;
        }

        public PluginBuilder AddDescription(string description)
        {
            model.Description = description;
            return this;
        }

        public PluginBuilder AddImageUrl(string imageUrl)
        {
            model.ImageUrl = imageUrl;
            return this;
        }

        public PluginBuilder AddVersion(string version)
        {
            model.Version = version;
            return this;
        }

        public PluginInfo Build()
        {
            return model;
        }

    }
}
