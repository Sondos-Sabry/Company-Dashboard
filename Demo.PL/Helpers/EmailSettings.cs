using Demo.DAL.Models;
using Demo.PL.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using MimeKit;
//using MimeKit.Net.Smtp;


namespace Demo.PL.Helpers
{
    public class EmailSettings : IMailSettings
    {
        private MailSettings options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            this.options = options.Value;
        }

		public void SendEmail(Email email)
		{
			//sender
			var mail = new MimeMessage
			{
				Sender = MailboxAddress.Parse(options.Email),
				Subject = email.Subject
			};

			//send to who ?
			mail.To.Add(MailboxAddress.Parse(email.To));
		   
			// body
			var builder = new BodyBuilder();
			builder.TextBody = email.Body;
			mail.Body = builder.ToMessageBody();
			mail.From.Add(new MailboxAddress(options.DisplayName, options.Email));

			//open conection om mail provider
			using var smtp = new SmtpClient();
			smtp.Connect(options.Host, options.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(options.Email, options.Password);
			smtp.Send(mail);
			smtp.Disconnect(true);
		}


	}
}
