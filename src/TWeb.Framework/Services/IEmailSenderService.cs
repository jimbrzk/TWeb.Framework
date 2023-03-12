using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Services
{
    public interface IEmailSenderService
    {
        public void SendEmail(string email, string subject, string body);
        public void SendEmailAsync(string email, string subject, string body, CancellationToken ct = default);
    }
}
