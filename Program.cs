using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
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
				var html = ReadUrl(options.Url);

				// send email
				MailMessage message = new MailMessage() { Body = html, IsBodyHtml = true, };
				if (!string.IsNullOrWhiteSpace(options.From)) { message.From = new MailAddress(options.From); }
				if (!string.IsNullOrWhiteSpace(options.Subject)) { message.Subject = options.Subject; }
				message.To.Add(options.To);

				if (!string.IsNullOrWhiteSpace(options.Attachment))
				{ 
					Attachment attachment = new Attachment(options.Attachment);
					message.Attachments.Add(attachment);
				}

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

		public static string ReadUrl(string url)
		{

			using (WebClient client = new WebClient())
			{
				// read page
				string htmlSource = client.DownloadString(url);
				
				// inline css
				var result = PreMailer.Net.PreMailer.MoveCssInline(htmlSource);

				return result.Html;
			}		
		}


		// command line options
		class Options
		{
			[Option('u', null, Required= true, HelpText = "Url")]
			public string Url { get; set; }

			[Option('a', null, Required= false, HelpText = "Attachment")]
			public string Attachment { get; set; }

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