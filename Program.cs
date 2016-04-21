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
					MailMessage message = new MailMessage(options.From, options.To, options.Url, result.Html) { IsBodyHtml = true };

					using (SmtpClient server = new SmtpClient())
					{
						server.Credentials = new System.Net.NetworkCredential(options.From, options.Password);
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

			[Option('f', null, Required= true, HelpText = "From")]
			public string From { get; set; }

			[Option('t', null, Required= true, HelpText = "To")]
			public string To { get; set; }

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