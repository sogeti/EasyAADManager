using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Data.Interfaces
{
    public interface IWebhookService
    {
        Task<string> CallWebHook(string userInjson, string webhookToCall);
    }
}
