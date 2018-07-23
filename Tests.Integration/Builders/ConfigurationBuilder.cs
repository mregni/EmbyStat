using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Controllers.ViewModels.Configuration;

namespace Tests.Integration.Builders
{
    public class ConfigurationBuilder
    {
        private readonly ConfigurationViewModel model;

        public ConfigurationBuilder()
        {
            model = new ConfigurationViewModel();
        }

        public ConfigurationBuilder AddUserName(string username)
        {
            model.Username = username;
            return this;
        }

        public ConfigurationBuilder AddAccessToken(string accessToken)
        {
            model.AccessToken = accessToken;
            return this;
        }

        public ConfigurationBuilder AddEmbyServerAddress(string embyServerAddress)
        {
            model.EmbyServerAddress = embyServerAddress;
            return this;
        }

        public ConfigurationBuilder AddEmbyUserId(string embyUserId)
        {
            model.EmbyUserId = embyUserId;
            return this;
        }

        public ConfigurationBuilder AddEmbyUserName(string embyUserName)
        {
            model.EmbyUserName = embyUserName;
            return this;
        }

        public ConfigurationBuilder AddLanguage(string language)
        {
            model.Language = language;
            return this;
        }

        public ConfigurationBuilder AddServerName(string serverName)
        {
            model.ServerName = serverName;
            return this;
        }

        public ConfigurationBuilder AddToShortMovie(int toShortMovie)
        {
            model.ToShortMovie = toShortMovie;
            return this;
        }

        public ConfigurationBuilder AddWizardFinished(bool wizardFinished)
        {
            model.WizardFinished = wizardFinished;
            return this;
        }

        public ConfigurationViewModel Build()
        {
            return model;
        }
    }
}
