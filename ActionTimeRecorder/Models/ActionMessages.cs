using System;
using System.Collections.Generic;
using System.Text;

namespace ActionTimeRecorder.Models
{
    public static class ActionMessages
    {
        public const string Success = "";
        public const string ErrorInvalidActionTemplate = "Error: Action does not match a valid action type. Action: {0}";
        public const string ErrorEmptyAction = "Error: Action JSON is empty";

        /// <summary>
        /// Returns a formatted error message for 
        /// an invalid action
        /// </summary>
        /// <param name="action">An invalid Json string representing action</param>
        public static string ErrorInvalidAction(string action)
        {
            return string.Format(ErrorInvalidActionTemplate, action);
        }
    }
}
