using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace SendPage
{
	class Program
	{
		static void Main(string[] args)
		{
			// get command line options
			var options = new Options();

			if (CommandLine.Parser.Default.ParseArguments(args, options))
			{
				// read source
				using (WebClient client = new WebClient())
				{
					// read page
					string htmlSource = client.DownloadString(options.Url);

					// inline css
					var result = PreMailer.Net.PreMailer.MoveCssInline(htmlSource);

					// send email
					MailMessage message = new MailMessage() { Body = result.Html, IsBodyHtml = true, }; 
					if (!string.IsNullOrWhiteSpace(options.From)) { message.From = new MailAddress(options.From); }
					if (!string.IsNullOrWhiteSpace(options.Subject)) { message.Subject = options.Subject; }
					message.To.Add(options.To);

					using (SmtpClient server = new SmtpClient())
					{
						if (!string.IsNullOrWhiteSpace(options.Login) && !string.IsNullOrWhiteSpace(options.Password))
						{
							server.Credentials = new System.Net.NetworkCredential(options.Login, options.Password);
						}
						server.Send(message);
					}
				}
			}
		}
		
		// command line options
		class Options
		{
			[Option('u', null, Required= true, HelpText = "Url")]
			public string Url { get; set; }

			[Option('f', null, Required= false, HelpText = "From")]
			public string From { get; set; }

			[Option('t', null, Required= true, HelpText = "To")]
			public string To { get; set; }
						
			[Option('s', null, Required= false, HelpText = "Subject")]
			public string Subject { get; set; }

			[Option('l', null, Required= false, HelpText = "Login")]
			public string Login { get; set; }

			[Option('p', null, Required= false, HelpText = "Password")]
			public string Password { get; set; }
			
			[HelpOption]
			public string GetUsage()
			{
				return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
			}
		}
	}
}