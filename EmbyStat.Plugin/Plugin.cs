//Sample: https://github.com/MediaBrowser/NfoMetadata/blob/master/NfoMetadata/Plugin.cs

using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Plugin;

public class Plugin : BasePlugin, IHasWebPages, IHasThumbImage
{
    public override Guid Id => new("ffa904b3-7bb1-4c23-ad5c-10a8c0f8b441");
    public override string Name => "EmbyStat plugin";
    public override string Description => "Calculate statistics and send it to your EmbyStat server";

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            new PluginPageInfo
            {
                Name = "embystat",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.embystat.html"
            },
            new PluginPageInfo
            {
                Name = "embystatjs",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.embystat.js"
            }
        };
    }

    public Stream GetThumbImage()
    {
        var type = GetType();
        return type.Assembly.GetManifestResourceStream(type.Namespace + ".Assets.logo.png");
    }

    public ImageFormat ThumbImageFormat => ImageFormat.Png;
}