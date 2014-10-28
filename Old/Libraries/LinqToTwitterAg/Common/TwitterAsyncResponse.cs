using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Response info from an asynchronous Twitter request
    /// </summary>
    public class TwitterAsyncResponse<TState>
    {
        public TwitterAsyncResponse()
        {
            Status = TwitterErrorStatus.Success;
            Message = "Your request succeeded. Error property is null.";
        }

        /// <summary>
        /// You can check this value in the callback to
        /// see if the request succeeded or failed
        /// </summary>
        public TwitterErrorStatus Status { get; set; }

        /// <summary>
        /// Additional info, specific to Status
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Will contain Exception if there was an error
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Request specific info
        /// </summary>
        public TState State { get; set; }
    }
}
