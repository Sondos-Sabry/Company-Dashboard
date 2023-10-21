using Demo.DAL.Models;
using Demo.PL.Settings;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


namespace Demo.PL.Helpers
{
	public class SmsSettings : ISmsMessage
	{
		private TwillioSettings options;

		public SmsSettings(IOptions <TwillioSettings> options )
        {
			this.options = options.Value; ;
		}
  

		public MessageResource Send(SmsMessage smsMessage)
		{
			TwilioClient.Init(options.AccountSID, options.AuthToken);

			var result = MessageResource.Create
				(
					body: smsMessage.Body,
					from: new Twilio.Types.PhoneNumber(options.TwillioPhoneNumber),
					to: smsMessage.PhoneNumber
				);

			return result;

		}
	}
}
