using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Models
{
    public class Settings
    {
        private readonly AppSettings _appSettings;

        public Settings(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public List<Setting> GetSettings()
        {
            var returnValue = new List<Setting>();

            var webHookSetting = new Setting()
            {
                Key = "Runbook Webhook",
                Value = _appSettings.WebHookUrlCreateUsersRunbook
            };
            returnValue.Add(webHookSetting);

            return returnValue;
        }
    }

    public class Setting
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
