using Demo.DAL.Models;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helpers
{
	public interface ISmsMessage
	{

		public MessageResource Send(SmsMessage smsMessage);
	}
}
