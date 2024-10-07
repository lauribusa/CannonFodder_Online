using UnityEngine;

namespace Assets.Features.Debug
{
    public enum DebugLevel
    {
        INFO,
        WARNING,
        ERROR,
    }
    [CreateAssetMenu(menuName = "Debug")]
    public class DebuggerSO: ScriptableObject
    {
        public void PrintInfo(string info)
        {
            Print(info, DebugLevel.INFO);
        }

        public void PrintWarning(string warning)
        {
            Print(warning, DebugLevel.WARNING);
        }

        public void PrintError(string error) 
        { 
            Print(error, DebugLevel.ERROR);
        }
        private void Print(string message, DebugLevel debugLevel)
        {
            switch (debugLevel)
            {
                case DebugLevel.INFO:
                    UnityEngine.Debug.Log(message);
                    break;
                case DebugLevel.WARNING:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case DebugLevel.ERROR:
                    UnityEngine.Debug.LogError(message);
                    break;
                default:
                    UnityEngine.Debug.Log(message);
                    break;
            }
        }
    }
}
