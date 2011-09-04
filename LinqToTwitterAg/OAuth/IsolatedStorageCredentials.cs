using System.Xml.Linq;

namespace LinqToTwitter
{
    public class IsolatedStorageCredentials : InMemoryCredentials
    {
        private const string StateFile = "LinqToTwitter.xml";
        private const string ConsumerKeyElement = "consumer_key";
        private const string ConsumerSecretElement = "consumer_secret";
        private const string CodeElement = "code";
        private const string AccessTokenElement = "access_token";
        private const string AuthorizingElement = "authorizing";

        public override string ConsumerKey
        {
            get
            {
                return GetState(ConsumerKeyElement);
            }
            set
            {
                SetState(ConsumerKeyElement, value);
            }
        }

        public override string ConsumerSecret
        {
            get
            {
                return GetState(ConsumerSecretElement);
            }
            set
            {
                SetState(ConsumerSecretElement, value);
            }
        }

        public override string OAuthToken
        {
            get
            {
                return GetState(CodeElement);
            }
            set
            {
                SetState(CodeElement, value);
            }
        }

        public override string AccessToken
        {
            get
            {
                return GetState(AccessTokenElement);
            }
            set
            {
                SetState(AccessTokenElement, value);
            }
        }

        private void SetState(string elementName, string value)
        {
            var state = XElement.Parse(State.Load(StateFile));

            if (state.Element(elementName).Value != value)
            {
                state.Element(elementName).Value = value;
                State.Save(state.ToString(), StateFile);
            }
        }

        private string GetState(string elementName)
        {
            var state = XElement.Parse(State.Load(StateFile));
            return state.Element(elementName).Value;
        }

        public void ClearState()
        {
            var state = XElement.Parse(State.Load(StateFile));

            state.Element(ConsumerKeyElement).Value = string.Empty;
            state.Element(ConsumerSecretElement).Value = string.Empty;
            state.Element(CodeElement).Value = string.Empty;
            state.Element(AccessTokenElement).Value = string.Empty;
            state.Element(AuthorizingElement).Value = string.Empty;

            State.Save(state.ToString(), StateFile);
        }
    }
}
