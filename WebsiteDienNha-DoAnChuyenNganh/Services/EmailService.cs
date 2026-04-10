namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class EmailService
	{
		public Task SendAsync(string to, string subject, string body)
		{
			// TODO: plug SMTP later
			return Task.CompletedTask;
		}
	}
}


