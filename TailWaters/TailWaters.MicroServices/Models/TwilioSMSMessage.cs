using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TailWaters.MicroServices.Models
{
    internal class TwilioSMSMessage
    {
        private Dictionary<string, string> _Message;
        private string body;

        public TwilioSMSMessage()
        {

        }

        public TwilioSMSMessage(string message)
        {
            _Message = message.Split('&')
                        .Select(value => value.Split('='))
                        .ToDictionary(pair => Uri.UnescapeDataString(pair[0]).Replace("+", " "),
                                        pair => Uri.UnescapeDataString(pair[1]).Replace("+", " "));
        }

        public string To { get => _Message[nameof(To)]; set => _Message[nameof(To)] = value; }
        public string ToCountry { get => _Message[nameof(ToCountry)]; set => _Message[nameof(ToCountry)] = value; }
        public string ToState { get => _Message[nameof(ToState)]; set => _Message[nameof(ToState)] = value; }
        public string ToCity { get => _Message[nameof(ToCity)]; set => _Message[nameof(ToCity)] = value; }
        public string ToZip { get => _Message[nameof(ToZip)]; set => _Message[nameof(ToZip)] = value; }

        public string From { get => _Message[nameof(From)]; set => _Message[nameof(From)] = value; }
        public string FromCountry { get => _Message[nameof(FromCountry)]; set => _Message[nameof(FromCountry)] = value; }
        public string FromState { get => _Message[nameof(FromState)]; set => _Message[nameof(FromState)] = value; }
        public string FromCity { get => _Message[nameof(FromCity)]; set => _Message[nameof(FromCity)] = value; }
        public string FromZip { get => _Message[nameof(FromZip)]; set => _Message[nameof(FromZip)] = value; }

        public string Body { get => _Message[nameof(Body)]; set => _Message[nameof(Body)] = value; }


        public static TwilioSMSMessage Parse(string message)
        {
            return new TwilioSMSMessage(message);
        }
    }
}
