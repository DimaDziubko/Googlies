using UnityEngine;

namespace _Game.Core._Logger
{
    public enum DebugStatus
    {
        Success,
        Warning,
        Fault,
        Info 
    }
    public class MyLogger : IMyLogger
    {
        public void Log(string message, DebugStatus status)
        {
#if LOG_TO_DEVICE
            switch (status)
            {
                case DebugStatus.Success:
                    Debug.Log($"<color=#7FFF00>{message}</color>");
                    break;
                case DebugStatus.Warning:
                    Debug.Log($"<color=yellow>{message}</color>");
                    break;
                case DebugStatus.Fault:
                    Debug.Log($"<color=red>{message}</color>");
                    break;
                case DebugStatus.Info:
                    Debug.Log($"<color=#FFC0CB>{message}</color>");
                    break;
            }
#endif
        }

        public void Log(string message)
        {
#if LOG_TO_DEVICE
            Debug.Log(message);
#endif
        }

        public void LogWarning(string message)
        {
#if LOG_TO_DEVICE           
            Debug.LogWarning(message);
#endif            
        }

        public void LogError(string message)
        {
#if LOG_TO_DEVICE            
            Debug.LogError(message);
#endif           
        }
    }
}