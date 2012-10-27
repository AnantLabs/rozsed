using System;
using System.Runtime.InteropServices;

namespace ROZSED.Std
{
    public static class Exc
    {
        public static void Void()
        { }
        public static string Message(this Exception e, bool innerExceptionMsg = false, bool stackTrace = false, bool errorCode = false)
        {
            ExternalException ext;
            var message = e.Message;
            if ((ext = e as ExternalException) != null)
                switch (((ExternalException)e).ErrorCode)
                {
                    case -2147216882:
                        message = "Invalid spatial reference. Use ConstructFromHorizon() method."; break;
                    case -2147216889:
                        message = "No spatial reference exists."; break;
                    case -2147216880:
                        message = "Invalid Z domain."; break;
                    case -2147220952:
                        message = "License problem."; break;
                    case -2147467259:
                        message = "Unspecified error (geoprocessor failed, memory issue, licensing, ...)."; break;
                    case -2147220655:
                        message = "The table was not found."; break;
                    default:
                        if (errorCode)
                            message += "\n    ExternalException.ErrorCode = " + ext.ErrorCode;
                        break;
                }

            var innerMsg = "";
            if (innerExceptionMsg && e.InnerException != null)
                innerMsg = "\n" + e.InnerException.Message(true);

            return message + innerMsg + (stackTrace ? "\n" + e.StackTrace() : "");
        }
        /// <summary>
        /// Get the most inner StackTrace
        /// </summary>
        public static string StackTrace(this Exception e)
        {
            if (e.InnerException == null)
                return e.StackTrace;
            else
                return e.InnerException.StackTrace();
        }
    }
}
