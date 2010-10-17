using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public class StreamContent
    {
        private ITwitterExecute m_exec;

        public StreamContent(ITwitterExecute exec, string content)
        {
            m_exec = exec;
            Content = content;
        }

        public string Content { get; set; }

        public void CloseStream()
        {
            m_exec.CloseStream = true;
        }
    }
}
