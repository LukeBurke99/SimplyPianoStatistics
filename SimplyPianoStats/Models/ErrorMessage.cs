using Newtonsoft.Json;

namespace SimplyPianoStats.Models
{
	public class ErrorMessage
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		/// <summary>
		/// initialize an error message object that can be given to the user to display an error message.
		/// </summary>
		/// <param name="message">The message for the error</param>
		public ErrorMessage(string message)
		{
			this.Message = message;
		}
	}
}